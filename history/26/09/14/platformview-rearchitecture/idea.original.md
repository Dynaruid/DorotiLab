# Doroti PlatformView / WebView 설계 아이디어

작성일: 2026-09-11. 검토 기준: Doroti checkout `cf83abd603aa06b942051cabdeb5132f4dd774c7`, 저장소의 Flutter/Avalonia/`flutter_inappwebview-master` 참조 소스, `pointer_interceptor` 공식 문서·소스와 Qt 공식 문서. 사용자 지정에 따라 WebView 주 레퍼런스와 Linux Qt 채택 방향을 반영했다.

이 문서는 조사와 설계 제안이다. 아래 새 타입·패키지·단계는 아직 구현되지 않았으며 PlatformView/WebView의 빌드·실행·성능·기기 검증 결과를 뜻하지 않는다. 기존 `idea.md`는 공백만 있는 상태였다.

## 1. 권장 방향

**PlatformView를 Doroti의 공통 네이티브 뷰 호스팅 기능으로 만들고, WebView는 이를 사용하는 선택형 패키지로 구성하는 것이 좋다.** Flutter는 중요한 참고 구현이다. 기존 Doroti API와 연결되는 서비스·렌더 객체뿐 아니라, 엔진의 external-view embedder와 플랫폼별 입력·합성 처리를 함께 봐야 한다.

1. 기존 `AndroidView`, `UiKitView`, `AppKitView`, `PlatformViewLink`, `PlatformViewSurface`의 의미를 살리고 실제 host 계약으로 연결한다.
2. 공통 기반은 생성/해제, 배치, 입력, 포커스, 접근성, 프레임 수명이다. WebView의 URL·JS·쿠키·탐색 정책은 별도 계층에 둔다.
3. 초기 구현은 네이티브 뷰 계층/OS compositor를 이용한다. 모든 native control을 Skia texture로 바꾸는 것을 기본 전제로 삼지 않는다.
4. **단순 배치와 Doroti 위젯 사이의 교차 합성을 별도 지원 수준으로 노출한다.** WebView 위 메뉴·다이얼로그·반투명 위젯에는 실제 foreground surface 구현이 필요하다.
5. Windows의 WebView2 composition 경로와 Web의 main-DOM/worker-canvas 결합을 우선 검증한다. Android HCPP와 external texture는 고급 단계로 둔다.
6. **WebView API·플랫폼 분리·수명은 `reference/flutter_inappwebview-master`, 겹친 UI의 입력 보호는 `pointer_interceptor`를 주 레퍼런스로 삼는다.** Flutter engine은 공통 PlatformView 합성의 참고 기준으로 유지한다.
7. **Linux는 Qt의 WebView를 사용한다.** Qt WebView/Qt WebEngine의 구체적인 API 접점과 필요한 Qt 버전은 아래 Linux 항목에서 구분한다.

이 기반은 지도·네이티브 편집기에도 재사용할 수 있다. 카메라처럼 픽셀 생산이 중심인 기능은 이후 별도 external-texture 계약을 선택할 수 있다.

## 2. 현재 Doroti에서 확인한 상태

| 영역 | 현재 소스 | 설계에 주는 의미 |
|---|---|---|
| 위젯/서비스 | PlatformView 계열 위젯과 `PlatformViewsService`, Android/Darwin controller가 존재 | API를 처음부터 복제할 필요는 없지만 타입 존재가 실행 지원을 뜻하지는 않는다. |
| 렌더 객체 | `RenderAndroidView`, `RenderDarwinPlatformView`, `PlatformViewRenderBox`, `PlatformViewLayer`가 존재 | 레이아웃·gesture arena·scene 연결의 출발점이 있다. |
| SceneBuilder | `addPlatformView()`와 `addTexture()`가 명령을 추가하지만 typed `HostPayload`를 설정하지 않음 | native instance 참조와 합성 상태의 명시적 payload가 필요하다. |
| Skia renderer | `DrawScene()`에 두 명령의 분기가 없고 default에서 `NotSupportedException` 발생 | controller/plugin만 추가해서는 표시되지 않는다. |
| Capability | view별 registry와 누락 시 예외 계약이 있으나 전용 PlatformView capability는 없음 | `platform.views` 같은 선택 capability가 자연스럽다. |
| 채널 | `flutter/platform_views`, `flutter/platform_views_2` 선언이 있으나 검색한 product host 소스에서 대응 native view 구현을 찾지 못함 | 일반 plugin transport와 native view manager를 구분해야 한다. |
| Web | `_html_element_view_web.cs`는 Widgets 프로젝트에서 compile 제외. DOM root는 canvas·IME textarea·semantics로 구성 | 파일을 포함하는 것만으로 지원되지 않는다. 실제 DOM registry/compositor가 필요하다. |
| Web renderer | 기본 `worker-direct-webgpu`, 명시적 `worker-direct-webgl`; OffscreenCanvas 사용 | DOM은 main이 소유하고 worker에는 ID·불변 배치만 전달한다. |
| Windows | top-level HWND의 topmost DirectComposition target에 단일 content visual 연결 | child HWND를 추가하면 보일 것이라고 가정할 수 없다. 다중 visual root 실험이 필요하다. |
| Android | Graphite host는 `SurfaceView`; 소스에 MAUI semantics/IME overlay 아래 합성된다고 명시 | native WebView 배치와 Doroti foreground overlay는 다른 문제다. |
| Apple | UIKit Graphite view와 AppKit Metal view가 별도로 존재 | iOS/Mac Catalyst와 native AppKit macOS backend를 구분한다. |
| Linux | native template host의 Graphite 경로는 `QWindow`, 비교 경로는 `QOpenGLWindow` | 일반 QWidget WebView 예제를 바로 적용할 수 없다. |
| Graphite | render owner thread, generation, GPU 완료 후 반환을 관리 | UI thread의 native view 수명과 render 자원 수명을 연결해야 한다. |

확인한 로컬 근거:

- [Widgets](Doroti/src/Doroti.Framework.Widgets/platform_view.cs), [Services](Doroti/src/Doroti.Framework.Services/platform_views.cs), [Rendering](Doroti/src/Doroti.Framework.Rendering/platform_view.cs), [Layer](Doroti/src/Doroti.Framework.Rendering/layer.cs)
- [SceneBuilder/scene 계약](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), [SkiaSceneRenderer](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs), [Capabilities](Doroti/src/Doroti.Ui/Capabilities.cs), [frame/epoch 계약](Doroti/src/Doroti.Ui/ViewContracts.cs)
- [Widgets compile 제외 목록](Doroti/src/Doroti.Framework.Widgets/Doroti.Framework.Widgets.csproj), [Web DOM](Doroti/src/Doroti.Host.Web/Web/doroti.web.dom.ts), [Web renderer 선택](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
- [Windows composition](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp), [Android SurfaceView](Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs), [UIKit Graphite](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs), [AppKit Metal](Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs), [Qt native host](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/src/doroti_qt_host.cpp), [Graphite session](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.cs)

문서와 소스의 시간 차이에 주의한다. [ADR-025](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md)는 ANGLE 시절 본문과 Vulkan 기본값 변경 amendment가 함께 있다. 위 표는 현재 소스를 기준으로 했다. 최신 [NativeAOT 실행 기록](Doroti/docs/validation/nativeaot-2026-09-10.md)도 실제 iPhone host probe와 전체 Testbed/배포 게이트를 구분한다. 과거 환경 부재 기록이나 host probe 성공을 신규 WebView의 AOT 지원 여부로 전용하지 않는다.

## 3. Flutter와 다른 구현에서 참고할 부분

### 3.1 External-view embedder가 핵심

Flutter `PlatformViewLayer::Preroll()`은 transform과 mutator stack을 embedder에 전달한다. `Paint()`는 `CompositeEmbeddedView()`가 돌려준 canvas로 바꾸어 native view 이후의 Flutter 그리기를 다른 구간에 기록한다. PlatformView는 단순한 `drawImage()`가 아니라 **화면 합성 경계**다.

```text
Doroti 배경 → PlatformView A → Doroti 중간 그림 → PlatformView B → Doroti 전경
```

이 순서를 하나의 canvas와 최상단 native control들만으로 표현할 수는 없다. 여러 raster surface와 native visual의 순서를 관리하거나, 지원되는 view에 한해 texture 합성을 선택해야 한다. [로컬 platform_view_layer.cc](reference/flutter-master/engine/src/flutter/flow/layers/platform_view_layer.cc), [embedded_views.h](reference/flutter-master/engine/src/flutter/flow/embedded_views.h)

Doroti 이식 파일의 provenance 주석은 Flutter `56b8e1a8`을 가리킨다. 이는 파일의 이식 기준이며 당일 웹 문서 버전이나 참조 폴더 전체의 재검증된 commit을 의미하지 않는다.

### 3.2 Android: 여러 방식의 비용을 비교

| Flutter 방식 | 참고할 점 | Doroti 판단 |
|---|---|---|
| Virtual Display | 별도 display 결과를 texture로 사용 | IME·접근성·중간 버퍼 때문에 초기 기본값으로 권하지 않는다. |
| Hybrid Composition | native view hierarchy와 Flutter overlay 합성 | 네이티브 동작 보존에 유리하지만 SurfaceView·frame scheduling을 맞춰야 한다. |
| Texture Layer Hybrid Composition | native view draw를 texture 대상으로 돌림 | 빠른 WebView scroll, SurfaceView 자식, 확대경 등의 제약을 재현 실험해야 한다. |
| HCPP | native surface transaction 동기화 | 고급 연구 후보. Doroti에서 바로 켤 수 있는 기능이 아니다. |

확인한 공식 문서에서 HCPP는 Flutter 3.44부터의 실험적 opt-in이며 Android API 34+, Impeller Vulkan을 요구한다. Graphite Vulkan은 Impeller가 아니므로 transaction/overlay 설계 원리를 참고하되 Doroti backend 구현과 검증은 별도로 한다. [Flutter Android 공식 문서](https://docs.flutter.dev/platform-integration/android/platform-views)

로컬 소스도 기존 controller와 `PlatformViewsController2`가 분리되어 있으며 후자는 pending/active `SurfaceControl.Transaction`을 모아 적용한다. 구버전 전략 전체를 포팅하기보다 필요한 최소 전략부터 구현한다. [기존 controller](reference/flutter-master/engine/src/flutter/shell/platform/android/io/flutter/plugin/platform/PlatformViewsController.java), [SurfaceControl controller](reference/flutter-master/engine/src/flutter/shell/platform/android/io/flutter/plugin/platform/PlatformViewsController2.java)

### 3.3 Apple: native hierarchy와 overlay 수명

Flutter iOS는 UIView를 계층에 추가하는 hybrid composition을 사용한다. 로컬 iOS embedder도 preroll/composite/submit을 controller로 넘기며 overlay surface를 별도로 다룬다. Doroti에서도 WKWebView를 붙이는 작업과 Metal 전경 surface 합성을 구분해야 한다. [Flutter iOS 문서](https://docs.flutter.dev/platform-integration/ios/platform-views), [로컬 iOS embedder](reference/flutter-master/engine/src/flutter/shell/platform/darwin/ios/ios_external_view_embedder.mm)

macOS 문서는 NSView hosting의 참고 자료다. UIKit 처리의 이름만 바꾸지 말고 AppKit responder·좌표계·수명을 별도로 연결한다. [Flutter macOS 문서](https://docs.flutter.dev/platform-integration/macos/platform-views)

### 3.4 Web: DOM 보존과 canvas 분할

Flutter web content manager는 factory와 생성한 DOM content를 보관하고 embedder는 composition order·clip chain·canvas 수를 관리한다. DOM을 매 프레임 재생성하지 않는 점과 overlay를 줄이는 방식을 참고한다. [content_manager.dart](reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/platform_views/content_manager.dart), [embedder.dart](reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/platform_views/embedder.dart)

`HtmlElementView` 공식 문서는 canvas 사이에 HTML을 끼우려면 overlay 분할이 필요하고 iframe 내부 pointer event는 Flutter로 전달되지 않는다고 설명한다. 투명 canvas 하나를 추가하는 것으로 입력까지 해결된다고 가정하면 안 된다. [HtmlElementView API](https://api.flutter.dev/flutter/widgets/HtmlElementView-class.html)

### 3.5 WebView 주 레퍼런스: flutter_inappwebview

사용자가 지정한 [로컬 flutter_inappwebview-master](reference/flutter_inappwebview-master/README.md)를 WebView 설계의 주 레퍼런스로 삼는다. 확인한 [pubspec](reference/flutter_inappwebview-master/flutter_inappwebview/pubspec.yaml)의 버전은 `6.2.0-beta.3`이다. 이는 로컬 스냅샷의 표기이며 최신 release 또는 Doroti 검증 버전이라는 뜻은 아니다. `webview_flutter`는 앞서 살펴본 controller/widget 분리의 보조 자료로만 남긴다.

| 검토 지점 | 실제 확인한 구조 | Doroti에 반영할 부분 |
|---|---|---|
| 공통 widget | `InAppWebView`가 `PlatformInAppWebViewWidget`에 위임하고 creation params를 분리 | C# 공통 widget/controller와 host별 factory·options 분리 |
| controller interface | JS 실행·handler 등록/해제·keep-alive 해제 등 기능별 계약 | 필요한 기능부터 capability 및 명령/이벤트 계약으로 대응 |
| 수명 | `keepAlive`, headless view, WebViewEnvironment가 별도 개념 | widget detach, native instance 유지, profile/environment 수명을 구분 |
| Windows | WebView2 composition controller, mouse/pointer 전달, native manager와 texture bridge 존재 | WebView2 초기화·입력·비동기 수명 로직 참고. Flutter texture 운송은 Doroti compositor와 별도 판단 |
| Web | `HtmlElementView`와 iframe/container 사용 | main-DOM element 소유권, dispose/keep-alive, 브라우저 제약을 반영 |
| Linux | 로컬 Linux 패키지는 WPE WebKit 구현 | 공통 기능 의미는 참고하고 native backend는 사용자가 선택한 Qt로 작성 |

소스 근거: [InAppWebView widget](reference/flutter_inappwebview-master/flutter_inappwebview/lib/src/in_app_webview/in_app_webview.dart), [controller 계약](reference/flutter_inappwebview-master/flutter_inappwebview_platform_interface/lib/src/in_app_webview/platform_inappwebview_controller.dart), [Windows WebView](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/in_app_webview/in_app_webview.cpp), [Windows manager](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/in_app_webview/in_app_webview_manager.cpp), [Web widget](reference/flutter_inappwebview-master/flutter_inappwebview_web/lib/src/in_app_webview/in_app_webview.dart), [Web iframe](reference/flutter_inappwebview-master/flutter_inappwebview_web/lib/web/in_app_web_view_web_element.dart), [Linux backend 설명](reference/flutter_inappwebview-master/flutter_inappwebview_linux/README.md).

초기 범위는 inline WebView·navigation·JS message·settings·수명이다. headless/in-app browser window·고급 interception·전체 API 동등성은 후속 범위로 둔다. reference의 거대한 옵션/이벤트 목록을 그대로 C# 단일 타입에 복제하지 않고 실제 지원표를 만든다.

Flutter native plugin은 FlutterEngine·registrar·messenger·texture registry 등에 의존할 수 있다. 채널 이름이 같아도 기존 Flutter plugin 바이너리를 그대로 쓸 수 있다는 뜻은 아니다. 플랫폼 SDK를 직접 연결하거나 관련 로직을 검토해 Doroti adapter로 구현한다.

### 3.6 Avalonia: 작은 호스팅 계약

로컬 Avalonia `NativeControlHost`는 native handle과 TopLevel attachment를 구분하고 bounds를 root 기준으로 변환·반올림하며 detach 후 파괴를 지연해 reparent를 처리한다. 이는 Doroti 수명·좌표 계약에 유용하다. 다만 AABB 위치 갱신과 `ShowInBounds()`는 실제 회전이나 임의 Skia clip 지원의 증거가 아니다. [NativeControlHost.cs](reference/Avalonia-main/src/Avalonia.Controls/NativeControlHost.cs), [INativeControlHostImpl.cs](reference/Avalonia-main/src/Avalonia.Controls/Platform/INativeControlHostImpl.cs)

### 3.7 입력 보호 주 레퍼런스: pointer_interceptor

`pointer_interceptor`는 Web에서 빈 HTML element, iOS에서 빈 UIView를 child 뒤의 paint order에 배치해 아래 platform view가 입력을 먼저 가져가는 것을 막는다. `intercepting=false`이면 child만 반환하고 `debug`로 보호 영역을 표시한다. iOS에서 많은 instance를 만들면 native view 비용이 커질 수 있다. [공식 패키지 설명](https://pub.dev/packages/pointer_interceptor)

검토한 공식 Web 소스는 `HtmlElementView.fromTagName('div', isVisible: false)`와 child를 Stack으로 구성하고, `mousedown.preventDefault()`로 focus 손실을 방지한다. 여기서 `isVisible: false`는 픽셀을 그리지 않는다는 합성 힌트이며 DOM hit testing을 비활성화한다는 의미가 아니다. [공식 Web 구현](https://github.com/flutter/packages/blob/main/packages/pointer_interceptor/pointer_interceptor_web/lib/pointer_interceptor_web.dart)

Doroti에는 `PointerInterceptor(child, intercepting, debug)` 성격의 재사용 가능한 위젯을 제안한다. 구현은 frame의 transform/clip/paint order를 따르는 input-shield placement로 내려간다. Web에서는 차단 element에서 발생한 입력을 Doroti의 기존 입력 ingress로 연결하되 중복 dispatch를 막는다. iOS는 native hit-test 경계를 보호하는 view를 검증하고, Windows/Qt처럼 host가 직접 입력을 중재하는 경우에는 같은 의미를 routing 정책으로 구현할 수 있는지 확인한다.

이 위젯은 **전경 UI가 보여야 하는 문제와 별개인 입력 보호 기능**이다. foreground raster surface를 만들거나 iframe 내부 gesture/DOM/키보드에 접근하게 해주지 않는다. `IgnorePointer`/`AbsorbPointer` 같은 framework 내부 hit-test 처리만으로 이미 native/DOM에서 가로챈 입력을 되찾을 수 있다고 가정하지 않는다.

보호 영역은 메뉴/버튼의 실제 영역에 맞추고 modal barrier일 때만 필요한 전체 영역을 덮는다. `pointer-events:none`으로 WebView 전체 상호작용을 끄는 대체 구현은 사용하지 않는다. 메뉴가 닫히면 shield와 event listener를 제거하고, mousedown focus 보존은 native TextField로의 의도한 focus 이동을 방해하지 않는지 검증한다. pointer/wheel/drag/capture와 keyboard/focus는 별도 계약이다.

## 4. 계층과 패키지 제안

```text
App: WebViewWidget + WebViewController / 일반 PlatformView
  → Framework Widgets · Rendering · Services
  → Doroti.Ui: typed scene payload · host capability
  → Doroti.Hosting: instance registry · composition plan · channel adapter
  → Host: HWND/DComp | Android View | UIView/NSView | DOM | Qt
      ↕
    Skia renderer: native view 앞/뒤의 Doroti raster 구간
```

| 위치/제안 이름 | 책임 |
|---|---|
| 기존 Framework 프로젝트 | 기존 플랫폼별 API 연결, 공통 PlatformView·PointerInterceptor 위젯, render object, 입력 중재 |
| 기존 `Doroti.Ui` | `IPlatformViewHostCapability`, handle, immutable placement, scene payload, support DTO |
| 기존 `Doroti.Hosting` | registry, 수명 coordinator, composition planner, 기존 채널 adapter |
| 기존 `Doroti.Skia.Rendering` | raster segment 렌더링, native content를 포함하는 효과/캡처 지원 판정 |
| 기존 host 프로젝트 | native container, UI dispatcher, focus/accessibility, compositor transaction |
| 신규 `Doroti.WebView` | flutter_inappwebview를 기준으로 controller/widget, navigation/message/profile 공통 계약 |
| 신규 WebView backend 패키지 | Windows/Android/UIKit/AppKit/Web와 Linux Qt 구현 및 선택 의존성 |

이름은 제안이다. WebView2·Qt WebEngine처럼 사용하지 않는 앱까지 배포할 필요 없는 의존성은 선택 패키지로 격리한다. [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md)에 따라 제품 소스를 직접 수정하며 전체 Flutter 재번역은 필요하지 않다. [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md)에 맞춰 native SDK/COM/pointer는 runner/backend 안에 둔다.

기존 [DorotiApplicationBoundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs)의 manifest·plugin handler·resource capability를 활용한다. 채널은 명령 전달용이고 frame별 placement를 JSON/MethodChannel 여러 번으로 전달하는 주 경로로 삼지 않는다. host→framework callback, 특히 `viewFocused` handler 설치와 다중 view routing도 구현 범위다.

## 5. 공통 계약에서 먼저 정할 것

### 5.1 Identity와 소유권

아래는 계약 설명용 의사 코드이며 현재 컴파일 가능한 API가 아니다.

```csharp
readonly record struct PlatformViewHandle(
    ulong OwnerViewId, long InstanceId, long InstanceGeneration);

interface IPlatformViewHostCapability
{
    PlatformViewSupport QuerySupport(PlatformViewRequest request);
    ValueTask<PlatformViewHandle> CreateAsync(
        PlatformViewCreateInfo info, CancellationToken cancellationToken);
    ValueTask DisposeAsync(PlatformViewHandle handle);
}

record PlatformViewPlacement(
    PlatformViewHandle Handle, Rect LogicalBounds,
    TransformSnapshot Transform, ClipSnapshot[] Clips,
    double Opacity, int PaintOrder, bool Visible,
    PlatformInputPolicy InputPolicy);

record PlatformCompositionPlan(
    DorotiViewEpoch ViewEpoch, long FrameId,
    CompositionSegment[] OrderedSegments);
```

`OwnerViewId`는 Doroti 최상위 view, `InstanceId`는 삽입한 native view, `InstanceGeneration`은 인스턴스 수명이다. presenter surface generation과 섞지 않는다. resize/GPU 재생성마다 WebView까지 재생성하면 탐색·입력·미디어 상태가 초기화될 수 있다.

factory 정의는 앱 단위로 공유할 수 있지만 live instance는 owner view별로 관리한다. 초기에는 한 instance의 동시 다중 attach를 금지한다. window 간 이동은 명시적 detach/attach와 backend 지원을 검증한 뒤 허용한다.

### 5.2 수명과 비동기 경쟁

```text
Requested → Creating → Ready → Attached ↔ Hidden/Detached → Disposing → Disposed
                 └→ Failed
```

- 0 크기 또는 미준비 상태에서는 표시를 지연한다. bounded layout을 요구하며 HTML 전체 높이를 자동 intrinsic size로 계산하는 기능은 초기 계약에 넣지 않는다.
- create 중 dispose되면 늦은 생성 결과를 표시하지 않고 UI thread에서 해제한다. 비동기 명령·이벤트는 handle/generation으로 검증한다.
- rebuild·scroll·일시적 비가시성과 재생성을 구분한다. offscreen의 hide/pause/detach/dispose는 policy로 결정한다.
- native 생성 완료, navigation 완료, 첫 content frame 표시를 구분한다. 공통 `ready`를 실제 픽셀 표시의 증거로 사용하지 않는다.
- remove 시 새 scene 참조와 hit testing/focus를 끊고, 이전 plan/compositor/GPU가 사용하는 자원은 retirement 이후 회수한다.
- owner window close, 부분 생성 실패, 반복 dispose에서도 instance와 callback을 모두 정리한다.

### 5.3 프레임 합성과 스레드

scene에서 동일한 배치 스냅샷을 뽑는다. platform UI thread가 native 생성·배치·focus를 실행하고 Graphite context/recorder는 기존 render owner가 유지한다.

1. retained scene까지 순회하며 transform·clip·opacity·paint order를 수집한다.
2. raster/native 구간으로 composition plan을 만든다. shadow/filter bounds도 겹침 판단에 포함한다.
3. native view/backend 능력으로 plan을 검증하고 미지원 효과는 실제 렌더 전에 원인을 보고한다.
4. raster surface와 해당 frame/epoch의 native placement를 commit한다. 오래된 placement가 새 화면에 적용되지 않게 한다.
5. GPU 완료, native placement 적용, compositor 표시 관측을 구분한다. 제출 성공만으로 전체 화면을 `Presented`로 기록하지 않는다.

모든 OS에서 WebView 내부 스크롤/영상 frame과 Doroti frame을 원자적으로 동기화할 수 있다고 약속하지 않는다. 초기 보장은 native view의 **위치·크기·순서와 Doroti 화면의 정합성**이다. 웹 콘텐츠 내부 rendering clock은 독립적이다. 원자 transaction이 없는 backend는 지원 수준을 표시하고 실제 오차를 측정한다.

native view가 없는 scene은 기존 단일 surface 경로를 유지한다. overlay는 겹침 구간 기준으로 합치고 pool·수·메모리 상한을 둔다. `SaveLayer`, group opacity, backdrop filter는 단순 canvas 분할만으로 의미가 보존되지 않을 수 있으므로 별도 처리 또는 명시적 미지원으로 둔다.

### 5.4 입력·IME·접근성

| 항목 | 필요한 계약 |
|---|---|
| Pointer | native 직접 수신과 Doroti 중재 후 전달을 구분. 같은 event를 이중 전달하지 않음 |
| Gesture | 기존 arena와 부모 scroll 경쟁을 정의. 중재형은 승패까지 buffer하고 패자 cancel. cross-origin iframe에 동일 중재를 약속하지 않음 |
| Hit test | 실제 합성 좌표/clip 사용. hidden view와 modal 뒤 view의 입력 차단 |
| PointerInterceptor | 전경 위젯과 같은 epoch/clip의 shield, intercepting on/off, debug 영역, listener 해제, 중복 event 방지 |
| Mouse/pen | hover, leave, wheel, capture, drag-outside, pen/touch 및 transform 역변환/DPR 처리 |
| Focus | Doroti node와 native focus 왕복, Tab/Shift+Tab, activation, modal 종료 후 복귀 |
| IME | native WebView가 focus를 가지면 native IME 사용. Doroti hidden editor/textarea의 동시 활성화 방지 |
| Semantics | platformViewId와 native subtree를 연결. 내부 문서를 중복 생성하지 않고 읽기 순서·focus 경계 검증 |
| Capture | raster capture에서 native content가 빠질 수 있음을 결과에 명시. 전체 화면 capture는 별도 구현/검증 |

## 6. 합성 지원 수준

`supportsPlatformView=true` 하나보다 **view 종류 × backend × OS/runtime × 효과**로 조회한다. 요청한 화면을 만들 수 없으면 구체적인 이유를 돌려주고 texture/snapshot/다른 renderer로 조용히 전환하지 않는다.

| 수준 | 지원 범위 | 용도 |
|---|---|---|
| `NativeOverlay` | native 사각 영역 배치. Doroti 전경과 교차 겹침 미지원 | 첫 실험, 별도 패널/전체 페이지 |
| `InterleavedComposition` | Doroti raster → native → Doroti raster 순서 | 메뉴·다이얼로그·겹치는 UI의 제품 목표 |
| `ExternalTexture` | texture를 제공할 수 있는 producer만 Skia 합성 | 카메라/영상 등 후속 기능 |
| `Snapshot` | 정지 이미지, live input 중단 | 명시적 전환 효과/프리뷰 |

세부 flag는 rect/rounded clip, affine/perspective transform, opacity, backdrop sampling, accessibility, gesture mediation, capture inclusion, synchronized placement로 나눈다. 초기 baseline은 이동·resize·DPR·rect clip·hide/show이며 rounded clip/opacity는 backend 검증 후 광고한다.

제한형 overlay에서 modal을 쓰려면 WebView hide/detach 후 복귀 같은 명시적 제한 모드를 둘 수 있다. 이는 반투명 modal 뒤의 live WebView가 보이는 디자인과 결과가 다르다. 일반 Stack 호환은 interleaved 게이트가 통과해야 선언한다.

## 7. 플랫폼별 구현 후보

### Windows

현재 root visual에 붙은 Doroti surface를 container의 child로 옮기고 배경/native/전경 visual을 순서대로 두는 안을 먼저 실험한다. WebView2 `ICoreWebView2CompositionController`는 `RootVisualTarget`으로 DirectComposition visual을 연결하고 mouse/pointer 전달 API를 제공한다. bounds·focus·cursor·capture 등은 host 책임이다. generic HWND와 WebView2 composition factory는 서로 다른 지원 수준을 보고해야 한다. [Microsoft composition controller](https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/win32/icorewebview2compositioncontroller?view=webview2-1.0.3537.50)

첫 PoC는 기존 Vulkan/Windows Presentation과 동일 top-level window에서 결합해 foreground alpha·resize/DPI·입력·present retirement를 확인한다. 현재 topmost target이 child HWND를 가릴 수 있으므로 단순 child WebView조차 성공을 전제하지 않는다. composition device/thread 소유권과 native C ABI 확장은 이 단계에서 결정한다. WebView 픽셀을 texture로 캡처하는 경로가 아니다.

로컬 inappwebview Windows의 [custom platform view](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/custom_platform_view/custom_platform_view.cc)와 [GPU texture bridge](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/custom_platform_view/texture_bridge_gpu.cc)는 Flutter 쪽 출력 운송의 참고다. WebView2 composition controller를 사용한다는 공통점만으로 Doroti의 직접 DComp 합성과 같은 출력 경로라고 간주하지 않는다.

WebView2는 message pump가 있는 STA UI thread에서 생성·호출해야 한다. `.Result`나 동기 wait로 UI thread를 막지 않는다. managed wrapper와 native C++ COM adapter 중 기존 ABI/AOT 배포에 맞는 것을 spike에서 정한다. runtime 존재·버전·초기화 실패와 Windows MAUI adapter는 별도 검증한다. [WebView2 threading](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/threading-model)

### Android

`FrameLayout` 등 container에 Doroti SurfaceView와 `android.webkit.WebView`를 배치하는 제한형 PoC부터 한다. 표시가 되어도 단일 Doroti SurfaceView의 일부 그림만 WebView 위에 올릴 수는 없으므로 foreground surface는 별도 작업이다.

native focus/IME/accessibility를 사용하고 clip·rotation·keyboard inset·Activity background/foreground·Surface 재생성과 WebView 수명의 분리를 검증한다. 내부 영상처럼 추가 surface를 만드는 콘텐츠도 시험한다. 이후 SurfaceControl transaction을 연구하되 HCPP 조건 때문에 Doroti 최소 OS를 임의로 올리거나 Impeller를 Graphite에 직접 연결하지 않는다. texture 방식은 import/fence/SurfaceView 자식 처리 비용을 측정한 뒤 선택한다.

### iOS / Mac Catalyst / native macOS

UIKit은 WKWebView와 Doroti Metal view를 공통 container에 배치하고 AppKit은 NSView/responder chain adapter를 둔다. controller의 공통 기능은 공유해도 native view 코드는 분리한다. 전경 그림에는 별도 Metal surface와 ordering이 필요하다. [Apple WKWebView](https://developer.apple.com/documentation/webkit/wkwebview)

touch 중재·scroll inset·selection menu·WebView process 종료/복구를 실기기에서 확인한다. 신규 binding/delegate/JS bridge의 NativeAOT는 실제 publish/ILC/install/run으로 검증한다. 기존 host probe 성공과는 별개다. MAUI `HybridWebView`에 일괄 의존하는 안은 이 검증 전에는 채택하지 않는다.

### Web

main DOM에 instance registry와 안정된 container를 만들고 element 수명을 보존한다. worker에는 ID와 frame별 placement만 보낸다. iframe을 WebGPU texture로 가져오거나 DOM object를 worker로 넘기는 것을 기본 경로로 삼지 않는다.

초기는 canvas 위 DOM overlay와 겹침 제한이다. 일반 교차 합성에는 여러 canvas/DOM slot과 raster segment 출력이 필요하다. 현재 [worker protocol v4](Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts)를 확장하면 loader/main/worker 버전을 함께 갱신하고 stale packet을 거절한다.

WebView DOM/container 수명은 로컬 inappwebview Web 구현을 기준으로 하고, 전경 메뉴·버튼·modal 영역의 입력은 3.7절의 PointerInterceptor로 보호한다. 필요한 시각적 foreground 합성이 준비된 뒤 `iframe → input shield → Doroti 전경` 순서를 맞춘다. 영역 밖에서는 iframe이 정상 입력을 받고 영역 안에서는 Doroti 위젯만 반응해야 한다.

iframe backend는 native WebView와 동등하지 않다.

- 사이트의 frame-ancestors/X-Frame-Options로 embedding이 거절될 수 있다. [CSP frame-ancestors](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Security-Policy/frame-ancestors)
- cross-origin iframe의 DOM/history/cookies/임의 JS 실행은 host가 일반적으로 제어할 수 없다. 협력 페이지에는 origin/source를 검증하는 postMessage 계약을 사용할 수 있다. [iframe](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/iframe), [postMessage](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage)
- 현재 WebGPU의 cross-origin isolation 요구와 iframe 서버의 COEP 등을 함께 검증해야 한다. iframe을 위해 기존 renderer를 몰래 바꾸지 않는다. [crossOriginIsolated](https://developer.mozilla.org/en-US/docs/Web/API/Window/crossOriginIsolated)

Web backend에는 EmbeddedWebContent 성격을 명시하고 LoadUri 같은 최소 기능만 공통화한다. JS/cookie/navigation interception은 capability로 분리한다. embedding 불가 사이트는 기존 [URL launcher](Doroti/src/Doroti.Ui/UrlLauncher.cs)를 사용하는 별도 앱 선택지로 둔다.

### Linux: Qt WebView 채택

**Linux backend는 Qt의 WebView를 사용하는 것으로 정한다.** 로컬 inappwebview의 WPE WebKit backend를 가져오는 대신 공통 API를 Qt adapter로 연결한다. CEF/Wry/WPE를 병행 후보나 자동 fallback으로 두지 않는다.

정확한 모듈 관계도 구분한다. Qt WebView는 Linux에서 Qt WebEngine에 의존하므로 WebView wrapper를 선택해도 WebEngine runtime 배포가 필요하다. 공식 문서는 `QtWebView::initialize()`를 application/context 생성 전에 호출하도록 안내한다. 현재 native host의 `QApplication` 생성보다 앞에 초기화 접점을 둬야 한다. [Qt WebView 문서](https://doc.qt.io/qt-6/qtwebview-index.html)

| Qt API 접점 | 판단 |
|---|---|
| Qt WebView `QWebView` | Qt 6.11부터 제공되는 QWindow 기반 C++ API. 현재 Doroti QWindow 구조와 결합하는 우선 실험 경로 |
| Qt WebView QML API | QML/Quick host가 필요한 경우의 Qt 내부 선택지. Doroti에 QML shell을 새로 도입하는 비용을 먼저 확인 |
| Qt WebEngine `QWebEngineView`/`QWebEnginePage` | rich JS/message/profile 기능 때문에 직접 API가 필요하면 사용하는 Qt 내부 상세 구현안. QWebView의 private backend에 임의 접근하지 않음 |

`QWebView`를 선택하려면 Qt 6.11 이상이 필요하지만 현재 [CMake](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/CMakeLists.txt)의 최소값은 6.5다. 실제 설치 버전·최소 지원 버전은 착수 시 확인한다. 또한 QWebView 공개 API만으로 inappwebview의 JS bridge·세밀한 profile 기능을 모두 구현할 수 있다고 전제하지 않는다. QWebView는 Qt Resource 로딩 제약도 있으므로 앱 asset mapping을 별도 검증한다. 이 API/버전 선택은 구현 전에 확정할 사항이며 이번에는 빌드 설정을 변경하지 않는다. [QWebView API](https://doc.qt.io/qt-6/qwebview.html), [QWebEngineView API](https://doc.qt.io/qt-6/qwebengineview.html)

공통으로 window parent·geometry·focus·clip·overlay/input shield와 X11/Wayland를 검증한다. QWidget shell/`createWindowContainer()`를 쓰는 상세 구현에서는 opaque native window stacking 제약이 있으므로 비겹침 표시 성공을 일반 overlay 성공으로 기록하지 않는다. Qt WebEngine process/resources, JS messaging, local content, profile 및 종료 수명까지 Qt backend의 완료 조건에 넣는다. [QWidget createWindowContainer](https://doc.qt.io/qt-6/qwidget.html#createWindowContainer)

## 8. WebView 공통 API 범위

controller는 widget rebuild와 독립적으로 소유하고 widget은 해당 native view를 표시한다. 생성은 명시적 async factory 또는 readiness task로 노출한다. 명령은 native readiness 이후 순서대로 실행하고 navigate/dispose와 경쟁하는 오래된 결과를 버린다.

기능 이름과 이벤트 의미는 inappwebview의 controller/settings/creation params를 기준으로 대응표를 만든다. URLRequest·navigation callbacks·JS handler·user scripts·environment·keep-alive를 검토하되 Doroti에서는 typed async 계약과 실제 플랫폼 지원 범위로 정리한다. keep-alive는 widget detach 후 native instance 유지이고 headless는 표시 없이 독립 수명을 갖는 별도 기능이다. 두 기능을 숨김 상태 하나로 합치지 않는다.

| 기능 | 초기 계약 제안 |
|---|---|
| 탐색 | LoadUri/LoadHtml(base URI 정책 포함), reload/stop, back/forward와 가능 여부 |
| 상태 | URI/title/loading/progress, navigation ID, main-frame/subresource error 구분 |
| 정책 | allow/cancel/external, redirect/new window 구분. 동기 결정이 필요한 backend는 사전 policy 사용, UI blocking 금지 |
| JS | 명시적 enable, async 실행/직렬화 가능한 결과. iframe 미지원은 capability 오류 |
| 메시지 | versioned envelope, origin/frame/source 검증, 요청 ID, timeout/dispose 취소 |
| 세션 | profile별 cookies/storage/cache, persistent/ephemeral 지원 조회. dispose와 데이터 삭제 분리 |
| 확장 | download, file chooser, permission, media autoplay, fullscreen, recovery, devtools의 typed backend option |

최소 공통 API와 native-only API를 나눈다. 함수명만 공통화한 뒤 불가능한 동작을 무시하지 않는다. 모든 OS 설정을 거대한 shared interface에 넣기보다 플랫폼 옵션과 feature query를 둔다.

앱 내 HTML/JS/CSS에는 `LoadAppContentAsync(resourceKey)`를 제안하고 기존 resource manifest와 연결한다.

| Backend | 로컬 콘텐츠 후보 |
|---|---|
| Windows | WebView2 virtual-host mapping 또는 resource interception |
| Android | AndroidX WebViewAssetLoader |
| Apple | WKURLSchemeHandler 또는 범위가 제한된 file/base URL 정책을 요구 웹 API와 함께 검증 |
| Linux Qt | 선택한 Qt WebView API의 asset 제약 확인; WebEngine 직접 adapter라면 URL scheme handler로 연결 |
| Web | 앱이 서비스하는 same-origin asset URL |

WebView2 mapping은 로컬 파일에 HTTP(S) origin을 제공할 수 있다. Android 문서도 WebViewAssetLoader/적절한 base URL을 권한다. 모든 backend에 loopback HTTP 서버를 띄우기보다 기본 플랫폼 기능부터 검토하고 relative URL/fetch/미디어 Range·seek를 시험한다. [WebView2 local content](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/working-with-local-content), [Android local content](https://developer.android.com/develop/ui/views/layout/webapps/load-local-content)

JS bridge는 허용된 origin/frame에만 활성화하고 native 객체·reflection을 공개하지 않는 명시적 message API로 구성한다. navigation 뒤 이전 페이지의 응답은 navigation ID로 구분한다. 인증서 오류 무시·임의 파일 접근·모든 권한 자동 승인은 기본값에 넣지 않는다.

## 9. 구현 순서와 완료 조건

아래는 착수 시 사용할 후보 단계다. 문서 작성만으로 어느 단계도 통과한 것으로 표시하지 않는다.

| 단계 | 작업 | 완료 판단 |
|---|---|---|
| P0 | inappwebview 기능 대응표와 pointer_interceptor 의미 정리, payload/capability/수명 정의 | 미지원 오류, stale create/dispose, 두 owner view 격리 검증 |
| P1 Windows | native button/WebView2 생성, 기존 DComp/Presentation에 배경/native/전경 결합 | 실제 입력·전경 메뉴·alpha·resize/DPI·retirement 확인. 실패하면 compositor 결정을 먼저 수정 |
| P2 Web | inappwebview DOM 수명·iframe·worker epoch, PointerInterceptor shield 연결 | 보호 안/밖 입력, intercepting on/off, focus·dispose, 두 renderer의 브라우저 검증 |
| P3 WebView | inappwebview 기준 controller/widget, URI/asset/JS message/navigation/profile | 대응표별 지원/미지원 확인, Web은 선언한 범위만 동작 |
| P4 Android/Apple | native hierarchy, focus/IME/accessibility/lifecycle, overlay | 기기별 입력·회전·재진입·dispose 증거. Apple AOT는 publish/ILC/install/run 별도 |
| P5 Qt | Qt WebView API/최소 버전 확정, 초기화·host 결합·기능 매핑·WebEngine 배포 | X11/Wayland, stacking/input shield, JS/profile 지원표, runtime resources 확인 |
| P6 고급 합성 | overlay 최적화, Android transactions, 필요한 producer의 texture | 정합성·성능·GPU 수명 통과 후 해당 capability 승격 |

P1의 제한형 overlay 성공을 interleaved 성공으로 기록하지 않는다. P2에서도 단순 DOM 배치와 다중 canvas 합성을 별도 게이트로 둔다.

`PlatformViewLab`에 넣을 대표 시나리오:

1. native button/WebView 두 종류, 여러 instance, 두 owner window.
2. 부모 scroll, 빠른 resize, DPR 1/1.25/1.5/2, route push/pop 반복.
3. WebView 위 메뉴/tooltip/modal barrier, 두 native view 사이 Doroti 위젯, 반투명 중첩.
4. rect/rounded clip, opacity, transform 및 미지원 효과 오류.
5. 한글 IME/selection/Tab, Doroti TextField로 focus 복귀, touch와 부모 scroll 경쟁.
6. 로컬 HTML/JS/CSS, relative URL/fetch, 영상 seek, 탐색 거부, late JS response, process 종료.
7. create 중 dispose, close 중 callback, surface/device 재생성, background/foreground, offstage 복귀.
8. accessibility 읽기 순서·가림과 raster capture/실제 화면 capture 차이.
9. PointerInterceptor on/off/debug, scroll/clip 이후 보호 영역 정합성, 메뉴 닫은 뒤 iframe 입력 복귀, wheel/drag/capture, focus 보존, 반복 생성 시 listener/native shield 누수.

native view 0/1/여러 개에서 baseline 대비 frame p50/p95/p99, input latency, overlay 수, GPU/native memory, 반복 복귀 후 잔존 instance를 측정한다. 절대 목표값은 대상 기기와 기존 Doroti 기준으로 정하며 이번 조사에서 수치를 만들어내지 않는다. 테스트를 실행할 때는 저장소 지침에 따라 **외부 20분 timeout**을 적용한다. 소스·빌드·자동화·실기기·물리적 입력 확인은 독립 결과로 남긴다.

## 10. 채택 전 남은 결정

- Windows 다중 visual root와 투명 foreground surface가 기존 present/retirement 계약을 유지하는가?
- Web에서 여러 canvas를 기존 worker 소유권 안에서 어떻게 pool/commit하고 DOM placement와 맞출 것인가?
- 초기 제품이 비겹침 WebView 패널만 요구하는가, 반투명 dialog/메뉴까지 요구하는가? 설계는 후자의 확장점을 확보하되 게이트를 분리한다.
- Android 우선 대상에서 native hierarchy의 scroll 정합성은 어느 수준인가?
- generic native view의 focus/accessibility를 backend별로 어디까지 공통화할 수 있는가?
- WebView profile 격리와 persistent data 삭제 범위는 어디까지 공개할 것인가?
- Linux는 Qt로 확정하되 QWebView/Qt 버전과 필요한 WebEngine 직접 API의 범위를 어떻게 정할 것인가?

설계 기준은 **Flutter engine의 PlatformView 합성 + flutter_inappwebview의 WebView 기능/수명 + pointer_interceptor의 전경 입력 보호**다. Linux는 **Qt WebView 계열 backend**로 구성한다. 이를 typed host 기반과 선택형 WebView 패키지로 연결하고, Windows 겹침 합성과 Web DOM/worker·입력 보호를 먼저 검증한다.

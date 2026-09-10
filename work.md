# MediaQuery · SafeArea 전체 플랫폼 정비 계획

작성일: 2026-09-10. 상태: **소스/공식 문서 검토 및 계획 작성 완료, 구현 미착수**.

확정 설계: **MAUI는 시스템 UI/키보드를 미리 피하지 않고 가능한 전체 native content 영역을 점유한다. MAUI는 raw geometry와 edge/occlusion 정보를 공급하고, safe area 소비와 keyboard 회피는 Doroti가 담당한다.** 사용자 후속 지시를 반영한 필수 조건이다.

## 1. 목표와 범위

Doroti 앱이 `MediaQuery`와 `SafeArea`만으로 화면 크기, 배율, 시스템 UI, 키보드, 접근성 설정 변화에 대응하도록 플랫폼 수집부터 위젯 소비까지 연결한다. `SliverSafeArea`, `Scaffold`, Material/Cupertino overlay의 inset 소비도 같은 계약으로 검증한다.

이번 요청에서는 이 문서만 작성한다. 런타임·샘플·네이티브 코드 변경이나 빌드/실기기 테스트는 수행하지 않았다. 이후 “work.md 전체작업”의 범위는 MQ-0~MQ-9이며, 모바일 inset 연결만으로 전체 작업을 완료 처리하지 않는다.

대상은 현재 저장소의 제품 호스트/타깃이다.

| 플랫폼 | 구현 경로 | 검증 구분 |
|---|---|---|
| Android | `Doroti.Host.Maui`, Android MAUI arm64/x64 타깃 | API 24 이상 호환 경로, API 30 이상 typed insets, API 34 이상 글자 확대, 실제 지원 target SDK의 edge-to-edge |
| iOS/iPadOS | `Doroti.Host.Maui`, ios-arm64 및 simulator arm64/x64 | iPhone, iPad, simulator와 실기기 구분 |
| macOS AppKit | `Doroti.Host.Maui`의 `MACOS`, `Doroti.Target.MacOS.Maui.osx-arm64` | 네이티브 AppKit/Metal 경로 |
| Mac Catalyst | `Doroti.Host.Maui`의 `MACCATALYST`, maccatalyst-arm64 | UIKit 기반 macOS 경로; AppKit 결과로 대체하지 않음 |
| Windows | `Doroti.Host.WindowsAppSdk` + Native, `Doroti.Host.Maui` | 두 제품 호스트 각각 검증 |
| Linux | `Doroti.Host.Qt`, linux-x64 | Wayland/X11, Qt 최소 버전과 최신 기능 차이 |
| Web | `Doroti.Host.Web` | 전체 페이지/embedded root, desktop/mobile browser, worker-direct-webgpu/webgl |

열거형에만 존재하는 Fuchsia나 새로운 OS/아키텍처 포트 신설은 범위 밖이다. 렌더러 교체, 일반 IME/AT 재구현도 범위 밖이나, metrics 변화로 인한 입력·focus·semantics 좌표 회귀 수정은 포함한다.

## 2. 검토 근거와 현재 문제

### 2.1 Flutter 기준

로컬 `reference/flutter-master`의 HEAD를 직접 확인했다.

`56b8e1a851a594b1a154f8ea93270807dab22b9a`

Doroti의 reviewed framework source 헤더도 이 revision을 가리킨다. 최신 Flutter라는 의미가 아니며, 구현 기준은 이 pin으로 유지한다. 공식 OS/browser 문서는 플랫폼 API 선택과 지원 범위를 판단하는 보조 근거다.

직접 확인한 주요 소스:

| Flutter 파일 (`reference/flutter-master/` 기준) | 확인한 계약 |
|---|---|
| `packages/flutter/lib/src/widgets/media_query.dart` | `fromView`, parent platformData, 세 종류 remove 연산, aspect 의존성, 값 비교, observer 수명, `SystemTextScaler` |
| `packages/flutter/lib/src/widgets/safe_area.dart` | 각 변의 `max(enabled ? padding : 0, minimum)`, 자식의 consumed padding 제거, `maintainBottomViewPadding`, SliverPadding |
| `packages/flutter/test/widgets/media_query_test.dart` | fromView/parent override/notification, copy/remove, property별 의존성 테스트 항목 |
| `packages/flutter/test/widgets/safe_area_test.dart` | basic/minimum/nested/changing, 부분 keyboard inset, zero area, SliverSafeArea |
| `engine/src/flutter/lib/ui/window.dart`, `hooks.dart` | physical metrics, `padding = max(0, viewPadding - viewInsets)`, display feature를 logical 좌표로 변환하는 경계 |
| `engine/src/flutter/lib/ui/platform_dispatcher.dart` | 플랫폼 설정, system font scaling의 native 조회·보간/캐시 경로 |
| `engine/src/flutter/shell/platform/android/io/flutter/embedding/android/FlutterView.java` | system bars/IME/system gestures 분리, cutout/waterfall, 구버전 fallback, folding feature, fontScale |
| `engine/src/flutter/shell/platform/darwin/ios/framework/Source/FlutterViewController.mm`, `KeyboardInsetManager.swift` | safeAreaInsets × scale, keyboard frame/state/animation 경로, docked/floating 구분 |
| `engine/src/flutter/lib/web_ui/lib/src/engine/view_embedder/dimensions_provider/full_page_dimensions_provider.dart` | viewport 기반 크기, iOS 분기, 편집 상태를 반영한 keyboard inset |
| 같은 디렉터리의 `custom_element_dimensions_provider.dart` | element size와 DPR 관찰; 해당 pin의 embedded keyboard inset은 0 |

Flutter pin에 있는 제한까지 구분한다. 예를 들어 embedded Web keyboard 지원을 추가하면 Doroti의 의도적 확장이다. 또한 pin의 `MediaQueryData` equality는 `textScaleFactor`를 비교하므로, 이를 조사 없이 C# 번역 오류라고 분류하지 않는다.

### 2.2 Doroti 소스에서 확인한 사실

| 위치 | 현재 상태 | 영향/조치 |
|---|---|---|
| `Doroti/src/Doroti.Framework.Widgets/safe_area.cs` | SafeArea/SliverSafeArea와 minimum/nested consumption/maintainBottomViewPadding 기본 구조 존재 | 위젯을 처음부터 재작성할 문제로 보지 않고 동작 계약부터 고정 |
| `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`, `DorotiView.padding` | `metrics.viewPadding`을 그대로 반환 | keyboard가 padding을 덮어도 padding이 감소하지 않는 확정된 계약 차이 |
| `Doroti/src/Doroti.Ui/ViewContracts.cs` | `ViewMetrics`에 size/DPR/세 inset이 존재 | 새로운 평행 metrics 시스템 대신 기존 계약 확장 |
| `PlatformDispatcher.cs`, `DorotiView` | displayFeatures/displayCornerRadii/gestureSettings가 metrics와 별도 setter | 동시 갱신·변경 알림·복사본 안정성 계약 필요 |
| `Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs`, `Metrics` | 세 inset을 항상 zero로 생성 | Android/iOS/Catalyst/AppKit/Windows MAUI 모두 실제 occlusion 공급 누락 |
| `Doroti/src/Doroti.Host.Maui/DorotiMauiApplication.cs`, `DorotiMauiSurface.cs` | ContentPage 안의 Grid이며 SafeAreaEdges 정책을 명시하지 않음 | MAUI 자동 소비와 Doroti 소비 중복 여부를 native 실제 bounds로 확인해야 함 |
| `Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedProductHost.cs`, `ApplyMetrics` | inset zero; native Metrics 구조체에도 inset 필드 없음 | managed 변경만으로 완성 불가; native 이벤트/ABI 전달 필요 |
| `Doroti/src/Doroti.Host.Qt/QtHostAdapter.cs`, `ApplyMetrics`/`BeginFrame` | inset zero, frame 경로도 zero로 재생성 | native 공급 및 frame 경로에서 기존 inset을 덮어쓰지 않는 처리 필요 |
| `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs` | BrowserHostSnapshot에 inset 없음, ToMetrics는 zero | DOM → snapshot → worker/managed 전체 전송 경로 확장 필요 |
| `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`, `observeFullPageViewport` | root layout box를 size 기준으로 사용, visualViewport는 변경 신호로 사용 | pinch zoom 회귀 방지 의도를 보존하면서 별도 occlusion 계산 추가 |
| `PlatformDispatcher.cs`, 설정 getter/`scaleFontSize` | 설정이 implicitView에 의존; 여러 view이면 implicitView가 null; scaling은 선형 곱 | multi-view 기본값 회귀와 native non-linear font scaling 해결 필요 |
| 같은 파일의 `DispatchPlatformConfiguration` | 설정 변경 시 text scale/brightness callback을 모두 호출 | locale/24h/접근성 등을 포함한 실제 diff와 알림 계약 정리 |
| `MauiHostAdapter.Configuration`, Web `ToConfiguration`/`ApplySnapshot` | 일부 theme/locale 경로는 존재하나 나머지 MediaQuery 관련 값·실시간 수집이 제한적 | 지원 가능한 필드와 근거 있는 fallback을 플랫폼별로 명시 |
| `Doroti/src/Doroti.Framework.Material/ScaffoldSlotMediaQuery.cs` | slot별 inset 변환 및 size-only 최적화 존재 | 이를 보존하며 실제 host metrics로 회귀 검증 |
| `Doroti/validation/fcr7-material-widget/ScaffoldMetricsContracts.cs` | 직접 주입한 MediaQueryData로 keyboard/padding 소비·복구 테스트 존재 | OS → host → fromView가 연결되었다는 증거는 아님 |
| `Doroti/validation/framework-work/Program.cs` | size/DPR/inset/접근성 aspect 체크 존재 | mounted tree, host event, 수명 및 전체 field coverage 확장 |

따라서 우선순위는 **native/DOM 수집 → view 좌표·단위 정규화 → 일관된 snapshot/알림 → MediaQuery 변환 → SafeArea/Scaffold 소비**다. 앱에서 status bar 높이/키보드 높이를 하드코딩하는 방식은 사용하지 않는다.

## 3. 공통 설계

### 3.1 값과 좌표의 책임

1. `physicalSize`는 해당 Doroti 렌더 view의 drawable 크기다. 모니터 전체 크기나 키보드 차감 후 가용 크기를 임의로 섞지 않는다. `MediaQuery.size = physicalSize / DPR`이며 실제 부모 constraints는 별개다.
2. host가 수집한 값은 **현재 view 원점 기준 physical pixels**로 정규화한다. Android px는 변환 없이, UIKit/AppKit point·WinUI DIP·Qt logical unit·Web CSS px는 해당 view의 scale로 변환한다. 주 모니터/전역 display scale 대신 실제 창/화면 값을 사용한다.
3. `viewPadding`은 system UI/cutout 때문에 안전하게 배치할 수 없는 변 영역, `viewInsets`는 IME 등 완전히 가리는 변 영역, `systemGestureInsets`는 OS gesture와 충돌하는 영역이다. keyboard가 올라와도 viewPadding을 keyboard 높이로 바꾸지 않는다. immersive/system bar 표시 상태에 따른 변화는 반영한다.
4. `DorotiView.padding`은 각 변에서 `max(0, viewPadding - viewInsets)`로 도출한다. host가 padding을 별도로 작성하여 서로 모순되는 값을 만들지 않는다.
5. `MediaQueryData.CreateFromView`에서 inset을 DPR로 한 번만 나눈다. public `DorotiView.displayFeatures`는 Flutter처럼 logical bounds, corner radii는 physical 값이며 MediaQuery 변환 시 DPR 적용이다. host의 raw feature bounds가 physical이면 view 경계에서 한 번만 변환한다.
6. native window에 있는 가림 사각형을 view로 변환·교차시킨다. 이미 부모가 제외한 시스템 UI나 view 밖 키보드 영역을 다시 inset으로 보고하지 않는다. floating/split keyboard를 무조건 전체 폭 bottom inset으로 바꾸지 않으며, 4변 inset으로 표현할 수 없는 내부 가림은 별도 내부 관측 정보와 명시적 제한으로 남긴다.
7. DPR은 finite > 0, inset은 finite/non-negative인지 검증한다. Web zoom-out 등 DPR < 1도 누락하지 않는다. 초기 zero-size/minimized/detached는 측정 불가 상태로 다루고, 마지막 값을 유지할지 새 zero metrics를 발행할지는 renderer의 zero-surface 계약과 맞춘다. 임의 크기나 높이를 만들어 정상값처럼 보고하지 않는다.

### 3.2 Snapshot과 변경 전달

기존 `ViewMetrics`, `PlatformConfiguration`, `IViewHostCapability`를 중심으로 확장한다. 필요하면 내부 `ViewEnvironmentSnapshot`/provider를 두되 플랫폼 SDK 객체를 framework에 노출하지 않는다. 명칭과 구체 타입은 MQ-1에서 확정한다.

- metrics snapshot에 inset/feature/gesture/corner 정보를 일관되게 담고 리스트는 불변 복사본으로 유지한다. `CreateFromView` 중 서로 다른 세대의 크기와 inset을 읽지 않도록 snapshot을 한 번 캡처하는 경계를 둔다.
- **metrics generation, resize epoch, surface generation을 구분한다.** 키보드나 접근성 변화만으로 GPU surface 재생성·context loss를 일으키지 않는다. 기존 exact resize admission과 `DorotiViewEpoch`/frame transaction 의미를 유지한다.
- 순서는 수집 → 검증/정규화 → snapshot 교체 → dispatcher notification → 필요한 frame 예약이다. 크기가 그대로여도 inset 변화는 전달하며, 동일 값 재수신은 불필요한 rebuild를 만들지 않는다.
- 한 frame 안의 연속 native 이벤트는 최종 일관된 snapshot으로 모을 수 있다. keyboard animation은 frame마다 필요한 진행 값을 전달하고 마지막 값·취소·즉시 hide를 유실하지 않는다. 고정 polling이나 프레임마다 DOM 강제 측정은 사용하지 않는다.
- UI thread/worker 간 순서와 generation으로 오래된 이벤트를 거부한다. detach/reattach/window 변경/종료 때 observer를 해제하고 초기 snapshot을 재수집한다.
- geometry는 view별 소유, 플랫폼 설정은 dispatcher의 명시적 snapshot 및 host environment scope로 관리한다. 여러 view 등록 때문에 설정이 기본값으로 돌아가지 않도록 한다. framework 알림 API가 전역이면 각 fromView가 자신의 snapshot을 비교하도록 한다.
- text scale·brightness·accessibility·locale/24h 변경을 구분한다. 기존 public callback 및 WidgetsBinding observer 계약을 유지하면서 값 변경에 필요한 경로를 보완한다. 항상 모든 callback을 호출하는 우회에 의존하지 않는다.

### 3.3 MediaQuery 전체 필드 정책

| 묶음 | 대상 | 구현/검증 기준 |
|---|---|---|
| 기하 | size, width, height, orientation, DPR, padding/viewPadding/viewInsets/systemGestureInsets | 모든 host의 실제 geometry, orientation은 size에서 도출, 단위 변환/동시 갱신 |
| 텍스트 | textScaler 및 호환 textScaleFactor | 플랫폼 native scaling, Android non-linear 포함; font scale을 DPR/page zoom과 중복 적용하지 않음 |
| 화면/사용자 설정 | platformBrightness, alwaysUse24HourFormat | 초기값+실시간 변경, 수동 theme/parent MediaQuery override 유지 |
| 접근성 | accessibleNavigation, invertColors, highContrast, onOffSwitchLabels, disableAnimations, reduceMotion, boldText, supportsAnnounce | 각 OS가 실제 제공하는 신호에만 매핑; semantics tree 활성화와 AT 사용 여부를 혼동하지 않음 |
| 입력/장치 | navigationMode, gestureSettings, supportsShowingSystemContextMenu | Flutter pin의 플랫폼 정책과 Doroti 실제 서비스 지원 여부를 함께 근거로 결정 |
| 화면 특수 형상 | displayFeatures, displayCornerRadii | native 공급 가능 여부, logical/physical 경계, 회전/fold state 갱신, removeDisplayFeatures/subscreen 변환 |
| 타이포그래피 override | lineHeightScaleFactorOverride, letterSpacingOverride, wordSpacingOverride, paragraphSpacingOverride | parent/app override의 전달·복사·제거/알림 보존; OS API 없는 값은 null |

MQ-0에서 위 **모든 필드 × 모든 host**의 source API, 최소 버전, 초기값, 변경 이벤트, 지원 상태, fallback 이유를 표로 만든다. API 미제공 항목은 public 기본값(`0/false/null/empty/traditional` 등 Flutter 계약에 맞는 값)을 유지하고 내부 진단에는 `unsupported` 사유를 남긴다. 실제 측정 결과 0인 `measuredZero`, 수집 실패/미검증인 `notVerified`를 구별한다. 공급 API가 있는데 연결하지 않은 것은 `unsupported`로 숨기지 않는다.

## 4. 플랫폼별 구성

### 4.1 MAUI 공통

`MauiHostAdapter`가 플랫폼별 metrics/environment provider의 snapshot을 받게 한다. provider는 실제 native render view/window에 attach하며 handler 교체에 대응한다. Graphite/Metal/기존 surface handler 모두 같은 수집 경계를 사용한다.

MAUI의 역할은 **공간 제공과 정보 수집**으로 고정한다. `DorotiMauiApplication`의 ContentPage, `DorotiMauiSurface` Grid, 내부 layout/overlay에 safe-area 자동 소비를 끄는 정책(`SafeAreaEdges=None` 또는 해당 handler의 동등한 설정)을 명시한다. MAUI .NET 10에서 ContentPage는 None, Layout은 Container가 기본이므로 바깥 Page 설정만으로 완료 처리하지 않는다. [MAUI safe area 문서](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/safe-area?view=net-maui-10.0)

native render surface는 가능한 전체 content 영역에 배치하며 system bar/notch/home indicator/IME 때문에 MAUI가 padding을 추가하거나 surface를 축소·이동하지 않게 한다. 여기서 전체 영역은 OS가 해당 창에 제공하는 영역이며, 다른 창이나 사용할 수 없는 non-client 영역을 강제로 점유한다는 뜻은 아니다. standalone에서는 edge-to-edge 영역을 확보하고, 명시적으로 제한된 embedded 부모 안에서는 그 부모가 제공하는 영역을 채운다. Doroti가 소유하는 MAUI 하위 트리는 어느 경우에도 추가 inset 소비를 하지 않는다.

정보 수집은 자동 배치와 분리한다. layout의 safe area 적용을 끄더라도 native WindowInsets/safeAreaInsets/keyboard frame을 계속 수신하고 view-local raw metrics로 전달해야 한다. 단순히 inset 이벤트 자체를 없애거나 모두 consumed/zero로 만들어서는 안 된다.

render view, hidden Entry/Editor, semantics overlay는 동일 원점/변환을 사용한다. hidden Entry/Editor가 유발하는 MAUI의 추가 keyboard scroll, pan, layout 축소를 해제하고 실제 배치는 Doroti Scaffold의 `resizeToAvoidBottomInset`, SafeArea 및 앱 widget 정책으로 결정한다. **MAUI의 선제 회피 해제와 OS window soft-input 설정은 구분한다.** 확인한 Flutter pin의 Android 템플릿(`packages/flutter_tools/templates/app/android.tmpl/app/src/main/AndroidManifest.xml.tmpl`)도 `adjustResize`를 사용한다. 따라서 adjustResize를 무조건 끄거나 overlay 모드를 모든 OS에 강제하는 것을 Flutter parity 조건으로 삼지 않는다. 해당 API/edge-to-edge 구성에서의 실제 native drawable bounds 및 inset 전달을 Flutter와 대조해 MQ-3/MQ-4에서 설정을 확정한다. native viewport 자체가 변경되면 그 크기와 잔여 occlusion을 일관되게 보고하고 MAUI가 추가로 한 번 더 피하지 않게 한다.

필수 검증은 keyboard hidden → shown에서 native drawable bounds와 MAUI render surface bounds를 함께 관찰해 MAUI가 별도 축소·이동을 추가하지 않는지, system bar/cutout 영역까지 surface가 배치되는지, SafeArea를 넣지 않은 Doroti 배경은 해당 영역까지 그려지고 SafeArea를 넣은 자식만 안쪽으로 이동하는지다. native bounds가 유지되는 환경에서는 surface도 유지되고 inset만 변해야 한다. Scaffold의 회피 옵션을 false로 두었을 때 MAUI가 대신 화면을 추가 이동시키면 실패다.

### 4.2 Android

- native render view의 WindowInsets/호환 계층에서 system bars, IME, system gestures, display cutout/waterfall을 분리한다. Android 30 이상 typed API와 최소 API 24까지의 fallback을 각각 구현한다. pin Flutter의 구버전 keyboard 추정은 근거와 한계를 남기며 최신 API에까지 적용하지 않는다. [WindowInsets](https://developer.android.com/reference/android/view/WindowInsets)
- edge-to-edge, immersive, gesture/3-button navigation, cutout 방향, split-screen/freeform, rotation, IME animation을 다룬다. native soft-input 설정은 Flutter의 `adjustResize` 사용과 해당 OS 동작을 기준으로 비교하여 결정하며, MAUI의 추가 resize/pan/여백 소비를 해제한다. drawable/native/window bounds와 IME inset을 함께 기록해 native resize와 중복 차감을 구분한다. WindowInsets는 수집을 유지하며 MAUI 자식의 자동 배치만 막는 소비/전달 정책을 정한다.
- fold/hinge는 WindowManager 계열 feature bounds를 view-local로 변환한다. gesture slop, supported rounded corners는 각 API 버전 조건을 둔다.
- fontScale 변경과 native font-size 변환을 연결한다. Android 14 이상은 단순 fontSize × fontScale과 차이가 있으므로 Flutter의 native 조회/보간 의미와 맞는 capability를 설계한다. [Android 14 font scaling](https://developer.android.com/about/versions/14/features)

### 4.3 iOS/iPadOS와 Mac Catalyst

- 해당 UIView의 safeAreaInsets, layout/safe-area 변경, 해당 UIWindowScene의 scale을 수집한다. 화면 전체 UIScreen 크기를 render view 크기로 사용하지 않는다. [UIView.safeAreaInsets](https://developer.apple.com/documentation/uikit/uiview/safeareainsets)
- keyboard frame notification을 해당 scene/window/view 좌표로 변환한다. docked/floating/split/hardware keyboard, quick-type/accessory bar, interactive dismissal, background 복귀를 구분한다. pin `KeyboardInsetManager.swift`를 세부 구현 근거로 삼고 animation curve/duration과 view lifecycle을 맞춘다. [Keyboard frame notification](https://developer.apple.com/documentation/uikit/uiresponder/keyboardwillchangeframenotification)
- Dynamic Type와 접근성 변경 notification을 수집하되 Flutter pin의 scaling 정책과 대조한다. Reduce Motion과 disableAnimations를 조사 없이 동일 필드로 단순 복사하지 않는다.
- Catalyst는 title bar/toolbar/창 크기 변경·다중 창을 별도로 검증한다. iPhone의 bottom inset 값을 관례적으로 넣지 않는다.

### 4.4 네이티브 macOS AppKit

`DorotiMacOSMetalView`/surface/layout handler의 NSView/NSWindow를 기준으로 safeAreaInsets, content layout, backing scale, fullscreen/notch, screen 변경을 수집한다. AppKit의 flipped 좌표와 Metal drawable 좌표 변환을 명시한다. Dock/menu bar가 이미 content bounds 밖이면 inset은 0이다. [NSView.safeAreaInsets](https://developer.apple.com/documentation/appkit/nsview/safeareainsets)

NSApplication/NSWorkspace 등 환경 설정 notification의 실제 지원값을 표에 기록하고, 일반 데스크톱에서 사용할 수 없는 keyboard/gesture/feature 정보는 근거 있는 fallback으로 둔다. UIKit provider를 그대로 재사용하지 않는다.

### 4.5 Windows 두 호스트

- HWND/client area 또는 MAUI render element의 실제 bounds/DPI를 기준으로 한다. title bar/taskbar/work area를 무조건 safe padding으로 넣지 않는다. content가 확장되어 겹치는 경우에만 실제 겹침을 보고한다.
- touch keyboard occlusion은 해당 top-level HWND의 InputPane interop 및 실제 이벤트 지원을 확인하고 view 좌표로 변환한다. floating keyboard, 자동 content relocation, docking, DPI 변경을 검증한다. 단순 “키보드 열림=true”를 고정 bottom 높이로 바꾸지 않는다. [IInputPaneInterop.GetForWindow](https://learn.microsoft.com/en-us/windows/win32/api/inputpaneinterop/nf-inputpaneinterop-iinputpaneinterop-getforwindow)
- Windows text scale, contrast/animation/theme/locale/24h 설정의 초기값과 변경을 전달한다. native host와 MAUI에서 동일 테스트 벡터를 사용한다.
- `WindowsNativeV1.Metrics`, native 대응 구조체/exports/layout query를 함께 변경한다. ABI version/size/offset 검증 및 제품 패키지/템플릿 호환 정책을 포함한다. inset-only 변경을 resize/surface generation과 강제로 결합하지 않는다.

### 4.6 Linux Qt

- 현재 template은 Qt **6.5** 이상이다. `QWindow.safeAreaMargins`는 **6.9** 이상이므로 version guard 및 구버전 fallback이 필요하다. 이 기능 때문에 최소 Qt 버전을 묵시적으로 올리지 않는다. [QWindow](https://doc.qt.io/qt-6/qwindow.html)
- QInputMethod keyboardRectangle/visible 변경과 native surface의 geometry/DPR, QScreen 변경을 연결한다. keyboardRectangle이 비거나 compositor가 정보를 주지 않는 경우를 명시한다. Wayland/X11 결과를 구분한다. [QInputMethod](https://doc.qt.io/qt-6/qinputmethod.html)
- `QtNativeV2`와 `doroti_qt_host_v2.h`/native producer, 구조체 검증, template/package를 함께 변경한다. `BeginFrame`이 수집된 inset을 zero로 덮어쓰지 않게 한다. theme/font/accessibility 설정은 Qt/desktop 환경의 실제 제공 범위와 Flutter Linux 정책을 비교한다.

### 4.7 Web

- DOM main thread에서 CSS `env(safe-area-inset-*)`를 측정하고 root 좌표로 정규화한다. standalone template의 viewport-fit 정책을 명시한다. embedded root에는 페이지 전체 inset을 그대로 넣지 않고 root와 실제 viewport 가림 영역의 교차를 사용한다. [CSS env](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/env)
- layout viewport/root 크기와 visual viewport를 분리한다. keyboard 판단에 focus/편집 상태, viewport offset/scale, 회전·주소창 변화, resize-content/resize-visual/overlay 동작을 함께 사용한다. `innerHeight - visualViewport.height` 하나로 항상 keyboard를 판정하지 않는다. [VisualViewport](https://developer.mozilla.org/en-US/docs/Web/API/VisualViewport)
- VirtualKeyboard API가 실제 제공되는 환경은 geometrychange/boundingRect를 활용한다. 지원 감지 및 browser fallback이 필요하며 overlaysContent 설정으로 embedding 페이지 전체 동작을 묵시적으로 바꾸지 않는다. [VirtualKeyboard API](https://developer.mozilla.org/en-US/docs/Web/API/VirtualKeyboard_API)
- 기존 root-based size/pinch zoom 방어를 보존한다. DPR < 1 clamp도 geometry contract와 함께 검토한다. mobile Safari/Chromium에서 IME 표시·닫기·회전·pinch/page zoom을 각각 검증한다.
- snapshot/protocol/version/JSON validation/worker forwarding을 함께 확장한다. resize generation이 그대로인 inset-only 메시지도 최종 managed snapshot에 도달해야 한다. WebGPU와 WebGL이 동일 metrics provider를 사용한다.
- matchMedia의 theme/reduced-motion/contrast 등 지원 신호를 동적으로 반영한다. 브라우저가 OS font scale/AT 상태를 노출하지 않는 경우 추정값을 강제하지 않는다. viewport segments 등 기능 감지 가능한 feature만 제공한다.

## 5. 단계별 작업 및 완료 조건

아래 체크는 **향후 구현 완료 여부**다. 이 문서를 작성한 것만으로 체크하지 않는다.

| 단계 | 작업/산출물 | 완료 조건 |
|---|---|---|
| [ ] MQ-0 기준·coverage 고정 | Flutter pin/hash와 관련 framework/engine/test anchor, 전체 field×host 매핑, native dependencies/minimum versions, 현재 실패 사례 기록 | 모든 필드/host에 owner·초기값·변경 이벤트·fallback·검증 방식 지정; Android edge-to-edge/MAUI 소비/Web mode 정책 확정 |
| [ ] MQ-1 공통 view 계약 | `ViewMetrics`/설정 snapshot, derived padding, feature/gesture 전달, 단위/검증, generation 모델 | DPR 1/2/3 및 fractional, inset-only, feature-only, stale event, two-view 설정 테스트 통과; surface 재생성 없음 |
| [ ] MQ-2 framework parity | MediaQuery fromView/copy/remove/equality/aspect/observer, SafeArea/SliverSafeArea, parent overrides | Flutter test 기반 mounted 결과와 rebuild 대상 일치; 기존 Scaffold size-only 최적화 보존 |
| [ ] MQ-3 MAUI 공통·Android | provider attach/detach, MAUI 자동 inset/keyboard 소비 해제, raw surface, WindowInsets/IME/cutout/fold/settings/font scaling | native raw → view metrics → MediaQuery → 실제 layout 일치; keyboard 중 native bounds를 채우며 MAUI 추가 회피 없음; 최소 API fallback과 현대 API, physical Android 확인 |
| [ ] MQ-4 UIKit | iOS/iPadOS/Catalyst provider, keyboard animation/scene, Dynamic Type/접근성 | iPhone/iPad 실기기 및 Catalyst 별도 결과; 중복 소비/회전/복귀/keyboard hide 잔존 없음 |
| [ ] MQ-5 AppKit | native macOS provider와 환경 notification | notch/fullscreen/backing scale/다중 창·모니터 좌표와 실제 UI 검증 |
| [ ] MQ-6 Windows | WindowsAppSdk native ABI 및 MAUI 공급, DPI/input pane/settings | 두 호스트 각각 inset-only/keyboard/DPI/settings 검증, ABI와 패키지 검증 통과 |
| [ ] MQ-7 Linux Qt | guarded native API, ABI/template 공급, settings/event | Qt 6.5 fallback/6.9+ API 및 Wayland/X11 증거; frame 경로에서 inset 보존 |
| [ ] MQ-8 Web | DOM 수집, geometry 분류, protocol/worker 전달, templates/settings | full-page/embedded, 두 renderer, mobile/desktop browser 회귀 통과; 확대/주소창을 keyboard로 오인하지 않음 |
| [ ] MQ-9 통합·제품 검증 | 테스트베드 사례, package consumer/template 확인, 플랫폼 evidence/status 표 및 docs | 아래 완료 기준 전부 충족; 실제 미실행 플랫폼을 성공으로 표시하지 않음 |

의존성은 MQ-0 → MQ-1 → MQ-2이며 MQ-3~MQ-8은 공통 계약 위에서 구현한다. 각 host 단계에는 geometry뿐 아니라 3.3의 해당 플랫폼 전체 필드와 unsupported 근거가 포함된다. MQ-9는 모든 host 결과를 집계한다. API 선택이 어려운 host 하나 때문에 다른 host의 구현·자동 검증을 멈추지 않는다.

새 파일을 추가할 경우 기존 host 디렉터리 안에 provider를 배치한다. common framework에 `#if ANDROID` 등 OS 분기를 추가하지 않는다. reviewed source를 수정하면 기존 Flutter source/hash/anchor evidence 갱신 규칙도 적용한다.

## 6. 검증 계획

### 6.1 공통 결정적 테스트

기존 `Doroti/validation/framework-work`와 `Doroti/validation/fcr7-material-widget`의 harness를 활용하고, 필요 시 focused `media-query-safe-area` 검증 프로젝트를 추가한다. 새 프로젝트/runner는 아직 존재하지 않는 계획 산출물이다.

- 기본 수치: DPR=3, physicalSize=1080×2400, viewPadding.bottom=72일 때 logical size=360×800, viewPadding.bottom=24. viewInsets.bottom이 0 → 30 → 900 → 0이면 padding.bottom은 logical 24 → 14 → 0 → 24로 변한다. 모든 변과 asymmetric 값도 검사한다.
- SafeArea의 네 flag 조합, minimum이 작은/큰 경우와 flag=false여도 minimum 적용, nested 소비, maintainBottomViewPadding true/false, keyboard 부분/전체 가림, zero area, SliverSafeArea/reverse scroll 및 RTL을 비교한다.
- removePadding/removeViewInsets/removeViewPadding의 관련 필드 보정, no-op, copyWith, removeDisplayFeatures의 sub-screen 이동/경계/회전, corner radii와 typography override 보존을 검사한다.
- `of` 전체 의존성과 `sizeOf/widthOf/heightOf/paddingOf/viewInsetsOf/textScalerOf/...` 선택 의존성을 mounted tree에서 검사한다. 동일값, 다른 필드만 변경, 부모 override 변경, view 교체, observer 해제, multi-view를 포함한다.
- pin의 equality/hash 의미와 현재 list identity 동작을 먼저 테스트로 고정한다. 불변 feature list 재사용으로 불필요 알림을 줄이되 Flutter와의 의도적 차이는 별도 기록한다. textScaler 선형/비선형, clamp/noScaling과 native 설정 갱신도 확인한다.
- host fake snapshot 이벤트로 **host → Dispatcher → Binding → View → MediaQuery**를 실행한다. MediaQueryData 수동 주입만으로 연결 테스트를 대체하지 않는다.
- metrics-only 변경 중 input/caret/hit-test/semantics 좌표가 같은 view transform을 사용하는지 확인하고 이전 resize generation과 섞인 frame을 검출한다.

### 6.2 실제 UI 시나리오

테스트베드에 현재 root/fromView 값과 nested SafeArea 소비 후 값을 관찰할 수 있는 전용 사례를 만든다. 진단용 화면에 raw/normalized/logical 값, viewId/generation을 표시하고 일반 제품 UI에는 진단 컨트롤을 넣지 않는다.

| 시나리오 | 확인 결과 |
|---|---|
| AppBar + Scaffold body + bottom bar + SafeArea | top/bottom 이중 여백 없음, resizeToAvoidBottomInset true/false와 maintainBottomViewPadding 의도 일치 |
| MAUI raw surface + SafeArea 유무 + keyboard 회피 옵션 | 시스템 edge까지 surface/배경이 차지하고 SafeArea 자식만 회피; IME 전후 native bounds 대비 surface 추가 축소 없음, Scaffold 회피=false일 때 MAUI 추가 이동 없음 |
| 하단 TextField, multiline editor, focus 이동 | keyboard 표시/숨김/종류 변경 시 가림·불필요 점프 없음, caret/selection/IME 위치 일치 |
| Dialog, modal bottom sheet, popup, Cupertino page/nav bar/action sheet | route/overlay의 safe area·keyboard 소비가 Flutter pin과 일치 |
| ListView 자동 padding, CustomScrollView + SliverSafeArea | 첫/마지막 항목 가림 없음, 중첩/스크롤 위치 보존 |
| 회전/split-screen/fold/fullscreen/창 resize/DPI 이동 | inset/size/DPR/feature가 같은 좌표계로 갱신, 중간 frame 혼합/최종 stale 값 없음 |
| OS 글자 크기·명암/접근성 설정 변경 | 해당 aspect만 갱신, app/parent override 유지, 잘림/입력 회귀 확인 |
| 재연결/재개/종료 및 여러 view | listener 중복/누수 없음, view 간 geometry/설정 오염 없음 |
| Web keyboard + 주소창 + page/pinch zoom + embedded | size 축소와 keyboard 차감 중복 없음, 바깥 페이지/다른 root 영향 없음 |

실기기 및 브라우저 matrix는 최소 Android gesture/3-button·구 API/현대 API, iPhone notch/home indicator·iPad, Catalyst, native macOS, Windows 두 host, Linux Wayland/X11, desktop Chromium/Firefox/Safari와 mobile Chromium/Safari를 포함한다. 각 renderer/browser 조합의 지원 여부와 실제 실행 여부를 분리한다. foldable/notched monitor/touch keyboard처럼 특정 하드웨어가 필요한 항목은 장치 정보와 함께 별도 evidence로 남긴다.

### 6.3 실행·증거 정책

- `.github/copilot-instructions.md`에 따라 **각 테스트 실행 timeout은 20분(1200초)**이다. `dotnet run` 형태의 console harness와 browser/native runner에도 외부 timeout을 적용하고 초과 시 해당 실행의 자식 프로세스만 종료한다.
- framework 테스트, 각 host build/ABI/protocol 검사, 소비자 app/template build, native/DOM adapter fixture, 실제 UI 순서로 필요한 검증을 수행한다. 문서 작성인 현재는 테스트를 실행하지 않았다.
- 증거는 `Doroti/validation/evidence/media-query-safe-area/` 아래에 계획한다. pin·commit/dirty state·OS/SDK·host/RID·renderer·DPR·window/keyboard mode·raw snapshot·normalized metrics·MediaQuery·실제 bounds·event sequence·결과를 기록한다. JSON/log 및 필요한 screenshot/video를 함께 둔다.
- 상태는 `sourceVerified`, `automatedPassed`, `devicePassed`, `unsupported`, `notVerified`, `skippedByUser`, `failed`를 구분한다. 지금은 source 검토와 계획만 완료했으며 어떤 플랫폼도 이번 작업의 devicePassed가 아니다.
- 환경/장치가 없어 실행하지 못하면 구체 조건과 후속 재현 절차를 기록한다. build 성공·fixture 성공·과거 renderer 실행을 전체 플랫폼 제품 검증으로 대체하지 않는다.

## 7. 최종 완료 기준

- 모든 제품 host가 실제 view에 맞는 metrics를 초기 표시부터 동적 변경까지 공급한다. 지원 API가 없는 항목은 field×host 표에 사유와 fallback이 있고, 공급 누락을 zero 성공으로 숨기지 않는다.
- physical/logical 변환은 정해진 경계에서 한 번만 수행되며 derived padding·nested consumption·Scaffold/overlay 동작이 Flutter 기준과 맞는다.
- inset-only 및 환경 설정 변경이 resize 없이 전달되고 surface 재생성/불필요 전체 rebuild를 유발하지 않는다. input/focus/accessibility semantics와 resize admission 회귀가 없다.
- MAUI가 소유하는 container/render surface/입력 보조 계층은 inset·keyboard를 선제 소비하지 않는다. 가능한 전체 native content 영역과 raw edge 정보를 공급하고 Doroti만 배치를 결정한다. 플랫폼 강제 제한은 증거와 함께 명시하며, embedded 사용자의 명시적 부모 bounds를 전역으로 바꾸지 않는다. Web 및 다른 host도 자동 소비와 Doroti 소비를 중복 적용하지 않는다.
- 공통 계약 테스트와 각 host의 빌드/ABI/protocol/adapter 검증이 통과하며 제품 UI 결과는 플랫폼별로 확인된다. 미실행 장치/필수 시나리오가 남으면 해당 제품 검증은 미완료로 표시하고 전체 플랫폼 완료를 선언하지 않는다.
- 사용법·지원 한계·intentional Flutter 확장·재현 절차와 증거 index를 갱신한다. 과거 검증 기록은 보존한다.

# Doroti 창 API 재구성 검토 및 작업계획

작성일: 2026-09-25

검토 기준: `7f74005f`, `.github/copilot-instructions.md`, 사용자가 제공한 Sudoku Flutter 앱과 아래 공식 웹 문서.

상태: **구현·검증 진행 / 전체 PARTIAL (2026-09-25)**. 사용자의 전체 작업 요청에 따라 Desktop 패키지, manager/controller, SDK companion, Windows MAUI 기본 창 경로를 구현했다. 계약 검사 25개, Windows 실제 창 조작, native caption 재질 조합, 저장소 밖 package-only Windows 실행을 검증했다. **W3의 Hidden/custom 상단바·위젯, W4의 WindowsAppSDK/AppKit/Qt adapter, 일부 환경·시각 gate는 미완료**다. 아래 제안 예제 중 현재 제공되는 API와 제한은 [구현 API 문서](Doroti/docs/desktop-windows.md), 검증 근거는 [실행 결과](Doroti/validation/desktop-window/README.md)를 기준으로 한다. [Runtime 전환 보관 요약](history/26-09-24/runtime-dotnet-migration-summary.md)과 [Linux Qt 작업 보관 요약](history/26-09-24/linux-qt-improvements-summary.md)의 잔여 작업은 별도로 유지한다.

사용자 확정 방향(2026-09-25 추가): **새 창 API는 데스크톱 OS 전용으로 제공하고, 향후 다중 창 지원 시 사용 코드를 다시 설계하지 않도록 창 관리·창별 제어·창 콘텐츠 생성을 처음부터 분리한다.** 이번 범위는 다중 창의 API·수명 설계까지이며, 실제 여러 네이티브 창의 동시 실행 구현은 후속 단계로 둔다.

## 1. 권장 방향

`window_manager`의 **옵션 선언 → 창 준비 → 표시·포커스 → 실행 중 제어·이벤트** 흐름을 Doroti에 도입한다. 공개 진입점은 앱 범위 `DorotiWindowManager`, 창별 `DorotiWindowController`, 생성 요청 `WindowCreateOptions`와 설정 `WindowOptions`로 구성하고, 아크릴·제목 표시줄은 `WindowAppearanceOptions` 안에서 조합한다. C# 앱에서는 `Task`, `CancellationToken`, 명시적 창 소유권을 사용한다.

중요한 설계 결정은 다음과 같다.

- `WindowOptions`는 위치·크기·동작·외형을 한곳에 선언하는 초기 설정이다.
- `DorotiWindowManager`는 앱이 가진 창의 생성·조회·목록·앱 종료 정책을 담당한다. 기본 창도 같은 관리 대상이며 별도 singleton 창 경로를 만들지 않는다.
- `DorotiWindowController`는 실제 최상위 창 하나를 제어한다. 프로세스 전역의 암묵적 현재 창을 만들지 않는다.
- 제목 표시줄의 **표시 방식**, **배경 재질**, **콘텐츠·버튼 소유자**를 구분한다. 기존 `unified/solid`만으로는 숨김·커스텀 제목 표시줄까지 표현하기 어렵다.
- 창 배경색, 뒤쪽 데스크톱 재질, 앱 콘텐츠의 투명도, 창 전체 opacity는 서로 다른 기능이다.
- 시작 옵션과 실행 중 변경은 같은 검증·적용 경로를 공유한다. 지원하지 않는 옵션을 성공으로 처리하지 않는다.
- Windows/macOS/Linux의 네이티브 데스크톱 앱에서만 새 API를 제공한다. Windows MAUI를 첫 구현 대상으로 삼고 WindowsAppSDK raw, macOS AppKit, Linux Qt로 확장한다. Web·Android·iOS에는 이 API의 참조·등록·호출 경로를 제공하지 않는다. 데스크톱 OS에서 실행한 브라우저도 Web 대상이다.

## 2. 참고 코드와 웹 문서 검토

### 2.1 사용자가 제공한 실제 사용 방식

검토 파일: [Sudoku main.dart](C:/Users/parti/Labo/my_sudoku_v2/my_sudoku_app_flutter_v2/lib/main.dart). 같은 프로젝트의 `pubspec.yaml`은 `window_manager: ^0.5.1`, `pubspec.lock`은 **0.5.1**이다.

데스크톱에서만 초기화하고, `WindowOptions`로 `450×800`, 최소 `350×500`, 흰 배경, 일반 제목 표시줄, 작업 표시줄 표시, always-on-top 해제를 지정한다. 준비 콜백에서 `show()`와 `focus()`를 차례로 호출한 뒤 앱을 시작한다. 앱 초기화 코드에서 네이티브 HWND·NSWindow를 직접 다루지 않는 점이 Doroti에도 유용하다.

이 편의성을 유지하되 Doroti의 SDK가 만든 host와 framework bootstrap 순서에 맞춰 연결해야 한다. 현재 동기 `Configure()` 안에서 창 생성이나 첫 프레임을 `.Wait()`/`.Result`로 기다리면 초기화 순환 대기가 생길 수 있다.

### 2.2 공식 레퍼런스와 적용 판단

| 근거 | 확인 내용 | Doroti에 적용할 판단 |
| --- | --- | --- |
| [window_manager 패키지](https://pub.dev/packages/window_manager) | 검토 시 공개 버전 0.5.2, Windows/macOS/Linux 지원, nativeapi 이전 안내 존재 | 사용 경험과 계약을 참고한다. 플러그인 또는 향후 C++ 코어를 새 의존성으로 추가하지 않는다. |
| [WindowOptions](https://pub.dev/documentation/window_manager/latest/window_manager/WindowOptions-class.html) | 크기·최소/최대 크기·배경·제목·titleBarStyle 등의 초기 옵션 묶음 | 한 번에 선언하는 진입점을 도입하고 외형은 중첩 옵션으로 확장한다. |
| [WindowManager](https://pub.dev/documentation/window_manager/latest/window_manager/WindowManager-class.html) | 표시, 포커스, 위치·크기, 창 상태, 닫기, 드래그 API | 주요 조작을 창 컨트롤러로 제공한다. 도킹·배지 등 전체 API의 일대일 복제는 후순위다. |
| [waitUntilReadyToShow 구현](https://pub.dev/documentation/window_manager/latest/window_manager/WindowManager/waitUntilReadyToShow.html) | 옵션을 순차 적용하고 `VoidCallback`을 호출한다. 콜백 반환 비동기 작업은 await하지 않는다. | Doroti 콜백은 `Func<..., Task>`로 받고 실패·취소·수명까지 관찰한다. 준비 완료와 표시 완료를 구분한다. |
| [Quick Start: Hidden at launch](https://leanflutter.dev/documentation/window_manager/quick-start#hidden-at-launch) | 초기 외형 깜빡임을 피하려면 runner의 자동 표시도 제어해야 한다. | 새 API wrapper만 추가하지 말고 각 runner의 최초 show/activate 경로를 함께 변경한다. |
| [TitleBarStyle](https://pub.dev/documentation/window_manager/latest/window_manager/TitleBarStyle.html), [DragToMoveArea](https://pub.dev/documentation/window_manager/latest/window_manager/DragToMoveArea-class.html) | normal/hidden과 이동 영역 위젯 제공 | 숨김·이동 영역은 도입하되 버튼·입력 제외 영역·DPI까지 Doroti 계약에 포함한다. |
| [WindowListener](https://pub.dev/documentation/window_manager/latest/window_manager/WindowListener-class.html) | 포커스, 이동·리사이즈, 창 상태와 닫기 이벤트 제공 | typed 상태 이벤트와 비동기 닫기 결정 계약으로 구성한다. |
| [flutter_acrylic](https://pub.dev/packages/flutter_acrylic) | 창 재질은 별도 패키지에서 제공하는 영역 | Doroti는 이미 가진 재질 구현을 창 API와 통합한다. window_manager 자체가 아크릴 옵션을 제공한다고 가정하지 않는다. |

플랫폼 근거도 함께 검토했다.

- [Windows 제목 표시줄 문서](https://learn.microsoft.com/en-us/windows/apps/develop/title-bar): 사용자 콘텐츠를 확장하는 경로는 드래그 영역과 시스템 버튼 공간을 다뤄야 한다. 네이티브 제목 표시줄 경로와 구현 책임을 분리한다.
- [DWM_SYSTEMBACKDROP_TYPE](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwm_systembackdrop_type): non-client 영역에도 시스템 재질을 지정하며, Windows 11 build 22621부터 지원한다. `TransientWindow`의 Acrylic은 Windows가 선택한 변형이므로 본문 `TintOpacity`와 정확히 같은 효과를 약속하지 않는다.
- [Qt QWindow](https://doc.qt.io/qt-6/qwindow.html#startSystemMove): OS 이동·크기 변경을 제공하며, Wayland에서는 임의 `setPosition`이 지원되지 않는다. 위치 지정과 사용자 드래그를 별도 capability로 판정한다.
- [AppKit titlebarAppearsTransparent](https://developer.apple.com/documentation/appkit/nswindow/titlebarappearstransparent): macOS의 제목 표시줄 투명화 진입점. 상세 재질·레이아웃은 현재 `AppKitWindowBackdrop` 구현과 대상 SDK로 추가 검증한다.

## 3. Doroti 현재 구조와 실제 개선 지점

아래는 현재 소스를 읽어 확인한 내용이다. 새 API의 구현 완료나 이번 턴의 실행 검증 결과를 뜻하지 않는다.

| 현재 계약·구현 | 위치 | 개선 지점 |
| --- | --- | --- |
| `DorotiViewConfiguration(title, logicalSize, backgroundColor, darkBackgroundColor, backdrop, appearance, ...)` | [ViewContracts.cs](Doroti/src/Doroti.Ui/ViewContracts.cs) | 화면/view 설정과 데스크톱 창 설정이 섞여 있고 최소 크기·최초 표시 정책이 없다. |
| `WindowAppearanceOptions`와 `WindowBackdropOptions` | 같은 파일 | `backdrop`/`appearance.backdrop` 중복 입력, macOS override, `unified/solid`를 호환성을 유지하며 재구성해야 한다. |
| `IViewHostCapability.Show/Resize/Close`, metrics/lifecycle/close 이벤트 | 같은 파일 | 기본 기능은 이미 있다. 새 컨트롤러 아래로 연결하고 중복 소유자를 만들지 않는다. |
| `DorotiView` capability 접근과 `SingletonDorotiWindow` | [PlatformDispatcher.cs](Doroti/src/Doroti.Ui/PlatformDispatcher.cs), ViewContracts | view는 물리 창과 일대일이 아닐 수 있다. legacy metrics facade를 전역 창 관리자로 바꾸지 않는다. |
| 동기 `IDorotiApplicationStartup.Configure`, `UseView`, descriptor 생성 | [DorotiApplicationBootstrap.cs](Doroti/src/Doroti.Hosting/DorotiApplicationBootstrap.cs) | 선언 단계와 비동기 창 준비 hook을 구분할 연결 지점이 필요하다. |
| Bootstrap/AttachView/DetachView/Shutdown | [DorotiHostSession.cs](Doroti/src/Doroti.Hosting/DorotiHostSession.cs) | view 등록 dictionary가 있다는 것만으로 여러 OS 창이 지원되지는 않는다. 창별 content factory와 session/dispatcher 소유권을 연결하고, 한 창 종료와 전체 Shutdown을 분리해야 한다. |
| MAUI Window 생성 | [DorotiMauiApplication.cs](Doroti/src/Doroti.Host.Maui/DorotiMauiApplication.cs) | 이 생성부는 제목만 설정하고 `logicalSize`를 Window 크기에 직접 반영하지 않는다. 생성 이후를 포함해 옵션 소비 경로 전체를 조사한다. |
| Windows MAUI 아크릴과 기본 caption | [WindowsWindowBackdrop.cs](Doroti/src/Doroti.Host.Maui/WindowsWindowBackdrop.cs), [WindowsNativeCaption.cs](Doroti/src/Doroti.Host.Maui/WindowsNativeCaption.cs) | 생성 시 옵션에 묶여 있다. caption은 backdrop 옵션만 받아 `appearance.titlebarStyle`을 구별할 수 없다. 본문·caption 설정을 같은 외형 정책으로 전달해야 한다. |
| WindowsAppSDK의 실행 중 아크릴 변경 | [WindowsAcrylicOptionsState.cs](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsAcrylicOptionsState.cs) | 내부 `doroti/windows/experimental-acrylic` 메시지 채널과 revision/대체 처리 존재. 공통 typed 공개 창 API로 올리고 기존 채널은 adapter로 전환한다. |
| macOS 재질·제목 표시줄 | [AppKitWindowBackdrop.cs](Doroti/src/Doroti.Host.Maui/AppKitWindowBackdrop.cs) | Acrylic/Liquid Glass와 unified/solid 경로를 보존한다. Windows tint와 동일한 의미라고 취급하지 않는다. |
| Qt 제목 표시줄 테마와 MaterialApp 연동 | [QtTitlebarAppearance.cs](Doroti/src/Doroti.Host.Qt/QtTitlebarAppearance.cs), [WindowTitlebar.cs](Doroti/src/Doroti.Ui/WindowTitlebar.cs), [Material app.cs](Doroti/src/Doroti.Framework.Material/app.cs) | 앱 테마 자동 전달과 명시적 창 외형 변경이 경쟁하지 않도록 우선순위를 정한다. |

직전 Windows MAUI 수정은 기본 caption에 아크릴이 보이는 경로를 추가했다. 이를 `Normal + Backdrop`의 기존 기반으로 사용한다. 네이티브 caption을 초기화한 뒤 AppWindow 색상·확장 설정이 다시 custom caption을 만들지 않도록 현재 초기화 순서를 보존한다. [기존 검증 기록](Doroti/validation/windows-maui/README.md)의 리사이즈 시각 연속성 **PARTIAL**은 이 API 계획으로 해소되지 않는다.

## 4. 제안하는 공개 API

### 4.1 역할과 소유권

| 구성 | 책임 |
| --- | --- |
| `DorotiWindowManager` | 앱 범위 창 생성·ID 조회·목록 snapshot·생성/종료 이벤트. UI 조작은 대상 controller에 위임 |
| `WindowCreateOptions` / `WindowContent` | 창 설정, 창마다 새로 만드는 콘텐츠 factory, 선택적 owner 관계와 생성 hook |
| `WindowOptions` | Title, Size, MinimumSize, MaximumSize, Position/Placement, AlwaysOnTop, SkipTaskbar, Resizable, 초기 창 상태, StartupVisibility, Appearance |
| `WindowAppearanceOptions` | BackgroundColor, Theme 정책, Backdrop, TitleBar 및 플랫폼별 외형 override |
| `WindowBackdropOptions` | 기존 Mode/Fallback/AcrylicKind/TintColor/TintOpacity/LuminosityOpacity의 의미 보존 |
| `WindowTitleBarOptions` | Style, Background, Frame, Buttons, 색상·높이 등 지원되는 세부 옵션 |
| `DorotiWindowController` | 해당 WindowId의 초기화·준비, 표시·포커스, 조작, 상태 조회, 외형 변경, 이벤트, 닫기 |
| `DesktopWindowContext` | 해당 창의 `Window`와 앱의 `Windows` manager를 콘텐츠·hook에 명시적으로 전달 |
| `WindowCapabilities` / `WindowState` | 현재 host·OS·창 상태에서의 지원 범위와 실제 관측 상태 |
| `IWindowHostCapability` 등 내부 host 계약 | owner UI thread에서 네이티브 실행·응답·이벤트 전달 |
| `WindowTitleBar`, `WindowDragRegion`, `WindowCaptionButtons` 위젯 | 커스텀 상단바의 레이아웃·입력 영역·접근성, 컨트롤러 호출 |

새 공개 API는 **데스크톱 전용 `Doroti.Desktop` 패키지·namespace**에 둔다. manager/controller/options와 desktop startup adapter가 기존 `Doroti.Ui`·`Doroti.Hosting`을 사용하고, 커스텀 상단바 및 Widget 콘텐츠 adapter는 선택적 `Doroti.Desktop.Widgets`에 둔다. Ui/Hosting/Framework 공통 패키지에서 Desktop 패키지를 역참조하지 않는다. 패키지명은 제안이며 W0에서 고정한다. 새 타입은 C# PascalCase를 기준으로 하되 기존 Flutter API 전체의 casing을 바꾸는 작업은 제외한다.

controller는 `WindowId + lifetime`에 귀속한다. 여러 view가 한 창을 공유하면 동일한 controller를 사용하되, embedded view에는 기본적으로 창 변경 권한을 주지 않는다. 데스크톱 전용 `DesktopWindowScope.Of(context)`는 해당 Widget 트리에 주입된 `DesktopWindowContext`만 반환한다. 공통 `DorotiView`에 desktop 전용 멤버를 추가하거나 현재 활성 창을 추측하지 않는다.

### 4.1.1 데스크톱 전용 제공 경계

- 기본 방침은 **컴파일·패키지 경계에서 분리**하는 것이다. `if (isDesktop)` 아래에서 모바일에도 같은 API를 호출하게 하는 범용 facade나 no-op 구현은 만들지 않는다.
- SDK가 desktop 전용 startup/source/project와 패키지 참조를 Windows/macOS/Linux runner에만 포함한다. 예: 공통 `AppStartup` + 별도 `DesktopStartup : IDorotiDesktopApplicationStartup`. 기존 공유 앱 assembly가 target-neutral이면 별도 desktop companion assembly를 사용한다. 플랫폼별 빌드로 전환할 경우 target별 출력·중간 파일 격리까지 설계해야 한다.
- Web/Android/iOS 빌드에는 desktop startup 및 `Doroti.Desktop.Widgets`를 포함하지 않는다. 앱 개발자는 데스크톱 파일에서 한 번 설정하고 공통 UI에는 매번 OS 분기를 넣지 않는다.
- 비데스크톱 runner가 Desktop 패키지·startup을 잘못 참조하면 SDK build diagnostic으로 거절한다. 리플렉션/수동 assembly 로딩 등으로 경계를 우회해도 지원 host 없는 초기화는 명시적으로 실패하며 조용히 무시하지 않는다.
- OS 식별만으로 동작을 허용하지 않는다. macOS의 MacCatalyst처럼 별도 host adapter가 필요한 대상은 해당 데스크톱 adapter의 준비·검증 여부로 허용한다. embedded 데스크톱 UI는 OS 제한과 별도로 창 소유 host의 명시적 controller 연결이 필요하다.
- 기존 Ui의 view metrics 및 legacy 외형 record를 즉시 제거하는 것은 별도 호환 문제다. 기존 계약은 이전 기간에 유지하되 새 Desktop API가 공통 패키지로 누출되지 않게 한다.

### 4.2 사용자가 쓰던 흐름에 대응하는 예제

아래는 **제안 API**이며 데스크톱 startup의 `Configure(DesktopApplicationBuilder desktop)` 본문이다. `UseMainWindow`는 기본 창의 생성 요청을 등록한다. 기본 창도 manager의 동일한 생성 파이프라인으로 만들며, 이후 추가 창과 같은 controller·이벤트·외형 계약을 사용한다.

```csharp
var options = new WindowOptions
{
    Title = "Sudoku",
    Size = new Size(450, 800),
    MinimumSize = new Size(350, 500),
    AlwaysOnTop = false,
    SkipTaskbar = false,
    StartupVisibility = WindowStartupVisibility.Manual,
    Appearance = new WindowAppearanceOptions
    {
        BackgroundColor = new Color(0xffffffff),
        TitleBar = new WindowTitleBarOptions
        {
            Style = WindowTitleBarStyle.Normal,
            Background = WindowTitleBarBackground.System,
        },
    },
};

desktop.UseMainWindow(new WindowCreateOptions
{
    Options = options,
    Content = WidgetWindowContent.Create(() => new SudokuApp()),
    OnCreated = async (context, cancellationToken) =>
    {
        var window = context.Window;
        await window.EnsureInitializedAsync(cancellationToken);
        await window.WaitUntilReadyToShowAsync(cancellationToken);
        await window.ShowAsync(cancellationToken);
        await window.FocusAsync(cancellationToken);
    },
});
```

일반 앱의 기본값은 `StartupVisibility.WhenReady`로 두어 명시적 hook을 생략할 수 있게 한다.
`UseMainWindow(new WindowCreateOptions { Options = options, Content = ... })`만으로 옵션 적용 후 안전하게 표시하는 것이 기본 경로다. 위 Manual 예제는 사용자가 표시 시점을 직접 결정할 때의 대응 방식이다. `EnsureInitializedAsync`는 같은 창에 반복 호출해도 안전하다.

`WindowCreateOptions`가 창 생성별 설정·콘텐츠의 유일한 소유자가 된다. `WindowOptions`는 재사용 가능한 immutable 설정이고 WindowId·현재 상태를 담지 않는다. 별도 view 초기 크기를 생략하면 해당 창의 client size로 root view 설정을 생성한다. 기존 `UseView`/entrypoint 경로는 기본 창에 한해 legacy adapter로 연결하고, factory가 지정된 경우 동일 root content를 다시 attach하지 않는다.

`WidgetWindowContent.Create`는 데스크톱 Widget 패키지의 편의 adapter다. 매 창마다 factory를 실행하여 새 Widget tree·상태를 만든다. 저수준 사용자는 `WindowContent.FromEntrypoint(() => new MyEntrypoint())`를 사용할 수 있다. 앱 공용 서비스는 공유할 수 있지만 mounted Widget·view·entrypoint 인스턴스를 여러 창에 그대로 재사용하지 않는다.

### 4.2.1 향후 추가 창을 여는 사용 형태

다음은 **다중 창 기능 도입 후의 목표 예제**다. 당장 두 번째 네이티브 창이 구현되어 있다는 뜻이 아니다. 기존 단일 창의 설정·조작 코드는 그대로 두고 생성만 manager를 통해 추가한다.

```csharp
// 데스크톱 Widget 트리에서 이 화면이 속한 창과 앱의 manager를 얻는다.
var desktopContext = DesktopWindowScope.Of(context);
var currentWindow = desktopContext.Window;
var windows = desktopContext.Windows;

var settingsWindow = await windows.CreateWindowAsync(
    new WindowCreateOptions
    {
        Options = new WindowOptions
        {
            Title = "설정",
            Size = new Size(640, 480),
            StartupVisibility = WindowStartupVisibility.Manual,
            Appearance = acrylicAppearance,
        },
        Content = WidgetWindowContent.Create(() => new SettingsApp()),
    },
    cancellationToken);

await settingsWindow.WaitUntilReadyToShowAsync(cancellationToken);
await settingsWindow.ShowAsync(cancellationToken);
await settingsWindow.FocusAsync(cancellationToken);

WindowId settingsId = settingsWindow.Id;
if (windows.TryGetWindow(settingsId, out var sameWindow))
    await sameWindow.SetTitleAsync("설정 · 변경됨", cancellationToken);

await settingsWindow.CloseAsync(cancellationToken); // 설정 창만 닫는다.
```

현재 창 조작은 `currentWindow`, 다른 창 조작은 반환받은 controller 또는 WindowId 조회를 사용한다. `windows.Show()`처럼 대상이 생략된 manager 조작 API는 제공하지 않는다. `DesktopWindowScope`는 전경 창이 바뀌어도 자신의 창을 가리킨다. 데스크톱 전용 서비스는 manager/controller를 생성자 주입받아 동일하게 사용한다.

manager의 목표 공개 형태:

| API | 계약 |
| --- | --- |
| `CreateWindowAsync(WindowCreateOptions, CancellationToken)` | 새 창의 native/controller/content 연결을 초기화해 반환. 첫 frame·표시는 별도 readiness/visibility 계약 |
| `TryGetWindow(WindowId, out DorotiWindowController)` | 같은 앱 manager가 소유한 살아 있는 창만 조회 |
| `GetWindows()` | 읽기 전용 목록 snapshot. 목록의 순서가 기본 창·전경 창을 뜻하지 않음 |
| `MainWindowId` | startup에서 지정한 기본 창의 식별자. 닫혀도 다른 창을 자동 승격하지 않음 |
| `WindowCreated` / `WindowClosed` | WindowId와 대상 창 정보를 포함하는 앱 범위 이벤트 |
| `Capabilities` | 추가 창 생성 지원과 필요 시 최대 창 수. 창별 재질·조작 capability와 별도 |

manager는 **앱 인스턴스 범위**로 주입하며 static singleton으로 노출하지 않는다. 기본 창은 `UseMainWindow`의 생성 요청을 manager에 등록하는 편의 기능이다. 추가 창 생성도 같은 options validation·native attach·content factory·OnCreated·ReadyToShow 경로를 따른다. 생성 실패/취소 시 registry·view·native 자원을 정리하며 실패한 controller를 정상 창 목록에 남기지 않는다. 반환 이전 생성 취소는 생성 자원을 정리하고, 반환 이후 토큰 취소는 이미 만들어진 창을 암묵적으로 닫지 않는다.

`OnCreated`는 생성 요청별 hook이며 `CreateWindowAsync`의 반환을 그 hook 전체 완료에 종속시키지 않는다. hook이 ReadyToShow를 await해도 framework는 계속 진행해야 한다. manager가 hook Task의 실패·취소를 추적한다. 준비 전 실패는 준비 대기자를 실패로 종결하고, 준비 완료 후 실패는 별도의 초기화 작업 실패로 보고하여 이미 종결된 readiness 결과를 바꾸지 않는다. `StartupVisibility.WhenReady`의 자동 표시는 동일한 readiness task를 사용하며, 중복 show나 별도의 예외 누락 경로를 만들지 않는다.

### 4.2.2 다중 창을 위한 초기 계약과 후속 구현 경계

- `WindowId`는 HWND나 viewId와 다른 opaque 식별자이며 앱 수명 동안 재사용하지 않는다. 종료된 controller는 항상 종료 상태를 유지하고, 나중에 만들어진 창을 가리키지 않는다.
- 창별로 Content, UI scope, capability, 외형/상태 revision, 입력·IME·접근성, 구독과 취소 수명을 분리한다. 하나의 OS 창에 여러 view를 붙이는 경우와 여러 OS 창 생성은 구별한다.
- 기본 창도 다른 창과 같은 `CloseAsync`를 사용한다. 한 창의 종료가 공용 dispatcher·앱 서비스·다른 창의 renderer를 종료하지 않도록 한다. 공유 GPU 자원은 마지막 소유자가 해제한다.
- 앱 종료 정책은 manager의 `WindowLifetimePolicy`로 분리한다. `OnLastWindowClosed`와 `Explicit`을 우선 설계하고, 기존 `terminateAfterLastWindowClosed`는 adapter로 매핑한다. 숨김/최소화 창은 살아 있는 창에 포함한다. 기본 창 종료만으로 다른 창을 암묵적으로 닫지 않는다.
- 향후 owned window는 `OwnerWindowId`와 명시적인 owner 종료 정책을 생성 요청에 추가한다. 기본은 서로 독립인 최상위 창이다. modal은 owner와 같은 의미가 아니며 별도 후속 기능으로 둔다. owner의 같은-manager 검증, 순환 방지, owner 닫기와 child 취소 동작을 계약화한 뒤 제공한다.
- `WindowContent`의 선택은 생성 시 고정한다. 창 간 문서/서비스 공유는 명시적인 앱 모델을 사용한다. 창 이동·재부모화, 창 간 Widget 인스턴스 이전, 다중 프로세스 창·IPC는 이번 범위에서 제외한다.
- 초기 제품은 manager를 통해 기본 창 하나만 제공할 수 있다. 그 경우 추가 생성 요청은 capability 검사에서 **자원 할당 전에 Unsupported**로 종결하며 기본 창을 재사용해 반환하지 않는다. `CreateWindowAsync`의 public 노출 시점은 W0에서 정하되 목표 signature·factory·소유권은 지금 고정한다.
- 초기 단계에서는 fake-host 두 창으로 격리 계약을 검증한다. 실제 두 창 생성·창별 event loop/host session·동시 렌더링 검증은 W6 후속 단계에서 수행한다. 단일 창 빌드/실행 통과를 다중 창 지원 완료로 보고하지 않는다.

### 4.3 아크릴과 제목 표시줄 조합 예제

```csharp
var acrylicAppearance = new WindowAppearanceOptions
{
    BackgroundColor = new Color(0x00000000),
    Backdrop = new WindowBackdropOptions
    {
        Mode = WindowBackdropMode.Acrylic,
        AcrylicKind = WindowAcrylicKind.Thin,
        Fallback = WindowBackdropFallback.Solid,
    },
    TitleBar = new WindowTitleBarOptions
    {
        Style = WindowTitleBarStyle.Normal,
        Background = WindowTitleBarBackground.Backdrop,
    },
};

await window.ApplyAppearanceAsync(acrylicAppearance, cancellationToken);

// 검색창·탭 등을 넣는 상단바: 아래 설정과 WindowTitleBar 위젯을 함께 사용한다.
await window.ApplyAppearanceAsync(acrylicAppearance with
{
    TitleBar = acrylicAppearance.TitleBar with
    {
        Style = WindowTitleBarStyle.Hidden,
        Buttons = WindowCaptionButtonMode.Native,
    },
}, cancellationToken);
```

예제의 기존 enum/record PascalCase는 새 계약으로 이전한 뒤의 표기다. 현재 소스의 소문자 positional record와 혼용 가능한 코드라는 의미가 아니다. 초기 구현 단계에서 명명·생성자 형태를 한 번 확정하고 모든 예제·template에 반영한다.

`ApplyAppearanceAsync`는 완전한 외형 snapshot 교체다. 일부 값만 변경하는 `UpdateAppearanceAsync(current => current with { ... })`는 controller의 직렬 처리 큐 안에서 최신 requested snapshot을 기준으로 계산한다. 호출자가 오래된 상태를 읽어 수정한 결과로 다른 옵션을 덮어쓰는 경쟁을 피한다. nullable tint 값은 재질의 기본값 복원을 의미하며, 값 미지정과 기존 값 유지는 별도 API 의미로 구분한다.

### 4.4 제목 표시줄 조합의 의미

| 조합 | 의미와 제약 |
| --- | --- |
| Normal + System | OS 기본 제목·버튼·드래그 영역. 기존 사용자 경험의 기본값 |
| Normal + Solid | 별도 단색 caption. 본문 Acrylic과 함께 사용할 수 있어야 함 |
| Normal + Backdrop | 기본 제목·버튼을 유지하고 caption에도 재질 적용. Windows DWM은 본문과 같은 tint 수치 적용을 보장하지 않음 |
| Hidden + Native buttons + Backdrop | 앱이 제목 영역을 그리며 OS 버튼·예약 공간은 host가 관리. 검색창·탭을 넣는 권장 커스텀 경로 |
| Hidden + Custom buttons + Backdrop | Framework가 버튼까지 제공. 접근성·키보드·최대화 상태 동기화 검증을 통과한 host만 지원 |
| Hidden + Frameless | 제목 줄과 테두리 제거. resize/drag/system menu 책임을 helper가 제공해야 함 |

`Style=Normal/Hidden`은 제목 콘텐츠 소유권, `Background=System/Solid/Backdrop`은 재질, `Frame=Standard/Frameless`는 테두리 정책이다. `Normal + Frameless`처럼 모순인 조합은 사전 거절한다. `Hidden`이 자동으로 테두리·시스템 버튼까지 제거한다는 뜻은 아니다. macOS traffic lights, Windows caption buttons, Linux 장식 방식은 capability로 구체화한다.

기본 caption에는 임의 Widget을 삽입할 수 없다. 사용자 Widget이 필요하면 명시적으로 Hidden 경로를 택한다. 반대로 정확한 tint 일치를 원한다는 이유로 Normal 요청을 숨김 상단바로 자동 전환하지 않는다.

### 4.5 실행 중 조작과 이벤트

첫 제공 범위는 `ShowAsync/HideAsync/FocusAsync`, `SetSizeAsync/SetBoundsAsync/CenterAsync`, `SetMinimumSizeAsync/SetMaximumSizeAsync`, `SetAlwaysOnTopAsync/SetSkipTaskbarAsync/SetResizableAsync`, `MinimizeAsync/MaximizeAsync/RestoreAsync/SetFullScreenAsync`, `SetTitleAsync`, 외형 변경과 `CloseAsync`다. `StartDraggingAsync/StartResizingAsync`는 유효한 사용자 입력 시점에 OS 작업을 시작한다.

WindowState는 Bounds, ClientSize, Scale, Visible, Focused, PresentationState, RequestedAppearance, EffectiveAppearance를 구분한다. 조회와 이벤트는 OS가 보고한 상태를 기준으로 한다. 포커스 요청이 OS 정책으로 거부될 수 있으므로 요청 완료를 `Focused=true`와 동일시하지 않는다.

상태 변경은 typed 이벤트와 disposable subscription으로 제공한다. `Closing`은 동기 C# event의 `async void` 대신 `Func<WindowClosingContext, CancellationToken, Task<WindowCloseDecision>>` 형태로 등록한다. 사용자 닫기·Alt+F4·API 닫기가 같은 경로를 거치며, 중복 요청은 진행 중인 한 번의 결정에 합류한다. Cancel 시 창과 renderer를 유지하고, 허용 시 **해당 창 소유 작업** drain→view detach→native destroy→Closed 순서로 한 번만 종료한다. registry 제거와 WindowClosed 통지 이후 manager가 앱 종료 정책을 평가하며 다른 창의 자원은 유지한다. 강제 종료 API는 첫 범위에서 제외한다.

## 5. 구현 전에 고정할 동작 계약

### 5.1 준비와 표시 수명

**각 창에 대해** `Created(hidden) → NativeAttached → Initialized → ReadyToShow → Visible/Hidden → Closing → Closed`를 정의한다. manager와 view/render 수명은 별도로 계속 진행하며 한 창의 초기화 대기가 다른 창의 초기화를 막지 않는다.

- NativeAttached에서 controller를 전달하고 비동기 hook을 추적한다. host는 hook 전체 완료를 기다린 뒤 framework를 붙이는 구조를 만들지 않는다.
- 초기 options 사전 검증 → 창·caption 정책 → 물리 창 생성 → view attach → 레이아웃/재질 → 준비된 첫 frame → ReadyToShow 순서를 host별로 명시한다.
- ReadyToShow는 대상 geometry의 초기 콘텐츠와 외형이 표시 가능한 상태라는 뜻이다. 화면 scan-out 완료나 사용자가 보았다는 보장은 아니다.
- hidden 상태에서 frame callback이 오지 않는 host는 visible에 의존하지 않는 준비 경로를 구현한다. 불가능하면 해당 readiness 보장을 지원한다고 광고하지 않는다. 무한 대기하거나 임의 타이머로 성공 처리하지 않는다.
- 준비 실패·취소·닫힘은 모든 대기자를 종결한다. 최초 표시 전에 오류가 나면 잠깐 보이는 기본 창을 남기지 않고 소유 자원을 정리한다.
- 반복 초기화·재표시가 크기·최대화·사용자 이동 위치를 다시 초기 옵션으로 되돌리지 않도록 한다.

### 5.2 크기·좌표·상단바 metrics

새 `Size/MinimumSize/MaximumSize`는 **client 영역의 논리 단위**로 통일하고 이름·XML 문서에 명시한다. 바깥 프레임 Bounds와 client size는 별도 조회/설정으로 구분한다. Flutter 플러그인과 프레임 포함 여부가 같다고 가정하지 않으며, legacy `logicalSize`의 host별 의미를 조사한 후 adapter로 보존한다.

Position/Bounds는 대상 모니터 좌표계·배율 기준을 명시하고, 혼합 DPI에서 전역 pixel과 DIP를 단일 배율로 변환하지 않는다. 음수 모니터 위치, 작업 영역, 최소/최대 충돌, NaN/Infinity, 최대화·전체화면 시 복원 Bounds를 다룬다. Wayland에서 위치를 읽거나 지정할 수 없으면 추정값을 실제값으로 반환하지 않는다.

`WindowChromeMetrics`에 caption 높이, content inset, native buttons의 좌우 예약 영역과 hit-test 좌표 변환 정보를 제공한다. DPI·최대화·RTL·caption 스타일 변경 시 갱신하고, framework layout과 native input이 같은 metrics revision을 사용하도록 한다. SafeArea/MediaQuery inset을 중복 적용하지 않는다.

### 5.3 조합 capability와 실패 처리

단순 `SupportsAcrylic=true` 대신 `Evaluate(options)`로 요청 조합을 사전 판정한다. 기능·설정별로 `Supported`, `Unsupported`, `RequiresRecreation` 및 이유를 반환하고, 요청값과 실제 적용값을 기록한다. OS 설정으로 재질이 단색이 된 경우의 `SystemPolicyFallback`은 기능 부재와 구분한다.

- 기본 정책은 지원하지 않는 명시 옵션을 거절하는 것이다. 재질 fallback은 사용자가 지정한 범위에서만 적용한다.
- Caption의 DWM 재질 변형, 고대비·투명 효과 해제, 비활성 상태를 EffectiveAppearance에 반영한다.
- 사전 검증 실패는 네이티브 변경 전에 반환한다. 적용 중 일부 단계가 실패하면 직전 snapshot 복구를 시도하고, 복구 실패 시 실제 부분 상태와 실패를 보고한다. OS 전체 원자 적용을 약속하지 않는다.
- HWND/NSWindow 재생성이 필요한 변경은 기본 거절한다. 첫 구현의 runtime Acrylic mode on/off 또는 frame 전환이 재생성을 요구하면 지원 한계로 기록한다. 이를 지원하려면 별도 명시적 재생성 계약과 입력/IME/접근성 수명 검증이 먼저 필요하다.
- 임의 크기·포커스 같은 OS 재량 작업은 적용된 실제 상태를 반환한다. 모든 호출을 조용한 no-op으로 처리하지 않는다.

### 5.4 동시 변경·테마·렌더러 경계

- 외형 변경은 창별 직렬 큐에서 처리한다. 아직 시작하지 않은 외형 snapshot만 최신 요청으로 대체하고, 각 요청은 Applied/Superseded/Rejected/Failed/Canceled 중 한 번 종결한다. 닫기·표시·상태 변경 명령까지 함께 버리지 않는다.
- appearance revision과 resize/view/frame generation은 독립이다. 실제 리사이즈 중 스타일 변경은 안전 시점까지 defer하며 이전 hit-test 영역을 새 geometry에 적용하지 않는다.
- Theme의 소유자를 System/App/Explicit 중 하나로 정한다. 명시 외형 설정이 MaterialApp 자동 색 전달로 즉시 덮어써지지 않게 한다. 앱 테마 추종은 opt-in으로 연결한다.
- 불투명 Scaffold가 아크릴을 가리는 경우를 문서·샘플로 설명한다. runtime 재질 전환 샘플은 window 재질과 해당 UI의 투명도를 함께 변경한다. 현재 SampleApp의 토글은 콘텐츠 opacity 변경이며 window 재질 자체 on/off 검증과 구분한다.
- 아크릴 tint 변경만으로 GPU device/Graphite session을 재생성하지 않는다. 창 전체 opacity는 재질 opacity와 별도 기능으로 후순위에 두고 조합 지원을 검증한다.
- WebGL2 기본/명시 WebGPU 정책, Windows 렌더러 선택, 기존 Flutter `Duration` 계약은 변경하지 않는다.

### 5.5 사용자 정의 상단바

`WindowTitleBar`는 앱의 제목·검색·탭 영역과 native controls 예약 공간을 배치한다. `WindowDragRegion`은 OS 이동을 시작하고 버튼·텍스트 입력·메뉴·WebView 영역을 제외한다. transform·clip·DPI 변경 및 위젯 detach 시 등록 영역을 갱신/해제한다.

Windows는 드래그, 8방향 resize, 더블클릭 최대화/복원, 우클릭 시스템 메뉴, Alt+Space/Alt+F4, native 버튼 hover 및 Snap 동작을 확인한다. Custom 버튼에는 이름·role·상태·키보드 접근을 제공한다. Snap 등 OS 통합을 확인하지 못한 custom 경로를 native와 동등하다고 표시하지 않는다. macOS와 Qt는 해당 OS 동작에 맞게 검증한다.

## 6. 단계별 작업계획

체크박스는 해당 항목 전체의 충족 여부다. 부분 구현은 체크하지 않고 아래에 기록한다. 구현 상태와 실제 검증 범위를 구별한다.

| 단계 | 현재 상태 |
| --- | --- |
| W0 | 계약·인벤토리 문서화 완료. 실제 다중 창은 W6로 분리 |
| W1 | 핵심 구현 및 25개 fake-host 계약 PASS. 한 창의 다중 view/일부 경합 확대 검증 잔여 |
| W2 | Windows 기본 경로와 200% DPI 창 조작 PASS. 최초 노출 전체 프레임 캡처·혼합 DPI 잔여 |
| W3 | Normal+System/Solid/Backdrop 픽셀 회귀 PASS. Hidden/custom/frameless·App theme bridge 미구현 |
| W4 | 비데스크톱 package negative build PASS. WindowsAppSDK/AppKit/Qt adapter 미구현, 명시적 거절 |
| W5 | Testbed/선택형 companion template/이전 문서/package-only Windows 실행 PASS. 커스텀 상단바 예제 등 잔여 |
| W6 | 이번 범위 밖. 실제 다중 네이티브 창 미구현 |

### W0 — 데스크톱 경계·다중 창을 고려한 API 계약 확정

- [x] public API/호출부/target host/SDK/template/validation 인벤토리를 작성한다. `UseView`, 기존 backdrop 메시지, 종료 경로와 최초 show 위치를 추적한다.
- [x] 사용자 Sudoku 설정을 Doroti 예제로 옮기고 크기의 client/frame 의미를 확인한다.
- [x] 옵션·controller·결과·이벤트 타입, 기본값, namespace, 충돌 정책, owner 모델과 지원 조합표를 확정한다.
- [x] Desktop 패키지/전용 startup의 참조 경계를 확정한다. 공유 앱 assembly의 target-neutral 빌드와 양립하는 companion assembly·SDK 등록 경로를 설계한다.
- [x] manager/CreateWindowAsync/WindowId/WindowContent/MainWindow/앱 종료 정책을 확정하고 기본 창·추가 창·창 종료 후 조회 사용 예제를 작성한다.
- [x] 최근 Windows caption 수정과 resize PARTIAL을 실행 기준선으로 기록한다. 이전 산출물이 삭제되었으면 새 검증 결과로 대체하되 과거 PASS를 재사용하지 않는다.
- 완료 기준: 데스크톱 전용 진입점과 기본/추가 창의 일관된 사용 모양이 정해지고, 세 외형 예제 및 지원 불가 조합이 명시됨. 실제 다중 창 구현을 요구하는 단계와 API 설계만 고정하는 항목을 구분함.

### W1 — Desktop 패키지·manager·controller·startup 연결

- [x] `WindowOptions`, 확장 appearance/titlebar 옵션, capability 평가와 effective state를 구현한다.
- [x] 앱 범위 manager와 WindowId registry, 창별 controller/context/content factory, owner dispatch, cancellation/폐기, 비동기 hook과 구독 해제를 구현한다.
- [x] desktop startup의 `UseMainWindow(WindowCreateOptions)` 및 SDK 연결을 추가하고 `UseView` legacy adapter를 기본 창의 단일 생성 경로로 만든다.
- [x] 비데스크톱 빌드에서 새 패키지·startup 참조를 거절하는 diagnostic과 desktop-only source/assembly 구성을 구현한다. 공통 Ui/Hosting에 Desktop 역참조가 없는지 확인한다.
- [ ] fake host 계약 검사로 두 창의 격리, 한 창의 여러 view, 준비 전 호출, 닫힘 중 대기, 예외 전파와 중복 초기화를 확인한다.
- [x] 창 종료 후 ID 미재사용, stale controller 실패, 개별 Close와 앱 종료 분리, 추가 생성 미지원 시 사전 거절을 확인한다.
- 완료 기준: 실제 OS 기능을 허위 지원하지 않는 Desktop 계약과 단일 창 C# 소비 예제. fake-host 다중 창 계약 통과와 네이티브 다중 창 미구현 상태를 명시함.

### W2 — Windows MAUI 기본 창 제어·준비 후 표시

- [x] `DorotiMauiApplication`, platform lifecycle, surface/capability 등록을 연결한다. 초기 옵션 적용 전 자동 표시/활성화를 제어한다.
- [x] 초기 size/min/max/title/background/taskbar/topmost, 위치·표시·포커스·최대화·복원·닫기 API와 이벤트를 구현한다.
- [x] 첫 frame readiness와 미리 생성해야 하는 caption 정책을 분리한다. hook을 await하느라 framework bootstrap이 막히지 않게 한다.
- [x] 기본 창 생성·종료도 manager를 경유시킨다. window.Closed에서 앱 전체를 즉시 Exit하는 경로와 window/view ID 고정값을 조사해 앱 종료 정책·식별자 할당 경계로 옮긴다.
- [ ] Sudoku의 `450×800`, 최소 `350×500` 예제를 실제 창 측정과 표시 시작 캡처로 검증한다.
- 완료 기준: 기존 옵션 누락 없이 단순 예제가 동작하고, 시작 시 기본 외형 노출·무한 대기·중복 caption이 없음.

### W3 — Windows MAUI 아크릴·상단바 조합과 실행 중 변경

- [x] `WindowsWindowBackdrop`와 `WindowsNativeCaption`이 동일한 appearance snapshot을 소비하도록 묶는다. `solid/unified` 기존 입력을 각각 Solid/Backdrop에 매핑한다.
- [ ] tint/kind/theme/fallback 갱신, nullable 기본값 복원, native caption 정책 복구, 활성·고대비 상태를 연결한다.
- [x] Normal+System/Solid/Backdrop 조합을 구현하고 실제 caption/body 색상 응답을 각각 검사한다.
- [ ] Hidden+Native buttons를 구현하고 chrome metrics·native drag/hit testing을 framework에 전달한다.
- [ ] WindowTitleBar/DragRegion/CaptionButtons 위젯을 추가한다. Custom buttons/Frameless는 필수 OS 동작·접근성 검증이 끝난 조합만 공개 지원한다.
- [ ] 연속 외형 요청, resize 중 요청, 적용 중 close를 bounded 검증한다. 지원하지 않는 runtime mode/frame 변경은 RequiresRecreation으로 종결한다.
- 완료 기준: 세 대표 사용 예제가 화면·입력 검증을 통과하며, 단지 API 속성이 적용됐다는 이유로 시각 성공을 판정하지 않음.

### W4 — 다른 데스크톱 host와 비데스크톱 경계

- [ ] WindowsAppSDK runner/native ABI에 공통 command/event 연결을 추가한다. 기존 AcrylicOptionsState의 validation/revision을 재사용하고 공용 controller가 문자열 JSON을 요구하지 않게 한다.
- [ ] macOS MAUI/AppKit에 기본 창 조작·Acrylic/Liquid Glass·제목 표시줄 조합을 연결한다. MacCatalyst를 AppKit과 자동 동등 취급하지 않는다.
- [ ] Qt managed/native/QML과 앱 template에 상태·크기·OS drag/resize·caption 경로를 연결한다. X11/Wayland별 지원을 분리한다.
- [ ] Linux의 기존 in-app blur를 뒤쪽 데스크톱 Acrylic으로 광고하지 않는다. compositor 의존 재질은 검증된 capability만 제공한다.
- [ ] Web/Android/iOS 빌드에 Desktop 패키지·startup·위젯 assembly가 포함되지 않는지 검사하고 잘못된 참조는 실패시킨다. 데스크톱 embedded MAUI는 host가 명시적으로 연결한 창 controller만 사용하도록 별도 검증한다.
- 완료 기준: 동일 C# 계약을 사용하되 플랫폼별 구현·미지원·미검증 항목이 구별된 표와 실행 근거.

### W5 — 샘플·이전·패키지·문서

- [ ] Testbed와 template에 일반 창, native Acrylic 창, 사용자 정의 검색 상단바 창을 추가한다.
- [ ] startup와 runtime 옵션 변경, 테마 추종, close 취소, 이벤트 해제 예제를 제공한다.
- [ ] 이전 API→새 API 매핑, 옵션 우선순위와 호환 기간을 문서화한다. 기존 내부 Acrylic 채널은 같은 controller 적용 경로로 위임한다.
- [x] 저장소 밖 앱에서 로컬 package restore/build 및 Windows 실행을 확인한다. SDK 생성물과 template 경로가 source project 참조에 의존하지 않게 한다.
- [x] 공통 앱+desktop companion 구성 예제와 후속 추가 창 생성 예제를 제공한다. 다중 창 예제는 해당 기능이 실제 지원되기 전까지 목표 API 예제로 표시한다.
- [x] 최종 지원표와 각 gate의 PASS/PARTIAL/notVerified를 기록하고, 확인되지 않은 host를 지원 완료로 표시하지 않는다.
- 완료 기준: 사용자 앱 코드에 HWND/host-specific 분기가 없어도 지원되는 세 예제를 사용할 수 있음.

의존 순서: **W0 → W1 → W2 → W3 → W4 → W5**. W3의 커스텀 창은 기본 native caption 경로와 별도로 검증한다. W4의 Qt 변경은 [Linux Qt 보관 작업](history/26-09-24/linux-qt-improvements-summary.md)의 GPU·입력·접근성 수정과 충돌을 확인하며, 그 계획을 이번 창 API 작업 완료로 대체하지 않는다.

### W6 — 후속 다중 네이티브 창 구현 (이번 첫 구현 범위 밖)

- [ ] manager의 동일한 CreateWindowAsync 계약을 이용해 두 개 이상 OS 창을 생성한다. host별 session/dispatcher 공유·분리 방식을 정하고 공용 자원의 수명을 분리한다.
- [ ] 각 창의 콘텐츠 factory를 실제 view에 연결한다. 단일 view를 가정하는 sample entrypoint, 고정 ID, native ABI의 창 식별·callback routing을 다중 창에 맞춘다.
- [ ] 창마다 다른 caption/Acrylic/size/theme, focus/IME/접근성, 동시에 진행되는 resize·close·생성을 검증한다.
- [ ] 기본 창을 먼저 닫아도 다른 독립 창이 계속 동작하고, 마지막 창 종료 정책과 취소가 정확히 동작하는지 검증한다. Explicit 정책의 창 0개 상태에서 다시 생성하는 경로도 확인한다.
- [ ] owner 관계·modal은 위 기본 다중 창과 별도 capability로 구현·검증한다. renderer 성능·메모리 증가는 창별/공용으로 나누어 측정한다.
- 완료 기준: 같은 공개 API로 실제 두 창 이상이 독립 동작하는 host만 다중 창 지원으로 표시. 완료 전에는 W1의 fake-host 결과를 네이티브 다중 창 PASS로 표시하지 않음.

## 7. 호환·이전 규칙

| 기존 API | 이전안 |
| --- | --- |
| `UseView(DorotiViewConfiguration)` | legacy 기본 창 생성 adapter로 유지. 새 desktop startup은 `UseMainWindow(WindowCreateOptions)`로 이전 |
| `title/logicalSize` | `WindowOptions.Title/Size`; 기존 host별 size 의미는 변환·검증 후 이전 |
| `backgroundColor/darkBackgroundColor` | Appearance의 명시 배경 및 테마별 값으로 이전 |
| 직접 `backdrop`와 `appearance.backdrop` | 기존 appearance 우선 규칙을 legacy 입력에 유지. 새 API에서는 Backdrop 한곳만 사용 |
| `WindowTitlebarStyle.unified/solid` | Normal + Backdrop / Normal + Solid. OS별 기존 외형 차이는 문서화 |
| `macOSBackdrop` | typed macOS appearance override에 이전하며 기존 Liquid Glass 하위 OS 동작 보존 |
| `SingletonDorotiWindow` | 기존 view metrics facade 유지. 새 창 controller와 역할 분리 |
| `terminateAfterLastWindowClosed` | manager의 `WindowLifetimePolicy`로 이전. 개별 창 옵션에서 앱 전체 종료를 결정하지 않음 |
| `IWindowTitlebarHostCapability.SetTheme` | App-theme bridge로 유지하거나 adapter 제공. 명시적 Appearance보다 우선하지 않음 |
| 내부 Windows Acrylic 채널 | 전환 기간 같은 적용 파이프라인으로 위임, 새 앱은 typed API 사용 |

새 desktop `UseMainWindow`와 legacy 설정이 같은 기본 창의 옵션·콘텐츠를 동시에 지정하면 충돌 필드를 diagnostic과 함께 거절한다. 값이 우연히 같다는 이유로 두 소유자를 유지하지 않는다. 공통 앱의 Web/mobile 설정과 desktop companion의 창 생성 설정은 다른 target의 선언으로 구분해 잘못 충돌 처리하지 않는다. 새 API 미사용 앱은 기존 경로를 유지한다. Obsolete 표시는 실제 소비처·warnings-as-errors 대응과 package/template 전환이 준비된 단계에서 추가한다. 즉시 제거하거나 전체 Framework public API를 일괄 변경하지 않는다.

## 8. 검증 계획과 완료 판정

실행한 검사는 [Desktop 실행 결과](Doroti/validation/desktop-window/README.md)에 기록했다. 아래 표는 전체 수락 기준이며 아직 모두 통과한 것은 아니다. 각 명령은 `python Doroti/validation/run-with-timeout.py <command>`로 실행한다. 제한 시간은 1,200초이며 공유 `obj` 빌드는 직렬 실행한다. 케이스별 반복은 통상 30회 이내로 제한한다.

| Gate | 필수 확인 | 합격 근거 |
| --- | --- | --- |
| 데스크톱 경계 | Windows/macOS/Linux desktop-only 참조, Web/Android/iOS 잘못된 package/startup 참조, 공통 패키지의 역참조 | 지원 target 소비 예제와 비데스크톱 negative build. 런타임 no-op을 합격으로 인정하지 않음 |
| 계약 | 옵션 충돌·불법 조합, 2개 창 격리, 준비/취소/닫기 경쟁, snapshot 대체, callback 예외 | 의미 있는 fake-host 계약 검사, 각 요청 정확히 한 번 종결 |
| 다중 창 준비 | manager lifetime, WindowId 미재사용, 생성 실패 rollback, 기본 창/추가 창의 동일 경로, 개별/앱 종료 구분 | W1 fake-host 두 창 검증. W6 전까지 실제 네이티브 다중 창은 미구현으로 기록 |
| 최초 표시 | 옵션 적용 전 창 노출, 첫 geometry, 최초 frame, hook 실패 | Windows 실제 표시 캡처와 요청/적용 상태 기록 |
| Sudoku 동등 사용 | 초기 client 450×800, 최소 350×500, normal caption, 작업 표시줄, topmost 해제, show/focus | DPI를 반영한 실제 bounds/state와 OS 동작 |
| 외형 조합 | native/custom × solid/acrylic, caption/body 재질 구별, tint 기본값 복원 | 통제 배경 픽셀 비교와 원본 캡처. 투명 pixel만으로 데스크톱 blur 성공 판정 금지 |
| 사용자 정의 입력 | drag 제외 검색창·버튼, resize, 더블클릭·메뉴·시스템 키, focus/IME | OS 입력 및 접근성 검사. native controls와 custom controls를 구별 |
| 동적 변경 | 외형 변경 중 resize/최대화/close, 마지막 요청 적용, renderer 수명 | revision/종결 결과 + 실제 표시 캡처. generation 일치만으로 시각 PASS 금지 |
| 환경·fallback | light/dark, inactive, 고대비, transparency off, 100/150/200% DPI와 혼합 모니터 | 환경별 적용 상태. 확보 못한 환경은 notVerified |
| host·배포 | MAUI/raw Windows, AppKit, Qt X11/Wayland, Web/mobile 경계, package/template | 각 실행 환경을 따로 기록하고 빌드 PASS를 실행 PASS로 확장하지 않음 |

기존 [Windows MAUI 검증](Doroti/validation/windows-maui/README.md)의 `verify-acrylic.py`, `verify-drag.py`, `verify-resize.py`, `verify-flicker.py`를 필요한 범위에서 확장한다. 커스텀 caption에 기존 native-caption ROI를 그대로 사용하지 않고, 스타일별 위치·버튼·inset 기준을 검증한다. 시작/종료 시나리오와 실제 material on/off 검사는 별도로 추가한다.

예정 실행 예:

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-drag.py
```

기존 resize 시각 gate가 계속 실패하면 창 API의 통과 항목과 별개로 전체 resize 연속성은 PARTIAL로 유지한다. `Doroti/artifacts`의 원시 캡처는 삭제 가능한 산출물이므로 최종 문서에는 실행 조건·요약·바이너리 식별 정보를 남기고, 삭제된 파일을 남아 있는 증거로 링크하지 않는다.

## 9. 이번 검토의 결론

**데스크톱 전용 패키지·startup → 앱 범위 WindowManager → 창별 WindowController와 콘텐츠 factory**를 기본 구조로 확정한다. 기본 창은 `UseMainWindow`, 향후 추가 창은 `CreateWindowAsync`로 만들고, 생성 이후에는 같은 조작·외형·이벤트 API를 사용한다. 이를 통해 사용자가 편하게 썼던 선언·준비·표시 흐름과 아크릴·커스텀 상단바 조합을 유지하면서, 나중에 다중 창을 추가할 때 전역 단일 창 API를 다시 걷어내는 일을 피한다.

2026-09-25 전체 작업 요청으로 제품 구현과 bounded 검증을 진행했다. 외부 Sudoku 프로젝트는 수정하지 않았다. 미완료 항목을 체크한 것으로 처리하지 않으며, 다음 구현 지점은 Hidden+Native chrome/입력·위젯, App theme bridge, WindowsAppSDK/AppKit/Qt adapter와 내부 Acrylic 채널 위임이다. 기존 resize 시각 연속성은 PARTIAL을 유지한다.

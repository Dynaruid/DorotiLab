# Doroti single-project + MAUI GPU shell 개편 계획

## 0. 결정과 범위

- 상태: 검토와 작업계획만 작성했다. 아직 template, runtime, target package, DemoApp 구현은 변경하지 않았다.
- 생성 결과에는 `.csproj`를 하나만 둔다.
- 공용 Doroti 앱은 `src/App.cs`, 공용 진입점은 프로젝트 루트 `Program.cs`가 소유한다.
- Windows와 macOS 계열 shell은 .NET MAUI로 교체한다.
- MAUI 화면은 XAML 없이 순수 C#으로 구성한다. 생성 결과에 `App.xaml`, `MainPage.xaml`, `AppShell.xaml`과 `InitializeComponent()` 의존성을 두지 않는다.
- GPU surface는 `SkiaSharp.Views.Maui.Controls.SKGLView` 3.119.4를 사용하고 `SKCanvasView` software fallback은 두지 않는다.
- MAUI의 Mac desktop target은 AppKit `osx-arm64`가 아니라 Mac Catalyst이므로 새 계약은 `net10.0-maccatalyst` / `maccatalyst-arm64`로 명명한다.
- Web은 기존 Blazor WebAssembly/WebGL2 host를 유지하되 같은 root project와 `src/App.cs`를 사용한다.
- Android/iOS는 이번 작업에서 생성하거나 지원한다고 주장하지 않는다.

## 1. 검토 결과

### 확인된 MAUI/SkiaSharp 계약

- `SkiaSharp.Views.Maui.Controls` 3.119.4 package는 .NET 10 Windows와 Mac Catalyst asset을 포함하고 `Microsoft.Maui.Controls/Core` 10.0.20에 대응한다.
- `UseSkiaSharp()`는 `SKCanvasViewHandler`와 `SKGLViewHandler`를 등록한다.
- Windows `SKGLViewHandler`의 native view는 WinUI 3 `SKSwapChainPanel`이며 GPU surface와 `GRContext`를 제공한다.
- Mac Catalyst `SKGLViewHandler`의 native view는 `SKMetalView`이며 Metal-backed surface와 `GRContext`를 제공한다.
- `HasRenderLoop=false`일 때 `InvalidateSurface()`로 demand-driven frame을 요청할 수 있다. Doroti scheduler는 이 모드를 기본으로 사용하고 무조건 continuous loop를 켜지 않는다.
- `PaintSurface`는 Doroti가 그릴 `SKSurface`, backend render target, pixel size를 제공하므로 MAUI adapter가 WGL/NSOpenGL context를 직접 생성하지 않는다.

### XAML 없는 구성 결론

- shell은 `Application -> Window -> ContentPage -> SKGLView` 하나이므로 CommunityToolkit C# Markup 없이 일반 C# object initializer가 가장 작다.
- `CommunityToolkit.Maui.Markup`은 복잡한 native overlay/layout을 만드는 기능이며 현재 full-surface Doroti view에는 필수 dependency가 아니다.
- 공용 MAUI application은 `Application.CreateWindow()`를 override해 C#으로 `Window`와 `ContentPage`를 만든다.
- Windows는 XAML compiler가 만들던 WinUI entrypoint 동작을 root `Program.cs -> PlatformBootstrap` C# 코드로 명시한다. WinRT COM wrapper 초기화, dispatcher synchronization context, `Microsoft.UI.Xaml.Application.Start`를 XAML 없이 구성해야 한다.
- Mac Catalyst는 root `Program.cs -> PlatformBootstrap`이 `UIApplication.Main`과 `MauiUIApplicationDelegate`를 연결한다.
- `Package.appxmanifest`, `Info.plist`, `Entitlements.plist`는 native packaging manifest이지 XAML UI가 아니므로 유지한다.

### 기존 계약에서 바뀌는 부분

- Windows graphics identity는 `win32/wgl-opengl-skia`에서 `winui3/SKSwapChainPanel/ANGLE-DirectX-Skia` 계열로 바뀐다.
- Mac graphics identity는 `AppKit/NSOpenGL`에서 `UIKit-MacCatalyst/SKMetalView/Metal-Skia`로 바뀐다.
- 현재 `DesktopWindowBackend`는 창과 OpenGL context를 직접 소유한다. MAUI에서는 창/surface를 framework가 소유하므로 단순 factory 교체가 아니라 host ownership을 역전한 `Doroti.Host.Maui` adapter가 필요하다.
- `SKGLView.Touch`만으로는 desktop hover, wheel, keyboard, IME, cursor, clipboard, 전체 semantics tree를 충족하지 못한다. 플랫폼 native view/handler에서 해당 capability를 별도로 연결해야 한다.
- 기존 Windows WGL 및 macOS NSOpenGL PASS는 predecessor evidence로 보존하되 MAUI shell PASS로 승계하지 않는다.

## 2. 확정할 생성 구조

`SampleApp` 생성 예시는 다음과 같다.

```text
SampleApp/
├─ SampleApp.csproj
├─ Program.cs
├─ packages.windows.lock.json
├─ packages.maccatalyst.lock.json
├─ packages.web.lock.json
├─ src/
│  └─ App.cs
├─ Resources/
│  ├─ AppIcon/
│  └─ Splash/
└─ Platforms/
   ├─ Maui/
   │  ├─ MauiProgram.cs
   │  ├─ DorotiMauiApplication.cs
   │  ├─ DorotiMauiPage.cs
   │  └─ DorotiSkiaView.cs
   ├─ Windows/
   │  ├─ PlatformBootstrap.cs
   │  ├─ WindowsMauiApplication.cs
   │  ├─ Package.appxmanifest
   │  ├─ app.manifest
   │  └─ application-manifest.json
   ├─ MacCatalyst/
   │  ├─ PlatformBootstrap.cs
   │  ├─ AppDelegate.cs
   │  ├─ Info.plist
   │  ├─ Entitlements.plist
   │  └─ application-manifest.json
   └─ Web/
      ├─ PlatformBootstrap.cs
      ├─ application-manifest.json
      └─ wwwroot/
         ├─ index.html
         ├─ doroti-app-manifest.json
         ├─ assets/
         ├─ locales/
         └─ plugins/
```

### 파일 소유권

- `src/App.cs`: user-owned Doroti widget/state와 target-neutral `App.Definition`만 둔다. MAUI, WinUI, UIKit, Blazor type과 `#if`를 넣지 않는다.
- root `Program.cs`: `PlatformBootstrap.RunAsync(args, App.Definition)`만 호출한다.
- `Platforms/Maui/*`: Windows/Mac Catalyst가 공유하는 MAUI builder, C# Application/Window/Page, `SKGLView` bridge를 둔다.
- `Platforms/Windows/*`: WinUI application start, native input/IME/UIA/manifest를 둔다.
- `Platforms/MacCatalyst/*`: UIKit application start, delegate, native input/text/accessibility/plist를 둔다.
- `Platforms/Web/*`: 기존 Blazor/WebGL2 composition root와 static web assets를 둔다.
- target별 `application-manifest.json` 하나만 `Doroti.Application.Manifest` logical resource로 embed한다.

## 3. single-project build 계약

- `Doroti.App.Sdk` project SDK가 한 `.csproj`의 target selection과 SDK import를 소유한다.
- target 선택 우선순위는 명시적 `DorotiTarget`, 명시적 RID, host OS desktop 기본값 순이다.
- target mapping은 다음으로 고정한다.
  - `DorotiTarget=Windows`: `net10.0-windows10.0.19041.0`, `win-x64`, `UseMaui=true`
  - `DorotiTarget=MacCatalyst`: `net10.0-maccatalyst`, `maccatalyst-arm64`, `UseMaui=true`
  - `DorotiTarget=Web`: `net10.0`, `browser-wasm`, Blazor WebAssembly SDK
- `Program.cs`와 `src/**/*.cs`는 항상 compile한다.
- Windows/Mac Catalyst는 `Platforms/Maui/**/*.cs`와 선택 target 폴더만 compile하고 Web source/static asset을 제외한다.
- Web은 `Platforms/Web/**/*.cs`만 compile하고 MAUI package/source/resource를 graph에서 제외한다.
- XAML item은 0개여야 하며 `MauiXaml`, `ApplicationDefinition`, generated `*.g.cs` entrypoint에 의존하지 않는다.
- `BaseIntermediateOutputPath`, `OutputPath`, publish state, `NuGetLockFilePath`는 target별로 분리한다.
- target/RID 충돌, 지원하지 않는 platform, MAUI workload 미설치는 stable `DOROTIAPPxxx` 진단으로 fail closed한다.

기본 명령:

```powershell
dotnet run --project .\SampleApp.csproj -p:DorotiTarget=Windows
dotnet publish .\SampleApp.csproj -c Release -p:DorotiTarget=MacCatalyst -r maccatalyst-arm64
dotnet publish .\SampleApp.csproj -c Release -p:DorotiTarget=Web
```

## 4. MAUI host 설계

### GPU/frame bridge

- 새 `Doroti.Host.Maui`가 `SKGLView.PaintSurface`에서 전달된 `SKSurface`에 기존 Doroti scene/display-list를 그린다.
- surface/context를 외부 host가 소유하는 Web 경로와 공통 raster contract를 추출하고, MAUI adapter가 desktop OpenGL context를 재생성하지 않게 한다.
- Doroti frame request를 MAUI dispatcher에서 `InvalidateSurface()`로 coalesce한다.
- `HasRenderLoop=false`를 기본으로 하고 animation/vsync cadence가 부족할 때만 측정 근거를 통해 scheduling을 보강한다.
- `CanvasSize`, device density, logical/pixel transform, surface origin을 frame diagnostics에 기록한다.
- `GRContext` 교체, window minimize/restore, display/DPI 변경, app suspend/resume 뒤 surface generation을 갱신하고 stale frame을 폐기한다.
- strict mode에서는 `SKCanvasView`나 CPU framebuffer로 fallback하지 않는다.

### 입력/텍스트/접근성

- 공통 `SKGLView.Touch`는 기본 press/move/release bridge로만 사용한다.
- Windows handler는 `SKSwapChainPanel`의 pointer hover/down/move/up/cancel/wheel/capture, keyboard, focus를 Doroti raw input으로 정규화한다.
- Mac Catalyst handler는 `SKMetalView`/UIKit의 pointer, hover, scroll, key/focus event를 같은 Doroti input contract로 정규화한다.
- IME는 Windows CoreText/WinUI text input 경로와 Mac Catalyst UIKit text input 경로를 각각 연결하고 composing range/caret geometry를 검증한다.
- clipboard/cursor/window placement/native handle은 MAUI Essentials 또는 platform API를 adapter 뒤에 둔다.
- semantics tree는 `SKGLView` 단일 label로 축소하지 않는다. Windows AutomationPeer/provider와 Mac Catalyst UIAccessibility element tree로 Doroti semantics node/action/focus/bounds를 노출한다.

### C# application 구성

- `MauiProgram.CreateMauiApp`는 `UseMauiApp<DorotiMauiApplication>()`, `UseSkiaSharp()`, Doroti definition/host service 등록만 수행한다.
- `DorotiMauiApplication.CreateWindow`는 C#으로 `Window(new DorotiMauiPage(...))`를 반환한다.
- `DorotiMauiPage`는 `Content = new DorotiSkiaView(...)`로 full-surface view를 구성한다.
- generated shell에 NavigationPage, Shell, XAML resource dictionary, MAUI widget UI를 추가하지 않는다.
- C# Markup은 baseline dependency에서 제외한다. 향후 native toolbar/overlay가 실제 요구될 때 별도 opt-in으로 평가한다.

## 5. 작업 단계

### M0. XAML-free MAUI + SKGLView feasibility gate

- repository 밖 fixture에서 `SkiaSharp.Views.Maui.Controls` 3.119.4와 MAUI 10.0.20을 고정한다.
- `.csproj` 하나, root `Program.cs`, C# Application/Window/Page, Windows/Mac Catalyst platform source만으로 XAML 없이 build되는 최소 앱을 만든다.
- Windows에서 `UseSkiaSharp()` handler 등록, `SKGLView`, non-null `GRContext`, GPU-backed `PaintSurface`, invalidate-driven 반복 frame을 실제 창으로 증명한다.
- Mac arm64에서 같은 fixture가 `SKMetalView`/Metal surface로 build/publish/present되는지 별도 증명한다.
- Windows XAML generated Main을 제거한 수동 entrypoint와 Mac Catalyst `UIApplication.Main`이 debugger/Release publish 모두에서 정상인지 확인한다.

완료 조건:

- 생성/fixture XAML 파일과 XAML compile item이 0개다.
- Windows `SKSwapChainPanel`, Mac Catalyst `SKMetalView` backend identity가 실제 runtime에서 확인된다.
- CPU fallback 0, non-empty GPU frame, resize/DPI 반영, clean shutdown이 확인된다.
- 실패 시 기존 shell로 되돌려 완료 처리하지 않고 MAUI feasibility를 미완료로 유지한다.

### M1. Doroti.App.Sdk의 MAUI/Web target selection

- custom project SDK가 MAUI Windows, MAUI Mac Catalyst, Blazor WebAssembly SDK를 target별로 import하게 한다.
- 한 project에서 `Windows -> Web -> Windows` 연속 clean/incremental build를 수행해 package/source/obj 오염이 없는지 확인한다.
- Mac Catalyst graph/lock은 Mac runner에서 생성하고 locked restore identity를 확인한다.
- 선택하지 않은 `Platforms/*` compile/resource/static asset이 0개인지 MSBuild binlog/diagnostic target으로 검사한다.

### M2. Doroti.Host.Maui GPU composition

- `SKSurface` 제공형 raster contract를 분리하고 Web/MAUI가 공유하게 한다.
- `DorotiSkiaView`와 session/view/frame lifecycle adapter를 구현한다.
- invalidate coalescing, frame terminal state, context/surface generation, readback/resource diagnostics를 구현한다.
- Windows ANGLE/DirectX와 Mac Catalyst Metal을 서로 다른 backend identity로 기록한다.

### M3. desktop capability closure

- pointer/hover/wheel/capture/drag, keyboard/focus, cursor를 platform handler에 연결한다.
- IME composing/commit/caret, clipboard, window resize/DPI/display 이동을 연결한다.
- Windows UIA와 Mac Catalyst UIAccessibility semantics/action bridge를 구현한다.
- 기존 synthetic API가 MAUI native event를 실제로 통과하도록 하며 widget state 직접 호출로 대체하지 않는다.

### M4. MAUI target package 전환

- `Doroti.Target.Windows.Maui.win-x64`와 `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64` package를 추가한다.
- 기존 Win32/AppKit target과 한시적으로 differential run을 수행해 visual/input/compositing/resource 결과를 비교한다.
- MAUI target이 acceptance gate를 통과하면 template/default target을 전환한다.
- 전환 뒤 `Doroti.Vendor.Avalonia.Win32`, `Doroti.Vendor.Avalonia.Native`, old target package와 source-port 전용 경로를 제거한다.
- predecessor evidence는 history로 보존하고 현재 product manifest/evidence는 새 backend/RID로 다시 생성한다.

### M5. doroti-app template 이관

- 기존 `App/*.csproj`, `BrowserHost/`, 별도 application project를 제거한다.
- 확정한 single-project/C#/`Platforms/*` 구조를 template content로 만든다.
- Windows/Mac Catalyst app icon, splash, packaging manifest를 추가한다.
- source-name 치환 후 assembly/application ID/bundle ID/Web manifest가 일치하게 한다.
- 생성 결과에 `.csproj` 1개, XAML 0개, user Razor 0개, Flutter/Dart scaffold 0개를 강제한다.

### M6. DorotiDemoApp dogfood 이관

- `DorotiDemoApp.Web.csproj`를 제거하고 `DorotiDemoApp.csproj` 하나로 통합한다.
- 공용 Material gallery/widget/state를 `src/App.cs`와 `src/*`로 분리한다.
- root `Program.cs`를 template과 같은 thin bootstrap으로 축소한다.
- Demo 전용 smoke/evidence 코드는 generated template에 유입시키지 않고 platform validation adapter로 분리한다.
- 기존 “same Program.cs source” evidence를 “same `src/App.cs` + same root bootstrap” 계약으로 갱신한다.

### M7. validator/evidence/docs 동기화

- `validate-g7-web-build.ps1`의 Graph/Template/Compile/Publish를 single-project target selector에 맞춘다.
- `validate-g6-baseline.ps1`, `validate-g6-generated-demo.ps1`, `validate-g6-material-demo.ps1`, `validate-g7-baseline.ps1`, `validate-g7-product.ps1`, `validate-g7-macos-shell.ps1`의 old project/source/backend 가정을 바꾼다.
- evidence에는 `applicationSource`, `bootstrapSource`, `targetFramework`, `rid`, `mauiVersion`, `skiaSharpVersion`, native view type, graphics backend, context/surface generation을 기록한다.
- root/DemoApp English/Korean README를 함께 갱신한다.
- Goal7에서 old Windows/macOS evidence와 new MAUI evidence를 분리하고 실행하지 않은 physical/cross-target gate는 `notVerified`로 둔다.

## 6. 검증 매트릭스

모든 test/validator timeout은 저장소 지침에 따라 20분으로 설정한다.

### 구조/package-only

- 외부 임시 디렉터리에서 template pack/install/create/locked restore/build/publish.
- project 수 1, XAML 0, `InitializeComponent` 0, 선택되지 않은 platform compile item 0.
- promoted package feed만 사용하고 repository-private project/path fallback 0.
- target 전환 반복 시 assets/lock/output identity와 platform leakage 검사.

### Windows MAUI

- `net10.0-windows10.0.19041.0` / `win-x64` Release build/publish.
- 실제 WinUI 3 window와 `SKSwapChainPanel` GPU surface 확인.
- mount/layout/paint/present, resize/DPI, context recreate, pointer/hover/wheel/keyboard/IME/clipboard/cursor/UIA.
- frame cadence/latency, zero failed/software-fallback frames, balanced surface/window/native resources.

### Mac Catalyst MAUI

- Mac arm64에서 `net10.0-maccatalyst` / `maccatalyst-arm64` restore/build/publish/signing-free local run.
- 실제 UIKit window와 `SKMetalView`/Metal GPU surface 확인.
- resize/scale, pointer/scroll/keyboard/text/clipboard/accessibility, suspend/resume/context recreate.
- 기존 `osx-arm64` AppKit 결과와 구분하며 Mac Catalyst live run 전에는 PASS로 기록하지 않는다.

### Web

- 기존 strict WebGL2/Blazor compile/publish/repeat identity/package-only gate 유지.
- Web assets에 MAUI/WinUI/UIKit/SKSwapChainPanel/SKMetalView/desktop native package가 0개인지 검사.
- MAUI 성공을 Web interaction/physical PASS로 간주하지 않는다.

### 회귀/마감

- 관련 shared compiler/framework validator.
- Windows MAUI live, Web build/publish, Mac Catalyst native gate를 각각 실행.
- 변경 파일 대상 format 검사와 `git diff --check`.
- template, DemoApp, validators, evidence, English/Korean docs의 경로/명령/backend identity 최종 검색.

## 7. 비범위

- MAUI controls로 Doroti widget UI를 다시 만드는 작업.
- XAML, AppShell, NavigationPage 기반 앱 구조.
- `SKCanvasView` software fallback.
- 이번 단계의 Android/iOS target 추가.
- 기존 Win32/AppKit shell을 영구 fallback으로 유지하는 작업. differential/전환 기간 뒤 제거한다.
- C# Markup package 기본 포함. native overlay 요구가 생기면 별도 opt-in으로 검토한다.

## 8. 외부 계약 확인 자료

- .NET MAUI single project: https://learn.microsoft.com/dotnet/maui/fundamentals/single-project
- .NET MAUI supported/runtime platforms: https://learn.microsoft.com/dotnet/maui/what-is-maui
- SKGLView API: https://learn.microsoft.com/dotnet/api/skiasharp.views.maui.controls.skglview
- SkiaSharp 3.119.4 handler registration/source (package commit): https://github.com/mono/SkiaSharp/tree/f568ac94dd768ef9a2f593537cfde2dd0d348ef5/source/SkiaSharp.Views.Maui
- C# Markup 검토 자료: https://learn.microsoft.com/dotnet/communitytoolkit/maui/markup/markup

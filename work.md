# Doroti single-project + MAUI GPU shell 개편 계획

## 0. 결정과 범위

- 상태(2026-08-16): M0 Windows feasibility는 `PASS`, Mac Catalyst native feasibility는 `notVerified`다. M1~M7의 single-project SDK, MAUI host, target package, template, DemoApp, Web validator, Windows live validator와 문서 이관을 구현했다. Windows MAUI와 Web build/publish는 `PASS`지만 desktop capability M3, Mac Catalyst native 실행, differential removal, physical/cross-target gate가 남아 전체 전환은 `PARTIAL`이다.
- 생성 결과에는 `.csproj`를 하나만 둔다.
- 공용 Doroti 앱은 `src/App.cs`, 공용 진입점은 프로젝트 루트 `Program.cs`가 소유한다.
- Windows와 macOS 계열 shell은 .NET MAUI로 교체한다.
- MAUI 화면은 CommunityToolkit C# Markup과 순수 C#으로 구성한다. Windows만 WinUI theme resource/generated entrypoint 초기화용 빈 `Platforms/Windows/App.xaml`과 그 `InitializeComponent()` 1회를 허용하며 `MainPage.xaml`, `AppShell.xaml`, XAML style/resource UI는 두지 않는다.
- GPU surface는 `SkiaSharp.Views.Maui.Controls.SKGLView` 3.119.4를 사용하고 `SKCanvasView` software fallback은 두지 않는다.
- MAUI의 Mac desktop target은 AppKit `osx-arm64`가 아니라 Mac Catalyst이므로 새 계약은 `net10.0-maccatalyst` / `maccatalyst-arm64`로 명명한다.
- Web은 기존 Blazor WebAssembly/WebGL2 host를 유지하되 같은 root project와 `src/App.cs`를 사용한다.
- Android/iOS는 이번 작업에서 생성하거나 지원한다고 주장하지 않는다.

## 1. 검토 결과

### 확인된 MAUI/SkiaSharp 계약

- `SkiaSharp.Views.Maui.Controls` 3.119.4 package는 .NET 10 Windows와 Mac Catalyst asset을 포함한다. fixture는 `Microsoft.Maui.Controls` 10.0.90과 `CommunityToolkit.Maui.Markup` 8.0.0을 고정한다.
- `UseSkiaSharp()`는 `SKCanvasViewHandler`와 `SKGLViewHandler`를 등록한다.
- Windows `SKGLViewHandler`의 native view는 WinUI 3 `SKSwapChainPanel`이며 GPU surface와 `GRContext`를 제공한다.
- Mac Catalyst `SKGLViewHandler`의 native view는 `SKMetalView`이며 Metal-backed surface와 `GRContext`를 제공한다.
- `HasRenderLoop=false`일 때 `InvalidateSurface()`로 demand-driven frame을 요청할 수 있다. Doroti scheduler는 이 모드를 기본으로 사용하고 무조건 continuous loop를 켜지 않는다.
- `PaintSurface`는 Doroti가 그릴 `SKSurface`, backend render target, pixel size를 제공하므로 MAUI adapter가 WGL/NSOpenGL context를 직접 생성하지 않는다.

### C# UI와 Windows bootstrap 구성 결론

- shell은 `Application -> Window -> ContentPage -> SKGLView`를 C#으로 만들고 `CommunityToolkit.Maui.Markup`을 생성 shell의 C# UI 계약으로 등록한다. 현재 full-surface view는 일반 object initializer만으로도 표현되지만 이후 native overlay/layout도 같은 XAML-free UI 경로를 사용한다.
- `UseMauiCommunityToolkitMarkup()`은 C# UI/바인딩 helper와 Hot Reload 연결을 제공할 뿐 WinUI application resource bootstrap을 대체하지 않는다.
- 공용 MAUI application은 `Application.CreateWindow()`를 override해 C#으로 `Window`와 `ContentPage`를 만든다.
- Windows는 빈 `Platforms/Windows/App.xaml`을 유일한 `ApplicationDefinition`으로 컴파일해 WinUI generated entrypoint와 theme resource bootstrap을 사용하고, code-behind는 `MauiProgram.CreateMauiApp()`만 연결한다. 공용/화면 UI는 이 XAML에 넣지 않는다.
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
   │  ├─ App.xaml
   │  ├─ App.xaml.cs
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
- root `Program.cs`: generated entrypoint가 없는 Mac Catalyst/Web target에서 `PlatformBootstrap.RunAsync(args, App.Definition)`만 호출한다. Windows는 `App.xaml` generated Main이 공용 `MauiProgram`을 호출한다.
- `Platforms/Maui/*`: Windows/Mac Catalyst가 공유하는 MAUI builder, C# Application/Window/Page, `SKGLView` bridge를 둔다.
- `Platforms/Windows/*`: 빈 bootstrap `App.xaml`, 얇은 code-behind, native input/IME/UIA/manifest를 둔다. `App.xaml`에는 UI/resource dictionary를 추가하지 않는다.
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
- Windows는 `Platforms/Windows/App.xaml` `ApplicationDefinition`과 그 generated entrypoint를 정확히 1개 허용한다. `MauiXaml`, 다른 `ApplicationDefinition`, page/shell/style XAML은 0개여야 하며 Mac Catalyst/Web에는 XAML compile item이 0개여야 한다.
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

- `MauiProgram.CreateMauiApp`는 `UseMauiApp<DorotiMauiApplication>()`, `UseMauiCommunityToolkitMarkup()`, `UseSkiaSharp()`, Doroti definition/host service 등록을 수행한다.
- `DorotiMauiApplication.CreateWindow`는 C#으로 `Window(new DorotiMauiPage(...))`를 반환한다.
- `DorotiMauiPage`는 `Content = new DorotiSkiaView(...)`로 full-surface view를 구성한다.
- generated shell에 NavigationPage, Shell, XAML resource dictionary, MAUI widget UI를 추가하지 않는다.
- C# Markup은 baseline dependency로 고정한다. full-surface `SKGLView`는 일반 C#으로 유지하고 native toolbar/overlay가 필요할 때만 Markup helper를 사용한다.

## 5. 작업 단계

### M0. C# Markup MAUI + SKGLView feasibility gate

- repository fixture에서 `SkiaSharp.Views.Maui.Controls` 3.119.4, MAUI 10.0.90, `CommunityToolkit.Maui.Markup` 8.0.0을 고정한다.
- `.csproj` 하나, C# Application/Window/Page, Windows 초기화 전용 `App.xaml` 1개, Mac Catalyst platform source로 최소 앱을 만든다.
- Windows에서 `UseSkiaSharp()` handler 등록, `SKGLView`, non-null `GRContext`, GPU-backed `PaintSurface`, invalidate-driven 반복 frame을 실제 창으로 증명한다.
- Mac arm64에서 같은 fixture가 `SKMetalView`/Metal surface로 build/publish/present되는지 별도 증명한다.
- Windows `App.xaml` generated Main과 Mac Catalyst `UIApplication.Main`이 Debug/Release publish에서 정상인지 확인한다.

완료 조건:

- Windows bootstrap `App.xaml`/`ApplicationDefinition`이 정확히 1개이고 공용/page/shell/style XAML과 `MauiXaml`은 0개다. Mac Catalyst/Web XAML compile item도 0개다.
- Windows `SKSwapChainPanel`, Mac Catalyst `SKMetalView` backend identity가 실제 runtime에서 확인된다.
- CPU fallback 0, non-empty GPU frame, resize/DPI 반영, clean shutdown이 확인된다.
- 실패 시 기존 shell로 되돌려 완료 처리하지 않고 MAUI feasibility를 미완료로 유지한다.

실행 결과(2026-08-16):

- `reference/Maui.Markup-main`의 공식 sample 구조를 대조했다. 공용 Application/ResourceDictionary/Shell/Page는 C#이지만 Windows는 빈 `Platforms/Windows/App.xaml`과 `InitializeComponent()`를 유지하며, `UseMauiCommunityToolkitMarkup()` 자체는 WinUI resource bootstrap을 수행하지 않는다.
- `reference/MauiSampleApp`을 `.csproj` 1개, Windows bootstrap XAML 1개, 공용/page/shell/style XAML 0개, C# `Application/Window/Page`, `SKGLView` 구성으로 바꾸고 MAUI 10.0.90 + Markup 8.0.0 + SkiaSharp 3.119.4를 lock했다.
- XAML 0 수동 `Application.Start` negative probe는 `XamlControlsResources`에서 `AcrylicBackgroundFillColorDefaultBrush`를 찾지 못해 exit `0xC000027B`였다. MAUI core initializer를 제거하면 더 진행하지만 `NavigationViewButtonHolderGridMargin`이 없어 실패했다. 따라서 Toolkit은 UI XAML을 대체하지만 Windows `ApplicationDefinition` 초기화까지 대체하지 않는다는 경계를 확인했다.
- Windows locked restore와 Debug build는 경고/오류 0, Release publish/live는 exit 0으로 통과했다. 실제 native view는 `SkiaSharp.Views.Maui.Handlers.SKGLViewHandler+MauiSKSwapChainPanel`, GPU context/surface는 non-null, `HasRenderLoop=false`, 3 demand-driven frames를 기록했다.
- live resize에서 backend surface가 1894x1014에서 1414x1003 pixel로 바뀌고 density 2가 유지됐으며, `CloseMainWindow()` 뒤 exit 0을 확인했다. CPU/`SKCanvasView` fallback은 0이다.
- Mac Catalyst는 Windows host package-only Debug compile이 경고/오류 0으로 통과했다. Mac arm64 `SKMetalView`/Metal publish/present, resize/scale, clean shutdown은 `notVerified`다.
- Windows M0은 `PASS`지만 M0 전체는 Mac arm64 native gate가 남아 `PARTIAL`이다. M1~M7의 현재 실행 결과와 남은 gate는 아래 후속 결과 절에 기록한다.

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
- 생성 결과에 `.csproj` 1개, Windows bootstrap XAML 1개, 그 외 XAML 0개, user Razor 0개, Flutter/Dart scaffold 0개를 강제한다.

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

실행 결과(2026-08-16):

- M1 `PARTIAL`: `Doroti.App.Sdk/0.2.0-beta`가 명시 target/RID와 host default를 Windows `net10.0-windows10.0.19041.0/win-x64`, Mac Catalyst `net10.0-maccatalyst/maccatalyst-arm64`, Web `net10.0/browser-wasm`으로 매핑한다. app과 MAUI host의 target별 `obj/bin/publish/lock` 및 source/XAML graph를 분리했고 Windows -> Web -> Mac Catalyst Windows-host cross-build -> Windows `--no-restore` 반복 build와 `DOROTIAPP004` fail-closed를 통과했다. Mac runner locked restore/native 실행은 `notVerified`다.
- M2 `PARTIAL`: `Doroti.Host.Maui`가 externally owned `SKSurface`에 Doroti scene을 raster하고 session/view/frame lifecycle, invalidate coalescing, context/surface generation, strict GPU diagnostics를 제공한다. Windows live는 submitted/presented 3/3, failed 0, software fallback 0이다. resize/DPI/context-recreate와 Mac Metal live는 `notVerified`다.
- M3 `PARTIAL`: 공용 touch press/move/release, focus request, clipboard와 text capability adapter를 연결했다. Windows/Mac native hover/wheel/capture/key/IME와 UIA/UIAccessibility tree/action은 구현·검증되지 않았으므로 `notVerified`다.
- M4 `PARTIAL`: `Doroti.Target.Windows.Maui.win-x64`와 `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64`를 추가하고 single-project/template default를 전환했다. Native differential acceptance가 끝나지 않아 기존 Win32/AppKit/Avalonia source-port package는 제거하지 않았으며 predecessor evidence로만 유지한다.
- M5 `PARTIAL`: template을 `.csproj` 1개, root `Program.cs`, `src/App.cs`, `Platforms/*`, Windows bootstrap XAML 1개와 다른 XAML 0개 구조로 이관했다. 저장소 밖 local package feed에서 create/restore/Web native-link compile/publish를 통과했다. Windows package-only와 Mac Catalyst package-only/native gate는 `notVerified`다.
- M6 `PARTIAL`: `DorotiDemoApp.Web.csproj`와 `WebHost/`를 제거하고 `DorotiDemoApp.csproj` 하나로 통합했다. Material app은 `src/App.cs`, root bootstrap은 얇은 `Program.cs`로 분리했다. 기존 Demo 전용 Win32/AppKit smoke adapter는 새 native capability gate가 완성되지 않아 이관하지 않았다.
- M7 `PARTIAL`: `validate-g7-web-build.ps1`의 Graph/Template/Compile/Publish와 새 `validate-g7-maui-single-project.ps1`의 Windows/Mac Catalyst/Web Graph, target 순환 Build, Windows Live, Evidence를 통과했고 English/Korean root/runtime/Demo README와 solution graph를 갱신했다. G6 predecessor validator의 native input/IME/accessibility 가정은 M3 후속 작업으로 남긴다.

## 6. 검증 매트릭스

모든 test/validator timeout은 저장소 지침에 따라 20분으로 설정한다.

### 구조/package-only

- 외부 임시 디렉터리에서 template pack/install/create/locked restore/build/publish.
- project 수 1, Windows bootstrap `ApplicationDefinition`/`InitializeComponent` 각 1개, 그 외 XAML 0개, 선택되지 않은 platform compile item 0.
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
- Windows 초기화용 빈 `App.xaml` 외의 XAML UI, AppShell, NavigationPage 기반 앱 구조.
- `SKCanvasView` software fallback.
- 이번 단계의 Android/iOS target 추가.
- 기존 Win32/AppKit shell을 영구 fallback으로 유지하는 작업. differential/전환 기간 뒤 제거한다.

## 8. 외부 계약 확인 자료

- .NET MAUI single project: https://learn.microsoft.com/dotnet/maui/fundamentals/single-project
- .NET MAUI supported/runtime platforms: https://learn.microsoft.com/dotnet/maui/what-is-maui
- SKGLView API: https://learn.microsoft.com/dotnet/api/skiasharp.views.maui.controls.skglview
- SkiaSharp 3.119.4 handler registration/source (package commit): https://github.com/mono/SkiaSharp/tree/f568ac94dd768ef9a2f593537cfde2dd0d348ef5/source/SkiaSharp.Views.Maui
- C# Markup 검토 자료: https://learn.microsoft.com/dotnet/communitytoolkit/maui/markup/markup

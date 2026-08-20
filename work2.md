# Doroti macOS AppKit + Mac Catalyst 이중 backend 계획

- 작성일: 2026-08-20
- 대상: `Doroti/src/Doroti.Host.Maui`, AppKit/Mac Catalyst target package, 두 macOS runner, runner SDK, template, native bridge, 검증/문서
- 목표: 기존 Mac Catalyst/UIKit 제품을 유지하면서 `net10.0-macos` 기반의 native AppKit 제품을 추가하고, 두 backend를 영구적인 first-class target으로 병행한다.
- AppKit 첫 제품 범위: .NET 10, macOS 14 이상, `osx-arm64`, AppKit + Metal + Skia, 단일 window/view
- 상태 원칙: 이 문서는 구현 계획이다. 아직 실행하지 않은 build/live/signing gate는 `pass`가 아니라 `notVerified`다.

## 0. 검토 결론

AppKit backend 추가는 가능하지만 `net10.0-maccatalyst`를 `net10.0-macos`로 바꾸는 수준의 작업은 아니다. 현재 Doroti macOS 경로는 UIKit 기반 Mac Catalyst이고, Microsoft의 macOS backend는 별도 preview package가 제공하는 AppKit handler/hosting 계층이다. startup, render surface, raw input, native binding, RID, package/lock file, template, validator와 product identity를 독립적으로 추가해야 한다.

가장 큰 기술적 차이는 렌더 표면이다.

- Microsoft AppKit backend는 `Grid`, `AbsoluteLayout`, `Entry`, `Editor`, `GraphicsView`, window/lifecycle 및 AppKit accessibility handler를 제공한다.
- Doroti는 일반 MAUI control을 주 화면으로 그리지 않고 `SKGLView`가 제공하는 GPU `SKSurface`에 retained scene을 그린다.
- `SkiaSharp.Views.Maui.Controls`의 `SKGLViewHandler`에는 `net10.0-macos` 구현이 없다. 해당 TFM에서는 reference handler가 `object`를 platform view로 사용하고 생성 시 `NotImplementedException`을 던진다.
- 공식 `SkiaSharp.Views` 4.151.1은 `net10.0-macos26.0`용 `SkiaSharp.Views.Mac.SKMetalView`를 제공한다. 다만 이 view의 paint callback은 내부 Metal command buffer가 commit/present되기 전에 호출되고 completion을 외부로 노출하지 않는다. Doroti의 terminal present ACK를 정확히 유지하려면 AppKit 전용 `MTKView`/Metal surface adapter가 command-buffer completion까지 소유해야 한다.

따라서 첫 구현 단계는 AppKit 창 안에서 Doroti scene을 GPU로 표시하고 실제 Metal completion을 받는 spike다. 이 gate의 성공 여부와 관계없이 기존 Mac Catalyst 제품은 유지하며, 두 backend 사이 자동 fallback은 두지 않는다.

### 영구 병행할 두 제품

| 항목 | Mac Catalyst backend | AppKit backend |
|---|---|---|
| workspace alias | `maccatalyst` | `macos` |
| 제품 identity | Mac Catalyst/UIKit | native macOS AppKit |
| runner | `DorotiDemoApp.MacCatalyst.csproj` | `DorotiDemoApp.MacOS.csproj` |
| TFM | `net10.0-maccatalyst` | `net10.0-macos` |
| RID | `maccatalyst-arm64` | `osx-arm64` |
| entry | generated `UIKit.UIApplication.Main` | generated `NSApplication.Init/Main` + `MacOSMauiApplication` delegate |
| native UI | UIKit | AppKit |
| GPU surface | SkiaSharp MAUI handler의 `SKMetalView` | Doroti AppKit handler의 `MTKView` + Metal/Skia surface |
| host descriptor | `MacCatalyst/Maui/UIKit-Main` | `macOS/Maui/AppKit-Main` |
| native binding | `net10.0-maccatalyst`, UIKit Swift | `net10.0-macos`, AppKit Swift |
| minimum OS | 15.0 Catalyst | macOS 14.0 |
| target package | `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64` | `Doroti.Target.MacOS.Maui.osx-arm64` |

### 확인한 환경 및 dependency 기준

- Microsoft 문서는 .NET 10, macOS 14 이상, Xcode command line tools를 요구하고 backend가 experimental이며 Microsoft 공식 지원 대상이 아님을 명시한다.
- 2026-08-20 현재 NuGet에서 확인한 최신 package는 `Microsoft.Maui.Platforms.MacOS`와 `.Essentials`의 `0.1.0-preview.12.26368.2`다. 이 package는 `net10.0-macos26.0` asset과 MAUI 10.0.41 이상 dependency를 가지며 package metadata의 source commit은 `229f764fd688754497fe5822213e7b13b4e9caa3`다.
- 저장소는 .NET SDK 10.0.400, MAUI 10.0.90, SkiaSharp 4.151.1을 사용한다. backend package와 MAUI 10.0.90 조합은 restore/build/live spike로 호환성을 증명해야 한다.
- 현재 개발 머신은 Apple Silicon, macOS 26.5.2, Xcode 26.6이며 `macos` workload `26.5.10315/10.0.100` 설치와 `net10.0-macos` locked restore를 확인했다.
- preview package를 `*`나 `*-*`로 참조하지 않는다. 최초 검증 버전을 중앙 package 관리에 정확히 고정하고 package source, version, source commit을 evidence에 남긴다.

검토 자료:

- Microsoft Learn: <https://learn.microsoft.com/en-us/dotnet/maui/developer-tools/platform-backends/macos?view=net-maui-10.0>
- Microsoft `dotnet/maui-labs` AppKit backend: <https://github.com/dotnet/maui-labs/tree/main/platforms/MacOS>
- SkiaSharp AppKit `SKMetalView`: <https://github.com/mono/SkiaSharp/blob/main/source/SkiaSharp.Views/SkiaSharp.Views/Platform/Apple/SKMetalView.cs>

## 1. 고정할 아키텍처 결정

- [x] `macos`는 native AppKit 제품, `maccatalyst`는 UIKit 기반 Mac Catalyst 제품만 의미한다. 어느 alias도 다른 backend로 자동 fallback하지 않는다.
- [x] 두 backend는 영구적인 first-class target이다. 어느 한쪽을 legacy, 임시, 제거 예정 제품으로 표시하지 않는다.
- [x] AppKit 첫 release RID는 `osx-arm64`, Catalyst RID는 기존 `maccatalyst-arm64`로 고정한다. `osx-x64`/universal app은 별도 후속 target으로 다룬다.
- [x] backend preview version은 중앙 package 관리에서 exact version으로 고정한다. 자동 wildcard update를 허용하지 않는다.
- [x] `Doroti.Host.Maui`의 Windows/iOS/Android 경로는 유지하되 `net10.0-macos` target을 추가하고 AppKit 전용 코드는 `MACOS` compile boundary로 격리한다.
- [x] macOS에서는 `SkiaSharp.Views.Maui.Controls.SKGLView`를 사용하지 않는다. 공용 MAUI host와 renderer에는 작은 surface 계약을 두고, 기존 SKGLView adapter와 새 AppKit Metal adapter를 분리한다.
- [x] AppKit GPU view는 `MTKView`와 Metal command queue를 직접 소유하며 full-frame CPU readback/copy나 software fallback을 제품 경로로 허용하지 않는다.
- [x] `presented` ACK는 paint callback, Skia flush, command-buffer commit 시점이 아니라 Metal command buffer completion에서만 발생한다.
- [x] scene raster semantics는 계속 `Doroti.Skia.Rendering`이 단일 소유한다. AppKit host에 renderer 복사본을 만들지 않는다.
- [ ] clipboard/display/theme 등 Essentials API는 `Microsoft.Maui.Platforms.MacOS.Essentials`를 통해 제공하되 각 Doroti capability의 실제 동작을 별도로 검증한다.
- [x] native plugin 계약 `doroti.native-platform-bridge/v1`과 method 세 개는 두 backend가 각각 유지하며 platform identity/toolchain과 artifact를 분리한다.
- [x] backend package의 internal type이나 reflection에 의존하지 않는다. 필요한 surface/input handler는 공개 `MacOSViewHandler<TVirtualView,TPlatformView>`와 AppKit API로 구성한다.
- [x] Mac Catalyst와 AppKit의 runner/target/package/template/native binding/lock/evidence는 서로 덮어쓰지 않는 독립 product graph로 유지한다.

## 2. 완료 정의

다음 조건이 모두 충족되어야 이중 backend 추가를 완료로 표시한다.

- [x] `doroti ... -Platform macos -Rid osx-arm64`는 AppKit runner만, `doroti ... -Platform maccatalyst -Rid maccatalyst-arm64`는 Catalyst runner만 선택한다.
- [ ] `dotnet restore`, Debug/Release build, `dotnet run`, SDK가 지원하는 publish가 `net10.0-macos/osx-arm64`에서 성공한다.
- [x] framework-dependent publish는 macOS SDK/ILLink 제약이 해소되기 전까지 완료 gate에서 제외하고 `NETSDK1102` blocked evidence로 유지한다.
- [x] 생성된 `.app`가 `NSApplication`/`NSWindow`/`NSView`를 사용하고 UIKit, Mac Catalyst runtime 또는 iOSSupport framework에 링크되지 않는다.
- [x] 실제 Doroti Material demo scene이 AppKit 창의 Metal surface에 GPU 렌더된다.
- [x] submit된 모든 scene은 `presented`, `replayed`, `superseded`, `failed` 중 정확히 하나의 terminal ACK를 받으며 Metal completion 이전에는 `presented`가 증가하지 않는다.
- [ ] resize, Retina DPR, screen 이동, light/dark theme, hide/unhide, minimize/restore, close에서 metrics/lifecycle/surface generation이 일관된다.
- [ ] mouse hover/down/move/up, drag, precise trackpad scroll, keyboard shortcut, focus와 cursor가 Doroti input pipeline에 도달한다.
- [ ] 단일/다중행 text field, selection, replacement, 한글 IME preedit/commit, emoji/surrogate pair가 동작한다.
- [ ] Doroti semantics tree와 action이 VoiceOver에서 노출되고 hidden text-input proxy가 중복 accessibility node로 읽히지 않는다.
- [x] AppKit Swift native bridge가 `platformInfo`, `echo`, `echoOnUiThread`를 수행하고 UI-thread completion을 보장한다.
- [ ] template으로 만든 새 앱이 repository project 없이 동일한 AppKit target을 restore/build/run한다.
- [ ] Mac Catalyst, Windows, Android, iOS, Web, Linux Qt의 기존 build/validation 결과가 회귀하지 않는다.
- [x] 문서, naming/ownership/native-bridge contract, solution, package map, lock file 및 evidence가 두 backend를 서로 다른 현재 제품으로 정확히 보고한다.

## 3. 실행 순서

### MAC-APPKIT-0. 도구 및 dependency baseline 고정

목표: 구현 실패와 환경 실패를 구분할 수 있는 재현 가능한 baseline을 만든다.

- [x] `dotnet workload install macos` 또는 저장소가 채택한 workload restore 절차로 `macos` workload를 설치하고 `dotnet workload list` evidence를 남긴다.
- [x] .NET SDK 10.0.400, Xcode 26.6, selected developer directory, macOS SDK, host architecture를 doctor 출력에 기록한다.
- [x] `Microsoft.Maui.Platforms.MacOS`와 `.Essentials` `0.1.0-preview.12.26368.2`를 `Doroti/Directory.Packages.props`에 exact pin으로 추가한다.
- [x] macOS target에만 `SkiaSharp.Views` 4.151.1을 사용하고 다른 MAUI TFM에는 기존 `SkiaSharp.Views.Maui.Controls` dependency를 유지한다.
- [x] backend package가 요구하는 MAUI 10.0.41과 저장소의 10.0.90 조합을 minimal AppKit sample의 restore/build/run으로 검증한다.
- [x] `CommunityToolkit.Maui.Markup` 8.0.0이 `net10.0-macos`에서 restore 및 runtime registration되는지 확인한다. 불가능하면 Doroti의 builder에서 macOS에 불필요한 registration만 조건부 제외한다.
- [x] package source, exact package version, NuGet repository commit과 lock file hash를 dependency evidence에 기록한다.

종료 조건:

- [x] minimal `net10.0-macos` MAUI AppKit 앱이 native 창을 열고 종료한다.
- [x] restore graph에 Mac Catalyst asset이 섞이지 않는다.
- [x] workload/package version이 문서와 CI에서 재현 가능하다.

### MAC-APPKIT-1. Metal surface feasibility spike

목표: production graph를 바꾸기 전에 AppKit backend 내부에서 Doroti의 strict GPU/present 계약이 가능한지 증명한다.

- [x] disposable spike project에서 `UseMauiAppMacOS<TApp>()`로 AppKit window를 만든다.
- [x] MAUI virtual view와 `MacOSViewHandler<TVirtualView,TPlatformView>` 기반 handler를 만들고 native view로 `MTKView`를 배치한다.
- [x] `MTLDevice`, command queue, `GRMtlBackendContext`, `GRContext`, drawable texture, `GRBackendRenderTarget`, `SKSurface` lifetime을 명시한다.
- [x] `DrawableSize`를 physical pixel 크기로 사용하고 AppKit logical point/DPR을 정확히 한 번만 변환한다.
- [x] fixed color/shape/text를 `Doroti.Skia.Rendering`을 통해 그려 CPU readback 없이 화면에 표시한다.
- [x] command buffer에 drawable present를 등록하고 completion handler에서만 present ACK를 발생시킨다.
- [ ] command buffer error/cancel, drawable 부재, zero size, device/context 재생성을 `failed` 또는 recoverable retry로 구분한다.
- [x] frame 요청은 main run loop/display cadence에 coalesce하며 active paint 중 동기 wait를 하지 않는다.
- [x] 20회 resize, Retina scale 확인, hide/unhide, minimize/restore 후 retained scene replay를 수행한다.
- [x] 진단에 device name, pixel format, sample/stencil, logical/physical size, DPR, drawable/context/surface generation, command-buffer status를 남긴다.

중단 조건:

- AppKit handler 안에 native Metal view를 안정적으로 layout할 수 없거나 실제 command-buffer completion을 얻지 못하면 AppKit production 구현을 진행하지 않는다. 이 경우에도 Catalyst backend는 그대로 유지한다. CPU bitmap surface나 callback-return 시점의 가짜 present ACK로 우회하지 않는다.

종료 조건:

- [x] 실제 Doroti scene이 AppKit window에 GPU 표시된다.
- [x] raster 요청과 terminal ACK가 1:1이며 ACK가 Metal completion보다 먼저 발생하지 않는다.
- [ ] surface/context recreate 뒤 stale completion이 새 frame을 완료시키지 않는다.

### MAC-APPKIT-2. 공용 MAUI host의 surface 경계 분리

목표: macOS 지원을 추가하면서 기존 네 MAUI target의 렌더 동작을 바꾸지 않는다.

- [x] `DorotiMauiSurface`, `MauiFrameworkHost`, `MauiHostAdapter`가 concrete `SKGLView`/`SKPaintGLSurfaceEventArgs` 대신 최소 `IMauiSkiaSurface` 계약을 받게 한다.
- [x] 계약에는 invalidate dispatch, logical size, physical size, context/surface generation, native view type, paint event, present completion, focus/pointer source와 dispose만 포함한다.
- [ ] Windows/iOS/Mac Catalyst/Android용 기존 `SKGLView` adapter를 만들어 현재 동작을 characterization test로 잠근다.
- [x] AppKit용 `DorotiMacOSMetalSurface`와 handler를 추가하고 `UseMauiAppMacOS`의 handler collection에 등록한다.
- [x] `MauiSkiaCapabilities`와 `Doroti.Skia.Rendering`은 어느 native view type도 참조하지 않게 유지한다.
- [x] evidence writer와 paint diagnostics가 AppKit에서 `net10.0-macos`, `osx-arm64`, `AppKit/MTKView/Metal-Skia`를 보고하게 한다.
- [ ] dispose 순서를 frame scheduling 중지 -> pending command completion 무효화 -> view detach -> GPU resource release -> session/boundary dispose로 고정한다.
- [x] 다른 target의 generated binaries나 cached `obj`에 의존하지 않는 clean restore/build를 수행한다.

종료 조건:

- [x] AppKit과 기존 SKGLView path가 같은 scene renderer 및 terminal state machine을 사용한다.
- [ ] Windows/Android/iOS MAUI validation이 추출 전과 동일하다.
- [x] macOS path에 `SkiaSharp.Views.Maui.Handlers.SKGLViewHandler`가 존재하지 않는다.

### MAC-APPKIT-3. AppKit application, lifecycle, platform service 연결

목표: Microsoft backend의 hosting model을 Doroti application/session lifetime에 연결한다.

- [x] `DorotiMauiPlatformApplications.cs`에 `MacOSMauiApplication` 기반 Doroti delegate를 추가한다.
- [x] macOS builder는 `.UseMauiAppMacOS<DorotiMauiApplication>()`를 호출하고 `.AddMacOSEssentials()`를 등록한다. 다른 platform의 `.UseMauiApp` 경로는 유지한다.
- [ ] `DidFinishLaunching`, activate/resign, hide/unhide, terminate와 MAUI Window event를 Doroti lifecycle에 중복 없이 매핑한다.
- [ ] red close button, `Cmd+Q`, programmatic close의 `CloseRequested -> detached -> Closed -> dispose` 순서와 exactly-once를 검증한다.
- [ ] backend가 close veto를 제공하지 않는 현재 제한을 계약에 명시한다. Doroti가 취소 가능한 close를 요구한다면 upstream 공개 hook 또는 Doroti-owned window delegate를 별도 설계하고 private handler reflection은 사용하지 않는다.
- [x] `NSScreen.BackingScaleFactor`와 window가 속한 screen을 사용하여 DPR을 구한다. `DeviceDisplay.Current.MainDisplayInfo`만으로 보조 monitor scale을 판단하지 않는다.
- [ ] effective appearance 변경, locale 변경, clipboard, focus request, app activation을 AppKit/Essentials 경로로 연결한다.
- [ ] AppKit main thread 밖에서 `NSView`, `NSWindow`, pasteboard, first responder를 접근하지 못하게 한다.

종료 조건:

- [ ] launch부터 window close까지 session/view/resource가 정확히 한 번 생성 및 해제된다.
- [ ] screen 이동과 theme/lifecycle 변화가 Doroti metrics/configuration event로 전달된다.
- [x] native platform service가 Mac Catalyst API를 호출하지 않는다.

### MAC-APPKIT-4. raw input, text input, cursor, accessibility

목표: 기존 Catalyst용 UIKit 입력 구현을 유지하면서 AppKit native event adapter를 별도로 추가한다.

- [x] `MauiNativeInput`에 `MACOS` adapter를 추가하거나 AppKit surface input adapter로 분리한다.
- [ ] `NSEvent`의 mouse entered/exited/moved/down/dragged/up, other/right button, precise scrolling delta, phase/momentum phase, modifier, timestamp를 canonical pointer data로 변환한다.
- [x] AppKit bottom-left 좌표를 Doroti top-left physical 좌표로 바꾸고 DPR을 정확히 한 번 적용한다.
- [ ] `KeyDown`/`KeyUp`, repeat, scan/key code, charactersIgnoringModifiers, Command/Option/Control/Shift를 physical/logical key 계약에 매핑한다.
- [ ] focus를 `NSWindow` key state와 first responder state에서 추적하며 focus 상실 시 pressed key/button을 synthesized up/cancel로 정리한다.
- [ ] `NSCursor` push/pop 누수를 만들지 않는 cursor owner를 두고 basic/click/text/resize/none을 매핑한다.
- [ ] 기존 hidden `Entry`/`Editor` bridge가 AppKit handler에서 text/selection/action을 왕복하는지 먼저 검증한다.
- [ ] 한글 IME preedit/composing range와 candidate caret rect가 손실되면 AppKit `NSTextInputClient` adapter를 구현한다. commit string만 전달하고 완료로 표시하지 않는다.
- [ ] `MauiSemanticsBridge`의 Label/Button/Entry overlay가 native `NSAccessibility` node/action으로 노출되는지 VoiceOver와 automation tree로 확인한다.
- [x] semantics overlay는 Doroti hit testing을 가로채지 않고 hidden text input은 accessibility tree에서 제외한다.

종료 조건:

- [ ] mouse/trackpad/keyboard/focus/cursor 자동 trace가 기대 sequence와 일치한다.
- [ ] 한글 조합, selection replace, delete/backspace, emoji 입력이 통과한다.
- [ ] VoiceOver label/value/role/focus/action과 scroll 중 coalescing이 통과한다.

### MAC-APPKIT-5. AppKit target package와 독립 runner graph 추가

목표: AppKit backend를 `macos` alias의 fixed target으로 추가하고 기존 Catalyst 제품을 `maccatalyst` alias의 독립 fixed target으로 유지한다.

- [x] `Doroti.Target.MacOS.Maui.osx-arm64` project를 추가한다.
- [x] target manifest를 `net10.0-macos`, `osx-arm64`, native view와 `AppKit/MTKView/Metal-Skia`, `softwareFallback=false`로 작성한다.
- [x] buildTransitive descriptor를 `macOS | Maui | AppKit-Main | osx-arm64 | net10.0-macos`로 고정한다.
- [x] `Doroti.Host.Maui.csproj`에 `net10.0-macos` target/RID condition과 별도 intermediate path를 추가해 다른 Apple TFM의 `obj`와 격리한다.
- [x] runner SDK에 `DorotiTarget=macOS`의 AppKit binding과 `DorotiTarget=MacCatalyst`의 Catalyst binding TFM/build item 검증을 각각 유지한다.
- [x] generated bootstrap에 `AppKit-Main`을 추가한다: `NSApplication.Init()`, delegate 지정, `NSApplication.Main(args)` 순서를 생성하고 이를 `StartupObject`로 설정한다.
- [x] `DorotiDemoApp/macos/DorotiDemoApp.MacOS.csproj`를 `net10.0-macos`, `osx-arm64`, `SupportedOSPlatformVersion=14.0`, AppKit entry/target package로 구성한다.
- [x] `doroti-workspace.json`에 `macos` AppKit runner와 `maccatalyst` Catalyst runner를 함께 선언하고 target graph validator가 두 identity를 구분하게 한다.
- [x] `.app` bundle의 `Info.plist`, icon, entitlements, bundle identifier, version을 AppKit 규칙에 맞춘다. UIKit 전용 `UIDeviceFamily`, orientation, device capability와 Catalyst app icon key를 제거한다.
- [ ] backend icon target이 실제로 처리 가능한 1024px PNG를 준비하고 mobile splash item은 macOS에서 조건부 제외한다.

종료 조건:

- [x] `WriteDorotiTargetGraph`가 선택한 platform에 대해 정확히 한 descriptor, runner, binding, package를 출력하며 다른 backend 항목을 섞지 않는다.
- [x] `doroti build/run/publish -Platform macos -Rid osx-arm64`와 `-Platform maccatalyst -Rid maccatalyst-arm64`가 각각 고정된 runner를 사용한다.
- [ ] clean AppKit bundle에는 UIKit/Catalyst identity가 없고, clean Catalyst bundle에는 AppKit preview package나 AppKit native bridge artifact가 없다.

### MAC-APPKIT-6. AppKit Swift/Xcode native bridge 추가

목표: 기존 Catalyst native plugin을 유지하면서 같은 contract를 구현하는 true macOS framework/binding을 별도로 추가한다.

- [x] `DorotiDemoApp.MacOS.Native.csproj`, `net10.0-macos`, `DorotiNativeBindingTarget=macOS` binding을 추가하고 기존 Catalyst binding project를 유지한다.
- [x] AppKit Xcode scheme `DorotiDemoAppNative-macOS`를 추가하고 기존 Catalyst scheme과 함께 shared scheme 상태를 유지한다.
- [x] native target에서 `SUPPORTED_PLATFORMS=macosx`, `SUPPORTS_MACCATALYST=NO`, `MACOSX_DEPLOYMENT_TARGET=14.0`을 사용한다.
- [x] AppKit target에는 `IPHONEOS_DEPLOYMENT_TARGET`, `TARGETED_DEVICE_FAMILY`, Catalyst product id와 UIKit import가 유입되지 않게 하되 Catalyst target의 설정은 유지한다.
- [x] Swift는 `import AppKit`을 사용하고 OS 정보는 `ProcessInfo.processInfo.operatingSystemVersionString` 등 macOS API에서 얻는다.
- [x] Objective-C export 이름과 C# `ApiDefinition.cs` 계약은 유지하되 namespace/assembly/platform string을 `MacOS`/`macOS`로 바꾼다.
- [ ] `echoOnMainThread`가 `DispatchQueue.main`에서 실행되고 cancellation/late callback이 disposed bridge를 되살리지 못하게 한다.
- [x] Xcode framework가 binding build와 runner `.app/Contents/Frameworks`까지 전달되고 code sign 대상에 포함되는지 확인한다.
- [x] native ABI JSON, toolchain 문서, scaffold command가 같은 scheme/TFM/target 값을 사용하게 한다.

종료 조건:

- [x] binding 단독 build와 runner transitive build가 통과한다.
- [x] live app에서 세 method가 통과하고 `platformInfo.platform == "macOS"`다.
- [x] `otool -L`, `file`, bundle 검사에서 Catalyst/UIKit artifact가 없다.

### MAC-APPKIT-7. template, CLI, contract, 문서 동기화

목표: Demo만 동작하는 일회성 port가 아니라 새 앱이 두 backend 중 하나를 명시적으로 선택해 재현할 수 있는 제품 경계를 만든다.

- [x] `Doroti.Templates`에 AppKit과 Catalyst runner, delegate, binding, Swift/Xcode project, manifest, resources를 각각 제공한다.
- [x] template filename/token replacement와 expected project count 검증을 업데이트한다.
- [x] `Doroti.slnx`, `Doroti.Product.slnx`, product naming map과 package map에 Mac Catalyst와 AppKit target을 함께 등록한다.
- [x] platform workspace ownership에 canonical target `macOS`와 `MacCatalyst`를 별도 identity로 기록한다.
- [x] native platform bridge contract에서 `trueAppKitMacOS=outOfScope`를 제거하고 두 backend의 TFM/RID/scheme/toolchain을 각각 기록한다.
- [x] `doroti doctor`가 macOS workload, Xcode, selected SDK, architecture, backend preview pin을 검사하고 actionable diagnostic을 낸다.
- [x] `doroti native doctor/build/open/add --Platform macos`가 새 Xcode scheme과 binding project를 사용한다.
- [x] root/Doroti/Demo README의 지원 표와 build/run 예제에 `macos/osx-arm64/AppKit`과 `maccatalyst/maccatalyst-arm64/UIKit`을 함께 기록하고 AppKit의 experimental/unsupported 상태와 macOS 14 minimum을 명시한다.
- [x] ADR-021/022에 이중 workspace/native bridge identity를 반영하고 AppKit Metal surface, preview dependency, 영구 병행 결정을 새 ADR로 기록한다.
- [ ] shader target support는 `maccatalyst`와 `macos`를 모두 유지하고 두 runtime effect를 각 backend에서 실제 검증한다.

종료 조건:

- [ ] 새 template 앱의 AppKit 및 Catalyst runner가 외부 임시 디렉터리에서 각각 restore/build/run한다.
- [x] source contract 검색이 AppKit product 경로의 `MacCatalyst`/UIKit 참조와 Catalyst product 경로의 AppKit preview 참조를 각각 0건으로 검증한다.
- [x] CLI, README, template, Demo가 동일한 두 platform 명령과 identity를 사용한다.

### MAC-APPKIT-8. 자동 검증과 live evidence

목표: build 성공이 아니라 native launch, GPU present, interaction, accessibility까지 증명한다.

- [x] `validate-app-targets.ps1`의 graph/package/native-interop/template/evidence에 AppKit과 Catalyst case를 독립적으로 둔다.
- [ ] fast CI와 macOS-hosted CI를 분리한다. non-macOS에서는 source/graph contract만 검사하고 AppKit build를 거짓 pass로 기록하지 않는다.
- [ ] macOS CI matrix에서 AppKit과 Catalyst 각각 clean restore, Release build, no-restore repeat build, 지원되는 publish를 수행한다.
- [x] app bundle layout, executable architecture, dylib/framework dependency, Info.plist, entitlements, code-sign verification을 자동화한다.
- [ ] live harness가 앱을 실행하고 첫 frame, retained replay, resize, theme, pointer, scroll, key, text, lifecycle, native bridge evidence를 수집한 뒤 정상 종료한다.
- [ ] frame invariant를 검증한다: terminal ACK 합, exactly-once, queue high-watermark, stale completion, software fallback, CPU copy/allocation counter.
- [ ] window를 두 screen/DPR 사이로 이동하고 surface generation 및 hit-test 좌표를 검증한다.
- [x] VoiceOver 및 한글 IME처럼 완전 자동화가 어려운 항목은 수동 evidence schema와 재현 절차를 두고 실행 전까지 `notVerified`로 남긴다.
- [ ] Catalyst/Windows/Android/iOS/Web/Linux regression shard를 실행하고 AppKit-specific package가 다른 target restore graph에 유입되지 않았는지 검사한다.

최소 evidence 항목:

```text
identity: macOS | net10.0-macos | osx-arm64 | AppKit-Main
backend: AppKit/MTKView/Metal-Skia
nativeViewType, metalDevice, pixelFormat, logicalSize, pixelSize, dpr
metricsGeneration, contextGeneration, surfaceGeneration
submitted, presented, replayed, superseded, failed, dropped
commandBuffersCommitted, completed, errored, staleCompletions
nativePointerEvents, keyEvents, textEdits, semanticsUpdates
softwareFallbackFrames, cpuReadbacks, fullFrameCopies
nativeBridgePlatform, appBundlePath, executableArchitectures
```

종료 조건:

- [ ] automated AppKit gate가 모두 pass다.
- [x] 실행하지 않은 physical input/IME/VoiceOver/signing/notarization gate는 명시적으로 `notVerified`다.
- [ ] evidence가 새 source fingerprint와 exact dependency pin을 가리킨다.

### MAC-APPKIT-9. 영구 병행 계약과 release 정책 고정

목표: AppKit과 Mac Catalyst를 서로 섞이지 않는 두 정식 제품으로 계속 제공한다.

- [x] `Doroti.Target.MacOS.Maui.osx-arm64`와 `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64`를 solution, package map, release manifest에 함께 유지한다.
- [x] Demo/template에 `.MacOS.csproj`와 `.MacCatalyst.csproj`, 각 binding, Swift target, Xcode scheme, plist/resource를 나란히 유지한다.
- [x] runner SDK가 `macos`와 `maccatalyst`를 명시적으로 선택하며 RID/TFM 불일치와 cross-backend binding 참조를 build 전에 거부한다.
- [x] `Doroti.Host.Maui`의 `net10.0-macos` AppKit adapter와 `net10.0-maccatalyst` SKGLView adapter를 공용 scene renderer 위의 독립 platform path로 유지한다.
- [ ] lock file, evidence, bundle 검사 결과와 source fingerprint를 backend별 디렉터리와 schema identity로 분리한다.
- [x] release note와 지원 표에 두 backend의 최소 OS, RID, package 지원 상태, artifact 호환성 및 명시적 CLI 명령을 함께 기록한다.
- [x] 한 backend의 build/live 실패가 다른 backend로 자동 fallback하거나 다른 backend의 pass evidence로 대체되지 않게 한다.
- [x] 향후 어느 backend든 중단하려면 별도의 사용자 결정, ADR, deprecation 기간과 migration release를 요구한다. 이 계획에서는 제거하지 않는다.

종료 조건:

- [x] repository의 현재 product graph에 AppKit과 Mac Catalyst target이 모두 존재한다.
- [x] `macos`와 `maccatalyst`가 문서, CLI, manifest, template, native bridge, evidence에서 일관된 서로 다른 제품을 뜻한다.
- [ ] 두 backend 각각 최종 clean clone 검증과 template consumer 검증이 통과한다.

## 4. 예상 변경 범위

### 핵심 product/host

- `Doroti/Directory.Build.props`, `Doroti/Directory.Packages.props`
- `Doroti/src/Doroti.Host.Maui/*`
- 새 `Doroti/src/Doroti.Target.MacOS.Maui.osx-arm64/*`
- `Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets`
- `Doroti/src/Doroti.App.Sdk/Sdk/Sdk.targets`
- `Doroti/Doroti.slnx`, `Doroti/Doroti.Product.slnx`

### Demo 및 native interop

- `DorotiDemoApp/doroti-workspace.json`
- `DorotiDemoApp/macos/*`
- `DorotiDemoApp/maccatalyst/*` 또는 기존 Catalyst runner의 명시적 유지 경로
- `DorotiDemoApp/macos/binding/*`
- `DorotiDemoApp/macos/native/*`
- `DorotiDemoApp/macos/Resources/*`

### template/tooling/contract/evidence

- `Doroti/templates/Doroti.Templates/content/doroti-app/macos/*`
- `Doroti/eng/doroti.ps1`, `validate-app-targets.ps1`, `validate.ps1`, 관련 FCR validator
- `Doroti/validation/contracts/*`, `Doroti/validation/evidence/*`
- `Doroti/docs/adr/*`, `Doroti/docs/native-bridge-toolchains.md`
- root/Doroti/Demo `README.md`, `README.ko.md`

`reference/MauiSampleApp`은 참고 자료다. Doroti product의 이중 backend 추가와 함께 자동 변경하지 않고, 별도 sample 확장이 필요할 때만 작업한다.

## 5. 주요 위험과 대응

| 위험 | 영향 | 대응 |
|---|---|---|
| backend가 experimental/unsupported | release 사이 API/handler 동작 변경 | exact version pin, 공개 API adapter 격리, dependency upgrade 전용 validation |
| 개발/CI의 `macos` workload 누락 또는 version drift | restore/build 자체 불가 또는 재현성 손실 | 설치된 `26.5.10315/10.0.100` baseline, doctor 선행 gate와 CI image 명시 |
| MAUI 10.0.90과 backend가 빌드된 10.0.41 차이 | binary/runtime incompatibility | minimal sample 및 full clean graph 검증, lock file 고정 |
| MAUI `SKGLView`에 macOS handler 없음 | startup 시 handler failure | AppKit 전용 `MTKView` handler 구현, Maui.Controls Skia view 미사용 |
| stock `SKMetalView`가 present completion을 노출하지 않음 | 거짓 `presented` evidence | Doroti-owned Metal command buffer와 completion state machine |
| AppKit 좌표계/DPR/screen 이동 | 잘못된 layout/hit test | flipped view 확인, window screen scale 사용, generation test |
| raw AppKit input과 UIKit key/cursor 구현 차이 | desktop interaction 손실 | `NSEvent`/`NSResponder` 전용 adapter 및 sequence tests |
| backend Entry/Editor의 IME 정보 부족 | 한글 preedit/selection 손실 | 먼저 characterization, 실패 시 `NSTextInputClient` adapter |
| close veto/public lifecycle hook 제한 | close ordering 또는 취소 불가 | 공개 hook만 사용, 요구 시 upstream 또는 owned delegate; 제한 문서화 |
| Swift framework가 Catalyst 설정을 보존 | 잘못된 slice/link/sign | AppKit-only Xcode target, bundle 및 `otool/file/codesign` gate |
| icon/splash build target 차이 | build 실패 또는 잘못된 bundle resource | macOS PNG icon fixture, mobile-only resource 조건 분리 |
| 두 backend identity 혼재 | CLI/문서/evidence의 거짓 성공 또는 잘못된 artifact 실행 | 명시적 alias, backend별 runner/package/lock/evidence, cross-reference validator |
| 한 backend 실패 시 암묵적 fallback | 사용자가 요청하지 않은 제품 실행과 거짓 성공 | fallback 금지, 선택된 backend 실패를 그대로 보고 |

## 6. 범위 밖

- `osx-x64`와 universal binary 지원
- Mac App Store 제출, notarization, production Developer ID 배포의 완료 판정
- backend package 자체의 private API fork 또는 repository vendoring
- MAUI AppKit native controls로 Doroti widget tree를 다시 구현하는 작업
- Linux Qt, Windows, Android, iOS, Web renderer 기능 변경
- 이번 이중 backend 추가와 무관한 MAUI/SkiaSharp dependency upgrade
- `reference/MauiSampleApp`의 동시 마이그레이션
- AppKit 또는 Mac Catalyst backend의 제거·deprecated 처리

## 7. 권장 첫 작업 묶음

첫 PR은 product graph 변경이 아니라 MAC-APPKIT-0/1만 포함한다.

1. macOS workload/doctor와 exact package pin을 준비한다.
2. 별도 spike runner에서 `UseMauiAppMacOS` + Doroti `MTKView` handler를 만든다.
3. 공용 `Doroti.Skia.Rendering`으로 데모 scene 한 장을 그린다.
4. Metal command-buffer completion 기반 terminal ACK와 resize/recreate evidence를 만든다.
5. 성공 시 surface abstraction 설계를 ADR로 고정하고 MAC-APPKIT-2 이후를 진행한다.

이 순서라면 가장 불확실한 GPU/present 경계를 먼저 검증하면서 현재 동작하는 Mac Catalyst product를 영구적인 독립 backend로 계속 유지할 수 있다.

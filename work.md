# Doroti Flutter-style platform workspace 개편 계획

- 작성일: 2026-08-19
- 상태: 계획 확정 전 검토안, 구현 및 신규 플랫폼 검증은 모두 `notVerified`
- 대상: `Doroti.App.Sdk`, `Doroti.Hosting`, `Doroti.Host.*`, `Doroti.Target.*`, `DorotiDemoApp`, `doroti-app` 템플릿, `Doroti/eng`, validation/evidence
- 비교 기준: `reference/flutter_sample_app/{android,ios,macos,linux,web,windows}`

## 1. 결론

Doroti 앱 루트에 다음 여섯 개의 소문자 플랫폼 폴더를 first-class workspace로 둔다.

```text
android/
ios/
linux/
macos/
web/
windows/
```

각 폴더는 manifest와 진입점만 모아 둔 소스 분류 폴더가 아니라, 해당 플랫폼을 독립적으로 restore/build/run/publish할 수 있는 runner 프로젝트의 루트가 된다. 루트 앱 프로젝트는 `Program : IDorotiApplicationStartup`, `src/App.cs`, 공용 asset만 소유하고 플랫폼 실행 파일을 직접 만들지 않는다.

현재의 단일 `DorotiDemoApp.csproj`가 `DorotiTarget` 값에 따라 TFM, RID, SDK, host, `Platforms/<Target>`를 바꾸는 방식은 제거한다. 대신 다음 경계를 사용한다.

- 루트 앱 프로젝트: 플랫폼 중립 Doroti 애플리케이션 assembly
- 플랫폼 runner 프로젝트: 고정 TFM/RID/host와 네이티브 entry point 소유
- `Doroti.Runner.Sdk`: runner descriptor 검증, bootstrap 생성, app assembly 연결
- `Doroti.Host.*`: surface, lifecycle, input, semantics, text input 등 Doroti 필수 host 구현
- `Doroti.Target.*`: 플랫폼/RID별 배포 dependency와 capability
- `Doroti/eng`: 루트 workspace manifest를 읽어 runner를 선택하는 공통 CLI

`Maui.NativeLibraryInterop`은 Android/iOS 앱 runner를 대신하지 않는다. 이 프로젝트는 Kotlin/Java 또는 Swift/Objective-C로 작성한 얇은 native wrapper를 .NET binding assembly로 가져오는 .NET -> native SDK 호출 경로다. 따라서 다음처럼 사용한다.

- Android/iOS runner와 앱 lifecycle은 .NET Android/iOS + MAUI native embedding/host 계약이 소유한다.
- 앱별 native SDK나 사용자 정의 Kotlin/Swift 플러그인은 각 플랫폼의 `native/` + `binding/` workspace에서 Native Library Interop 방식으로 빌드한다.
- Doroti 엔진의 필수 native bridge는 앱별 `native/` + `binding/`에 복제하지 않고 `Doroti.Host.*` 또는 `Doroti.Target.*`가 소유한다.
- Kotlin/Swift 앱이 .NET/Doroti 엔진을 역방향으로 직접 호스팅하는 것은 Native Library Interop의 기능이 아니다. 그 모델이 필요하면 별도 runtime embedding 프로젝트로 다루며 이번 개편의 완료 조건에 섞지 않는다.

### 1.1 Native Library Interop이 플랫폼 workspace처럼 보이는 이유와 적용 범위

Native Library Interop을 이 개편의 출발점으로 떠올린 것은 타당하다. 공식 template 자체가 Android Studio와 Xcode에서 직접 열 수 있는 실제 native project를 플랫폼별로 제공하기 때문이다. 개념적으로는 다음 구조다.

```text
android/
  native/                    # Gradle/Android Studio native library project
  Xxx.Android.Binding/       # 위 library를 .NET에 노출하는 binding project

macios/
  native/                    # Xcode native framework project
  Xxx.MaciOS.Binding/        # 위 framework를 .NET에 노출하는 binding project

sample/
  MauiSample.csproj          # binding을 소비하고 실제 앱을 실행하는 MAUI project
```

따라서 다음 부분은 Doroti의 플랫폼 workspace에 그대로 활용할 수 있다.

- `android/native`의 Gradle wrapper, settings, module, Maven dependency 관리
- `ios/native`의 Xcode project, Swift/Objective-C wrapper, SPM/CocoaPods dependency 관리
- Android의 `AndroidGradleProject` -> `.aar`/Java API -> .NET binding 연결
- iOS의 `XcodeProject` -> framework/XCFramework -> `ApiDefinition.cs` binding 연결
- native wrapper와 managed binding을 같은 플랫폼 폴더에서 함께 개발하고 버전 고정하는 방식
- Android Studio/Xcode에서 native wrapper 자체를 build/test/debug하는 개발 경험

하지만 Native Library Interop template의 `native/`는 Flutter의 `android/app` 또는 Xcode `Runner`처럼 최종 APK/IPA의 application entry point가 아니다. Android에서는 Android library module을, Apple에서는 framework/XCFramework를 만들고, 실제 앱 project가 그 결과를 참조한다. 호출 방향도 기본적으로 다음과 같다.

```text
Doroti .NET runner
  -> .NET binding assembly
    -> Kotlin/Java 또는 Swift/Objective-C wrapper
      -> platform SDK/native library
```

그러므로 Native Library Interop만 복사해서는 다음 항목이 생기지 않는다.

- Android `MainActivity`/`Application`에서 Doroti/.NET runtime을 시작하는 app host
- iOS `UIApplicationDelegate`/scene에서 Doroti/.NET runtime을 시작하는 app host
- Doroti surface, lifecycle, input, IME, semantics를 native window/view에 연결하는 host
- Android Studio 또는 Xcode의 Run 버튼으로 Doroti 앱 전체를 바로 실행하는 reverse embedding 경로

Doroti에서는 Native Library Interop의 native workspace를 버리지 않고, 그 바깥에 실제 .NET runner를 결합한다.

```text
android/
  DorotiDemoApp.Android.csproj      # 실제 APK를 만드는 Doroti/.NET runner
  MainActivity.cs
  MainApplication.cs
  native/                           # NLI 방식 Gradle native library workspace
  binding/                          # .NET Android binding

ios/
  DorotiDemoApp.iOS.csproj          # 실제 IPA/app을 만드는 Doroti/.NET runner
  AppDelegate.cs
  native/                           # NLI 방식 Xcode framework workspace
  binding/                          # .NET iOS binding
```

이 구조에서 Android Studio와 Xcode는 `native/` wrapper를 직접 다룰 수 있고, `dotnet build/run/publish`는 runner와 binding을 포함한 최종 Doroti 앱을 다룬다. 즉 “플랫폼별 실제 native workspace를 둔다”는 의도는 유지하지만, native library workspace와 application runner workspace를 같은 것으로 취급하지 않는다.

향후 Kotlin/Swift application project 자체가 최종 runner가 되어 Doroti를 직접 시작해야 한다면 호출 방향이 반대가 된다. 이 경우에는 Native Library Interop이 아니라 NativeAOT C ABI 또는 .NET runtime embedding, AOT/linking, callback/threading, native view ownership을 별도로 설계해야 한다. 그 선택은 현재의 runner + binding 개편을 완료한 뒤 독립 milestone에서 검토한다.

## 2. 현행 구조에서 확인한 사실

- `DorotiDemoApp/DorotiDemoApp.csproj` 하나가 `Doroti.App.Sdk/Sdk.props`를 통해 Windows, Android, Mac Catalyst, Web의 SDK/TFM/RID를 선택한다.
- 현재 지원 descriptor는 `Windows/win-x64`, `Android/android-arm64`, `Android/android-x64`, `MacCatalyst/maccatalyst-arm64`, `Web/browser-wasm`이다.
- `Sdk.targets`가 `Program.cs`, `src/**/*.cs`, 선택된 `Platforms/$(DorotiTarget)/**/*.cs`를 한 assembly로 컴파일한다.
- bootstrap과 plugin registration은 현재 `obj/<target>/Doroti.Generated/`에 생성된다.
- Android와 Windows는 네이티브 live 근거가 일부 있지만, iOS와 Linux target/host는 아직 없다.
- 현재 `macos`에 대응되는 제품은 native AppKit macOS가 아니라 Mac Catalyst다. 폴더 이름을 `macos/`로 바꾸더라도 이 차이를 숨기지 않는다.
- .NET MAUI에는 Linux app target이 없으므로 Linux는 `UseMaui=true`만 추가해서 닫을 수 없다.
- Flutter 샘플의 플랫폼 폴더는 Android Gradle app, Xcode runner, CMake runner, Web root처럼 각 toolchain이 직접 여는 네이티브 runner workspace다.

## 3. 목표와 비목표

### 목표

- 새 앱과 DemoApp 모두 동일한 `android/ios/linux/macos/web/windows` 구조를 가진다.
- 플랫폼별 manifest, icon, entitlement, entry point, native dependency가 자기 폴더 밖으로 새지 않는다.
- 각 runner는 다른 플랫폼 property를 주입하지 않아도 고정된 target graph를 가진다.
- 루트 CLI에서 동일한 명령 형태로 각 runner를 선택할 수 있다.
- Native Library Interop을 Android/iOS 사용자 native extension의 표준 경로로 제공한다.
- 기존 `IDorotiApplicationStartup`과 `DorotiApplicationDescriptor`를 모든 runner가 공유한다.
- 기존 Windows/Web/Android/Mac Catalyst 기능을 구조 개편 전후의 동등성 gate로 보호한다.
- 구조 검사, build, native live, physical, store/package acceptance를 서로 다른 evidence로 남긴다.

### 비목표

- Native Library Interop으로 Kotlin/Swift가 .NET runtime을 직접 시작하게 만들지 않는다.
- Mac Catalyst를 native AppKit macOS 지원으로 기록하지 않는다.
- Linux를 MAUI 지원으로 가장하지 않는다.
- 플랫폼 폴더에 Doroti 내부 bootstrap 또는 generated `.g.cs`를 체크인하지 않는다.
- 앱별 폴더에 rendering/input/IME/semantics의 framework workaround를 복제하지 않는다.
- 모든 앱이 쓰지 않는 빈 Android/iOS binding을 기본 build graph에 강제로 넣지 않는다.
- 폴더 이동만 통과한 것을 runtime parity로 기록하지 않는다.

## 4. 목표 디렉터리 구조

```text
DorotiDemoApp/
  DorotiDemoApp.csproj                  # net10.0 플랫폼 중립 앱 library
  Program.cs                            # IDorotiApplicationStartup
  src/
    App.cs
  assets/
    shaders/
      aurora.sksl
  doroti-workspace.json                 # 플랫폼 alias -> runner project

  android/
    DorotiDemoApp.Android.csproj        # net10.0-android runner
    MainActivity.cs
    MainApplication.cs
    AndroidManifest.xml
    Resources/
    application-manifest.json
    application-manifest.android-x64.json
    native/                             # optional Gradle wrapper + Android library module
    binding/                            # optional .NET Android binding project
      DorotiDemoApp.Android.Binding.csproj

  ios/
    DorotiDemoApp.iOS.csproj            # net10.0-ios runner
    Program.cs
    AppDelegate.cs
    Info.plist
    Entitlements.plist
    Resources/
    application-manifest.json
    native/                             # optional Xcode framework project
    binding/                            # optional .NET iOS binding project
      DorotiDemoApp.iOS.Binding.csproj

  macos/
    DorotiDemoApp.MacCatalyst.csproj    # 1차 구현의 실제 제품 identity
    Program.cs
    AppDelegate.cs
    Info.plist
    Entitlements.plist
    Resources/
    application-manifest.json

  windows/
    DorotiDemoApp.Windows.csproj
    App.xaml                            # WinUI 초기화용 유일한 XAML 허용
    App.xaml.cs
    Package.appxmanifest
    app.manifest
    Resources/
    application-manifest.json

  web/
    DorotiDemoApp.Web.csproj
    application-manifest.json
    tsconfig.json
    src/
      doroti_bootstrap.ts
      plugins/
    wwwroot/
      index.html
      doroti-app-manifest.json
      assets/
      locales/

  linux/
    DorotiDemoApp.Linux.csproj          # managed runner/orchestration
    application-manifest.json
    native/
      CMakeLists.txt
      src/                              # Qt/native surface shim
      resources/
```

원칙은 다음과 같다.

- runner의 `obj`, `bin`, publish, lock은 runner 폴더 아래에서 자연스럽게 격리한다.
- root app assembly는 어떤 platform SDK type도 참조하지 않는다.
- 공용 shader처럼 앱 논리에 속하는 asset은 `assets/`에 두고, icon/splash/manifest처럼 package identity에 속하는 asset은 각 플랫폼이 소유한다.
- `native/` + `binding/`은 해당 앱이 native wrapper를 실제 사용하는 경우만 생성하거나 project graph에 연결한다.
- `macos/`의 초기 runner 파일명과 descriptor에는 `MacCatalyst`를 남긴다. true macOS host가 생길 때 같은 폴더에 별도 runner를 추가하거나 명시적으로 교체한다.

## 5. 프로젝트 및 SDK 계약

### 5.1 루트 앱 프로젝트

- `DorotiDemoApp.csproj`는 `net10.0` library로 고정한다.
- `Program.cs`와 `src/**/*.cs`, 공용 Doroti asset만 컴파일한다.
- `Doroti.Hosting`과 framework project/package를 참조하되 `Microsoft.Maui.*`, Android, UIKit, WinUI, Blazor SDK를 참조하지 않는다.
- `DorotiApplicationType`의 기본값은 계속 `$(RootNamespace).Program`으로 유지한다.
- root build는 app assembly와 target-neutral contract test까지만 수행한다.

### 5.2 runner SDK

`Doroti.Runner.Sdk`를 신설하거나 `Doroti.App.Sdk` 내부를 app/runner 역할로 명확히 분리한다. 최종 public package는 역할이 드러나는 별도 `Doroti.Runner.Sdk`를 권장한다.

runner가 선언해야 할 최소 property/item은 다음과 같다.

- `DorotiAppProject`: 루트 앱 `.csproj`
- `DorotiTarget`: canonical target 이름
- `DorotiHostKind`: Maui, BlazorWebAssembly, Qt 등
- `DorotiNativeEntryKind`: WinUI-Xaml, Android-Application, UIKit-Main, Managed-Main, Qt-Main
- `TargetFramework`와 하나 이상의 허용 `RuntimeIdentifier`
- 정확히 하나의 `DorotiTargetDescriptor`

runner SDK가 해야 할 일은 다음과 같다.

- 루트 앱 assembly를 `ProjectReference`로 연결한다.
- runner `obj/Doroti.Generated/`에 bootstrap과 plugin registration을 생성한다.
- startup type, canonical target, TFM, RID, target package의 일치를 build 전에 검사한다.
- 다른 플랫폼 디렉터리의 source/resource가 compile item에 들어오면 실패시킨다.
- app asset과 platform package asset의 중복 logical name을 실패시킨다.
- generated file은 `WriteOnlyWhenDifferent`로 쓰고 source tree에 만들지 않는다.
- target graph evidence에 app project, runner project, host, entry kind, TFM, RID, binding project를 기록한다.

### 5.3 workspace manifest와 CLI

`doroti-workspace.json`은 경로 선택만 소유한다. TFM/RID/package 버전의 중복 source of truth로 만들지 않는다.

예시:

```json
{
  "schemaVersion": "doroti.workspace/v1",
  "applicationProject": "DorotiDemoApp.csproj",
  "platforms": {
    "android": "android/DorotiDemoApp.Android.csproj",
    "ios": "ios/DorotiDemoApp.iOS.csproj",
    "linux": "linux/DorotiDemoApp.Linux.csproj",
    "macos": "macos/DorotiDemoApp.MacCatalyst.csproj",
    "web": "web/DorotiDemoApp.Web.csproj",
    "windows": "windows/DorotiDemoApp.Windows.csproj"
  }
}
```

`Doroti/eng/doroti.ps1` 또는 제품 CLI에 다음 surface를 추가한다.

```text
doroti build   --app <path> --platform <name> [--rid <rid>]
doroti run     --app <path> --platform <name> [--device <id>]
doroti publish --app <path> --platform <name> [--rid <rid>]
doroti doctor  --app <path> --platform <name|all>
```

- CLI는 manifest에서 runner 경로를 찾고 해당 runner project에 `dotnet` 명령을 전달한다.
- runner 자체의 `dotnet build/run/publish`도 지원한다.
- 기존 `dotnet run --project DorotiDemoApp.csproj -p:DorotiTarget=...`는 한 번의 migration window 동안 명시적 호환 shim 또는 오류+새 명령 안내를 제공한 뒤 제거한다.
- 호환 shim이 `dotnet run` semantics를 보존할 수 없는 경우 조용히 다른 configuration을 실행하지 말고 즉시 actionable error를 낸다.

## 6. 플랫폼별 설계

### 6.1 Windows

- 현재 `Platforms/Windows`의 WinUI/MAUI shell을 `windows/` runner로 먼저 이동한다.
- 기존 `DorotiMauiWinUIApplication`, `DorotiMauiSurface`, Windows input/semantics/text-input 계약을 그대로 사용한다.
- WinUI resource bootstrap을 위해 `App.xaml` 하나만 허용하는 기존 결정을 유지한다.
- 기존 Debug `dotnet run`과 Release publish를 동일 configuration으로 비교한다.

### 6.2 Web

- 현재 Blazor WebAssembly project 역할, TypeScript source, `wwwroot`를 `web/`로 이동한다.
- `Microsoft.TypeScript.MSBuild`는 Web runner에서만 활성화한다.
- 생성 JS는 `web/obj/.../Doroti.Generated/wwwroot` 및 publish output에만 둔다.
- loader가 `Blazor.start()`를 한 번만 소유하는 계약을 유지한다.

### 6.3 Android

- `android/DorotiDemoApp.Android.csproj`는 `net10.0-android`와 허용 RID를 고정한 실제 app runner다.
- `MainActivity.cs`, `MainApplication.cs`, manifest/resource는 runner가 소유한다.
- `Doroti.Host.Maui`의 surface/lifecycle/input/IME/semantics는 app 폴더로 복사하지 않는다.
- Android native SDK가 필요하면 `android/native`에 Gradle Android library module을 만들고, `android/binding` project에서 `AndroidGradleProject`로 포함한다.
- Maven dependency는 runner/binding에서 `AndroidMavenLibrary` 또는 필요한 package reference로 명시해 Java dependency verification을 통과시킨다.
- binding은 root app이 아니라 Android runner만 참조한다. `native/` + `binding/`이 없는 앱은 reference 자체가 없다.
- arm64 device와 x64 emulator를 별도 runner RID gate로 관리한다.

### 6.4 iOS

- iOS를 Mac Catalyst의 alias로 추가하지 않고 `net10.0-ios`의 새 canonical `DorotiTarget=iOS`로 만든다.
- `Doroti.Host.Maui`의 UIKit 공통부와 `IOS`/`MACCATALYST` 차이를 분리한다.
- `Doroti.Target.iOS.Maui.ios-arm64` 및 simulator target package/descriptor를 추가한다.
- iOS runner는 .NET MAUI native embedding/host 초기화 후 `DorotiMauiSurface`를 native root view/controller에 연결한다.
- native Swift SDK가 필요하면 `ios/native` Xcode framework project를 `XcodeProject` item으로 `ios/binding` project에 연결한다.
- public Swift wrapper는 Objective-C에 노출 가능한 얇은 API로 제한하고 binding `ApiDefinition.cs`/enum source를 source-controlled ABI로 검증한다.
- device signing, simulator build, native launch, touch/keyboard/VoiceOver를 서로 다른 gate로 남긴다.

### 6.5 macOS

- 1차 cutover에서는 기존 `net10.0-maccatalyst` 제품을 `macos/DorotiDemoApp.MacCatalyst.csproj`로 이동한다.
- workspace alias는 `macos`여도 evidence와 target descriptor는 `MacCatalyst`라고 기록한다.
- `maccatalyst-arm64`와 필요한 x64/universal publish 정책을 별도로 검증한다.
- AppKit 기반 true macOS가 목표라면 Mac Catalyst 완료 뒤 `Doroti.Host.MacOS` 또는 Qt host의 별도 feasibility gate를 통과해야 한다.
- iOS binding을 무조건 공유하지 않는다. 실제 SDK가 양쪽을 지원할 때만 공용 source와 platform별 framework output을 명시적으로 구성한다.

### 6.6 Linux

- Linux는 MAUI runner가 아니라 `Doroti.Host.Qt` + `Doroti.Target.Linux.Qt.<rid>` 경계를 사용한다.
- 1차 권장 구조는 managed runner가 process/lifetime을 소유하고 `linux/native`의 C ABI/Qt surface shim을 P/Invoke하는 방식이다.
- CMake는 Qt native shim과 resource를 빌드하고, `.csproj` publish가 managed assembly/native library를 한 bundle로 조립한다.
- feasibility에서 다음 두 안을 비교하고 하나만 제품 경로로 남긴다.
  - managed-owned process + Qt C ABI shim
  - C++ runner + `hostfxr`/`nethost`로 managed Doroti startup 로드
- 선택 기준은 GPU surface 공유, UI-thread dispatch, IME, clipboard, pointer/keyboard, accessibility, NativeAOT/trimming, 디버깅 가능성이다.
- X11에서만 성공한 것을 Wayland 지원으로 기록하지 않는다.

## 7. Native Library Interop 운영 규칙

- upstream 전체 repository를 app에 submodule로 넣지 않는다. template 구조를 기반으로 Doroti가 소유하는 binding/native project만 생성한다.
- 도입 시 사용한 upstream commit과 .NET/MAUI/Android Gradle/Xcode toolchain을 lock/evidence에 기록한다.
- Android는 `AndroidGradleProject`, Apple은 `XcodeProject` build item을 사용하는 현재 SDK 계약을 우선한다.
- binding public API는 Doroti plugin/channel 또는 앱 service abstraction 뒤에 둔다. shared app code가 generated Java/ObjC binding type을 직접 참조하지 않는다.
- native wrapper는 필요한 API slice만 노출하고 platform SDK 전체를 재바인딩하지 않는다.
- native dependency의 license, Maven/SPM/CocoaPods provenance, package hash를 release evidence에 포함한다.
- wrapper build, binding build, managed call, callback/threading, trimming/AOT, final package 포함 여부를 각각 검증한다.
- 기본 `doroti-app` 템플릿에는 빈 binding build를 강제하지 않는다. `doroti add native-interop --platform android|ios` 같은 opt-in scaffold를 제공한다.
- Demo/validation fixture에는 최소 `platformInfo` 또는 echo 호출을 넣어 native -> binding artifact -> runner DI -> shared abstraction의 실제 연결을 증명한다.

## 8. 단계별 실행 계획

### W0. 계약 동결과 feasibility

- [ ] 현재 DemoApp의 Windows/Web/Android/Mac Catalyst target graph, package, startup, asset, plugin, live evidence를 baseline으로 캡처한다.
- [ ] `reference/flutter_sample_app`의 각 runner가 소유하는 파일을 Doroti ownership matrix와 1:1로 대응시킨다.
- [ ] `doroti.workspace/v1` schema와 platform alias/canonical target 규칙을 확정한다.
- [ ] Native Library Interop template revision과 active .NET 10 build item 호환성을 Android 및 iOS 최소 binding으로 검증한다.
- [ ] MAUI native embedding으로 Android/iOS/WinUI app project가 `DorotiMauiSurface`를 소유할 수 있는지 compile probe를 만든다.
- [ ] Linux Qt의 process ownership 두 안을 spike하고 한 안을 ADR로 선택한다.
- [ ] true macOS는 이번 범위인지, Mac Catalyst workspace까지가 이번 범위인지 acceptance 문구를 확정한다.

완료 조건:

- source ownership, build direction, runtime ownership이 겹치지 않는 ADR이 있다.
- Android/iOS에서 NLI가 app runner가 아니라 binding 경로라는 사실이 contract test와 문서에 반영된다.
- 실패한 spike를 숨기지 않고 해당 플랫폼 상태를 `blocked` 또는 `notVerified`로 남긴다.

### W1. 플랫폼 중립 앱 assembly 분리

- [ ] `Doroti.App.Sdk`에서 target SDK/TFM 선택과 platform compile glob을 분리한다.
- [ ] root app project를 `net10.0` library로 만들고 `Program.cs`, `src`, 공용 asset만 포함한다.
- [ ] `DorotiApplicationFactory`가 referenced app assembly의 startup type을 runner context로 생성하는 contract를 고정한다.
- [ ] shader/plugin/resource lookup에서 entry assembly 가정을 제거하고 descriptor의 application assembly를 사용한다.
- [ ] app project에 MAUI/Android/UIKit/WinUI/Blazor symbol 또는 reference가 들어오면 실패하는 validator를 추가한다.

완료 조건:

- root app project가 모든 host workload 없이 build된다.
- synthetic runner가 app assembly를 참조해 정확히 하나의 descriptor를 생성한다.
- app assembly fingerprint는 runner 플랫폼을 바꿔도 동일하다.

### W2. `Doroti.Runner.Sdk`와 workspace CLI

- [ ] fixed-target runner SDK props/targets와 target descriptor validation을 구현한다.
- [ ] bootstrap/plugin registration을 각 runner `obj/Doroti.Generated`에 생성한다.
- [ ] `doroti-workspace.json` parser/schema validation과 path traversal/duplicate alias 검사를 추가한다.
- [ ] `build/run/publish/doctor --app --platform --rid` dispatch를 구현한다.
- [ ] runner별 `obj/bin/publish/packages.lock.json` 격리와 no-restore 재빌드를 검증한다.
- [ ] 구 command의 compatibility/deprecation 동작을 자동화 테스트로 고정한다.

완료 조건:

- 한 runner build가 다른 runner의 restore graph나 output timestamp를 바꾸지 않는다.
- 잘못된 target/TFM/RID/app project 조합은 build 전 명확한 `DOROTIAPPxxx` 오류로 실패한다.

### W3. 기존 Windows와 Web runner 이관

- [ ] Windows shell/resource/manifest를 `windows/`로 옮기고 기존 Debug run과 Release publish를 재현한다.
- [ ] Web project/TypeScript/wwwroot/plugin을 `web/`로 옮긴다.
- [ ] Windows native repaint/input/semantics와 Web browser interaction을 구조 전 baseline과 비교한다.
- [ ] old/new source를 동시에 compile하는 transitional glob을 남기지 않는다.

완료 조건:

- Windows Graph/Build/Debug live/Release publish가 각각 PASS다.
- Web strict TypeScript/build/publish/browser interaction이 각각 PASS다.
- Windows PASS를 다른 플랫폼 evidence로 재사용하지 않는다.

### W4. Android runner와 Native Library Interop

- [ ] 기존 Android shell/manifest/resource를 `android/` runner로 이관한다.
- [ ] arm64/x64 target package와 lock/output을 runner 기준으로 재연결한다.
- [ ] optional `android/interop` scaffold와 binding project를 추가한다.
- [ ] 최소 native wrapper 호출과 callback/UI-thread dispatch fixture를 만든다.
- [ ] emulator x64와 physical arm64에서 lifecycle, GPU, touch, keyboard/IME, TalkBack, package 설치를 분리 검증한다.

완료 조건:

- Gradle native library, .NET binding, runner package, managed invocation이 end-to-end로 증명된다.
- interop을 제거한 앱도 Gradle wrapper 없이 Android runner를 build할 수 있다.
- physical 미실행 항목은 `notVerified`다.

### W5. Mac Catalyst 이관 및 iOS 신설

- [ ] 기존 Mac Catalyst를 `macos/` runner로 이관하고 이름/evidence의 platform identity를 정리한다.
- [ ] iOS TFM/RID/descriptor/target package를 SDK와 package graph에 추가한다.
- [ ] UIKit host의 iOS/Mac Catalyst 공통부와 차이점을 분리한다.
- [ ] `ios/` runner와 optional Xcode framework/binding workspace를 추가한다.
- [ ] Apple build는 Mac agent에서 restore/build/simulator/device/signing gate를 실행한다.
- [ ] Objective-C binding API와 Swift wrapper ABI drift 검사를 추가한다.

완료 조건:

- Mac Catalyst native launch와 iOS simulator launch는 별도 PASS/FAIL로 기록된다.
- iOS device, VoiceOver, signing/store archive가 실행되지 않았다면 `notVerified`다.
- Windows cross-build만으로 Apple native PASS를 만들지 않는다.

### W6. Linux Qt runner

- [ ] W0에서 선택한 process ownership 모델로 `Doroti.Host.Qt`를 구현한다.
- [ ] CMake native shim과 `net10.0/linux-x64` runner/publish bundle을 연결한다.
- [ ] Skia surface/context lifecycle, vsync, resize, scale, input, IME, clipboard, cursor, semantics bridge를 host contract에 맞춘다.
- [ ] X11과 Wayland 환경을 분리해 native launch/interaction/soak를 수행한다.
- [ ] framework-owned Qt 코드와 app-owned `linux/` customization hook을 분리한다.

완료 조건:

- Linux runner가 root app assembly를 수정하지 않고 실행된다.
- native surface 재생성 뒤 retained scene이 복구되고 input-to-present가 측정된다.
- X11/Wayland 중 실행하지 않은 환경은 `notVerified`다.

### W7. 템플릿과 DemoApp cutover

- [ ] `Doroti.Templates/content/doroti-app`에 여섯 플랫폼 workspace와 manifest를 생성한다.
- [ ] application name/id/version 치환이 모든 runner manifest/project에 일관되게 적용되는지 검사한다.
- [ ] native interop opt-in scaffold command와 rename-safe placeholder를 추가한다.
- [ ] DemoApp을 목표 구조로 옮기고 기존 `Platforms/`와 target-switching csproj path를 제거한다.
- [ ] README에 IDE/toolchain별 open/build/run 방법과 platform support status를 기록한다.
- [ ] source tree에 `bin/obj/.gradle/DerivedData/Xcode build` artifact가 들어오지 않도록 ignore/audit를 갱신한다.

완료 조건:

- 새로 생성한 앱의 tree가 DemoApp과 같은 contract를 만족한다.
- 생성 직후 root app build와 현재 host에서 가능한 runner build가 성공한다.
- template에 `Platforms/`, checked-in generated bootstrap, 빈 mandatory interop project가 없다.

### W8. validation/evidence와 release 전환

- [ ] `validate-app-targets.ps1`를 workspace manifest/runner project 기준으로 바꾼다.
- [ ] 구조, build, live, physical, native interop, package/store shard를 분리한다.
- [ ] `Developer` suite에는 빠른 root app + Windows/Web + contract gate를 넣고 20분 timeout을 유지한다.
- [ ] `Release` suite에는 현재 CI host에서 실행 불가능한 platform gate를 PASS로 합성하지 않는 evidence validator를 넣는다.
- [ ] target package, template package, docs, source manifest, product naming map을 함께 갱신한다.
- [ ] 한 migration window 뒤 old `DorotiTarget` single-project compatibility shim과 old validation branch를 제거한다.

완료 조건:

- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`가 PASS다.
- 관련 target/package/template validator와 `git diff --check`가 PASS다.
- 모든 evidence는 source fingerprint, runner project, TFM, RID, host/toolchain, 실제 실행 환경을 포함한다.

## 9. 검증 매트릭스

| Gate | 자동 구조/빌드 | native live | physical/device | 현재 계획 상태 |
|---|---:|---:|---:|---|
| root app platform-neutral build | 필요 | 해당 없음 | 해당 없음 | `notVerified` |
| Windows runner Debug/Release | 필요 | 필요 | 선택 | `notVerified` |
| Web build/publish/browser | 필요 | 필요 | 해당 없음 | `notVerified` |
| Android x64 emulator | 필요 | 필요 | 해당 없음 | `notVerified` |
| Android arm64 | 필요 | 필요 | 필요 | `notVerified` |
| Android NLI binding/call | 필요 | 필요 | 최종 package에서 필요 | `notVerified` |
| Mac Catalyst arm64 | 필요 | 필요 | Apple host 필요 | `notVerified` |
| iOS simulator | 필요 | 필요 | 해당 없음 | `notVerified` |
| iOS arm64/signing/VoiceOver | 필요 | 필요 | 필요 | `notVerified` |
| iOS NLI binding/call | 필요 | 필요 | 최종 archive에서 필요 | `notVerified` |
| Linux X11 | 필요 | 필요 | 환경별 | `notVerified` |
| Linux Wayland | 필요 | 필요 | 환경별 | `notVerified` |
| true AppKit macOS | 별도 범위 결정 필요 | 필요 | Apple host 필요 | `notVerified` |

## 10. 제거 및 금지 목록

최종 cutover에서 다음을 제거한다.

- `DorotiDemoApp/Platforms/**`
- 템플릿의 `Platforms/**`
- 하나의 app project에서 `DorotiTarget`으로 SDK/TFM/RID를 바꾸는 props/targets branch
- platform 이름으로 root `obj/bin/lock`을 수동 multiplex하는 규칙
- old platform compile glob과 source leakage validator
- obsolete compatibility/deprecation path

다음은 새 구조에도 만들지 않는다.

- `Platforms/Maui`, `Platforms/Doroti`
- checked-in `DorotiBootstrap.g.cs` 또는 plugin registration generated source
- DemoApp-only native host workaround
- 플랫폼 실패를 삼키는 broad exception handler
- Android/iOS native SDK type이 노출된 shared app API
- Mac Catalyst 결과를 true macOS PASS로 변환하는 evidence
- build PASS를 input/IME/accessibility/GPU/store acceptance PASS로 변환하는 evidence

## 11. 주요 위험과 대응

- 프로젝트 수 증가: root manifest와 CLI가 진입점을 통합하고 runner SDK가 반복 설정을 흡수한다.
- app/runner 간 resource lookup 회귀: descriptor에 application assembly/resource catalog를 명시하고 shader/plugin fixture로 보호한다.
- iOS와 Mac Catalyst의 UIKit 코드 중복: host 내부 공통 구현만 공유하고 app-owned manifest/entry point는 분리한다.
- NLI toolchain drift: upstream revision, Gradle/Xcode/.NET SDK를 lock하고 binding ABI test를 둔다.
- Android/iOS build 시간 증가: interop project는 실제 사용 시에만 graph에 연결하고 incremental output을 runner별로 격리한다.
- Linux 범위 폭증: W0 feasibility에서 process ownership과 GPU/input 최소 vertical slice가 실패하면 W6를 독립 milestone로 차단한다.
- 기존 command 파손: compatibility window와 정확한 오류 안내를 제공하되 구/신 구조를 장기간 동시에 유지하지 않는다.

## 12. 검토에 사용한 외부 기준

- [CommunityToolkit/Maui.NativeLibraryInterop](https://github.com/CommunityToolkit/Maui.NativeLibraryInterop)
- [Native Library Interop overview](https://learn.microsoft.com/dotnet/communitytoolkit/maui/native-library-interop/)
- [Native Library Interop getting started](https://learn.microsoft.com/dotnet/communitytoolkit/maui/native-library-interop/get-started)
- [.NET MAUI native embedding](https://learn.microsoft.com/dotnet/maui/platform-integration/native-embedding?view=net-maui-10.0)

Native Library Interop의 현재 template는 Android에서 Gradle library를 `AndroidGradleProject`로, iOS/Mac Catalyst에서 Xcode framework를 `XcodeProject`로 binding project에 연결한다. 이것은 native SDK binding에는 적합하지만 Flutter의 native runner와 동일한 app bootstrap은 아니므로 위 계획에서 두 역할을 분리했다.

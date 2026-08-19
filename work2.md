# Doroti 기본 Native Bridge workspace 전환 계획

- 작성일: 2026-08-19
- 상태: 기본 native bridge 구조 전환 및 Windows 자동 검증 완료; Apple Xcode/native live/physical/signing은 `notVerified`
- 선행 상태: `work.md`의 플랫폼 중립 앱 + 6개 fixed-target runner 전환 결과를 기준선으로 사용
- 대상: Android, iOS, Mac Catalyst의 native project, binding project, runner 연결, 템플릿, CLI, validation/evidence

실행 결과(2026-08-19):

- root + 6개 runner + Android/iOS/Mac Catalyst binding 3개로 10-project workspace 전환을 완료했다.
- Android Gradle unit test, AAR -> binding -> arm64/x64 runner package, Windows/Web/Linux 및 Apple managed cross-build, package-only Java/Kotlin template를 자동 검증했다.
- `Developer` suite와 app-target `All` shard가 PASS했다. 현재 evidence는 `Doroti/validation/evidence/app-targets-evidence.json`이다.
- Android Studio UI sync, emulator/physical 실행, iOS/Mac Catalyst Xcode build·launch·signing·archive는 실행 환경 증거가 없으므로 `notVerified`다.

## 0. 결론

현재 구조는 유지할 가치가 있다. 루트 앱은 플랫폼 중립 assembly이고, `android/`, `ios/`, `macos/`의 `.csproj`가 최종 APK/IPA/App bundle을 만드는 runner다. 바꿀 부분은 Native Library Interop을 별도 opt-in scaffold로 두는 정책이다.

새 기본 정책은 다음과 같다.

- Android 앱에는 Android Studio에서 바로 열 수 있는 Gradle library project와 이를 소비하는 .NET Android binding project를 기본 생성한다.
- iOS 앱에는 Xcode에서 바로 열 수 있는 framework project와 이를 소비하는 .NET iOS binding project를 기본 생성한다.
- `macos` 앱에는 Mac Catalyst용 Xcode framework project와 .NET Mac Catalyst binding project를 별도로 기본 생성한다.
- 세 binding project는 각각 `AndroidGradleProject` 또는 `XcodeProject`를 실제 build item으로 선언한다.
- 각 platform runner는 자기 binding project를 기본 참조한다. 새 앱을 만든 직후 추가 wiring 없이 Kotlin/Java 또는 Swift/Objective-C 코드를 추가할 수 있어야 한다.
- native bridge가 비어 있어도 최소 `platformInfo`/echo 계약은 항상 존재하고 전체 build graph에 연결된다.

단, `AndroidGradleProject`와 `XcodeProject`를 최종 앱 runner로 오해하지 않는다.

- `AndroidGradleProject`는 Gradle project의 AAR/APK 출력을 빌드할 수 있지만, Native Library Interop의 정상 경로는 Android library module의 AAR을 .NET binding에 넣는 것이다.
- `XcodeProject`는 Xcode framework project를 빌드하고 XCFramework를 `NativeReference`로 소비하는 항목이다.
- 따라서 Kotlin `Application`/`Activity` 또는 Swift `AppDelegate`가 .NET runtime을 역방향으로 시작하는 구조는 이 계획에 포함하지 않는다.
- 최종 app shell까지 Android Studio/Xcode가 소유해야 한다면 NativeAOT C ABI 또는 hostfxr/runtime embedding을 별도 ADR과 feasibility milestone로 설계해야 한다.

즉 목표 호출 방향은 다음으로 고정한다.

```text
Doroti platform runner
  -> platform .NET binding
    -> Gradle AAR 또는 Xcode XCFramework
      -> Kotlin/Java 또는 Swift/Objective-C native code
        -> platform SDK
```

## 1. 현행 구조 검토

현재 작업 트리에서 확인한 사실:

- `DorotiDemoApp.csproj`는 `net10.0` 플랫폼 중립 library다.
- Android, iOS, Mac Catalyst는 이미 서로 분리된 fixed-target runner `.csproj`를 가진다.
- Android runner가 `MainActivity.cs`, `MainApplication.cs`, manifest, resource와 최종 package를 소유한다.
- iOS와 Mac Catalyst runner가 각각 `AppDelegate.cs`, plist, entitlement, resource와 최종 bundle을 소유한다.
- `Doroti.Runner.Sdk`는 `DorotiNativeBindingProject`를 graph 출력에 기록하지만, 실제 binding reference를 생성하거나 target 호환성을 검사하지는 않는다.
- `Doroti/scaffolds/native-interop/android`에는 `AndroidGradleProject`를 사용하는 검증된 최소 scaffold가 있다.
- `Doroti/scaffolds/native-interop/ios`에는 `XcodeProject`를 사용하는 검증된 최소 scaffold가 있다.
- Mac Catalyst 전용 native interop scaffold와 binding project는 없다.
- 기본 DemoApp 및 `doroti-app` 템플릿에는 native/binding project가 없고, CLI `scaffold-interop` 실행 후 runner를 수동 연결해야 한다.
- 현재 `NativeInterop` validation은 중앙 scaffold만 빌드한다. 생성된 실제 앱의 runner -> binding -> native artifact 연결은 증명하지 않는다.

따라서 이번 변경은 기존 runner 개편을 되돌리는 작업이 아니라, 이미 존재하는 native interop vertical slice를 실제 앱과 템플릿의 기본 계약으로 승격하는 후속 cutover다.

## 2. 목표와 비목표

### 목표

- 새 앱 생성 직후 `android/native`를 Android Studio로, `ios/native`와 `macos/native`를 Xcode로 열 수 있다.
- native project 이름, package/bundle identity, namespace가 앱 이름 치환과 함께 유효하게 생성된다.
- runner build 한 번으로 native project, binding, 최종 app package가 순서대로 빌드된다.
- native API를 shared app code에 직접 노출하지 않고 platform service/bridge 뒤에서 사용한다.
- Gradle/Xcode configuration과 .NET `Debug`/`Release`가 명시적으로 대응한다.
- Android, iOS, Mac Catalyst가 서로의 native output과 `obj/bin`을 공유하지 않는다.
- template, DemoApp, SDK, CLI, validators, docs, evidence를 한 계약으로 함께 전환한다.
- 자동 build, native live, physical/device, signing/archive 검증을 분리해 기록한다.

### 비목표

- Gradle `application` module이 최종 Doroti APK의 주인이 되게 하지 않는다.
- Xcode application target이 최종 Doroti iOS/Mac Catalyst app의 주인이 되게 하지 않는다.
- Kotlin/Swift에서 .NET runtime을 직접 bootstrap하지 않는다.
- Mac Catalyst를 AppKit 기반 true macOS 지원으로 기록하지 않는다.
- Android/iOS/Mac의 binding type을 플랫폼 중립 `DorotiDemoApp.csproj`에서 참조하지 않는다.
- 앱마다 `Doroti.Host.Maui`의 rendering/input/IME/semantics 구현을 native bridge에 복사하지 않는다.
- Windows 교차 빌드 결과를 Xcode native build, simulator launch, signing 또는 archive PASS로 승격하지 않는다.
- checked-in AAR, XCFramework, Gradle build output, DerivedData를 source of truth로 사용하지 않는다.

## 3. 목표 디렉터리 구조

```text
DorotiDemoApp/
  DorotiDemoApp.csproj
  Program.cs
  src/
  assets/

  android/
    DorotiDemoApp.Android.csproj
    MainActivity.cs
    MainApplication.cs
    AndroidManifest.xml
    Resources/
    binding/
      DorotiDemoApp.Android.Native.csproj
      InteropContract.cs
      Transforms/
        Metadata.xml
    native/
      settings.gradle.kts
      build.gradle.kts
      gradle.properties
      gradlew
      gradlew.bat
      gradle/wrapper/**
      bridge/
        build.gradle.kts
        proguard-rules.pro
        src/main/AndroidManifest.xml
        src/main/java|kotlin/**

  ios/
    DorotiDemoApp.iOS.csproj
    AppDelegate.cs
    Info.plist
    Entitlements.plist
    Resources/
    binding/
      DorotiDemoApp.iOS.Native.csproj
      ApiDefinition.cs
      StructsAndEnums.cs
      abi-contract.json
    native/
      DorotiDemoAppNative.xcodeproj/**
      DorotiDemoAppNative/
        DorotiDemoAppNative.h
        DorotiNativeBridge.swift

  macos/
    DorotiDemoApp.MacCatalyst.csproj
    AppDelegate.cs
    Info.plist
    Entitlements.plist
    Resources/
    binding/
      DorotiDemoApp.MacCatalyst.Native.csproj
      ApiDefinition.cs
      StructsAndEnums.cs
      abi-contract.json
    native/
      DorotiDemoAppNative.xcodeproj/**
      DorotiDemoAppNative/
        DorotiDemoAppNative.h
        DorotiNativeBridge.swift
```

### 3.1 Apple project 분리 원칙

iOS와 Mac Catalyst는 UIKit 계열 코드를 일부 공유할 수 있지만 native project와 binding output은 플랫폼 폴더별로 분리한다.

- iOS binding은 `net10.0-ios`만 target한다.
- Mac binding은 `net10.0-maccatalyst`만 target한다.
- 각 Xcode scheme은 자기 support destination만 생성한다.
- 공용 Swift/Objective-C 코드가 실제로 생기면 `apple-shared` 같은 제3의 app workspace를 먼저 만들지 않는다. framework-owned 공용 source 또는 source package를 별도 계약으로 추출한다.
- iOS project가 Mac output을 대신 만들거나, macOS 폴더가 iOS native tree를 상대경로로 참조하지 않게 한다.

이 선택은 프로젝트가 조금 늘어나더라도 platform identity, signing, deployment target, output 격리를 명확히 한다.

## 4. MSBuild 계약

### 4.1 Android binding

`android/binding/*.Android.Native.csproj`가 다음 build item을 소유한다.

```xml
<AndroidGradleProject Include="..\native\build.gradle.kts"
                      ModuleName="bridge"
                      Configuration="$(Configuration)"
                      Bind="true"
                      Pack="true" />
```

규칙:

- `Include`는 Gradle wrapper와 `settings.gradle.kts`가 있는 top-level project를 가리킨다.
- `ModuleName`은 Android library module인 `bridge`로 고정한다.
- 기본 output은 AAR이어야 한다. APK를 binding input으로 사용하는 구성을 금지한다.
- native Maven dependency는 `AndroidMavenLibrary` 또는 대응 `PackageReference`로 binding/runner graph에 명시해 Java dependency verification을 통과시킨다.
- `Debug`/`Release` mapping과 `OutputPath`는 runner별 `obj/<rid>/<configuration>/native/android` 아래로 격리한다.
- Gradle wrapper version, Android Gradle Plugin, Kotlin plugin, compile/min SDK를 중앙 contract/evidence에 기록한다.

### 4.2 iOS binding

`ios/binding/*.iOS.Native.csproj`가 다음 build item을 소유한다.

```xml
<XcodeProject Include="..\native\DorotiDemoAppNative.xcodeproj"
              SchemeName="DorotiDemoAppNative-iOS"
              Configuration="$(Configuration)"
              Kind="Framework"
              SmartLink="true" />
```

규칙:

- Xcode target은 iOS device/simulator를 지원하는 framework/XCFramework를 생성한다.
- public Swift type과 method는 Objective-C에 노출 가능한 얇은 API로 제한한다.
- `ApiDefinition.cs`, `StructsAndEnums.cs`, `abi-contract.json`은 source-controlled ABI다.
- SPM dependency는 Xcode project에서 관리한다. CocoaPods/workspace가 필요하면 `XcodeProject`가 실제 scheme과 output을 처리할 수 있는지 별도 probe 후 허용한다.
- Xcode build output은 runner별 `obj/<rid>/<configuration>/native/xcode` 아래로 격리한다.

### 4.3 Mac Catalyst binding

`macos/binding/*.MacCatalyst.Native.csproj`는 `net10.0-maccatalyst`와 Mac Catalyst 전용 Xcode scheme을 사용한다.

```xml
<XcodeProject Include="..\native\DorotiDemoAppNative.xcodeproj"
              SchemeName="DorotiDemoAppNative-MacCatalyst"
              Configuration="$(Configuration)"
              Kind="Framework"
              SmartLink="true" />
```

규칙:

- `macos` alias의 canonical target은 계속 `MacCatalyst`다.
- iOS XCFramework를 무조건 재사용하지 않는다.
- arm64, 필요 시 x64/universal output을 서로 다른 RID gate로 검증한다.
- AppKit framework 또는 true macOS target은 이 binding project에 섞지 않는다.

### 4.4 Runner 연결

세 runner는 각자 다음 하나 이상의 item을 선언한다.

```xml
<DorotiNativeBindingProject Include="binding\DorotiDemoApp.Android.Native.csproj" />
```

`Doroti.Runner.Sdk`가 이를 실제 `ProjectReference`로 변환하고 다음을 build 전에 검사한다.

- binding path가 runner workspace 밖으로 탈출하지 않는다.
- Android runner에는 Android binding만, iOS runner에는 iOS binding만, Mac Catalyst runner에는 Mac Catalyst binding만 연결된다.
- binding TFM이 runner TFM과 호환된다.
- 같은 path/assembly가 중복 연결되지 않는다.
- platform-neutral app project는 native binding을 참조하지 않는다.
- graph fingerprint에 binding project, native project, build item, scheme/module, toolchain version이 포함된다.

Runner에 `ProjectReference`와 `DorotiNativeBindingProject`를 이중 수기 선언하지 않는다. SDK item 하나가 source of truth가 된다.

## 5. 기본 Native Bridge API

기본 프로젝트를 빈 껍데기로 만들지 않는다. 세 플랫폼 모두 동일한 의미의 최소 계약을 제공한다.

```text
platformInfo() -> platform, osVersion, bridgeVersion
echo(value) -> value
echoOnUiThread(value, callback) -> callback(value)
```

- shared app에는 `IDorotiNativePlatformBridge` 같은 Doroti-owned abstraction만 노출한다.
- runner/plugin registration이 platform binding adapter를 DI 또는 platform channel에 등록한다.
- generated Java/Objective-C binding type은 adapter 내부에서만 사용한다.
- callback은 UI thread ownership과 exception propagation을 명시한다.
- 기본 bridge version을 evidence에 기록해 잘못된 native artifact가 package에 들어간 경우를 잡는다.
- native project를 수정하지 않은 새 앱도 최소 호출 fixture로 end-to-end 연결을 검증할 수 있어야 한다.

## 6. 템플릿과 CLI 정책

### 6.1 기본 템플릿

`doroti-app`은 Android/iOS/Mac Catalyst native + binding project를 항상 생성한다.

- 기존 root + 6 runner + 3 binding으로 총 10개의 `.csproj`가 생성된다.
- Gradle/Xcode project 이름, Java/Kotlin package, Swift module, Objective-C name, managed namespace를 앱 이름과 application id에서 안전하게 치환한다.
- 이름이 숫자로 시작하거나 하이픈/공백/비 ASCII 문자를 포함할 때 언어별 identifier 규칙을 각각 적용한다.
- Gradle wrapper JAR과 scripts, `.xcodeproj` shared scheme은 추적한다.
- `.gradle`, `build`, `.idea`, `xcuserdata`, `DerivedData`, AAR, XCFramework는 무시한다.
- 처음 생성한 tree에서 추가 명령 없이 runner가 native binding을 참조해야 한다.

### 6.2 CLI

기존 `scaffold-interop`는 기본 bridge를 만드는 명령으로는 폐기한다.

권장 surface:

```text
doroti native doctor --app <path> --platform android|ios|macos
doroti native build  --app <path> --platform android|ios|macos [--rid <rid>]
doroti native open   --app <path> --platform android|ios|macos
doroti native add    --app <path> --platform android|ios|macos --name <bridge>
```

- `doctor`는 JDK/Gradle/Android SDK 또는 Xcode/xcode-select/signing prerequisite를 플랫폼별로 검사한다.
- `build`는 binding project를 직접 빌드하고 native artifact 위치를 출력한다.
- `open`은 Android Studio 또는 Xcode workspace path를 출력하거나 사용자가 명시적으로 실행을 요청한 경우에만 IDE를 연다.
- `add`는 기본 bridge 외 별도 SDK wrapper가 필요할 때 추가 named bridge를 만든다.
- 기존 `scaffold-interop`에는 한 migration window 동안 새 명령 안내 오류를 제공한 뒤 제거한다.
- CLI가 native toolchain 부재를 성공으로 삼키거나, `Release` 대신 다른 configuration을 조용히 실행하지 않게 한다.

## 7. 단계별 실행 계획

### N0. 기준선 동결과 계약 확정

- [x] 현재 `work.md` 결과의 root/Android/iOS/Mac runner graph와 자동 evidence fingerprint를 보존한다.
- [x] `AndroidGradleProject`가 library AAR을, `XcodeProject`가 framework/XCFramework를 소비한다는 SDK 계약을 ADR에 고정한다.
- [x] 최종 app runner는 .NET이 소유하고 native project는 기본 bridge/library라는 방향을 README와 ADR에 명시한다.
- [x] 현재 optional scaffold의 Android/iOS build output, toolchain version, rename 동작을 재현한다.
- [ ] Mac Catalyst 전용 최소 `XcodeProject` compile probe를 추가한다.

`notVerified`: Mac Catalyst project/scheme/binding과 Windows managed cross-build probe는 추가했지만 실제 Xcode compile은 Mac agent에서 실행하지 않았다.

완료 조건:

- app runner, binding, native project의 build direction과 source ownership이 겹치지 않는다.
- reverse runtime embedding은 별도 범위임이 문서와 validator에 명확하다.
- 구현 전 기준선의 구조/build/live/physical 상태가 구분되어 기록된다.

### N1. `Doroti.Runner.Sdk` native binding 계약

- [x] `DorotiNativeBindingProject`를 실제 `ProjectReference`로 변환한다.
- [x] target/TFM/path/중복/assembly identity validation과 명확한 `DOROTIRUNNERxxx` 진단을 추가한다.
- [x] binding/native graph fingerprint와 incremental input/output을 추가한다.
- [x] runner `obj/bin/lock`과 Gradle/Xcode output을 target/RID/configuration별로 격리한다.
- [ ] binding 변경은 해당 runner를 다시 빌드하지만 다른 platform runner timestamp는 바꾸지 않는 contract test를 추가한다.
- [x] root app이 Android/UIKit/MacCatalyst binding type/reference를 포함하면 실패하게 한다.

남은 자동화: binding source 변경 전후의 타 플랫폼 output timestamp를 직접 비교하는 전용 contract test는 아직 추가하지 않았다. 다만 Android arm64 -> x64 연속 build에서 발견된 generated-source 혼입은 `obj/**`·`bin/**` 제외 계약으로 수정하고 package gate로 고정했다.

완료 조건:

- binding item 하나만 선언해 build/reference/graph/evidence가 모두 연결된다.
- 잘못된 플랫폼 binding은 native toolchain 실행 전에 actionable error로 실패한다.
- Android 변경이 iOS/Mac output을, iOS 변경이 Mac output을 오염시키지 않는다.

### N2. Android 기본 Gradle bridge

- [x] 기존 Android scaffold를 template-owned 기본 `android/native` + `android/binding` 구조로 승격한다.
- [x] module 이름을 generic `interop`에서 app별 고정 `bridge`로 정리한다.
- [x] Java 또는 Kotlin 중 기본 언어 하나를 선택하고 template parameter로 다른 언어를 선택할 수 있게 한다.
- [x] `AndroidGradleProject` configuration/output metadata를 runner build와 동기화한다.
- [x] Android runner에 binding item을 기본 연결한다.
- [x] Maven dependency verification, trimming/AOT 보존 규칙, ProGuard/R8 consumer rule을 추가한다.
- [x] `platformInfo`, echo, UI-thread callback adapter와 managed fixture를 구현한다.
- [ ] Android Studio sync/build와 `dotnet build`가 동일 Gradle project를 사용함을 검증한다.

부분 검증: Gradle wrapper unit test와 `dotnet build`가 같은 `android/native` project를 사용하는 것은 확인했다. Android Studio UI sync 자체는 `notVerified`다.

완료 조건:

- `dotnet build android/...Android.csproj`가 Gradle AAR -> binding -> APK/AAB를 한 graph에서 생성한다.
- Android Studio에서 native project sync 및 bridge unit test가 성공한다.
- APK/AAB 안에 bridge artifact와 필요한 Maven dependency가 실제 포함된다.
- emulator/physical 미실행 항목은 `notVerified`로 남는다.

### N3. iOS 기본 Xcode bridge

- [x] iOS scaffold를 app별 `ios/native` + `ios/binding` 기본 구조로 승격한다.
- [x] Xcode project/module/scheme/header/Objective-C 이름을 rename-safe template token으로 바꾼다.
- [x] device와 simulator를 포함하는 iOS XCFramework scheme/output을 고정한다.
- [x] iOS runner에 binding item을 기본 연결한다.
- [x] Swift wrapper와 `ApiDefinition.cs`/`StructsAndEnums.cs`/ABI manifest drift validator를 추가한다.
- [x] SPM dependency 추가 예제와 license/provenance 기록 위치를 문서화한다.
- [x] `platformInfo`, echo, main-thread callback adapter와 managed fixture를 구현한다.

완료 조건:

- Mac agent에서 Xcode framework build, .NET binding build, iOS runner build가 연속 PASS다.
- simulator package가 실제 XCFramework slice와 managed binding을 포함한다.
- simulator launch와 device signing/VoiceOver/archive는 각각 별도 evidence다.

### N4. Mac Catalyst 기본 Xcode bridge

- [x] Mac Catalyst 전용 native Xcode framework와 binding scaffold를 신설한다.
- [x] `net10.0-maccatalyst`와 `maccatalyst-arm64` scheme/output을 고정한다.
- [x] Mac runner에 binding item을 기본 연결한다.
- [x] iOS ABI를 복사해 두지 않고 Mac Catalyst용 ABI manifest와 framework slice를 독립 검증한다.
- [x] x64/universal publish가 필요하면 RID별 slice 및 signing 정책을 추가한다.
- [x] `platformInfo`, echo, main-thread callback adapter와 managed fixture를 구현한다.

현재 canonical output은 arm64다. x64/universal 및 signing은 필요성이 확정될 때 별도 RID/release gate로 추가한다.

완료 조건:

- Mac agent에서 Xcode framework -> binding -> Mac Catalyst app build가 PASS다.
- native launch에서 managed -> Swift -> callback 왕복이 증명된다.
- evidence와 UI/문서에서 `macos` alias와 `MacCatalyst` identity를 함께 기록한다.
- true AppKit macOS는 계속 `outOfScope`다.

### N5. DemoApp과 template cutover

- [x] DemoApp에 세 기본 native/binding workspace를 생성하고 runner에 연결한다.
- [x] `Doroti.Templates/content/doroti-app`에 같은 구조를 반영한다.
- [x] 중앙 optional scaffold와 생성 앱 구조가 중복 source of truth가 되지 않게 template asset 또는 공용 generator 하나로 통합한다.
- [x] template token replacement를 Gradle, Java/Kotlin, Xcode pbxproj, Swift/ObjC, C# 전부에 적용한다.
- [x] 생성 project 수/경로/identity/bridge reference를 contract fixture로 고정한다.
- [x] 기존 `scaffold-interop` migration diagnostic과 추가 named bridge 명령을 구현한다.
- [x] README에 Android Studio/Xcode에서 여는 경로와 `dotnet` 전체 앱 build 흐름을 나란히 설명한다.

완료 조건:

- 새 앱을 생성한 직후 별도 scaffold/wiring 없이 세 native project가 존재하고 각 runner graph에 포함된다.
- DemoApp과 새 template 앱이 같은 ownership contract를 만족한다.
- rename 후 placeholder, 잘못된 module/scheme, 절대 경로가 남지 않는다.

### N6. Validation과 evidence 전환

- [x] `NativeInterop` shard를 중앙 scaffold build에서 DemoApp 및 package-only template의 실제 연결 검증으로 바꾼다.
- [x] `Graph`, `Build`, `Package`, `Template`, `NativeInterop` shard fingerprint에 native source/toolchain/output을 반영한다.
- [x] Android Gradle wrapper integrity, AAR 생성, binding API, APK/AAB 포함 여부를 각각 검사한다.
- [ ] iOS/Mac Xcode scheme, XCFramework slice, ABI drift, app bundle 포함 여부를 각각 검사한다.
- [x] managed call, callback, UI thread, exception propagation, lifecycle 재생성, trimming/AOT를 live gate로 분리한다.
- [x] Mac/Xcode가 없는 Windows cross-build에서는 Xcode build를 PASS로 기록하지 않는다.
- [x] template validation의 기대 project 수를 7에서 10으로 갱신한다.
- [x] `Developer` suite는 20분 timeout 안에서 가능한 구조/빠른 build만 수행하고 device/store gate는 `Release` evidence로 분리한다.

부분 검증: iOS/Mac의 독립 scheme, destination, managed ABI drift 및 Windows cross-build는 검사한다. 실제 XCFramework slice와 app bundle 포함 여부는 Mac/Xcode gate가 없어 `notVerified`다.

완료 조건:

- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`가 PASS다.
- 관련 app-target/template/native-interoperability validator와 `git diff --check`가 PASS다.
- source build PASS와 native live/device/signing/store 상태가 evidence에서 섞이지 않는다.

### N7. 정리와 전환 완료

- [x] optional-only라는 기존 ADR/README/CLI 문구를 모두 제거하거나 새 기본 정책으로 갱신한다.
- [x] 더 이상 쓰지 않는 중앙 scaffold 복사 경로와 obsolete validator branch를 제거한다.
- [x] product solution, package manifest, template package, source manifest, product naming map을 동기화한다.
- [x] dependency/toolchain lock과 native license/provenance 문서를 release input으로 추가한다.
- [x] 최종 source tree에 `bin/obj/.gradle/build/DerivedData/xcuserdata/AAR/XCFramework`가 추적되지 않는지 audit한다.
- [x] 전체 변경 후 기존 Windows/Web/Linux runner와 root app에 회귀가 없는지 재검증한다.

완료 조건:

- DemoApp, template, SDK, CLI, docs, validators가 모두 “기본 native bridge”라는 하나의 정책을 표현한다.
- Android/iOS/Mac native project를 제거하거나 연결하지 않은 불완전 앱은 조용히 성공하지 않고 명확히 실패한다.
- 모든 미실행 native/physical/store gate는 `notVerified`다.

## 8. 검증 매트릭스

| Gate | 구조/build | native live | physical/signing | 2026-08-19 실행 상태 |
|---|---:|---:|---:|---|
| root app platform-neutral | 필요 | 해당 없음 | 해당 없음 | 자동 build `PASS` |
| Android Gradle sync/library test | 필요 | Android Studio | 해당 없음 | Gradle library test `PASS`; Studio sync `notVerified` |
| Android AAR -> binding | 필요 | managed call 필요 | 선택 | 자동 build `PASS`; live call `notVerified` |
| Android runner APK/AAB 포함 | 필요 | emulator 필요 | arm64 device 필요 | APK/AAR 전파 `PASS`; live/physical `notVerified` |
| Android UI-thread callback/IME/lifecycle | 필요 | 필요 | physical 권장 | `notVerified` |
| iOS Xcode framework/XCFramework | Mac 필요 | simulator 필요 | device/signing 별도 | scheme/ABI `PASS`; Xcode build `notVerified` |
| iOS binding -> runner package | Mac 필요 | simulator 필요 | archive/device 필요 | Windows cross-build `PASS`; Mac package `notVerified` |
| iOS main-thread callback/VoiceOver | 필요 | 필요 | device 필요 | `notVerified` |
| Mac Catalyst Xcode framework/XCFramework | Mac 필요 | 필요 | signing 별도 | scheme/ABI `PASS`; Xcode build `notVerified` |
| Mac Catalyst binding -> app bundle | Mac 필요 | native launch 필요 | archive 필요 | Windows cross-build `PASS`; Mac app bundle `notVerified` |
| package-only template 10 projects | 필요 | 선택 | 해당 없음 | Java/Kotlin 생성 및 Windows/Web build `PASS` |
| Windows/Web/Linux regression | 필요 | 기존 target별 | 기존 target별 | 자동 build `PASS`; native live `notVerified` |
| true AppKit macOS | 별도 설계 | 필요 | 필요 | `outOfScope` |

기존 `work.md`의 PASS는 구조 변경 전 기준선일 뿐이며, 이번 변경 뒤 자동으로 재사용하지 않는다.

## 9. 제거 및 금지 목록

전환 완료 후 제거하거나 바꾼다.

- native bridge를 opt-in이라고 설명하는 README/ADR 문구
- 기본 bridge 생성을 담당하던 `scaffold-interop` command path
- 중앙 scaffold만 빌드하고 실제 template/runner 연결을 검사하지 않는 `NativeInterop` gate
- template 생성 project 수 7개 가정
- graph에는 기록하지만 실제 reference/validation에는 쓰지 않는 `DorotiNativeBindingProject` 동작

다음은 만들지 않는다.

- root app project의 platform binding reference
- Android Gradle application module과 .NET Android runner가 동시에 최종 APK를 소유하는 이중 app graph
- Xcode application target과 .NET iOS/Mac runner가 동시에 최종 bundle을 소유하는 이중 app graph
- iOS native output을 상대경로로 가져다 쓰는 Mac runner 또는 그 반대
- source-controlled AAR/XCFramework/DerivedData
- native dependency 오류를 broad exception handler로 숨기는 adapter
- Windows cross-build를 Apple native PASS로 기록하는 evidence
- Mac Catalyst를 true macOS로 표시하는 alias 또는 package 이름

## 10. 주요 위험과 대응

- 기본 앱의 project/toolchain 수 증가: `Doroti.Runner.Sdk`와 CLI가 반복 wiring을 흡수하고, 플랫폼별 doctor가 필요한 toolchain만 진단한다.
- native 기능을 쓰지 않는 앱의 build 시간 증가: 최소 bridge를 작게 유지하고 Gradle/Xcode incremental output을 runner별로 격리한다. 기본 연결을 끄는 우회 property는 1차 cutover에 만들지 않는다.
- Android dependency 누락: Maven dependency verification과 final package inspection을 함께 둔다.
- Xcode ABI drift: Swift/Objective-C export와 source-controlled C# binding API를 contract hash로 비교한다.
- Debug/Release 불일치: MSBuild configuration을 Gradle/Xcode configuration에 명시적으로 매핑하고 다른 configuration fallback을 금지한다.
- Windows에서 Apple build가 겉으로 성공하는 문제: cross-build assembly와 실제 Xcode framework build evidence를 별도 필드로 기록한다.
- iOS/Mac 코드 중복: 먼저 platform identity를 분리하고, 실제 공용 코드가 확인된 뒤 framework-owned package로 추출한다.
- template rename 파손: pbxproj/module/package/namespace별 identifier normalization fixture를 둔다.
- 기존 큰 미커밋 변경과 충돌: `work.md` 결과를 기준선으로 보존하고 N0부터 단계별로 작은 diff와 validator를 함께 적용한다.

## 11. 예상 주요 변경 파일

```text
Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.props
Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets
Doroti/scaffolds/native-interop/** 또는 이를 대체할 template asset/generator
Doroti/templates/Doroti.Templates/content/doroti-app/android/**
Doroti/templates/Doroti.Templates/content/doroti-app/ios/**
Doroti/templates/Doroti.Templates/content/doroti-app/macos/**
Doroti/templates/Doroti.Templates/content/doroti-app/.template.config/template.json
DorotiDemoApp/android/**
DorotiDemoApp/ios/**
DorotiDemoApp/macos/**
Doroti/eng/doroti.ps1
Doroti/eng/validate-app-targets.ps1
Doroti/validation/contracts/platform-workspace-ownership.json
Doroti/validation/evidence/**
Doroti/docs/adr/ADR-021-platform-runner-workspaces.md 또는 후속 ADR
Doroti/README*.md
DorotiDemoApp/README*.md
```

## 12. 검토 기준

- [Native Library Interop overview](https://learn.microsoft.com/dotnet/communitytoolkit/maui/native-library-interop/)
- [Native Library Interop getting started](https://learn.microsoft.com/dotnet/communitytoolkit/maui/native-library-interop/get-started)
- [.NET for Android build items: `AndroidGradleProject`](https://learn.microsoft.com/dotnet/android/building-apps/build-items#androidgradleproject)
- [.NET for iOS/Mac Catalyst build items: `XcodeProject`](https://learn.microsoft.com/dotnet/ios/building-apps/build-items#xcodeproject)
- [.NET MAUI native embedding](https://learn.microsoft.com/dotnet/maui/platform-integration/native-embedding?view=net-maui-10.0)

공식 build item 계약에 따라 이 계획은 Gradle/Xcode native **library/framework**를 .NET runner에 연결한다. Android Studio/Xcode가 최종 Doroti application process를 소유하는 reverse embedding은 동일한 기능으로 간주하지 않는다.

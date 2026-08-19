# Doroti 기본 Native Bridge workspace 전환 요약

- 기록일: 2026-08-19
- 상태: **기본 native bridge 구조와 Windows 호스트 자동 검증 완료, native live/physical/Apple Xcode·signing은 `notVerified`**
- 원본: 삭제한 루트 `work2.md`의 완료 상태를 압축한 역사 기록
- 선행 작업: [`platform-runner-workspace-summary.md`](platform-runner-workspace-summary.md)

## 1. 문서 성격

이 문서는 Android, iOS, Mac Catalyst의 Native Library Interop을 선택형 scaffold에서 기본 앱 workspace 계약으로 전환한 작업의 종료 시점을 보존한다. 새로운 active roadmap이나 실행 지시서가 아니다.

루트 앱과 6개 fixed-target runner 구조는 유지했다. 이번 변경은 Android Gradle library와 Apple Xcode framework, 각 .NET binding을 기본 생성하고 matching runner에 자동 연결한 후속 cutover다.

## 2. 최종 소유권과 호출 방향

최종 application package와 process는 계속 Doroti의 .NET platform runner가 소유한다.

```text
Doroti platform runner
  -> platform .NET binding
    -> Gradle AAR 또는 Xcode XCFramework
      -> Kotlin/Java 또는 Swift/Objective-C native code
        -> platform SDK
```

- `AndroidGradleProject`는 Android library module의 AAR을 binding에 넣는다.
- `XcodeProject`는 iOS 또는 Mac Catalyst framework/XCFramework를 `NativeReference`로 소비한다.
- root app은 플랫폼 중립 상태를 유지하고 native binding type을 직접 참조하지 않는다.
- Kotlin `Application`/`Activity`나 Swift `AppDelegate`가 .NET runtime을 역방향으로 시작하는 구조는 포함하지 않았다.
- `macos` workspace alias의 실제 target은 계속 Mac Catalyst이며 true AppKit macOS는 `outOfScope`다.

## 3. 최종 workspace

기본 앱은 root app, 6개 runner, 3개 binding으로 총 10개의 `.csproj`를 가진다. Android, iOS, Mac Catalyst에는 각각 독립 native project와 binding project가 있다.

```text
DorotiDemoApp/
  DorotiDemoApp.csproj
  android/
    DorotiDemoApp.Android.csproj
    binding/DorotiDemoApp.Android.Native.csproj
    native/                         # Gradle library project
  ios/
    DorotiDemoApp.iOS.csproj
    binding/DorotiDemoApp.iOS.Native.csproj
    native/                         # iOS Xcode framework project
  macos/
    DorotiDemoApp.MacCatalyst.csproj
    binding/DorotiDemoApp.MacCatalyst.Native.csproj
    native/                         # Mac Catalyst Xcode framework project
  linux/
  web/
  windows/
```

iOS와 Mac Catalyst는 source, scheme, ABI manifest, output을 서로 공유하지 않는다. target/RID/configuration별 `obj`, `bin`, lock, Gradle/Xcode output 격리도 유지한다.

## 4. 구현한 계약

### Runner SDK와 native binding

- `DorotiNativeBindingProject`를 실제 `ProjectReference`로 변환했다.
- binding path 탈출, target/TFM 불일치, 중복 path/assembly와 root app의 platform binding 유입을 build 전에 진단한다.
- graph fingerprint에 binding/native project, module 또는 scheme, build item과 toolchain 정보를 포함한다.
- runner가 `ProjectReference`와 `DorotiNativeBindingProject`를 이중 선언하지 않도록 SDK item 하나를 source of truth로 고정했다.
- Android arm64/x64 연속 build에서 발견한 generated-source 혼입을 `obj/**`와 `bin/**` 제외 계약으로 막고 package gate에 고정했다.

### 기본 bridge API

세 플랫폼에 같은 의미의 최소 bridge를 제공한다.

```text
platformInfo() -> platform, osVersion, bridgeVersion
echo(value) -> value
echoOnUiThread(value, callback) -> callback(value)
```

shared app에는 Doroti-owned abstraction만 노출하고 generated Java/Objective-C binding type은 platform adapter 내부에 가뒀다. UI-thread callback, exception propagation과 bridge version도 validation/evidence 계약에 포함했다.

### Android

- 기존 중앙 scaffold를 app별 `android/native` Gradle library와 `android/binding` 구조로 승격했다.
- `bridge` AAR을 `AndroidGradleProject`로 빌드해 Android binding과 runner package에 연결했다.
- Java/Kotlin template 선택, Maven dependency verification, trimming/AOT 보존과 ProGuard/R8 consumer rule을 반영했다.
- Gradle wrapper unit test와 `dotnet build`가 같은 native project를 사용하도록 고정했다.

### iOS와 Mac Catalyst

- 두 플랫폼에 독립 Xcode framework project, binding, scheme과 ABI manifest를 만들었다.
- Swift export와 `ApiDefinition.cs`/`StructsAndEnums.cs`의 drift를 managed contract로 검사한다.
- iOS device/simulator와 Mac Catalyst arm64 identity를 분리했다.
- Mac Catalyst x64/universal 및 signing은 필요성이 확정될 때 별도 RID/release gate로 추가한다.

### Template, CLI, 문서

- DemoApp과 `doroti-app` template을 동일한 기본 native/binding 구조로 전환했다.
- Gradle, Java/Kotlin, Xcode pbxproj, Swift/Objective-C와 C# 전체에 rename-safe token replacement를 적용했다.
- package-only template의 기대 project 수를 7개에서 10개로 갱신했다.
- 기본 bridge를 만들던 `scaffold-interop` 경로는 migration diagnostic으로 바꾸고 `doroti native doctor|build|open|add` surface를 추가했다.
- opt-in 정책 문구, 중앙 scaffold 복사 경로와 obsolete validator branch를 제거했다.
- native dependency/toolchain lock과 license/provenance를 release input으로 기록했다.

## 5. Validation과 기록된 자동 결과

삭제 전 `work2.md`에 기록된 2026-08-19 종료 evidence는 다음과 같다.

- root + 6개 runner + Android/iOS/Mac Catalyst binding의 10-project workspace: 전환 완료
- Android Gradle library unit test와 AAR 생성: `PASS`
- AAR -> Android binding -> arm64/x64 runner package 전파: `PASS`
- iOS/Mac Catalyst 독립 scheme 및 managed ABI drift 검사: `PASS`
- iOS/Mac Catalyst Windows managed cross-build: `PASS`
- package-only Java/Kotlin template 생성과 Windows/Web build: `PASS`
- Windows/Web/Linux 및 root app 회귀 자동 build: `PASS`
- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`: `PASS`
- app-target `All` shard: `PASS`
- evidence: `Doroti/validation/evidence/app-targets-evidence.json`
- 관련 app-target/template/native-interoperability validator와 `git diff --check`: `PASS`

이 목록은 삭제한 계획 문서의 종료 기록을 보존한 것이며, 이 역사 문서 작성 과정에서 build나 live gate를 다시 실행했다는 뜻은 아니다.

## 6. 남은 `notVerified` 경계

### Android

- Android Studio UI sync/build
- emulator/physical launch와 최종 managed -> native call/callback
- UI-thread callback, lifecycle, IME와 TalkBack의 실제 동작

### iOS

- Mac agent의 Xcode framework/XCFramework build와 slice 검사
- binding을 포함한 iOS runner package 및 simulator launch
- device signing, VoiceOver와 archive/store packaging
- managed -> Swift -> callback 왕복

### Mac Catalyst

- Mac agent의 Xcode framework -> binding -> app bundle build
- native launch와 managed -> Swift -> callback 왕복
- x64/universal output, signing과 archive

### 기타 target

- 이번 종료 기록의 Windows/Web/Linux 결과는 자동 build 회귀 증거다. 각 target의 native/browser live interaction과 physical acceptance를 대신하지 않는다.
- true AppKit macOS는 `outOfScope`다.

Windows cross-build PASS를 Apple Xcode/native PASS로 해석하지 않는다.

## 7. 완료 상태와 남은 자동화

| Milestone | 종료 상태 |
| --- | --- |
| N0 기준선/계약 | 방향과 문서 계약 완료, Mac Xcode compile probe `notVerified` |
| N1 Runner SDK binding | 핵심 계약 완료, 타 플랫폼 output timestamp 비교 전용 test 미추가 |
| N2 Android bridge | 구조·Gradle·AAR·package 자동 검증 완료, Studio/live/physical `notVerified` |
| N3 iOS bridge | 구조·scheme·ABI·Windows cross-build 완료, Xcode/live/signing `notVerified` |
| N4 Mac Catalyst bridge | 구조·scheme·ABI·Windows cross-build 완료, Xcode/live/signing `notVerified` |
| N5 DemoApp/template | 완료 |
| N6 Validation/evidence | Windows-host 자동 계약 완료, Apple artifact/app-bundle 검사 `notVerified` |
| N7 정리 | 완료 |

계획의 미체크 항목은 숨기지 않고 다음 경계로 보존한다.

- binding source 변경 전후에 다른 platform output timestamp가 변하지 않는지 직접 비교하는 전용 contract test
- Android Studio sync/build 자체 검증
- Mac agent에서 iOS/Mac Catalyst XCFramework slice와 최종 app bundle 포함 여부 검증

## 8. 제거 및 금지 상태

- 기본 native bridge를 opt-in으로 설명하는 정책과 template 7-project 가정을 제거했다.
- 중앙 scaffold만 빌드하고 실제 app 연결을 증명하지 않던 `NativeInterop` gate를 실제 DemoApp/package-only template 검증으로 전환했다.
- source-controlled AAR, XCFramework, `.gradle`, `build`, `DerivedData`, `xcuserdata`를 source of truth로 사용하지 않는다.
- Android Gradle application과 .NET runner가 APK를 함께 소유하거나, Xcode application과 .NET runner가 bundle을 함께 소유하는 이중 app graph를 만들지 않는다.
- iOS와 Mac Catalyst가 서로의 native output을 상대경로로 재사용하지 않는다.

## 9. 주요 변경 영역

- `Doroti/src/Doroti.Runner.Sdk/`
- `Doroti/templates/Doroti.Templates/content/doroti-app/android/`
- `Doroti/templates/Doroti.Templates/content/doroti-app/ios/`
- `Doroti/templates/Doroti.Templates/content/doroti-app/macos/`
- `Doroti/eng/doroti.ps1`
- `Doroti/eng/validate-app-targets.ps1`
- `Doroti/validation/contracts/`
- `Doroti/validation/evidence/`
- `Doroti/docs/adr/ADR-021-platform-runner-workspaces.md`
- `DorotiDemoApp/android/`
- `DorotiDemoApp/ios/`
- `DorotiDemoApp/macos/`

## 10. 재개 시 주의사항

- runner -> binding -> native library/framework 방향을 유지한다.
- root app의 platform-neutral 경계와 target/RID/configuration별 output 격리를 유지한다.
- Mac/Xcode가 없는 Windows cross-build에서 Apple native build를 PASS로 기록하지 않는다.
- 자동 build 성공을 emulator/device, callback, lifecycle, accessibility, signing, archive 또는 store acceptance로 승격하지 않는다.
- 남은 검증은 위 `notVerified` 항목별로 별도 evidence를 추가한다.

## 11. 종료 판단

선택형이던 Android/iOS Native Library Interop을 Android, iOS, Mac Catalyst의 기본 workspace로 승격하고 DemoApp, template, Runner SDK, CLI, 문서, validator와 evidence를 하나의 정책으로 전환했다. Windows 호스트에서 가능한 구조/build/package 검증은 완료했다. Android Studio, emulator/physical, Apple Xcode/native launch/signing/archive는 실행 증거가 없으므로 계속 `notVerified`다.

> 문서 성격: 삭제한 루트 `work2.md`의 기본 native bridge 전환 완료 요약과 evidence 경계.

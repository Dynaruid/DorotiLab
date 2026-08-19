# Doroti 플랫폼 runner workspace 개편 요약

- 기록일: 2026-08-19
- 상태: **구조 개편 및 현재 Windows 호스트의 자동 검증 완료, native live/physical/Apple/Qt 실행은 부분 미검증**
- 원본: 삭제한 루트 `work.md`의 완료 상태를 압축한 역사 기록
- 후속 결과: [`native-bridge-workspace-summary.md`](native-bridge-workspace-summary.md)

## 1. 문서 성격

이 문서는 Doroti 앱을 Flutter와 비슷한 플랫폼 workspace 구조로 전환한 작업의 종료 시점을 보존한다. 새로운 active roadmap이나 실행 지시서가 아니다.

기존에는 하나의 앱 `.csproj`가 `DorotiTarget`에 따라 SDK, TFM, RID, host와 `Platforms/<Target>` source를 바꿨다. 개편 후 루트 앱은 플랫폼 중립 assembly가 되었고, `android`, `ios`, `linux`, `macos`, `web`, `windows`가 각각 고정 target runner를 소유한다.

## 2. 최종 아키텍처

```text
DorotiDemoApp/
  DorotiDemoApp.csproj          # net10.0 플랫폼 중립 앱 library
  Program.cs                    # IDorotiApplicationStartup
  src/
  assets/
  doroti-workspace.json         # platform alias -> runner path

  android/                      # net10.0-android runner
  ios/                          # net10.0-ios runner
  linux/                        # net10.0/linux-x64 Qt runner
  macos/                        # net10.0-maccatalyst runner
  web/                          # browser-wasm runner
  windows/                      # WinUI/MAUI runner
```

소유권은 다음처럼 분리했다.

- 루트 앱: `Program : IDorotiApplicationStartup`, widget/app source, 공용 logical asset
- platform runner: 고정 TFM/RID, native entry point, manifest, entitlement, resource, 최종 build/run/publish
- `Doroti.Runner.Sdk`: target descriptor 검증, 앱 assembly 연결, bootstrap/plugin registration 생성
- `Doroti.Hosting`: target-neutral startup 및 application descriptor/boundary
- `Doroti.Host.*`: surface, lifecycle, input, IME, clipboard, semantics 같은 필수 host 구현
- `Doroti.Target.*`: platform/RID별 배포 dependency와 capability
- `Doroti/eng`: workspace manifest를 읽어 runner를 선택하는 공통 CLI와 validation entry point

`doroti-workspace.json`은 runner 경로만 소유한다. TFM, RID, host, native entry kind, target package의 source of truth는 각 runner project와 하나의 `DorotiTargetDescriptor`다.

## 3. 구현한 주요 변경

### 플랫폼 중립 앱과 runner SDK

- 루트 앱 project를 `net10.0` library로 분리하고 platform SDK/reference/source 유입을 금지했다.
- runner가 referenced app assembly에서 startup type을 생성하도록 `DorotiApplicationFactory` 계약을 고정했다.
- shader, plugin, resource lookup의 entry assembly 가정을 제거하고 descriptor의 application assembly를 사용한다.
- `Doroti.Runner.Sdk`가 runner별 generated bootstrap과 plugin registration을 `obj/Doroti.Generated` 아래에 만든다.
- runner별 `obj`, `bin`, publish, lock 경로를 target/RID/configuration별로 격리했다.
- 구 single-project `DorotiTarget` 진입은 compatibility shim 대신 `DOROTIAPP100` 안내 오류로 전환했다.

### Workspace CLI

`Doroti/eng/doroti.ps1`에 다음 platform dispatch를 추가했다.

```text
doroti build   --app <path> --platform <name> [--rid <rid>]
doroti run     --app <path> --platform <name> [--device <id>]
doroti publish --app <path> --platform <name> [--rid <rid>]
doroti doctor  --app <path> --platform <name|all>
```

manifest path traversal, duplicate alias, 존재하지 않는 runner와 target/TFM/RID 불일치를 build 전에 진단한다.

### Windows와 Web

- Windows shell, XAML bootstrap, package manifest와 resource를 `windows/` runner로 이동했다.
- Windows에서는 WinUI resource bootstrap을 위한 `App.xaml` 하나만 유지했다.
- Web의 Blazor project, TypeScript source, loader, plugin과 `wwwroot`를 `web/` runner로 이동했다.
- `Microsoft.TypeScript.MSBuild`는 Web runner에서만 활성화하며 생성 JS는 `obj`와 publish output에만 둔다.
- loader가 정확히 한 번의 `Blazor.start()`를 소유하는 계약을 유지했다.
- 기존 `Platforms/**`와 old/new source를 함께 compile하는 transitional glob을 제거했다.

### Android

- Android app shell, `MainActivity`, `MainApplication`, manifest와 resource를 `android/` runner로 이동했다.
- `android-arm64`와 `android-x64` target package, lock, output을 runner 기준으로 분리했다.
- 선택형 Native Library Interop scaffold를 추가했다.
- Android scaffold는 Gradle library를 `AndroidGradleProject`로 빌드하고 .NET Android binding으로 가져온다.
- 최소 native wrapper 호출과 UI-thread callback fixture를 추가했다.

### iOS와 Mac Catalyst

- 기존 Mac Catalyst app을 `macos/` runner로 이동했다.
- `macos` workspace alias와 canonical `MacCatalyst` target identity를 구분했다.
- `net10.0-ios`의 canonical `iOS` target, device/simulator RID, target package와 descriptor를 신설했다.
- UIKit host의 iOS/Mac Catalyst 공통부와 platform 차이를 분리했다.
- `ios/` runner와 Xcode framework + .NET binding scaffold를 추가했다.
- Swift wrapper의 Objective-C export와 source-controlled `ApiDefinition.cs` ABI drift 계약을 추가했다.

현재 `macos`는 AppKit 기반 true macOS가 아니라 Mac Catalyst다.

### Linux Qt

- Linux는 MAUI target으로 가장하지 않고 `Doroti.Host.Qt` + `Doroti.Target.Linux.Qt.linux-x64` 경계를 사용한다.
- managed runner가 process와 Doroti lifetime을 소유하고, Qt C ABI shim이 native event loop/window integration을 담당하는 모델을 선택했다.
- CMake native shim과 managed publish bundle의 구조적 vertical slice를 연결했다.
- framework-owned Qt 코드와 app-owned `linux/` customization hook을 분리했다.

### 템플릿과 DemoApp

- `doroti-app` 템플릿과 DemoApp을 같은 6개 platform workspace 구조로 전환했다.
- application name, id, version이 모든 runner manifest/project에 일관되게 치환되도록 검증했다.
- Android/iOS native interop은 `scaffold-interop`로 추가하는 opt-in 구조로 마감했다.
- 기존 `Platforms/**`, checked-in generated bootstrap과 target-switching root project path를 제거했다.
- `bin`, `obj`, `.gradle`, `DerivedData`, Xcode build output이 source tree에 들어오지 않도록 ignore/audit를 갱신했다.

## 4. Native Library Interop 경계

이 작업에서 Native Library Interop은 최종 application runner가 아니라 app-to-native SDK binding 경로다.

```text
Doroti .NET runner
  -> .NET binding
    -> Android Gradle AAR 또는 Xcode XCFramework
      -> Kotlin/Java 또는 Swift/Objective-C wrapper
```

- Android는 `AndroidGradleProject`를 사용한다.
- iOS는 `XcodeProject`와 source-controlled Objective-C binding API를 사용한다.
- binding은 matching platform runner만 참조하고 root app에는 노출하지 않는다.
- Kotlin/Swift application이 .NET runtime을 역방향으로 직접 시작하는 모델은 구현하지 않았다.

이 opt-in 정책을 기본 native bridge workspace로 바꾼 후속 작업은 [`native-bridge-workspace-summary.md`](native-bridge-workspace-summary.md)에 완료 기록으로 남겼다.

## 5. 현재 호스트에서 확인한 자동 결과

- `validate-app-targets.ps1`의 `Graph`, `Build`, `Package`, `NativeInterop`, `Template` shard: 각각 `PASS`
- 마지막 `-Shard All`: 각 shard PASS stamp와 evidence 생성까지 확인했으나 사용자 요청으로 종료하면서 최종 COMPLETE 출력 회수 전 프로세스 정리
- root platform-neutral app build: `PASS`
- Windows Release build/publish: `PASS`
- Web strict TypeScript build 및 Release publish: `PASS`
- Android `android-arm64`/`android-x64` Release build: `PASS`
- Mac Catalyst `maccatalyst-arm64` Windows cross-build: `PASS`
- iOS `iossimulator-x64`/`ios-arm64` Windows cross-build: `PASS`
- Linux `linux-x64` managed cross-build: `PASS`
- Android/iOS 중앙 Native Library Interop binding build: `PASS`
- rename-safe 외부 interop scaffold 생성/build: `PASS`
- package-only `doroti-app` 설치, identity/version 치환, 7개 project 생성: `PASS`
- 외부 template root/Windows/Web build: `PASS`
- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`: `PASS`
- `doroti doctor -App .\DorotiDemoApp`: `PASS`
- 종료 시점 `git diff --check`: `PASS`

이 결과는 2026-08-19의 미커밋 작업 트리에서 얻은 종료 기록이다. 구조를 추가 변경한 뒤에는 재사용하지 않고 다시 검증해야 한다.

## 6. 남은 검증 경계

다음 항목은 실행하지 않았거나 완료 조건을 충족하지 않았으므로 `notVerified`다.

### Windows/Web

- Windows Debug native launch
- Windows repaint/input/IME/semantics/scroll capture
- Web browser interaction

### Android

- x64 emulator launch와 package 설치
- arm64 physical lifecycle, GPU, touch, keyboard/IME, TalkBack
- 최종 app의 managed -> native interop call/callback

### Apple

- Mac agent의 Xcode restore/build
- Mac Catalyst native launch
- iOS simulator/device launch
- VoiceOver, signing, archive/store packaging
- Xcode framework의 실제 native call/callback

Windows cross-build PASS는 Apple native PASS가 아니다.

### Linux

- Qt native build
- 실제 Skia surface/context lifecycle, vsync, resize와 scale
- input, IME, clipboard, cursor, accessibility bridge 완결
- X11/Wayland launch, interaction과 soak

### 범위 밖

- true AppKit macOS: `outOfScope`

## 7. Milestone 종료 상태

| Milestone | 종료 상태 |
| --- | --- |
| W0 계약/feasibility | 구조 계약과 compile probe 완료 |
| W1 플랫폼 중립 앱 | 완료 |
| W2 Runner SDK/CLI | 완료 |
| W3 Windows/Web | 구조 및 Release build/publish 완료, native/browser live `notVerified` |
| W4 Android | runner와 opt-in interop 완료, emulator/physical `notVerified` |
| W5 Mac Catalyst/iOS | 구조와 Windows cross-build 완료, Apple native/device/signing `notVerified` |
| W6 Linux Qt | managed/CMake 구조 완료, Qt native/X11/Wayland `notVerified` |
| W7 Template/DemoApp | 완료 |
| W8 Validation/evidence | 자동 contract 전환 완료, 플랫폼별 live/physical 경계 유지 |

## 8. 주요 변경 영역

- `Doroti/src/Doroti.App.Sdk/`
- `Doroti/src/Doroti.Runner.Sdk/`
- `Doroti/src/Doroti.Hosting/`
- `Doroti/src/Doroti.Host.Maui/`
- `Doroti/src/Doroti.Host.Web/`
- `Doroti/src/Doroti.Host.Qt/`
- `Doroti/src/Doroti.Target.*`
- `Doroti/templates/Doroti.Templates/content/doroti-app/`
- `Doroti/scaffolds/native-interop/`
- `Doroti/eng/doroti.ps1`
- `Doroti/eng/validate-app-targets.ps1`
- `Doroti/validation/app-runner/`
- `Doroti/validation/contracts/`
- `Doroti/validation/evidence/`
- `Doroti/docs/adr/ADR-021-platform-runner-workspaces.md`
- `DorotiDemoApp/`

## 9. 재개 시 주의사항

- 현재 worktree의 기존 대규모 변경을 먼저 보존하고 소유권을 확인한다.
- root app, runner, binding, native project의 방향을 뒤섞지 않는다.
- target/RID/configuration별 `obj/bin/lock` 격리를 유지한다.
- generated bootstrap은 runner `obj/Doroti.Generated`에만 둔다.
- Windows/Web 결과를 Android, Apple, Linux evidence로 재사용하지 않는다.
- build 성공을 input, IME, accessibility, GPU, signing, archive 또는 store acceptance로 승격하지 않는다.
- 후속 native bridge 기본화 작업의 완료 상태와 남은 검증 경계는 [`native-bridge-workspace-summary.md`](native-bridge-workspace-summary.md)에서 확인한다.

## 10. 종료 판단

플랫폼 중립 앱과 6개 fixed-target runner로의 구조 전환, SDK/CLI/template/validator cutover와 현재 호스트의 대표 자동 build는 완료했다. 반면 Android 실기기, Apple native, Linux Qt native와 일부 Windows/Web live 검증은 남아 있으므로 전체 플랫폼 runtime 완료로 기록하지 않는다.

> 문서 성격: 삭제한 루트 `work.md`의 구조 개편 완료 요약과 evidence 경계. 후속 native bridge 전환 기록은 `native-bridge-workspace-summary.md`다.

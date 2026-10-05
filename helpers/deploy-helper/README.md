# iOS·Android 빌드·설치 CLI

`DorotiSampleApp2`와 `DorotiTestbedApp`을 선택해 빌드·설치·실행합니다.
[Doroti.DeployHelper](Doroti.DeployHelper/Doroti.DeployHelper.csproj)는 외부 NuGet 패키지가 없는
.NET 10 콘솔 CLI입니다. 공통 프로세스 실행·선택 로직과 iOS/Android 배포 구현을 나눕니다.

```text
helpers/deploy-helper/
  deploy.ps1                         공통 진입점: 플랫폼·앱 선택
  deploy-ios.ps1                     iOS 진입점
  deploy-android.ps1                 Android 진입점
  Doroti.DeployHelper/               공통 CLI와 플랫폼별 구현
  tests/Doroti.DeployHelper.Tests/   콘솔 회귀 테스트
```

아래 명령은 저장소 루트 `DorotiLab` 기준입니다. PowerShell 스크립트는 파일 위치로 저장소를 찾으므로
다른 작업 폴더에서도 실행할 수 있습니다. 공통 진입점에서 `-Platform`을 생략하면 플랫폼을 번호로 선택합니다.
비대화형 실행에서는 플랫폼·앱·기기를 명시하세요. 선택지가 하나면 해당 항목을 자동 사용합니다.

```powershell
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform android -App Testbed -List
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform ios -App Sample2 -Target simulator
```

## Android

Windows·macOS·Linux에서 사용할 수 있습니다. 루트 `global.json`이 선택하는 .NET 10 SDK,
앱 빌드에 선택한 SDK의 Android/MAUI workload, Android SDK와 JDK 17이 필요합니다.
PowerShell 진입점에는 PowerShell 7이 필요하고, 네이티브 바인딩은 프로젝트의 Gradle wrapper로 빌드합니다.
CLI는 Python 없이 명령의 시간 제한과 취소를 처리합니다.

`adb`는 PATH, `ANDROID_HOME`·`ANDROID_SDK_ROOT` 및 일반적인 로컬 SDK 설치 폴더에서 찾습니다.
`-AdbPath`로 실행 파일을 직접 지정할 수도 있습니다. 실기기는 USB 디버깅을 켜고 컴퓨터의 연결을 허용하세요.
에뮬레이터는 Android Studio에서 먼저 시작합니다. 실행 중인 에뮬레이터만 표시하며 자동 생성·부팅하지 않습니다.
앱의 Graphite 렌더링에는 Vulkan 1.2 지원이 필요합니다.

```powershell
# 앱 선택 → 실기기/실행 중인 에뮬레이터 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1

# .NET 10 기본: Release / Mono Profiled AOT
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -Target device
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Testbed -Mode CoreClrJit

# .NET 11 기본: Release / CoreCLR ReadyToRun
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Testbed -DotnetVersion 11
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -DotnetVersion 11 -Mode CoreClrJit

# 에뮬레이터의 ABI에 따라 android-x64 또는 android-arm64 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Testbed -Target emulator -Device emulator-5554 -Configuration Debug

# 목록만 출력 / 빌드 명령만 확인 / 설치 후 실행 생략
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -List
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -DryRun
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Testbed -Device YOUR_ADB_SERIAL -NoLaunch
```

| Android 옵션 | 동작 |
| --- | --- |
| `-App Sample2\|Testbed` | 생략 시 앱 선택 |
| `-Target auto\|device\|emulator` | 기본 `auto`; 실기기와 실행 중인 에뮬레이터 목록 |
| `-Device <serial>` / `-Serial <serial>` | `adb devices -l`의 정확한 시리얼 |
| `-DotnetVersion 10\|11` | 기본 `10`; 저장소의 `global.json`을 수정하지 않고 앱 SDK 선택 |
| `-Configuration Debug\|Release` | 기본 `Release` |
| `-Mode Mono\|MonoAot\|CoreClrJit\|CoreClrR2R` | 아래 런타임 표 참고 |
| `-Extra 'KEY=VALUE'` | 문자열 Intent extra 전달; 해당 앱이 처리하는 키 사용 |
| `-AdbPath <경로>` / `-DotnetPath <경로>` | 별도 설치한 실행 파일 |
| `-List` / `-DryRun` / `-NoLaunch` / `-Help` | 목록 / 명령 확인 / 실행 생략 / 도움말 |

| 모드 | 런타임 / 사전 컴파일 설정 | SDK |
| --- | --- | --- |
| `Mono` | Mono JIT; Mono AOT와 R2R 비활성화 | .NET 10 |
| `MonoAot` | Mono, `RunAOTCompilation=true`, `AndroidEnableProfiledAot=true`; Release 전용 | .NET 10 |
| `CoreClrJit` | CoreCLR, `PublishReadyToRun=false`; Mono AOT 비활성화 | .NET 10/11 |
| `CoreClrR2R` | CoreCLR, `PublishReadyToRun=true`; Mono AOT 비활성화 | .NET 10/11 |

모드 생략 시 .NET 10 Release는 `MonoAot`, .NET 10 Debug는 `Mono`, .NET 11은 `CoreClrR2R`입니다.
.NET 10 Android CoreCLR은 실험적 기능입니다. .NET 11은
[SampleApp2의 SDK 선택 파일](../../samples/DorotiSampleApp2/android/sdk/net11/global.json)의
`11.0.100-rc.1.26425.128` SDK와 MAUI `11.0.0-rc.1.26451.6`을 두 앱에 공통으로 사용합니다.
해당 SDK의 Android/MAUI workload를 미리 설치하세요. Android NativeAOT는 이 도구에서 제공하지 않습니다.

연결·디버깅 권한·부팅 완료를 확인하고 `arm64-v8a`는 `android-arm64`, `x86_64`는 `android-x64`로 빌드합니다.
다른 ABI는 빌드 전에 거부합니다. 빌드는 20분 제한이며, 빌드 성공 후 선택한 앱의 signed APK가 하나인지 확인합니다.
산출물은 `Doroti/artifacts/<sample2|testbed>-android-<mono|aot|jit|r2r>-net<10|11>-<debug|release>-<arm64|x64>`에 분리합니다.
모드 이름은 각각 `Mono`·`MonoAot`·`CoreClrJit`·`CoreClrR2R`에 대응하며 Windows 리소스 도구를 위해 경로를 짧게 유지합니다.
예: `testbed-android-jit-net11-release-arm64`. Debug APK에도 managed assembly를 포함해
별도 fast deployment 없이 `adb install`로 설치할 수 있게 합니다.

`adb install -r`로 기존 앱을 업데이트하고 앱 데이터를 유지합니다. 설치가 실패하면 실행하지 않습니다.
기본 실행은 현재 Android 사용자에 설치된 launcher component를 조회하고, 기존 프로세스를 종료한 뒤
해당 component를 명시해 실행하고 PID를 확인합니다. MAUI가 생성하는 activity 클래스 이름을 하드코딩하지 않습니다.
성공 시 APK·PID·logcat 명령을 출력합니다. 화면 표시·터치·접근성은 기기에서 별도로 확인하세요.
APK 서명은 프로젝트의 로컬 기기 테스트용 설정을 사용하며 스토어 서명은 별도입니다.

Android는 `-Environment` 대신 `-Extra`를 사용합니다. 임의의 환경변수가 아니라 앱이 처리하는 Intent 키입니다.
Testbed의 샘플 선택은 다음과 같습니다. 여러 extra는 PowerShell 세션에서 배열로 전달하거나 CLI의 `--extra`를 반복합니다.

```powershell
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Testbed -Extra 'doroti_sample=input'
./helpers/deploy-helper/deploy-android.ps1 -App Testbed -Extra @('doroti_sample=material', 'doroti_sample_webview_profile=shared')
```

```sh
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --platform android --app Testbed --device YOUR_ADB_SERIAL --mode CoreClrJit
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --platform android --app Testbed --extra doroti_sample=input --dry-run
```

SampleApp2의 기존 `samples/DorotiSampleApp2/android/deploy-android.ps1`도 공용 진입점으로 전달합니다.
기존 `-Mode`·`-DotnetVersion`·`-Serial` 호출을 사용할 수 있으며 산출물은 위의 새 경로로 생성됩니다.
ADB 명령은 [Android 공식 문서](https://developer.android.com/tools/adb),
런타임과 패키지 옵션은 [.NET Android 빌드 속성](https://learn.microsoft.com/dotnet/android/building-apps/build-properties)을 참고하세요.

## iOS

[deploy-ios.ps1](deploy-ios.ps1)은 `DorotiSampleApp2`와 `DorotiTestbedApp`의 iOS 빌드,
기기 설치와 실행을 처리합니다. [Doroti.DeployHelper](Doroti.DeployHelper/Doroti.DeployHelper.csproj)가
실제 작업을 수행하는 .NET 10 콘솔 CLI이며 외부 NuGet 패키지를 사용하지 않습니다.

### 준비

- macOS, 루트 `global.json`이 선택하는 .NET 10 SDK, Xcode와 앱 빌드에 선택한 SDK의 iOS/MAUI workload가 필요합니다.
- PowerShell 진입점을 사용하려면 PowerShell 7도 설치합니다. `dotnet run`으로 CLI를 직접 실행할 수도 있습니다.
- `dotnet`, `xcrun`, `security`, `plutil`을 PATH에서 찾을 수 있어야 합니다.
- 실기기는 Mac에 연결하고 잠금을 해제한 뒤 Mac 신뢰와 **설정 → 개인정보 보호 및 보안 → 개발자 모드**를 허용합니다.
- Xcode **Settings → Accounts**에서 개발 인증서와 provisioning profile을 준비합니다.
  프로필에 앱 ID, 선택한 인증서, 해당 기기 UDID가 포함되어야 합니다.
- 시뮬레이터는 Xcode에서 iOS runtime을 설치합니다. 종료된 시뮬레이터도 선택할 수 있으며 설치 단계에서 부팅합니다.

아래 명령은 저장소 루트 `DorotiLab` 기준입니다. 스크립트는 파일 위치를 기준으로
저장소를 찾으므로 호출한 작업 폴더와 관계없이 같은 프로젝트를 빌드합니다.

```powershell
# 앱 → iPhone/iPad 또는 시뮬레이터 → 실기기 개발 인증서 → 호환 프로필 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1

# 특정 앱의 실기기: 기본 .NET 10 / Release / NativeAot
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -Target device

# 시뮬레이터: 기본 .NET 10 / Debug / Mono, 인증서 선택 생략
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target simulator

# 현재 선택된 Xcode가 27인 .NET 11 RC1 NativeAOT 예제
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device -DotnetVersion 11 -Mode NativeAot -SkipXcodeValidation

# 목록만 출력 / 선택과 빌드 명령만 확인
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -List
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -DryRun
```

위 한 줄 `pwsh` 명령은 PowerShell과 zsh/bash 모두에서 실행할 수 있습니다.

선택지가 하나면 해당 항목을 사용하며, 여러 항목이면 번호로 선택합니다. `q` 또는 Ctrl+C로 종료합니다.
기기 목록은 `devicectl`이 알고 있는 실기기의 연결 상태와 `simctl`의 사용 가능한 iOS 시뮬레이터를 표시합니다.
기억된 오프라인 실기기도 표시되지만 실제 빌드는 연결·개발자 모드 확인을 통과해야 시작합니다.
인증서는 개인 키를 가진 유효한 `Apple Development` / `iPhone Developer` 항목만 표시합니다.
프로필은 다음 두 폴더에서 읽고, 만료 여부·iOS 개발용 여부·앱 ID·기기 UDID·인증서 SHA-1을 확인합니다.

- `~/Library/MobileDevice/Provisioning Profiles`
- `~/Library/Developer/Xcode/UserData/Provisioning Profiles`

호환 프로필이 없으면 빌드 전에 중단합니다. Xcode에서 해당 bundle ID의 앱을 선택한 기기로
개발 서명하거나 프로필을 다운로드한 뒤 다시 실행하세요. 프로필을 자동 생성하거나 갱신하지는 않습니다.

### 옵션

| PowerShell 옵션 | 동작 |
| --- | --- |
| `-App Sample2\|Testbed` | 생략 시 앱 선택 |
| `-Target auto\|device\|simulator` | 기본 `auto`: 실기기와 시뮬레이터 통합 목록 |
| `-Device <ID>` | `-List`에 표시된 기기 UDID, CoreDevice ID 또는 시뮬레이터 UDID |
| `-DotnetVersion 10\|11` | 기본 `10`; SDK 선택 위치를 자동 처리 |
| `-Mode Mono\|NativeAot` | 실기기 기본 `NativeAot`, 시뮬레이터 기본 `Mono` |
| `-Configuration Debug\|Release` | 실기기 기본 `Release`, 시뮬레이터 기본 `Debug` |
| `-CodesignKey '<인증서 이름 또는 SHA-1>'` | 목록에서 정확한 이름 또는 SHA-1을 지정; 생략 시 선택 |
| `-CodesignProvision '<UUID 또는 이름>'` | 호환 프로필의 UUID 또는 정확한 이름; 생략 시 선택 |
| `-SkipXcodeValidation` | 이번 빌드에만 `ValidateXcodeVersion=false` 적용 |
| `-Environment 'NAME=VALUE'` | 실행할 앱의 환경변수 전달 |
| `-NoLaunch` | 빌드·설치 후 실행 생략 |
| `-DryRun` | 선택·서명 조합·SDK와 명령 확인; 빌드·부팅·설치·실행 생략 |
| `-List` / `-Help` | 목록 / .NET CLI 도움말 출력 |
| `-DotnetPath <경로>` | 별도 설치한 `dotnet` 실행 파일 지정 |

비대화형 실행에서는 선택할 항목이 여러 개인 경우 ID를 지정해야 합니다.
아래 placeholder는 `-List` 출력의 실제 값으로 바꿉니다.

```powershell
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device `
    -Device YOUR_DEVICE_UDID -CodesignKey YOUR_CERTIFICATE_SHA1 `
    -CodesignProvision YOUR_PROFILE_UUID -Environment 'DOROTI_IOS_GRAPHITE=1'
```

PowerShell 없이 zsh/bash에서 .NET CLI를 직접 실행할 수도 있습니다.
`--` 뒤의 옵션은 앱에 전달합니다. 다음 명령은 루트 `global.json`의 SDK를 선택하도록 저장소 루트에서 실행합니다.

```sh
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --platform ios --app Sample2 --target simulator
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --platform ios --app Testbed --target device --dotnet-version 11 --skip-xcode-validation
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --help
```

여러 환경변수는 PowerShell 세션에서 스크립트를 직접 호출하거나 .NET CLI의 `--env`를 반복합니다.
`pwsh -File`로 배열을 넘기는 대신 다음 형태를 사용하세요.

```powershell
./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -Target device `
    -Environment @('DOROTI_SAMPLE=input', 'DOROTI_IOS_GRAPHITE=1')
```

```sh
dotnet run --project ./helpers/deploy-helper/Doroti.DeployHelper -c Release -- --platform ios --app Testbed --target device --env DOROTI_SAMPLE=input --env DOROTI_IOS_GRAPHITE=1
```

### 빌드 구성과 산출물

PowerShell 진입점은 저장소 루트로 이동해 `dotnet run`으로 CLI를 빌드·실행합니다.
CLI 자체는 루트 .NET 10 SDK를 사용하고, 선택한 iOS 앱의 SDK는 별도로 결정합니다.
첫 실행에는 CLI 복원·컴파일이 포함되며 이후에는 증분 빌드를 사용합니다.
`-List`와 `-DryRun`에서도 CLI 자체의 빌드는 수행하지만 iOS 앱은 빌드하거나 배포하지 않습니다.

.NET 10은 루트 `global.json`에서 SDK를 선택하고 `net10.0-ios27.0` / MAUI `10.0.110`을 사용합니다.
.NET 11은 `samples/DorotiTestbedApp/ios/global.json`에서 SDK를 선택하고
`net11.0-ios` / MAUI `11.0.0-rc.1.26451.6`을 사용합니다. 저장소의 `global.json`은 수정하지 않습니다.
.NET 11 RC1 / Xcode 27 조합은 `-SkipXcodeValidation`을 명시해야 하며,
버전 검사 우회가 공식 지원 조합으로 바꾸지는 않습니다.

두 SDK의 NativeAOT 빌드 모두 `Registrar=managed-static`, `_UseDynamicDependenciesForMarkNSObjects=false`,
`MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true`를 사용합니다.
.NET 11 RC1에서도 SDK가 생성한 NSObject 보존 정보의 인터페이스 멤버 참조로 `IL2037`이 재현되어
.NET 10의 호환 설정을 함께 적용합니다. 두 샘플의 `ios/NativeAotRoots.xml`은 UIKit 진입점·뷰·델리게이트를
명시적으로 보존합니다. SDK 업데이트 시 이 옵션과 보존 목록을 함께 재검증해야 합니다.
.NET 11 RC1은 추가로 `PrepareAssemblies=false`, `PostProcessAssemblies=false`를 지정해
assembly-preparer의 `MarkNSObjects` 오류(`MT2080`)를 피하고 ILLink의 managed registrar 경로를 사용합니다.

NativeAOT는 실기기 Release만 지원하며 `dotnet publish`로 생성합니다.
Mono는 `dotnet build`를 사용하고 `PublishAot=false`를 명시합니다.
실기기는 `ios-arm64`, 시뮬레이터는 Mac CPU에 따라 `iossimulator-arm64` 또는 `iossimulator-x64`를 선택합니다.
Apple Silicon에서는 Rosetta로 호출해도 ARM64를 선택합니다.

산출물은 `Doroti/artifacts/<sample2|testbed>-ios-<nativeaot|mono>-net<10|11>-<debug|release>-<RID>`에
앱·SDK·모드·구성·RID별로 분리합니다. 예: `sample2-ios-nativeaot-net10-release-ios-arm64`.
빌드 시간 제한은 20분이며, 실패하거나 취소하면 설치로 넘어가지 않습니다.
성공한 산출물의 bundle ID를 확인하고, 실기기는 `codesign --verify --deep --strict`를 통과한 뒤 설치합니다.
설치·실행 단계의 실패도 종료 코드로 전달하며 성공 시 `.app` 경로를 출력합니다.

설치는 기존 bundle ID의 앱을 업데이트합니다. 기본 실행은 기존 앱 프로세스를 종료하고
다시 시작하므로 메모리상의 화면 상태는 초기화됩니다. 앱 삭제나 시뮬레이터 초기화는 수행하지 않습니다.
이 CLI는 일반 빌드·설치용이며 Hot Reload는 각 샘플 README의 개발 명령을 사용합니다.

## 검증

저장소의 콘솔 검증 방식에 맞춘 .NET 테스트 프로젝트를 사용합니다. 외부 테스트 패키지도 필요하지 않습니다.
기기 JSON 형식, plist·프로필 호환성, SDK 선택, Android 런타임·ABI·APK·Intent 인자,
실패 시 배포 중단, 실제 .NET 프로세스의 인자 전달·취소·시간 제한을 검사합니다.
기기 도구는 테스트 대역을 사용하므로 macOS나 연결된 Android 기기 없이 실행할 수 있습니다.
실기기 설치·화면·입력 검증은 별도입니다.

```sh
dotnet build ./helpers/deploy-helper/Doroti.DeployHelper -c Release
python ./Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project ./helpers/deploy-helper/tests/Doroti.DeployHelper.Tests -c Release
```

서명 항목의 의미는 [Microsoft iOS 게시 문서](https://learn.microsoft.com/en-us/dotnet/maui/ios/deployment/publish-cli?view=net-maui-10.0),
프로필 구조는 [Apple TN3125](https://developer.apple.com/documentation/technotes/tn3125-inside-code-signing-provisioning-profiles)를 참고하세요.

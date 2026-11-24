# 모바일 배포·Web 미리보기·Windows Native AOT CLI

`DorotiSampleApp2`·`DorotiTestbedApp`·`DorotiCarouselApp`·`DorotiDemoApp`을 선택해
iOS·Android에 설치하고 실행하거나, Web 임시 서버 또는 Windows Native AOT 실행 파일로 확인합니다.
`-App` 값은 각각 `Sample2`·`Testbed`·`Carousel`·`Demo`이며, 생략하면 메뉴에서 선택합니다.
[Doroti.DeployHelper](Doroti.DeployHelper/Doroti.DeployHelper.csproj)는 외부 NuGet 패키지가 없는
.NET 11 콘솔 CLI입니다. Web 임시 서버는 .NET SDK에 포함된 ASP.NET Core 공유 런타임을 사용하며,
Python·Node·별도 서버 설치가 필요하지 않습니다. CLI의 SDK는 [global.json](global.json)이 선택하며,
앱 빌드 SDK는 플랫폼과 `-DotnetVersion`으로 결정합니다.

```text
helpers/deploy-helper/
  deploy.ps1                         공통 진입점: 플랫폼·앱 선택
  deploy-ios.ps1                     iOS 진입점
  deploy-android.ps1                 Android 진입점
  deploy-web.ps1                     Web publish → 임시 서버 → 브라우저 열기
  deploy-windows.ps1                 Windows Native AOT publish → EXE 실행
  Doroti.DeployHelper/               공통 CLI와 플랫폼별 구현
```

아래 명령은 저장소 루트 `DorotiLab` 기준입니다. PowerShell 스크립트는 파일 위치로 저장소를 찾으므로
다른 작업 폴더에서도 실행할 수 있습니다. 공통 진입점에서 `-Platform`을 생략하면 플랫폼을 번호로 선택합니다.
비대화형 실행에서는 플랫폼·앱과 모바일 기기를 명시하세요. 선택지가 하나면 해당 항목을 자동 사용합니다.

```powershell
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform android -App Testbed -List
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform ios -App Sample2 -Target simulator
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform web -App Demo
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform windows -App Demo
```

## Windows Native AOT

네 샘플의 `windowsappsdk` 프로젝트를 **.NET 10 / Release / Native AOT / `win-x64`**로 게시하고
출력된 EXE를 실행합니다. Windows App SDK와 네이티브 라이브러리가 포함된 게시 폴더를 함께 사용하세요.
Windows, 앱 빌드용 .NET 10 SDK, PowerShell 7, Visual Studio의 **Desktop development with C++** 워크로드,
Windows SDK `10.0.26100.0`이 필요합니다. 기본 Graphite 렌더러는 Vulkan 1.2를 사용합니다. [Microsoft Native AOT 빌드 요건](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows)

```powershell
# 앱 선택 메뉴
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1

# Demo를 게시하고 실행
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Demo

# Testbed의 입력 샘플을 실행
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Testbed -Environment 'DOROTI_SAMPLE=input'

# 게시만 수행 / 명령만 출력 / 앱 목록
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Carousel -NoLaunch
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Sample2 -DryRun
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -List
```

게시 경로는 `Doroti/artifacts/windows-aot-net10-release-x64/publish/<sample>/<id>/`입니다.
게시 제한은 20분입니다. 게시가 성공하고 x64 네이티브 실행 파일과 Windows 호스트를 확인하면 앱을 엽니다.
창을 닫으면 helper가 종료되며, `Ctrl+C`로 빌드 또는 현재 실행을 취소할 수 있습니다.
`-Environment`는 실행할 앱에 전달되며, 여러 값은 PowerShell 세션에서 배열로 지정합니다.
이 로컬 확인용 프로필에서는 트리밍과 AOT 경고를 게시 출력에 남깁니다. CLR COM에 의존하는 Windows 파일 선택기와
OS 드래그 앤 드롭, 동적 로드 방식의 D3D12 진단 프리젠터는 CoreCLR 실행에서 사용할 수 있습니다.

## Web

`dotnet publish`가 성공한 게시본의 `wwwroot`를 helper 내부 임시 서버에서 제공하고
기본 브라우저를 엽니다. `run`·`watch`·Hot Reload는 사용하지 않습니다.
Web 실행 확인의 기본·권장 프로필은 **Release / Mono WASM AOT**입니다.
첫 AOT 게시에는 시간이 더 걸리고 WASM 파일이 커질 수 있으며, 빠른 non-AOT 비교는 `-Mode Mono`로 명시합니다.
터미널에서는 **앱 → .NET 버전 → 게시 구성 → AOT 여부 → 실행 모드**를 번호로 선택합니다.
명령 옵션으로 지정한 항목은 다시 묻지 않으며, SDK는 설치된 .NET 10·11 중에서 선택합니다.
Debug 게시에는 non-AOT만, AOT를 미리 지정한 경우에는 Release만 표시합니다.
Web 빌드 메뉴에는 `(default)`를 표시하며 **Enter로 .NET 10 / Release / AOT / threaded 기본값을 선택**합니다.
선택지가 하나면 자동 사용하며, `q`·입력 종료 또는 `Ctrl+C`로 취소합니다.
입력을 리디렉션한 비대화형 실행에서는 생략한 옵션에
**.NET 10 / Release / Mono WASM AOT / threaded** 기본값을 사용합니다.
선택한 SDK의 `wasm-tools` workload가 필요합니다.

2026-11-16 사용자가 Release AOT와 non-AOT(Mono)를 직접 비교해 AOT가 훨씬 부드럽고
빌드는 더 오래 걸린다고 확인했습니다. 이 기본값의 근거로 정성적인 사용자 확인을 기록합니다.

```powershell
# 앱을 선택하고 빌드 메뉴에서 Enter로 기본 Release/AOT 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1

# 앱·.NET 11을 고정하고 나머지 빌드 옵션은 메뉴에서 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -DotnetVersion 11

# 선택 메뉴 없이 .NET 11 / Release / AOT / threaded로 바로 게시
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -DotnetVersion 11 -Configuration Release -Mode MonoAot -WebExecutionMode threaded

# non-AOT 또는 compatible을 고정하고 나머지는 메뉴에서 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -Mode Mono
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -WebExecutionMode compatible

# 고정 포트 / 브라우저 자동 열기 생략 / 게시 명령만 확인
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -Port 5192 -NoLaunch
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -DotnetVersion 11 -DryRun
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -List
```

서버는 `127.0.0.1`에만 바인딩하며 기본 포트는 사용 가능한 포트를 자동 배정합니다.
출력된 `Preview URL`을 열고, **해당 터미널에서 `Ctrl+C`를 누르면 서버와 deploy-helper가 함께 종료**됩니다.
서버를 별도 프로세스로 분리하지 않습니다. `-NoLaunch`는 브라우저 자동 열기만 생략하며 서버는 유지합니다.
`threaded`·`auto`에는 COOP/COEP 격리 헤더를 제공하고, `compatible`에는 제공하지 않습니다.
모든 응답에 `Cache-Control: no-store`를 적용하며 없는 runtime 자산은 404로 응답합니다.

| Web 옵션 | 동작 |
| --- | --- |
| `-App Sample2\|Testbed\|Carousel\|Demo` | 생략하면 앱 선택 |
| `-DotnetVersion 10\|11` | 설치된 SDK에서 메뉴 선택; Enter 기본값은 `10`, 해당 major의 최신 stable SDK 사용 |
| `-Mode MonoAot\|Mono` | Release 메뉴의 Enter 기본값은 `MonoAot`; `Mono`는 AOT를 끈 비교용 게시본 |
| `-Configuration Release\|Debug` | 메뉴의 Enter 기본값은 `Release`; Debug는 `Mono` 게시만 허용 |
| `-WebExecutionMode threaded\|compatible\|auto` | 메뉴의 Enter 기본값은 `threaded`; auto는 두 variant를 모두 게시 |
| `-Port 0..65535` | 기본 `0`: 자동 배정 |
| `-NoLaunch` | 서버는 실행하고 브라우저 자동 열기만 생략 |
| `-List` / `-DryRun` / `-Help` | 앱·SDK 목록 / 게시·서버 계획 확인 / 도움말 |
| `-DotnetPath <경로>` | 사용할 dotnet 실행 파일 |

이 helper는 `DorotiWebInterpretWidgetTree=false`를 지정해 MonoAot에서 위젯·렌더링·런타임
어셈블리를 AOT에 포함합니다. 샘플의 Safari용 부분 해석 기본값과 구분해 데스크톱에서 비교합니다.
.NET 11 선택은 로컬 확인용으로 `DorotiWebQualificationProbe=true`를 전달하며,
기존 제품 빌드의 net11 지원 차단은 유지합니다.

게시 명령은 20분 제한이며 실패하면 서버나 브라우저를 열지 않습니다.
각 게시에 하나의 배포 ID를 전달해 내부 빌드와 검증이 같은 generation을 사용합니다.
게시 후 runtime profile의 배포 ID·SDK·TFM·AOT·실행 모드가 요청과 일치하는지도 확인합니다.
산출물은 `Doroti/artifacts/<sample>-web-<aot|mono>-net<10|11>-<release|debug>-<threaded|compatible|auto>/`의
`build`와 `publish`로 분리합니다. 이 로컬 산출물은 기존 artifacts 정리 대상입니다.

공통 진입점의 `-Platform web`과 .NET CLI 직접 실행도 같은 동작입니다.

```sh
cd helpers/deploy-helper
dotnet run --project Doroti.DeployHelper -c Release -- --platform web --app Demo --dotnet-version 11
```

회귀 검사는 Web 게시·임시 서버와 iOS SDK·런타임 선택을 확인합니다.

```powershell
Push-Location ./helpers/deploy-helper
try {
    dotnet run --project tests/Doroti.DeployHelper.Tests -c Release
} finally { Pop-Location }
```

## Android

Windows·macOS·Linux에서 사용할 수 있습니다. 앱 빌드에 선택한 .NET SDK와 Android/MAUI 워크로드,
Android SDK와 JDK 17이 필요합니다. 앱 빌드 기본값은 `Doroti/global.json`이 선택하는 .NET 10입니다.
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
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Carousel -Target device

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
| `-App Sample2\|Testbed\|Carousel\|Demo` | 생략 시 앱 선택 |
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
`11.0.100` 정식 SDK와 MAUI `11.0.10`을 네 앱에 공통으로 사용합니다.
해당 SDK의 Android/MAUI workload를 미리 설치하세요. Android NativeAOT는 이 도구에서 제공하지 않습니다.

연결·디버깅 권한·부팅 완료를 확인하고 `arm64-v8a`는 `android-arm64`, `x86_64`는 `android-x64`로 빌드합니다.
다른 ABI는 빌드 전에 거부합니다. 빌드는 20분 제한이며, 빌드 성공 후 선택한 앱의 signed APK가 하나인지 확인합니다.
산출물은 `Doroti/artifacts/<sample2|testbed|carousel>-android-<mono|aot|jit|r2r>-net<10|11>-<debug|release>-<arm64|x64>`에 분리합니다.
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
cd helpers/deploy-helper
dotnet run --project Doroti.DeployHelper -c Release -- --platform android --app Testbed --device YOUR_ADB_SERIAL --mode CoreClrJit
dotnet run --project Doroti.DeployHelper -c Release -- --platform android --app Testbed --extra doroti_sample=input --dry-run
```

SampleApp2의 기존 `samples/DorotiSampleApp2/android/deploy-android.ps1`도 공용 진입점으로 전달합니다.
기존 `-Mode`·`-DotnetVersion`·`-Serial` 호출을 사용할 수 있으며 산출물은 위의 새 경로로 생성됩니다.
ADB 명령은 [Android 공식 문서](https://developer.android.com/tools/adb),
런타임과 패키지 옵션은 [.NET Android 빌드 속성](https://learn.microsoft.com/dotnet/android/building-apps/build-properties)을 참고하세요.

## iOS

[deploy-ios.ps1](deploy-ios.ps1)은 `DorotiSampleApp2`·`DorotiTestbedApp`·`DorotiCarouselApp`·`DorotiDemoApp`의 iOS 빌드,
기기 설치와 실행을 처리합니다. 기본 빌드는 .NET 11 SDK, `net11.0-ios`, MAUI `11.0.10`을 사용합니다.

### 준비

- macOS, Xcode 27.2, .NET SDK 11.0.100 계열의 정식 버전과 `ios`·`maui-ios` 워크로드가 필요합니다.
- PowerShell 진입점을 사용하려면 PowerShell 7도 설치합니다. `dotnet run`으로 CLI를 직접 실행할 수도 있습니다.
- `dotnet`, `xcrun`, `security`, `plutil`을 PATH에서 찾을 수 있어야 합니다.
- 실기기는 Mac에 연결하고 잠금을 해제한 뒤 Mac 신뢰와 **설정 → 개인정보 보호 및 보안 → 개발자 모드**를 허용합니다.
- Xcode **Settings → Accounts**에서 개발 인증서와 provisioning profile을 준비합니다.
  프로필에 앱 ID, 선택한 인증서, 해당 기기 UDID가 포함되어야 합니다.
- 시뮬레이터는 Xcode에서 iOS runtime을 설치합니다. 종료된 시뮬레이터도 선택할 수 있으며 설치 단계에서 부팅합니다.

설치 프로그램으로 설치한 시스템 SDK에는 아래 명령으로 워크로드를 추가합니다. 첫 번째 출력에서 .NET 11.0.100 계열이
선택됐는지 확인하세요. 워크로드 집합 `11.0.100.1`은 iOS `27.2.11039`를 포함합니다. [공식 릴리스 안내](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-xcode27.2.11039)

```sh
(
  set -e
  cd helpers/deploy-helper
  dotnet_host="$(command -v dotnet)"
  "$dotnet_host" --version
  sudo -H "$dotnet_host" workload update --version 11.0.100.1 --source https://api.nuget.org/v3/index.json
  sudo -H "$dotnet_host" workload install ios maui-ios --version 11.0.100.1 --source https://api.nuget.org/v3/index.json
  "$dotnet_host" workload --info
)
```

아래 명령은 저장소 루트 `DorotiLab` 기준입니다. 스크립트는 파일 위치를 기준으로
저장소를 찾으므로 호출한 작업 폴더와 관계없이 같은 프로젝트를 빌드합니다.

```powershell
# 앱 → iPhone/iPad 또는 시뮬레이터 → 실기기 개발 인증서 → 호환 프로필 선택
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1

# 실기기 기본 프로필 (.NET 11 / Release / NativeAot)
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -Target device
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Carousel -Target device

# 시뮬레이터 기본 프로필 (.NET 11 / Debug / CoreClr)
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target simulator
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Carousel -Target simulator

# 실기기 Debug와 .NET 10 호환 프로필
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device -Configuration Debug
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target simulator -DotnetVersion 10

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
| `-App Sample2\|Testbed\|Carousel\|Demo` | 생략 시 앱 선택 |
| `-Target auto\|device\|simulator` | 기본 `auto`: 실기기와 시뮬레이터 통합 목록 |
| `-Device <ID>` | `-List`에 표시된 기기 UDID, CoreDevice ID 또는 시뮬레이터 UDID |
| `-DotnetVersion 10\|11` | 기본 `11` |
| `-Mode CoreClr\|NativeAot\|Mono` | 실기기 Release 기본 `NativeAot`. 그 외에는 .NET 11에서 `CoreClr`, .NET 10에서 `Mono` |
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
`--` 뒤의 옵션은 배포 도구에 전달합니다. CLI를 직접 실행할 때는 .NET 11을 선택하는 `helpers/deploy-helper`로 이동합니다.

```sh
cd helpers/deploy-helper
dotnet run --project Doroti.DeployHelper -c Release -- --platform ios --app Sample2 --target simulator
dotnet run --project Doroti.DeployHelper -c Release -- --platform ios --app Testbed --target device --configuration Debug
dotnet run --project Doroti.DeployHelper -c Release -- --help
```

여러 환경변수는 PowerShell 세션에서 스크립트를 직접 호출하거나 .NET CLI의 `--env`를 반복합니다.
`pwsh -File`로 배열을 넘기는 대신 다음 형태를 사용하세요.

```powershell
./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -Target device `
    -Environment @('DOROTI_SAMPLE=input', 'DOROTI_IOS_GRAPHITE=1')
```

```sh
dotnet run --project Doroti.DeployHelper -c Release -- --platform ios --app Testbed --target device --env DOROTI_SAMPLE=input --env DOROTI_IOS_GRAPHITE=1
```

### 빌드 구성과 산출물

PowerShell 진입점은 helper 폴더의 .NET 11 SDK로 CLI를 빌드·실행합니다. `-DotnetPath`는 앱 빌드에 사용할
별도 dotnet 실행 파일을 지정합니다. `-List`와 `-DryRun`은 CLI 실행과 기기 조회만 수행합니다.

iOS 앱 빌드는 각 샘플의 `ios/global.json`에서 .NET 11을 선택합니다. `-DotnetVersion 10`을 지정하면
`Doroti/global.json`의 SDK에서 `net10.0-ios`와 MAUI `10.0.110`을 사용합니다. iOS 플랫폼 버전은 선택된 워크로드의 기본값을 따릅니다.
Xcode 버전 검사는 기본적으로 활성화되어 있습니다. [Xcode 요구 사항](https://learn.microsoft.com/en-us/dotnet/ios/troubleshooting/xcode-requirement)

실기기 Release NativeAOT는 `dotnet publish`를 사용합니다. **Debug**와 **시뮬레이터**는 .NET 11 CoreCLR로
`dotnet build`하고, .NET 10을 선택한 경우 Mono로 빌드합니다.
실기기 RID는 `ios-arm64`이며 시뮬레이터는 Mac 아키텍처에 따라 `iossimulator-arm64` 또는 `iossimulator-x64`를 선택합니다.
Apple Silicon에서는 Rosetta로 호출해도 ARM64를 선택합니다.

산출물은 `Doroti/artifacts/<app>-ios-<nativeaot|coreclr|mono>-net<10|11>-<debug|release>-<RID>`에 생성합니다.
예를 들어 Sample2 실기기 기본 빌드는 `sample2-ios-nativeaot-net11-release-ios-arm64`에 있습니다.
빌드 시간 제한은 20분이며, 실패하거나 취소하면 설치로 넘어가지 않습니다.
성공한 산출물의 bundle ID를 확인하고, 실기기는 `codesign --verify --deep --strict`를 통과한 뒤 설치합니다.
설치·실행 단계의 실패도 종료 코드로 전달하며 성공 시 `.app` 경로를 출력합니다.

설치는 기존 bundle ID의 앱을 업데이트합니다. 기본 실행은 기존 앱 프로세스를 종료하고
다시 시작하므로 메모리상의 화면 상태는 초기화됩니다. 앱 삭제나 시뮬레이터 초기화는 수행하지 않습니다.
이 CLI는 일반 빌드·설치용이며 Hot Reload는 각 샘플 README의 개발 명령을 사용합니다.

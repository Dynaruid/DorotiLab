# DorotiSampleApp2

Cupertino 스타일의 독립 Doroti 샘플 앱입니다. 공통 C# UI를 Android, iOS, Linux, macOS, Mac Catalyst, Windows, Web 실행 프로젝트에서 사용합니다.

## 빠른 시작

모든 명령은 저장소 루트 `DorotiLab`에서 실행합니다.
[global.json](../../global.json)에 지정된 .NET SDK와 대상 플랫폼의 빌드 도구가 필요합니다.

```powershell
# Windows App SDK
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiSampleApp2 -Platform windows

# Web 개발 서버
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug
```

Web 개발 서버가 출력하는 주소를 브라우저에서 엽니다. WebAssembly 빌드에는 `wasm-tools` 워크로드가 필요합니다.

## 살펴볼 기능

- **Components**: 카운터 버튼, 스위치, 슬라이더, 활동 표시기, 다이얼로그
- **Profile**: 이름 입력과 인사말, 탭 전환 시 입력 상태 유지
- **Settings**: 시스템 / 라이트 / 다크 테마 선택
- **Variable Blur**: 60개 항목의 ListView 위에 상단 고정 VariableBlur 오버레이, 강도 조절과 켜기/끄기
- **Fonts**: 글꼴·굵기 비교와 여러 줄 입력, 자동 줄바꿈 및 키보드 커서 이동
- **Upload**: 이미지·텍스트 파일 다중 선택과 OS 드래그 앤 드롭, 이미지 미리보기·텍스트 내용·파일 이름/크기 표시, 개별 삭제·전체 지우기

설정과 입력값은 앱 실행 중에만 유지됩니다.

## 플랫폼 선택

[doroti-workspace.json](doroti-workspace.json)에 플랫폼 실행 프로젝트가 등록되어 있습니다.

| CLI 플랫폼 | 실행 프로젝트 | 지원 RID / 호스트 |
| --- | --- | --- |
| `android` | `android/DorotiSampleApp2.Android.csproj` | `android-arm64`, `android-x64` / MAUI |
| `ios` | `ios/DorotiSampleApp2.iOS.csproj` | `iossimulator-arm64`, `iossimulator-x64`, `ios-arm64` / UIKit |
| `linux` | `linux/DorotiSampleApp2.Linux.csproj` | `linux-x64` / Qt Quick |
| `macos` | `macos/DorotiSampleApp2.MacOS.csproj` | `osx-arm64` / AppKit |
| `maccatalyst` | `macos/DorotiSampleApp2.MacCatalyst.csproj` | `maccatalyst-arm64` / MAUI |
| `windows` | `windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj` | `win-x64` / Windows App SDK |
| `web` | `web/DorotiSampleApp2.Web.csproj` | `browser-wasm` / Blazor WebAssembly |

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 describe -App ./samples/DorotiSampleApp2
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./samples/DorotiSampleApp2 -Platform android -Configuration Debug
```

`-Platform`으로 위 표의 대상을 선택합니다. `build`는 빌드, `run`은 실행, `publish`는 배포 산출물 생성에 사용합니다.
공통 UI와 앱 ID `dev.doroti.sample2`를 공유하며 AppKit 앱 ID는 `dev.doroti.sample2.macos`입니다.
Windows MAUI 비교용 프로젝트는 `windows/DorotiSampleApp2.Windows.csproj`이며 CLI 별칭은 `windows-maui`입니다.

## Android

Android 워크로드, Android SDK, JDK 17과 Vulkan 1.2를 지원하는 기기 또는 에뮬레이터가 필요합니다.
네이티브 바인딩은 포함된 Gradle wrapper로 빌드합니다.

```powershell
dotnet build ./samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj -c Debug
dotnet build ./samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj -c Debug -t:Run -p:RuntimeIdentifier=android-arm64
```

x64 에뮬레이터에서는 `-p:RuntimeIdentifier=android-x64`를 사용합니다.

### 기기·에뮬레이터에 설치

[공용 Android 배포 도구](../../helpers/deploy-helper/deploy-android.ps1)가 기기를 선택하고 빌드·업데이트 설치·실행을 처리합니다.
.NET 10 CLI와 PowerShell 7이 필요합니다. 실기기는 USB 디버깅과 컴퓨터의 연결을 허용하고, 에뮬레이터는 먼저 시작하세요.

```powershell
# .NET 10 / Release / Mono Profiled AOT
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2

# x64 또는 ARM64 에뮬레이터
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -Target emulator -Device emulator-5554 -Configuration Debug

# 기기 목록 / 빌드 명령 확인
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -List
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -DryRun
```

여러 기기가 연결되어 있으면 `-Device <ADB serial>`을 지정합니다. 기존 앱 데이터는 유지하며 `-NoLaunch`는 설치만 합니다.
.NET 10의 기본 모드는 Release에서 `MonoAot`, Debug에서 `Mono`입니다.
.NET 10 CoreCLR은 실험적 기능이며 .NET 11 프로필은 `CoreClrJit`·`CoreClrR2R`을 지원합니다.
런타임·SDK 선택과 전체 옵션은 [배포 도구 사용법](../../helpers/deploy-helper/README.md)을 참고하세요.
기존 [android/deploy-android.ps1](android/deploy-android.ps1)은 공용 도구로 전달하는 호환 진입점입니다.

### Android Hot Reload

USB 기기 또는 에뮬레이터를 연결하고 Debug 개발 세션을 시작합니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 dev -App ./samples/DorotiSampleApp2 -Platform android
```

.NET SDK 10.0.400 이상, Android workload/platform-tools와 Python 3이 필요합니다.
지원되는 C# 메서드 본문 변경은 저장하면 앱 상태를 유지하면서 적용됩니다.
Kotlin/Java·Android 리소스·프로젝트 변경과 미지원 C# 변경은 Restart가 필요합니다.
VS Code Doroti 확장에서도 프로젝트와 `android`를 선택한 뒤 Run을 사용할 수 있습니다. 종료는 Stop 또는 CLI Ctrl+C입니다.
자세한 설정은 [Hot Reload 사용법](../../Doroti/docs/development-hot-reload.md)을 참고하세요.

## iOS

Mac, 대상 .NET iOS 워크로드와 호환되는 Xcode가 필요합니다. 실기기는 개발 서명 설정도 필요합니다.
아래 예제는 .NET 10 Mono와 ARM64 시뮬레이터를 사용합니다.

```powershell
dotnet build ./samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Debug -t:Run -p:RuntimeIdentifier=iossimulator-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./samples/DorotiSampleApp2 -Platform ios -Configuration Release -Rid ios-arm64 -CompilationMode Mono
```

Intel Mac 시뮬레이터는 `iossimulator-x64`를 선택합니다.
CLI의 iOS Release 기본값은 실험적인 NativeAot이므로 Mono 빌드에는 위 명령처럼 `-CompilationMode Mono`를 명시합니다.

### 기기·인증서 선택 후 설치

[공용 iOS 배포 도구](../../helpers/deploy-helper/deploy-ios.ps1)가 기기·인증서·프로필을 선택하고 빌드·설치·실행합니다.
PowerShell 7과 .NET 10 CLI가 필요합니다. 실기기는 Mac 신뢰와 개발자 모드를 허용하고, Xcode에서 개발 인증서와 프로필을 준비하세요.

```powershell
# 실기기: .NET 10 / Release / NativeAot
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target device

# 시뮬레이터: .NET 10 / Debug / Mono
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -Target simulator
```

`-Device <UDID>`, `-CodesignKey '<인증서 이름 또는 SHA-1>'`, `-CodesignProvision <UUID>`로 선택을 고정할 수 있습니다.
Mono 실기기 Debug는 `-Mode Mono -Configuration Debug`를 사용합니다.
SDK·런타임 선택과 전체 옵션은 [배포 도구 사용법](../../helpers/deploy-helper/README.md)을 참고하세요.

### iOS Hot Reload

.NET 10 시뮬레이터를 부팅한 뒤 Debug 개발 세션을 시작합니다.
여러 시뮬레이터가 켜져 있으면 `-Device <UDID>`를 지정합니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 dev -App ./samples/DorotiSampleApp2 -Platform ios -IosTargetFramework net10.0-ios27.0
```

지원되는 C# 변경은 저장하면 적용됩니다. VS Code 확장에서도 `ios`를 선택한 뒤 Run과 Hot Reload를 사용할 수 있습니다.
지원하지 않는 편집은 Restart가 필요하며 상태가 초기화됩니다. Release/NativeAOT에는 이 핫리로드 경로를 사용하지 않습니다.
.NET 10 실기기의 네트워크 연결과 .NET 11 CoreCLR의 USB 개발 설정은 [iOS Hot Reload 사용법](../../Doroti/docs/development-hot-reload.md)을 참고하세요.

### .NET 10 + Xcode 27 NativeAOT 실기기 설치

직접 게시·서명·설치할 때 필요한 옵션은 [NativeAOT 빌드 안내](docs/ios-nativeaot.md#net-10--xcode-27)에 있습니다.

### .NET 11 RC1 + Xcode 27 Native AOT

SDK 선택과 Xcode 버전 검사 우회, registrar 설정은 [NativeAOT 빌드 안내](docs/ios-nativeaot.md#net-11-rc1--xcode-27)를 참고하세요.

## Linux

Linux x64에서 .NET SDK, CMake 3.24 이상, C++20 컴파일러, Qt 6.8 이상(Quick/QuickControls2/WebEngineQuick/WebChannel),
Wayland 개발 도구와 Vulkan 1.2 지원 GPU가 필요합니다. 자세한 네이티브 의존성은 [Qt 호스트 안내](../../packages/platforms/qt/native/README.md)를 참고하세요.

```powershell
dotnet run --project ./samples/DorotiSampleApp2/linux/DorotiSampleApp2.Linux.csproj -c Release
```

Linux에서 빌드하면 `linux/native`의 Qt 호스트도 CMake로 빌드하여 실행 파일 옆에 복사합니다.

## macOS / Mac Catalyst

Apple Silicon Mac, Xcode, 각각 `macos` / `maccatalyst` 워크로드가 필요합니다.
두 실행 프로젝트는 공통 Cupertino UI를 각각 AppKit과 Mac Catalyst 호스트로 실행합니다.

```powershell
dotnet build ./samples/DorotiSampleApp2/macos/DorotiSampleApp2.MacOS.csproj -c Release -t:Run
dotnet build ./samples/DorotiSampleApp2/macos/DorotiSampleApp2.MacCatalyst.csproj -c Release -t:Run
```

## Windows

```powershell
dotnet run --project ./samples/DorotiSampleApp2/windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj -c Release
```

Windows App SDK / Vulkan 호스트를 사용합니다.

## Web

빠른 개발 실행은 [빠른 시작](#빠른-시작)의 Debug 명령을 사용합니다.
Release 게시의 기본값은 Mono WASM AOT이며, 게시물을 제공하는 서버를 실행한 터미널을 열어 둡니다.
첫 AOT 게시에는 시간이 더 걸립니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --artifacts-path ./temp/testing/sample2-web-release
python Doroti/eng/serve-isolated-web.py ./temp/testing/sample2-web-release/publish/DorotiSampleApp2.Web/release/wwwroot --port 5089
```

브라우저에서 `http://127.0.0.1:5089`에 접속합니다. 서버는 WASM threads에 필요한 COOP/COEP 헤더를 제공합니다.
일반 `dotnet run -c Release`는 AOT 게시물을 사용하지 않습니다.

스플래시는 [web/src/doroti_bootstrap.ts](web/src/doroti_bootstrap.ts)의 `startDoroti({ splash: true, ... })`에서 설정합니다.
개발 중 API 프록시는 [web_dev_config.jsonc](web_dev_config.jsonc)의 `server.proxy`에 설정하고 개발 서버를 재시작합니다.
설정 예제는 [Web 개발 서버 안내](../../Doroti/docs/web-development-config.md)에 있습니다.

### WASM AOT 비교 실행

AOT·비-AOT 게시물과 WebGL2·WebGPU 렌더러를 비교하는 명령은 [Web 게시 안내](docs/web.md#wasm-aot-비교-실행)에 있습니다.

### Web 배포와 폰트

다른 기기에서 열 수 있는 HTTPS 터널은 다음 명령으로 시작합니다.

```powershell
pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 -App ./samples/DorotiSampleApp2 -OpenBrowser
```

종료는 `pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 stop`을 사용합니다.
필수 도구와 관리 명령은 [터널 안내](../../tools/doroti-cloudflared/README.md)를 참고하세요.

기본 웹 폰트는 CDN에서 로드합니다. 외부 폰트 요청 없이 Fonts 탭을 실행하려면 `Assets` 모드를 사용합니다.

```powershell
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug -p:DorotiSampleWebFontSource=Assets
```

로컬 폰트 구성과 지원 범위는 [Web 폰트 설정](docs/web.md#web-배포와-폰트)을 참고하세요.

## 파일 업로드 예제

**Upload** 탭에서 **파일 선택**을 누르거나 페이지 안에 파일을 드래그해서 놓습니다.
PNG/JPG/JPEG/GIF/WEBP/BMP 이미지는 최대 10 MiB, TXT/MD/CSV/JSON/LOG 텍스트는
최대 1 MiB이며 목록은 8개까지 유지합니다. UTF-8 또는 BOM이 있는 UTF-16 텍스트를
읽고, 긴 내용은 처음 4,000자까지만 표시합니다. 이미지 디코딩에 실패하면 해당 카드에 오류를 표시합니다.

이 예제는 파일을 앱 메모리로 읽어 미리 보는 동작이며 서버 업로드나 디스크 저장은 포함하지 않습니다.
파일 선택·드롭의 지원 여부는 실행 호스트에서 확인하며, 파일 드롭이 없는 환경에서는 선택 버튼을 사용합니다.
목록은 탭을 전환해도 유지되며 앱을 닫으면 초기화됩니다.
구현은 [src/FileUploadPage.cs](src/FileUploadPage.cs)에 있습니다.

## Variable Blur 예제

**Variable Blur** 탭은 60개 항목의 리스트 위에 상단 180 논리 단위의 블러를 적용합니다.
강도 슬라이더와 켜기/끄기 스위치, 다음 다섯 가지 모드를 비교할 수 있습니다.
블러는 위쪽에서 강하고 아래쪽에서 선명해지며, 블러 영역에서도 리스트를 스크롤할 수 있습니다.

| 모드 | 특징 |
| --- | --- |
| Full quality | 원본 해상도 Gaussian |
| Adaptive | 강도에 따라 원본·1/2·1/4 해상도를 혼합 |
| Fast adaptive (기본) | 적응형 해상도와 빠른 Gaussian 커널 |
| Fixed 1/4 | 전체 구간을 1/4 해상도로 처리하며 작은 글자와 가는 선이 부드러워질 수 있음 |
| Dual Kawase | 해상도 피라미드를 사용하는 블러 근사 |

Variable Blur에는 GPU 렌더러가 필요합니다.
구현은 [src/VariableBlurPage.cs](src/VariableBlurPage.cs)에 있습니다.

## 웹폰트 비교

**Fonts** 탭에서 Roboto/Galmuri11/SUITE Variable, 300/400/500/700/900 굵기와 연속 `wght` 슬라이더를 비교합니다.
한글·영문·숫자와 편집 가능한 여러 줄 입력을 제공합니다.

폰트 원본은 `assets/fonts/`에 둡니다. 웹 프로젝트의 `Content` + `Link` 매핑이 같은 웹 경로로 게시합니다.
폰트 추가와 CSS·WOFF2 등록 예제는 [공통 assets 폴더의 웹폰트](docs/web.md#공통-assets-폴더의-웹폰트)를 참고하세요.

## 소스와 라이선스

화면 구성은 [src/App.cs](src/App.cs), 공통 진입점은 [Program.cs](Program.cs)에 있습니다.
Cupertino Icons 1.0.9 폰트와 해당 라이선스는 `assets/fonts`에 포함되어 있습니다.
회귀 검사 명령과 블러 성능 측정 옵션은 [검사·프로파일링 안내](docs/validation.md)에 있습니다.

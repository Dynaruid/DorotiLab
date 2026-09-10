# DorotiTestbedApp

[English](README.md) | **한국어**

Doroti의 Material 위젯과 플랫폼별 호스트를 확인하는 샘플·진단 앱입니다.
하나의 공용 C# 앱을 Windows, macOS AppKit, Mac Catalyst, Linux, Android, iOS, Web runner로 실행합니다.

Material 샘플은 Components, Color, Typography, Elevation, 9개 seed 색상과 6개 이미지 테마,
로컬·URL 이미지 데모를 제공합니다. **기본 화면은 진단 갤러리**이며, 아래 명령은 샘플 모드를 명시적으로 엽니다.
진단 갤러리 맨 위의 **Open Material sample** 버튼으로도 샘플을 열 수 있습니다.
아이폰에서 앱을 종료한 뒤 아이콘으로 다시 열었을 때도 이 버튼을 사용하면 됩니다.

- [실행 준비](#실행-준비)
- [플랫폼별 샘플 실행](#material-샘플-모드)
- [화면과 렌더러 설정](#화면과-렌더러-설정)
- [빌드와 개발](#빌드와-개발)
- [문제 해결](#문제-해결)

## 실행 준비

모든 명령은 **저장소 루트 `DorotiLab`**에서 PowerShell 7로 실행합니다.
macOS/Linux의 기본 셸이 zsh/bash라면 먼저 `pwsh -NoProfile`을 실행하세요.
.NET SDK는 [global.json](../Doroti/global.json)의 **10.0.400 계열**을 사용하고,
각 플랫폼에 필요한 workload와 도구는 아래 실행 항목에서 확인하세요.

첫 실행은 패키지 복원과 빌드, 필요한 경우 기기 설치를 포함합니다. 예제는 모두 Release 구성입니다.
Debug가 필요하면 `dotnet run`은 `-c Debug`, workspace CLI는 `-Configuration Debug`를 지정합니다.
이미 실행 중인 native 앱은 닫고 다시 실행해야 모드 변경이 적용됩니다.

## Material 샘플 모드

[Windows](#windows-샘플) · [macOS AppKit](#macos-appkit-샘플) · [Mac Catalyst](#mac-catalyst-샘플) · [Linux](#linux-샘플) · [Android](#android-샘플) · [iOS](#ios-샘플) · [Web](#web-샘플)

Native 앱은 `dotnet run -e`로 앱 프로세스에 `DOROTI_TESTBED_MODE=sample`을 전달합니다.
Apple/Android에서는 셸 환경변수만 설정하는 것으로 전달을 보장할 수 없습니다.
함께 지정하는 `DOROTI_RESIZE_FIXTURE=none`은 샘플보다 우선하는 F0/F1/F2 진단 fixture를 해제합니다.
Web은 URL로 모드를 선택합니다.

### Windows 샘플

Windows 호스트에서 실행합니다. 기본 Windows App SDK/Vulkan runner입니다.

```powershell
dotnet run --project ./DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

독립 MAUI backend를 사용하려면 위 명령의 project를
`./DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj`로 바꿉니다.

### macOS AppKit 샘플

Apple Silicon, macOS 14 이상과 호환되는 Xcode/macOS workload가 필요합니다.
`macos`는 native AppKit runner입니다.

Graphite/Metal 실험 후보는 아래 `dotnet run` 명령에 `-e DOROTI_MACOS_GRAPHITE=1`을 추가해 선택합니다. 기본값은 Ganesh/Metal이며, 후보의 실행 범위와 미검증 항목은 [Apple 전환 보고서](../Doroti/docs/validation/native-graphite-apple-2026-09-09.md)에 있습니다. 해당 옵션을 빼면 기존 경로로 복귀합니다.

```powershell
dotnet run --project ./DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Release -r osx-arm64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Mac Catalyst 샘플

Apple Silicon macOS에서 Xcode/Mac Catalyst workload로 실행하는 UIKit runner입니다.
Mac UI idiom을 사용해 iPad 호환 모드의 77% 축소 없이 표시합니다.
렌더링과 입력 좌표는 Metal 뷰의 배율을 기준으로 처리합니다.
트랙패드 스크롤은 손가락을 뗀 뒤 감속하며, 일반 마우스 휠에는 추가 관성을 적용하지 않습니다.
두 Mac runner 모두 텍스트 필드에서 우클릭 또는 Control+클릭으로 편집 메뉴를 엽니다.
메뉴는 Flutter Cupertino 기준의 연속 곡선 모서리와 흐린 그림자를 사용하며,
항목에 마우스를 올리면 테마 강조색과 대비되는 글자색을 표시합니다.
`dotnet run --project ./Doroti/validation/fcr7-material-widget -- --mac-text-menu`로
밝은/어두운 테마의 강조·눌림 상태, 그림자와 편집 동작을 검증할 수 있습니다.

```powershell
dotnet run --project ./DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Linux 샘플

Linux x64 호스트에서 실행합니다. Qt 6.5 이상 Core/Gui/Widgets/OpenGL, CMake,
C++ compiler, `pkg-config`, Wayland client 개발 파일, `wayland-scanner`,
`wayland` 또는 `xcb` QPA plugin이 필요하며 native shim도 함께 빌드합니다.

```powershell
dotnet run --project ./DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Android 샘플

Android workload, Android SDK와 OpenJDK 17–21을 준비하고 에뮬레이터를 먼저 시작하거나
USB 디버깅을 허용한 기기를 연결합니다. 연결 절차는 [Android 연결 안내](#android-실기기와-에뮬레이터-연결)를 참고하세요.

```powershell
# 실행 가능한 기기의 ID 확인
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj --list-devices

# x64 에뮬레이터: emulator-5554를 실제 ID로 변경
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64 `
  --device emulator-5554 -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

arm64 실기기 또는 arm64 에뮬레이터는 `-r android-arm64`로 바꾸고 해당 `--device` ID를 지정합니다.
빠른 개발 빌드는 `-c Debug`를 사용합니다. Android 환경변수는 빌드에도 반영되므로
모드를 바꿀 때 `--no-build`를 사용하지 마세요.

**갤럭시 USB 실기기 실행 예제** (`device-serial`을 `adb devices -l`의 첫 열 값으로 변경):

```powershell
adb devices -l
adb -s device-serial shell getprop ro.product.cpu.abi

# ABI가 arm64-v8a인 갤럭시폰
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 `
  --device device-serial -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

설치 후 앱 목록의 **Doroti Material Testbed**로 다시 열 수 있습니다.
2026-09-09 갤럭시 `SM-S931N`에서 Release AOT APK를 아래 ADB 방법으로 설치하고
`Doroti Material 3`의 Components 화면 표시까지 확인했습니다. 전체 위젯·입력·성능 검증은 포함하지 않습니다.
자동 배포가 실패하면 [Android 빌드·배포 오류](#android-빌드배포-오류)를 참고하세요.

### iOS 샘플

Apple Silicon macOS에서 Xcode/iOS workload를 준비하고 Simulator를 먼저 시작합니다.

```powershell
# 실행 가능한 시뮬레이터/기기의 ID 확인
dotnet run --project ./DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj --list-devices

# simulator-udid를 위 목록의 실제 ID로 변경
dotnet run --project ./DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Release -r iossimulator-arm64 `
  --device simulator-udid -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

Intel Mac의 시뮬레이터는 `-r iossimulator-x64`를 사용합니다. 실제 iPhone/iPad는
`-r ios-arm64`와 해당 기기의 ID를 지정하며 별도 코드 서명·프로비저닝 설정이 필요합니다.
여기서 `--device`는 `dotnet run` 옵션입니다. `doroti.ps1`의 `-Device`는 현재 Android 전용입니다.

### Web 샘플

서버를 실행하고 이 터미널을 열어 둡니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release
```

브라우저에서 [Material 샘플 열기](http://127.0.0.1:5088/?dorotiTestbedMode=sample)를 누릅니다.
Web은 환경변수 대신 **URL의 `dorotiTestbedMode=sample`**로 화면을 선택합니다.
renderer 옵션을 생략하면 기본 SkiaSharp direct Worker/WebGPU로 실행됩니다.
서버는 실행한 터미널에서 `Ctrl+C`로 종료합니다.

## 화면과 렌더러 설정

### 진단 화면으로 돌아가기

Native 앱을 닫고 위 명령의 `-e DOROTI_TESTBED_MODE=sample`을
`-e DOROTI_TESTBED_MODE=diagnostics`로 바꿔 다시 실행합니다.
`-e`는 현재 셸의 환경변수를 변경하지 않습니다. 이전에 직접 설정한 값은 다음과 같이 해제합니다.

```powershell
Remove-Item Env:DOROTI_TESTBED_MODE -ErrorAction SilentlyContinue
Remove-Item Env:DOROTI_RESIZE_FIXTURE -ErrorAction SilentlyContinue
```

Web은 서버 재시작 없이 아래 URL로 화면을 바꿉니다. 모드를 생략해도 진단 화면이 열립니다.

### Web 렌더러와 측정 옵션

| 화면 / 렌더러 | 열기 |
| --- | --- |
| Material 샘플 / 기본 WebGPU | [샘플](http://127.0.0.1:5088/?dorotiTestbedMode=sample) |
| Material 샘플 / WebGL2 | [WebGL2 샘플](http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiRenderer=worker-direct-webgl) |
| 진단 / 기본 WebGPU | [진단](http://127.0.0.1:5088/?dorotiTestbedMode=diagnostics) |
| 진단 / WebGL2 | [WebGL2 진단](http://127.0.0.1:5088/?dorotiTestbedMode=diagnostics&dorotiRenderer=worker-direct-webgl) |

`dorotiRenderer`는 `worker-direct-webgpu`(기본값 및 `auto`) 또는 `worker-direct-webgl`을 선택합니다.
현재 Web 호스트는 SkiaSharp WASM과 Microsoft.TypeScript.MSBuild를 사용하며 CanvasKit/npm 복원은 필요 없습니다.
렌더러를 다시 빌드했다면 runner를 재시작하고 페이지를 새로고침하세요.
Components → Communication → Progress indicators의 재생 버튼으로 애니메이션을 확인할 수 있습니다.

측정할 때만 샘플 URL에 다음 쿼리를 추가합니다.

| 쿼리 | 용도 |
| --- | --- |
| `dorotiProgressScope=broad` | 기본 `local` 갱신과 화면 전체 재빌드 비교 |
| `dorotiResizeDiagnostics=1` | framework 작업량·타입 계측 |
| `dorotiResizeDiagnostics=0&dorotiInputMarkers=1` | 최소 입력→새 scene commit 계측; 실제 화면 표시 지연과는 다름 |

### Windows GPU와 Acrylic

Windows App SDK의 기본 렌더러는 Vulkan이며 ANGLE을 명시적으로 선택할 수 있습니다.
**Material 샘플은 불투명 surface**를 사용합니다. Acrylic은 진단 화면에서 확인하며,
Windows 11 24H2 이상에서는 일반 `WindowBackdropMode.acrylic`에 별도 실험 플래그가 필요 없습니다.

| 환경변수 | 값 / 동작 |
| --- | --- |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | 정확하거나 유일한 GPU 이름 일부(예: `AMD`); Vulkan에서 GPU 선호도보다 우선 |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | `NoPreference`(기본), `LowPowerPreference`, `HighPerformancePreference`; Vulkan/ANGLE 공통 |
| `DOROTI_WINDOWS_PRESENTER` | `Vulkan`(기본) 또는 `AngleD3D11` |

예를 들어 위 Windows 명령에 `-e DOROTI_WINDOWS_GPU_PREFERENCE=HighPerformancePreference`를 추가합니다.
이전에 `$env:`로 지정했다면 `Remove-Item Env:변수이름`으로 해제한 뒤 앱을 다시 실행하세요.
`experimentalAcrylic`은 이전 동작을 재현하는 호환 옵션입니다.

### 시스템 다크 모드와 색 팔레트

`MaterialApp`의 light/dark `ThemeData`와 `ThemeMode.system`으로 시스템 테마를 따릅니다.
팔레트는 `ColorScheme.CreateFromSeed`로 만들며 위젯은 `Theme.of(context).colorScheme`을 사용합니다.
창의 `backgroundColor`와 `darkBackgroundColor`도 같은 전환을 따릅니다.

Linux의 진단 창은 Acrylic과 transparent fallback을 요청합니다. Wayland compositor가
`ext-background-effect-v1` 또는 구형 KDE blur protocol을 제공하면 native blur를 요청하고,
없으면 transparent fallback을 사용합니다.

## 빌드와 개발

### Workspace CLI

[doroti-workspace.json](doroti-workspace.json)이 플랫폼별 runner 경로를 정의합니다.
CLI의 `build`, `run`, `publish`는 기본 Release 구성입니다. 샘플 모드를 확실히 전달하려면
위 플랫폼별 `dotnet run -e` 명령을 사용하세요.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 doctor -App ./DorotiTestbedApp -Platform all
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform macos -Rid osx-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform macos -Rid osx-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 publish -App ./DorotiTestbedApp -Platform web
```

### 플랫폼별 실행

위 CLI 예제의 `-Platform`과 `-Rid`를 대상에 맞게 바꿉니다.

| 대상 | `-Platform` | `-Rid` / 추가 옵션 |
| --- | --- | --- |
| Windows App SDK | `windows` | 기본값 사용 |
| Windows MAUI | `windows` | `-WindowsBackend Maui` |
| macOS AppKit | `macos` | `osx-arm64` |
| Mac Catalyst | `maccatalyst` | `maccatalyst-arm64` |
| Linux | `linux` | `linux-x64` |
| Android | `android` | `android-x64` 또는 `android-arm64`; 기기는 `-Device`로 선택 |
| iOS | `ios` | `iossimulator-arm64`, `iossimulator-x64` 또는 `ios-arm64` |
| Web | `web` | 생략 |

Web의 `ASPNETCORE_ENVIRONMENT=Development`는 빌드 구성과 별개입니다.
기본 실행은 **Release 빌드 + Development 호스팅 환경**이며 CLI 출력의
`Doroti artifact: configuration=...`에서 빌드 구성을 확인할 수 있습니다.

루트 `DorotiTestbedApp.csproj`는 공용 앱 라이브러리입니다.
`dotnet run --project DorotiTestbedApp.csproj -p:DorotiTarget=...`는 `DOROTIAPP100`으로 실패하므로
플랫폼 runner를 지정하세요. 다른 OS의 도구까지 요구하는 전체 solution보다 대상별 빌드를 사용합니다.

### Native bridge

Android, iOS, AppKit, Mac Catalyst는 앱 소유 native library와 binding을 포함합니다.
기본 ABI는 `platformInfo`, `echo`, UI-thread callback을 제공하며 최종 앱 runner와 별개입니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiTestbedApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiTestbedApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiTestbedApp -Platform ios
```

`native open`은 Android Studio/Xcode 프로젝트 경로를 출력합니다. IDE도 열려면 `-Launch`를 추가합니다.

### 프로젝트 구조와 리소스

| 경로 | 역할 |
| --- | --- |
| `Program.cs`, `src/`, `assets/` | 공용 시작 코드, 위젯 트리, 앱 리소스 |
| `doroti-workspace.json` | 플랫폼 alias와 runner 경로 |
| `windowsappsdk/`, `windows/` | Windows App SDK와 독립 MAUI runner |
| `web/` | WebAssembly Worker, TypeScript, `wwwroot` |
| `android/`, `ios/` | 모바일 runner, native 프로젝트, binding |
| `macos/` | 별도 AppKit·Mac Catalyst runner, binding, manifest |
| `linux/` | managed runner와 CMake/Qt 6 C ABI shim |

생성 bootstrap·plugin registration은 각 runner의 `obj` 아래 `Doroti.Generated`에 위치합니다.
플랫폼별 icon, splash, entitlement, 출력과 lock 파일은 해당 플랫폼 폴더가 소유합니다.
앱 ID는 `dev.doroti.testbed`, AppKit은 `dev.doroti.testbed.macos`입니다.

로컬 WebP와 MaterialIcons·Roboto regular/medium/bold는 embedded resource입니다.
라이선스·출처는 [샘플 소스](src/MaterialSample)와 [폰트](assets/fonts)를 참고하세요.
이미지 테마는 `flutter.github.io`, URL 이미지 데모는 `plus.unsplash.com`에 접근합니다.
테마 로드 실패 시 마지막 성공 테마를 유지하며 Retry로 재시도합니다.
표시용 이미지는 종횡비를 유지해 최대 너비 `1024 × DPR`로 디코딩하고,
`Extract colors`를 누를 때 원본으로 색상을 추출합니다.
외부 링크 실행 결과는 호스트의 요청 수락을 뜻하며, Web 팝업 차단은 화면에 표시합니다.

## 문제 해결

### Android 실기기와 에뮬레이터 연결

Android SDK Platform Tools의 `adb`를 `PATH`에 추가합니다. Windows에서 Android Studio의 기본 설치를
사용한다면 `& "$env:LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe" devices -l`로도 확인할 수 있습니다.

1. 실기기는 개발자 옵션의 **USB 디버깅**을 켜고 USB로 연결한 뒤 RSA 허용 창을 승인합니다.
2. 에뮬레이터는 Android Studio의 **Device Manager**에서 시스템 이미지의 ABI를 확인하고 먼저 시작합니다.
3. `adb devices -l`의 상태가 `device`인지 확인합니다. 첫 열의 serial을 샘플 명령의 `--device`에 전달합니다.
4. `adb -s device-serial shell getprop ro.product.cpu.abi`로 ABI를 확인하고 `android-arm64` 또는 `android-x64`를 선택합니다.

아래 예제의 `device-serial`, `device-ip`, 포트는 실제 값으로 바꿉니다.
Android 11 이상의 무선 연결은 **무선 디버깅 → 페어링 코드로 기기 페어링**의 주소를 사용합니다.
페어링 포트와 연결 포트는 다를 수 있습니다.

```powershell
adb pair device-ip:pairing-port
adb connect device-ip:debug-port
adb devices -l
```

| 증상 | 확인 / 해결 |
| --- | --- |
| `unauthorized` | 기기 잠금 해제, RSA 승인 후 USB 재연결 |
| `offline` | `adb kill-server`, `adb start-server` 실행 후 재연결 |
| 목록에 없음 | USB 디버깅, 데이터 케이블, 연결 모드, Windows 제조사 드라이버 확인 |
| 대상이 여러 개 | `--device`에 정확한 serial 지정; workspace CLI에서는 `-Device` |
| 설치 서명 충돌 | 앱 데이터 삭제를 감수할 수 있을 때 `adb -s device-serial uninstall dev.doroti.testbed` 후 재설치 |

`android-x64` Release는 Doroti 프레임워크를 포함한 사전 컴파일 가능한 메서드에 Mono AOT를 사용합니다.
기존 AOT 비활성화 설정은 x64 에뮬레이터의 시작 시 입력 응답 지연(ANR)을 유발했습니다.
Debug는 SDK의 인터프리터 기본값을 유지하며, Release의 normal AOT 모드에서도 동적 코드는 JIT로 실행할 수 있습니다.
첫 Release 빌드는 더 오래 걸리고 APK 크기가 커집니다. 진단 비교 시에는 `-p:RunAOTCompilation=false`를 명시할 수 있습니다.
Android JNI marshal methods는 계속 비활성화되어 있습니다.
arm64 Release AOT는 `android-arm64` 실기기에서 별도로 확인합니다.

### Android 빌드·배포 오류

**SkiaSharp 버전 충돌 (`MSB3277`, `CS1705`)**: 현재 패키지 설정과 달리 Android 호스트가
이전 SkiaSharp를 참조한다면 다음 두 복원 캐시를 갱신한 뒤 샘플 실행 명령을 다시 실행합니다.
2026-09-09에는 이 방법으로 `4.152`와 `4.154` 충돌을 해결했습니다.

```powershell
dotnet restore ./Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj `
  -p:RuntimeIdentifier=android-arm64 -p:TargetFramework=net10.0-android --force-evaluate -v minimal
dotnet restore ./Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj `
  -p:TargetFramework=net10.0-android -p:DorotiHostTargetFrameworks=net10.0-android --force-evaluate -v minimal
```

**APK 생성 후 자동 배포 실패 (`DOTNET_HOST_PATH`, `MSB4221`, `MSB4027`)**:
2026-09-09에는 `dotnet run`의 `DeployToDevice` 단계가 이 오류로 실패했습니다.
샘플 모드로 방금 생성된 서명 APK가 있다면 ADB로 직접 설치·실행할 수 있습니다.
아래 경로는 `android-arm64` Release 출력이며, `publish/`의 이전 APK와 구별합니다.
수정 시각이 이번 빌드와 일치하는지 확인한 뒤 설치하세요.

```powershell
$sampleApk = './DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk'
Get-Item $sampleApk | Select-Object FullName, LastWriteTime, Length
adb -s device-serial install -r --user 0 $sampleApk
adb -s device-serial shell monkey -p dev.doroti.testbed -c android.intent.category.LAUNCHER 1
```

`device-serial`은 연결된 기기의 serial로 바꿉니다. `--user 0`은 기본 사용자에 설치합니다.
갤럭시 보안 폴더 등 다른 프로필에 대한 shell 권한 오류가 발생하면 패키지 조회도
`adb -s device-serial shell pm list packages --user 0 dev.doroti.testbed`로 범위를 지정합니다.

### 실행했는데 샘플이 보이지 않을 때

- Native: 앱을 닫고 `-e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none`을 포함해 다시 실행합니다.
- Android: 모드 변경 시 `--no-build` 없이 빌드·재설치합니다.
- Web: URL에 `dorotiTestbedMode=sample`을 넣습니다. renderer 변경과 화면 선택은 별개입니다.
- iOS: Simulator 실행 상태와 `--device` ID를 확인합니다. 실제 기기는 서명·프로비저닝도 필요합니다.

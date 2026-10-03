# DorotiSampleApp2

Cupertino 스타일의 독립 Doroti 샘플 앱입니다. 공통 C# UI와 Android, iOS, Linux, macOS, Mac Catalyst, Windows, Web 실행 프로젝트로 구성됩니다.

- **Components**: 카운터 버튼, 스위치, 슬라이더, 활동 표시기, 다이얼로그
- **Profile**: 이름 입력과 인사말, 탭 전환 시 입력 상태 유지
- **Settings**: 시스템 / 라이트 / 다크 테마 선택
- **Variable Blur**: 60개 항목의 ListView 위에 상단 고정 VariableBlur 오버레이, 강도 조절과 켜기/끄기
- **Upload**: 이미지·텍스트 파일 다중 선택과 OS 드래그 앤 드롭, 이미지 미리보기·텍스트 내용·파일 이름/크기 표시, 개별 삭제·전체 지우기

## 파일 업로드 예제

**Upload** 탭에서 **파일 선택**을 누르거나 페이지 안에 파일을 드래그해서 놓습니다.
PNG/JPG/JPEG/GIF/WEBP/BMP 이미지는 최대 10 MiB, TXT/MD/CSV/JSON/LOG 텍스트는
최대 1 MiB이며 목록은 8개까지 유지합니다. UTF-8 또는 BOM이 있는 UTF-16 텍스트를
읽고, 긴 내용은 처음 4,000자까지만 표시합니다. 이미지 디코딩에 실패하면 해당 카드에 오류를 표시합니다.

이 예제는 파일을 앱 메모리로 읽어 미리 보는 동작이며 서버 업로드나 디스크 저장은 포함하지 않습니다.
파일 선택·드롭의 지원 여부는 실행 호스트에서 확인하며, 파일 드롭이 없는 환경에서는 선택 버튼을 사용합니다.
드롭은 Upload 탭이 보일 때만 Copy로 수신합니다. 파일 읽기 권한은 읽기 완료/취소 후 해제하고,
목록은 탭을 전환해도 유지되며 앱을 닫으면 초기화됩니다.
구현은 [src/FileUploadPage.cs](src/FileUploadPage.cs)에 있습니다.

검증 (2026-10-03): 공통 앱·Windows App SDK·Web Release 빌드 **PASS**.
CPU 자동 검증에서 실제 위젯 포인터 경로의 탭/버튼 전환, 합성 파일 선택·드롭과 미리보기,
읽기 권한 해제, 탭 상태 유지·수신 해제, 375px 레이아웃을 확인했습니다.
부분 읽기·UTF-8/UTF-16·빈 텍스트와 크기/형식 제한·읽기 취소도 **PASS**.
후속 수정: Windows/Qt가 요구하는 `.txt`, `.png` 형식의 확장자 필터를 전달하도록 고쳤습니다.
Windows 네이티브 파일 선택 창은 샘플의 실제 필터로 TXT·PNG 다중 선택, 파일 읽기·권한 해제,
취소를 자동 검증하여 **PASS**입니다(합성 다이얼로그 명령).
물리 마우스를 통한 앱→선택 창 조작, 탐색기 드래그, 모바일 기기·브라우저에서의 파일 입력은 **notVerified**입니다.

## Variable Blur 예제

Variable Blur 페이지는 리스트 상단 180 논리 단위에 `BackdropFilter`와
`ImageFilterConfig.CreateVariableBlur(startSigma: 강도, endSigma: 0, resolutionScale: 0.25,
adaptiveResolution: true, kernel: VariableBlurKernel.fastGaussian)`를 기본으로 적용합니다.
라디오 버튼으로 다음 다섯 가지 모드를 비교할 수 있습니다.

- **Full quality**: 전체 구간을 원본 해상도로 처리합니다.
- **Adaptive**: 약한 블러와 선명한 끝부분의 세부를 보존할 때 선택합니다. 약한 블러는 원본 해상도, 강한 블러는 1/2·1/4 해상도로 처리하고 경계를 혼합합니다.
- **Fast adaptive** (기본): 강도에 맞춰 Gaussian 가중치를 계산하고 인접 샘플 쌍을 bilinear 샘플링으로 묶습니다. 강한 구간에서도 윤곽이 여러 장 겹치지 않도록 샘플 수를 늘립니다. 작업 해상도 sigma 2~3에서는 기존 커널과 혼합합니다. 과거 고정 7회 커널보다 강한 블러의 처리 비용이 증가할 수 있습니다.
- **Fixed 1/4**: 부드러운 스크롤을 우선하는 선택입니다. 전체 구간을 가로·세로 각각 약 1/4 해상도의 Gaussian으로 처리합니다. 약한 구간과 선명한 끝부분도 축소 입력을 사용해 작은 글자·가는 선·사진 세부가 부드러워질 수 있습니다.
- **Dual Kawase**: 강도별 결과를 공유 피라미드에서 생성하고 분산을 보간합니다. 선명한 끝부분에는 Gaussian을 사용합니다. 정확한 Gaussian과는 다른 근사 모드입니다.

별도의 스위치로 블러 전체를 켜고 끕니다.
위쪽은 흐리고 아래쪽은 선명하며, 블러 영역에서도 리스트를 스크롤할 수 있습니다.
구현은 [src/VariableBlurPage.cs](src/VariableBlurPage.cs)에 있습니다.
VariableBlur에는 GPU 렌더러가 필요하며, 기본 CPU 래스터 검증에는 이 탭의 블러 렌더링이 포함되지 않습니다.

설정과 입력값은 앱 실행 중에만 유지됩니다. Cupertino Icons 1.0.9 폰트와 해당 라이선스는 `assets/fonts`에 포함되어 있습니다.

저장소 루트 `DorotiLab`에서 실행합니다. 루트 `global.json`에 지정된 .NET SDK와 플랫폼별 빌드 도구가 필요합니다.

## 플랫폼 선택

Android 개발 중 C# 변경을 적용하려면 USB 기기 또는 emulator 하나를 연결하고 다음 명령을 실행합니다. Debug Mono 메타데이터 핫리로드 세션을 시작하며, 지원되는 메서드 본문 변경은 저장하면 앱 상태를 유지하면서 적용됩니다.

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiSampleApp2 -Platform android
```

여러 기기가 연결되어 있으면 `-Device <ADB serial>`을 추가합니다. VS Code의 **Terminal → Run Task → Doroti: Android Hot Reload**에서 `DorotiSampleApp2`를 선택하거나, Doroti 확장 기능의 **Select Project → Select Target → android → Run**을 사용합니다. 확장 기능의 **Hot Reload** 버튼은 기기에서 변경된 화면 프레임이 완료된 뒤 성공을 표시합니다. .NET SDK 10.0.400+, Android workload/platform-tools와 Python 3이 필요합니다. Kotlin/Java·Android 리소스·프로젝트 변경과 미지원 C# 변경은 Restart가 필요하고, 종료는 확장 기능의 Stop 또는 CLI Ctrl+C를 사용합니다. [개발 계약과 검증 범위](../../Doroti/docs/development-hot-reload.md)를 참고하세요.

`doroti-workspace.json`에 모든 플랫폼 실행 프로젝트가 등록되어 있습니다.

| CLI 플랫폼 | 실행 프로젝트 | 지원 RID / 호스트 |
| --- | --- | --- |
| `android` | `android/DorotiSampleApp2.Android.csproj` | `android-arm64` (기본), `android-x64` / MAUI |
| `ios` | `ios/DorotiSampleApp2.iOS.csproj` | `iossimulator-arm64` (기본), `iossimulator-x64`, `ios-arm64` / UIKit |
| `linux` | `linux/DorotiSampleApp2.Linux.csproj` | `linux-x64` / Qt Quick |
| `macos` | `macos/DorotiSampleApp2.MacOS.csproj` | `osx-arm64` / AppKit |
| `maccatalyst` | `macos/DorotiSampleApp2.MacCatalyst.csproj` | `maccatalyst-arm64` / MAUI |
| `windows` | `windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj` | `win-x64` / Windows App SDK |
| `web` | `web/DorotiSampleApp2.Web.csproj` | `browser-wasm` / Blazor WebAssembly |

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 describe -App ./samples/DorotiSampleApp2
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./samples/DorotiSampleApp2 -Platform android -Configuration Debug
```

`-Platform`으로 위 표의 대상을 선택합니다. `build`는 빌드, `publish`는 배포 산출물 생성에 사용합니다.
공통 UI, Cupertino 아이콘과 앱 ID(`dev.doroti.sample2`)를 공유하며 AppKit 앱 ID는 `dev.doroti.sample2.macos`입니다.

Windows MAUI 비교용 프로젝트는 `windows/DorotiSampleApp2.Windows.csproj`입니다.
Windows MAUI의 Upload 탭도 실제 HWND 파일 선택·읽기·미리보기를 연결하며,
[2026-10-03 기본 연결 기록](../../Doroti/docs/validation/2026-10-03-windows-maui-basic-connections.md)에
synthetic dialog 선택·preview·정리와 미검증 범위를 남겼습니다.
workspace CLI의 Windows 기본 대상은 위 표의 Windows App SDK를 유지합니다.
네이티브 호스트는 공통 C 정책만 사용합니다. 프레임 설정 없이 실행하거나
`DOROTI_NATIVE_FRAME_MODE=C`를 지정합니다. A/B와 legacy 프레임 설정은 시작 시 거절합니다.
[설정·API 변경 안내](../../Doroti/docs/migrations/native-frame-c-only.md)를 참고하세요.
GPU consumer 수명과 직렬 fallback은 [공통 프레임 문서](../../Doroti/docs/native-frame-pipeline.md)를 참고하세요.
Android는 같은 이름의 `--es` Intent 인자를 사용합니다. Sample2의
`DOROTI_VARIABLE_BLUR_BENCHMARK`, `DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA`,
`DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC`도 cold launch의 `--es`로 전달할 수 있습니다.
이 인자 없이 실행하면 일반 Cupertino 페이지와 Fast adaptive 기본값을 사용합니다.

## Android

Android 워크로드, Android SDK와 JDK 17이 필요합니다. 네이티브 바인딩은 포함된 Gradle wrapper로 빌드합니다.
Vulkan 1.2를 지원하는 기기 또는 에뮬레이터가 필요합니다.

```powershell
dotnet build ./samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj -c Debug
dotnet build ./samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj -c Debug -t:Run -p:RuntimeIdentifier=android-arm64
```

x64 에뮬레이터에서는 `-p:RuntimeIdentifier=android-x64`를 사용합니다. RID별 빌드 산출물은 분리됩니다.

### Android 기기에 Release 설치

[android/deploy-android.ps1](android/deploy-android.ps1)이 구성 선택부터 빌드, APK 업데이트 설치,
앱 실행과 프로세스 확인까지 처리합니다. 특정 제조사에 한정되지 않으며 **ARM64 Android 기기**를 대상으로 합니다.
위의 Android 도구 외에 PowerShell 7과 Python이 필요하며, `dotnet`, `python`, `adb`가 PATH에 있어야 합니다.

기기에서 **USB 디버깅**을 켜고 연결한 뒤 디버깅 허용 창을 승인합니다.
`adb devices -l`에서 `device` 상태인지 확인하세요. `unauthorized`이면 기기에서 연결을 허용해야 합니다.

아래 예시는 **저장소 루트 `DorotiLab`**에서 실행합니다. 스크립트 자체는 호출한 작업 폴더에 관계없이
저장소와 프로젝트 경로를 찾습니다. 연결된 기기가 하나면 시리얼을 자동 선택합니다.

```powershell
# 기본 Release: Mono Profiled AOT + 필요한 코드의 JIT
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode MonoAot

# CoreCLR JIT: 앱의 ReadyToRun 사전 컴파일 비활성화
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode CoreClrJit

# CoreCLR ReadyToRun: R2R 사전 컴파일 코드 + JIT
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode CoreClrR2R
```

세 명령 중 원하는 구성 하나를 실행합니다. `-DotnetVersion`을 생략하면 .NET 10이며,
.NET 10에서 `-Mode`를 생략하면 `MonoAot`입니다.
.NET 10의 Android CoreCLR은 실험적 기능이며, ReadyToRun은 JIT를 유지하는 방식으로 NativeAOT와 다릅니다.

**.NET 11**은 `-DotnetVersion 11`로 선택합니다. `-Mode`를 생략하면 `CoreClrR2R`이며,
`MonoAot` 조합은 지원하지 않아 빌드 전에 중단합니다.

```powershell
# .NET 11 CoreCLR JIT
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -DotnetVersion 11 -Mode CoreClrJit

# .NET 11 CoreCLR ReadyToRun (11의 기본 모드)
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -DotnetVersion 11
```

.NET 11 프로필은 [전용 global.json](android/sdk/net11/global.json)의
SDK `11.0.100-rc.1.26425.128`과 MAUI `11.0.0-rc.1.26451.6`을 사용합니다.
해당 SDK 및 호환되는 Android/MAUI 워크로드를 미리 설치해야 합니다. 이 프로필은 RC 버전입니다.
저장소 루트의 `global.json`은 바꾸지 않고, 11을 선택한 호출에서만 전용 SDK를 사용합니다.

기기가 여러 대이면 `-Serial`로 선택합니다. **`R3CY30KZA4B`는 예시**이며,
제조사와 관계없이 `adb devices -l`에 표시된 실제 대상 시리얼로 바꿉니다.

```powershell
adb devices -l
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode CoreClrR2R -Serial R3CY30KZA4B
```

| `-Mode` | 런타임 / 사전 컴파일 설정 | .NET 10 산출물 폴더 |
| --- | --- | --- |
| `MonoAot` | Mono, `RunAOTCompilation=true`, `AndroidEnableProfiledAot=true` | `Doroti/artifacts/sample2-mono-aot` |
| `CoreClrJit` | CoreCLR, `PublishReadyToRun=false`, Mono AOT 비활성화 | `Doroti/artifacts/sample2-coreclr-jit` |
| `CoreClrR2R` | CoreCLR, `PublishReadyToRun=true`, Mono AOT 비활성화 | `Doroti/artifacts/sample2-coreclr-r2r` |

.NET 11의 산출물 폴더에는 `-net11`이 붙습니다. 예: `Doroti/artifacts/sample2-coreclr-r2r-net11`.
10은 `net10.0-android`, 11은 `net11.0-android` 앱·Android 호스트·바인딩을 빌드합니다.
공통 UI 및 플랫폼 독립 라이브러리는 호환되는 `net10.0`을 유지합니다.
모두 `Release`, `android-arm64`를 사용하며 프로젝트 기본 설정은 변경하지 않습니다.
빌드 중간 파일과 APK는 SDK 버전 및 구성별로 분리합니다. 빌드는 20분 제한으로 실행하고,
빌드 또는 설치가 실패하면 다음 단계로 진행하지 않습니다. ARM64가 아닌 기기는 빌드 전에 거부합니다.
x64 에뮬레이터는 이 스크립트 대신 위의 직접 빌드 명령을 사용합니다.

앱 ID는 모두 `dev.doroti.sample2`이므로 선택한 구성이 기존 설치를 업데이트합니다.
`install -r`은 앱 데이터를 유지하지만 재실행하면 메모리에만 있는 화면 상태와 입력값은 초기화됩니다.
APK는 로컬 기기 테스트용이며 스토어 배포용 서명 설정은 별도입니다.
성공하면 APK 경로, 실행 중인 PID와 해당 프로세스의 `adb logcat` 명령을 출력합니다.
프로세스 실행 확인과 별도로 화면 표시·터치 동작은 기기에서 확인하세요.

2026-09-29 Galaxy S25 (`SM-S931N`)에서 CoreCLR JIT와 CoreCLR ReadyToRun 모두
빌드·업데이트 설치·화면 표시·터치 동작을 확인했습니다.
JIT는 탭 전환, R2R은 버튼 카운터 `0 → 1`을 확인했으며, 전체 기능 및 성능 비교 검증은 포함하지 않습니다.
Mono AOT는 이 비교 작업에서 다시 빌드·실행하지 않았습니다.
배포 스크립트는 `CoreClrR2R`로 기기 자동 선택, 저장소 루트 밖에서 호출,
빌드·설치·프로세스 실행까지 검증했으며, 존재하지 않는 시리얼 지정 시 빌드 전 중단도 확인했습니다.

2026-09-30 같은 기기에서 **.NET 11 RC의 CoreCLR JIT / R2R 모두 PASS**:
스크립트 빌드·업데이트 설치·화면 표시·버튼 카운터 `0 → 1`을 확인했습니다.
두 APK의 CoreCLR/JIT 라이브러리는 .NET 11 런타임 파일과 일치하며, R2R 구성의 사전 컴파일 산출물도 확인했습니다.
.NET 10 기본 설정과 Android 호스트 재빌드는 통과했습니다. .NET 11의 전체 기능·TalkBack·성능 비교는 별도 검증 대상입니다.

## iOS

Mac, 선택한 .NET iOS 워크로드와 호환되는 Xcode가 필요합니다. 실제 기기는 서명 설정도 필요합니다.
직접 프로젝트를 빌드할 때는 .NET 10 Mono와 ARM64 시뮬레이터가 기본입니다.

텍스트 선택 magnifier는 iOS 17 이상 네이티브 앱에서 시스템 loupe를 사용합니다.
iOS 15~16과 iOS 웹에서는 Doroti magnifier를 사용합니다.

```powershell
dotnet build ./samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Debug -t:Run -p:RuntimeIdentifier=iossimulator-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./samples/DorotiSampleApp2 -Platform ios -Configuration Release -Rid ios-arm64 -CompilationMode Mono
```

Intel Mac 시뮬레이터는 `iossimulator-x64`를 선택합니다.
CLI의 iOS Release 기본값은 실험적인 NativeAot이므로, 위 명령처럼 `-CompilationMode Mono`를 명시합니다.

`net10.0-ios27.0`의 실기기(`ios-arm64`) Debug/Mono 프로필은 레이아웃·렌더링 엔진을 Mono AOT로 컴파일하고 앱과 iOS 진입 어셈블리만 해석합니다. 회전 중 매 프레임 실행되는 엔진까지 해석해서 표시 시점을 놓치는 것을 방지합니다. 시뮬레이터는 기존 빠른 빌드 프로필을 유지합니다. 앱 어셈블리 이름을 별도로 지정했거나 추가 개발 어셈블리의 해석이 필요하면 `DorotiIosDebugInterpretedAssemblies`에 쉼표로 구분한 이름을 지정할 수 있습니다. 명시적인 `MtouchInterpreter` 설정은 기본값보다 우선하며 NativeAOT 프로필은 별개입니다.

### iOS Hot Reload

기본 .NET 10 개발 경로는 iOS 시뮬레이터를 사용합니다. 실기기 USB는 .NET 11 CoreCLR 개발 프로필로 연결합니다. 시뮬레이터를 하나 부팅한 뒤 Debug 개발 세션을 시작합니다. 여러 시뮬레이터가 켜져 있으면 `-Device <UDID>`를 지정합니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 dev -App ./samples/DorotiSampleApp2 -Platform ios -IosTargetFramework net10.0-ios27.0
```

앱 C# 메서드를 수정하고 저장하면 SDK가 metadata delta를 적용합니다. VS Code 확장에서도 `ios` 선택 → Run → Hot Reload를 사용할 수 있습니다. `doroti.iosTargetFramework`는 위 명령과 같은 값으로 설정합니다. 지원하지 않는 편집은 명시적 Restart가 필요하며 상태가 초기화됩니다. 네이티브 Swift/바인딩 변경, Release/NativeAOT는 이 핫리로드 경로의 대상이 아닙니다. 준비 조건과 검증 범위는 [개발 세션 계약](../../Doroti/docs/development-hot-reload.md)을 참고하세요.

.NET 10 실기기는 `-Rid ios-arm64 -Device <UDID> -IosTargetFramework net10.0-ios27.0`을 지정하고 Mac과 iPhone을 같은 네트워크에 연결합니다. 개발 서명 프로필이 필요하며, iPhone의 로컬 네트워크 접근을 허용해야 합니다. 기본 네트워크 주소가 맞지 않으면 `-IosHotReloadHost <Mac의 LAN IPv4>` 또는 VS Code의 `doroti.iosHotReloadHost`를 지정합니다. 앱 설치·상태 확인은 기존 기기 연결을 사용하며 코드 변경은 네트워크로 전송합니다. 이 개발 모드에서는 Mono의 AOT/metadata update 충돌을 피하려고 UIKit 호스트 외의 프레임워크도 해석하므로 일반 실행보다 느릴 수 있습니다. 성능 측정은 일반 빌드로 진행하세요.

.NET 11의 USB 전용 경로는 `-Rid ios-arm64 -Device <UDID> -IosTargetFramework net11.0-ios -IosSdkVersion 11.0.100-rc.1.26425.128`을 지정합니다. 프로젝트의 `global.json`은 바꾸지 않습니다. 현재 RC1 SDK에는 수정된 runtime/crossgen2 `11.0.0-rc.2.26478.114`를 개발 모드에서만 사용합니다. 개발 서명, Xcode 27 사용 시 `ValidateXcodeVersion=false`, 바인딩 도구용 런타임과 `-DotnetPath` 준비는 [실기기 개발 명령](../../Doroti/docs/development-hot-reload.md)을 따릅니다. 이 프로필은 Debug 핫리로드 검증용이며 아래 Native AOT 게시 프로필과 구분합니다.

### .NET 11 RC1 + Xcode 27 Native AOT

2026-09-30 기준 [.NET 11 RC1 iOS 워크로드](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc1-12193)는
**Xcode 26.6**을 요구합니다. Xcode 27에서 이 프로필을 빌드·게시하려면 직접
`dotnet build` / `dotnet publish` 명령에 **`-p:ValidateXcodeVersion=false`**를 추가해야 합니다.
Native AOT 앱 생성에는 `dotnet publish`를 사용합니다. 이 옵션은 버전 검사만 건너뛰며,
[Microsoft가 지원하는 Xcode 조합](https://learn.microsoft.com/en-us/dotnet/ios/troubleshooting/xcode-requirement)으로 바꾸지는 않습니다.
서명·네이티브 링크·설치·실행 오류는 별도로 확인해야 합니다.

다음은 **저장소 루트 `DorotiLab`의 PowerShell**에서 실행하는 예제입니다.
SampleApp2에는 .NET 11 선택용 `global.json`이 없으므로, 기존 Testbed iOS 폴더의
`global.json`으로 SDK를 선택한 뒤 SampleApp2 프로젝트를 지정합니다.
인증서 이름과 provisioning profile UUID는 로컬 개발 서명 값으로 바꾸세요.

```powershell
$aotArtifacts = Join-Path (Get-Location).Path 'Doroti/artifacts/sample2-ios-nativeaot'
Push-Location ./samples/DorotiTestbedApp/ios
try {
    dotnet --version # 11.0.100-rc.1.26425.128 또는 해당 global.json이 허용하는 패치
    dotnet publish ../../DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj `
        -c Release -r ios-arm64 `
        -p:DorotiCompilationMode=NativeAot `
        -p:ValidateXcodeVersion=false `
        "-p:ArtifactsPath=$aotArtifacts" `
        '-p:CodesignKey=YOUR_DEVELOPMENT_CERTIFICATE' `
        '-p:CodesignProvision=YOUR_PROVISIONING_PROFILE_UUID'
    if ($LASTEXITCODE -ne 0) { throw 'Native AOT publish failed.' }
} finally {
    Pop-Location
}
```

우회는 해당 명령에만 적용합니다. 공통 프로젝트 설정에서 검사를 끄지 않습니다.
Xcode 26.6을 선택한 .NET 11 빌드나 Xcode 27을 지원하는 .NET 10 워크로드에는 이 옵션이 필요 없습니다.
[도구 업데이트 스크립트](../../scripts/update-dotnet-macos.py)로 .NET 11 워크로드를 RC1으로 업데이트해도
RC1의 Xcode 26.6 요구 사항은 유지됩니다.

2026-09-30 검증: .NET SDK `11.0.100-rc.1.26425.128`, iOS SDK pack `26.5.11720-net11-p6`,
Xcode 27.0에서 위 우회를 적용한 Release Native AOT publish·서명·iPhone 12/iOS 26.6.1 설치와
Components 화면 표시를 확인했습니다. `PublishAot=true`, `UseNativeAot=true`, `UseMonoRuntime=false`,
앱 번들 내 관리 DLL 0개를 확인했습니다. 이 결과는 RC1 iOS SDK pack `26.5.12193-net11-rc.1`의
검증 결과가 아니며, 전체 터치·회전 동작과 배포 적합성 검증도 포함하지 않습니다.

2026-10-01 RC1 pack 검증: 기본 `/usr/local/share/dotnet` 경로로 도구를 통합한 뒤
`net11.0-ios` Release/NativeAOT publish·서명 검증·iPhone 설치를 완료했습니다.
이 빌드는 preservation/class lookup 우회 옵션을 해당 명령에만 추가했으며 ILC 진단이 남았습니다.
첫 실행의 iOS 보안 오류는 사용자 개발자 신뢰 처리 후 해소됐고 Components 화면 표시와
프로세스 유지를 확인했습니다. NativeAOT 성능·전체 수동 조작은 미검증입니다.
정확한 게시 명령과 한계는 [설치 기록](../../history/26-10-03/works/results/2026-10-01-sample2-nativeaot-rc1.md)을 참고하세요.

## Linux

Linux x64에서 .NET SDK, CMake 3.24 이상, C++20 컴파일러, Qt 6.8 이상(Quick/QuickControls2/WebEngineQuick/WebChannel),
Wayland 개발 도구와 Vulkan 1.2 지원 GPU가 필요합니다. 자세한 네이티브 의존성은 [Qt 호스트 안내](linux/native/README.md)를 참고하세요.

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

기존 Material 샘플과 같은 Windows App SDK / Vulkan 호스트를 사용합니다.

## Web

```powershell
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release
```

브라우저에서 `http://127.0.0.1:5089`에 접속합니다. WebAssembly 빌드에는 `wasm-tools` 워크로드가 필요합니다.

Docker 기반 익명 HTTPS 터널로 다른 기기에서 열려면 다음을 실행합니다.

```powershell
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start -App ./samples/DorotiSampleApp2
```

Release 게시 후 접속 주소가 출력됩니다. 종료는 `tunnel.ps1 stop`을 사용합니다.
필수 도구와 관리 명령은 [터널 안내](../../tools/doroti-cloudflared/README.md)를 참고하세요.

웹 글꼴은 `web/WebFonts.cs`의 `PreloadLanguages = ["ko", "en"]` 힌트에 따라 첫 화면 전에
CDN에서 Roboto와 전체 Noto Sans KR을 로드합니다. 한글 자모와 11,172개 음절을 입력 전에
준비하므로 새로운 조합마다 폰트를 받느라 잠깐 사각형으로 표시되는 현상을 방지합니다.
그 외 문자권과 컬러 이모지는 필요한 Noto 폰트 조각을 자동 다운로드합니다.
힌트를 비우면 Roboto만 미리 로드하는 기본 동작으로 돌아갑니다.
Flutter의 CanvasKit/Skwasm처럼 엔진에 폰트 파일을
등록합니다. CSS 링크의 @font-face도 바이트로 읽어 등록하며, DOM body의 font-family는 상속하지 않습니다. 폰트가 추가되면 글자 폭과
레이아웃도 자동 갱신됩니다. 네이티브의 기본/미해결 Cupertino 폰트는 플랫폼 UI 폰트로
연결합니다. CDN 변경·다운로드 비활성화와 지원 범위는
[자동 웹 폰트 안내](../../Doroti/src/Doroti.Host.Web/Fonts/README.md)에 있습니다.
기본 CDN 모드는 Roboto/Noto와 Galmuri를 CDN에서 읽고, 비교용 SUITE는 로컬 CSS/WOFF2로 포함합니다. 네이티브 빌드는 Roboto를 CDN에서
받아 DLL에 포함하므로 실행 시에는 폰트 다운로드가 필요 없습니다.

웹에서도 선택적으로 기본 폰트를 앱 에셋에 포함할 수 있습니다:

```powershell
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:DorotiSampleWebFontSource=Assets
```

이 모드는 기존 샘플의 Roboto 3종과 라이선스를 웹 DLL에 포함하고, 기본 폰트 CDN 및
언어·이모지 자동 다운로드를 끕니다. 설정은 [web/WebFonts.cs](web/WebFonts.cs)에 있습니다.
Fonts 탭의 Galmuri/SUITE와 디코더도 로컬로 포함하므로 이 화면은 외부 폰트 요청 없이 동작합니다. SUITE에 없는 한글은 Galmuri로 폴백합니다. 다른 문자/이모지가 필요하면 그 폰트도 직접 포함하세요.
옵션을 생략하면 기존 CDN 모드로 실행됩니다. 모드를 바꿀 때는 개발 서버를 재시작합니다.

워크스페이스 CLI에서도 위 플랫폼 표의 별칭으로 선택할 수 있습니다.

화면 구현은 [src/App.cs](src/App.cs), 공통 진입점은 [Program.cs](Program.cs)에 있습니다.

## 검증

플랫폼 확장 검증 (2026-09-29):

| 범위 | 결과 |
| --- | --- |
| 워크스페이스 CLI / SDK 그래프 | **PASS**: 7개 플랫폼, Android 2개·iOS 3개 RID를 포함한 10개 구성 |
| Android ARM64 Debug | **PASS**: 빌드·설치, SM-S931N에서 Vulkan 화면 표시와 Components → Profile 터치 탭 전환 |
| Linux x64 Debug | **PARTIAL**: Windows에서 관리 코드 빌드 통과. Linux Qt 네이티브 빌드·실행은 **notVerified** |
| iOS / macOS / Mac Catalyst | 그래프·네이티브 프로젝트 참조 검증 통과. Apple 호스트의 빌드·서명·실행은 **notVerified** |
| 기존 Windows / Web | SDK 그래프 통과. 이번 확장 작업에서 실제 실행은 재검증하지 않음 |

위 결과는 모든 화면·입력·GPU 효과의 전체 플랫폼 동작을 보증하지 않습니다.

현재 CPU 회귀 진입점은 다음과 같습니다. 포인터 탭·입력·다이얼로그·viewport/DPR·리스트 수명과
캡처 정책을 확인하며, GPU Variable Blur 픽셀 비교는 포함하지 않습니다.

```sh
python3 Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Release
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kernel
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-capture
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kawase
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu # macOS Metal only
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-quality temp/testing/variable-blur/quality-review # separate still-image review
python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
```

과거 `Doroti/validation/cupertino-sample` 프로젝트는 현재 트리에 없습니다.
해당 프로젝트의 `--variable-blur`, `--high-dpi`, `--frame-benchmark` 명령을 현재 검증으로 사용하지 않습니다.

Variable Blur의 iPhone 반복 스크롤 측정은 서명된 **Release/Mono** 앱을 먼저 빌드한 뒤 실행합니다.
`--app`은 빌드 산출물 경로, `--device`는 연결된 실기기 식별자입니다.

```sh
dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:EnableCodeSigning=true '-p:CodesignKey=Apple Development' -p:CodesignProvision=<PROFILE_UUID>
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app samples/DorotiSampleApp2/ios/bin/ios-arm64/Release/net10.0-ios27.0/ios-arm64/DorotiSampleApp2.iOS.app --output temp/testing/variable-blur/new-run-sigma20 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 20 --conditions '전원·밝기·온도 조건 기록'
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app samples/DorotiSampleApp2/ios/bin/ios-arm64/Release/net10.0-ios27.0/ios-arm64/DorotiSampleApp2.iOS.app --output temp/testing/variable-blur/new-run-sigma32 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 32 --conditions '동일 전원·밝기·온도 조건 기록'
```

위 명령은 같은 바이너리에서 Off/Fixed/Adaptive, sigma 20/32를 각각 3회씩 총 18회 비교하고
두 번째 반복은 역순으로 진행합니다. 수집기에서 `--modes`를 생략하면 기존 6개 모드(Off/Full/Adaptive/Fast/Fixed/Kawase)를 각각 3회 실행합니다.
`--hz`에는 실제 설정된 표시 주사율을 입력합니다. 각 실행은 40초이며 최초 표시 이후
5~35초 구간의 실제 drawable 표시 간격을 집계합니다. 합성 스크롤이므로 물리 입력 검증은 아닙니다.
완료 표식·30초 표시 이력·렌더러 오류 여부를 검사하며 불완전한 실행을 PASS로 집계하지 않습니다.
앱 설치는 기존 `dev.doroti.sample2`를 업데이트하며 실행마다 해당 앱을 재시작합니다.

수동 프로파일링에서는 `DOROTI_VARIABLE_BLUR_BENCHMARK=off|full|adaptive|fast|fixed|kawase`로
탭·모드·반복 경로를 선택합니다. 미설정 시 일반 UI/Fast adaptive 기본값을 사용합니다.
Off로 benchmark를 시작한 뒤 블러를 다시 켜도 초기 선택은 Fast adaptive입니다.
정지 화면 비교에는 `DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC=1`을 함께 지정하고,
강도는 `DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA=0..32`로 지정할 수 있습니다.
정지 모드는 자동 스크롤 수집기와 함께 사용하지 않습니다.
`--variable-blur-quality`는 macOS Metal에서 DPR 3, sigma 0/1/2/4/8/20/32의 Full/Fixed PNG 14장을
별도 저장합니다. 작은 글자·1px 선·고주파 무늬·참조 사진의 축소 손실을 검토하는 장면이며,
Full과 Fixed의 픽셀 동등성이나 iPhone의 움직임 화질 검증을 뜻하지 않습니다.
`DOROTI_VARIABLE_BLUR_PROFILE=1`과 `DOROTI_MAUI_EVIDENCE=blur.json`을 함께 설정하면
iOS 앱의 Documents에 진단 JSON을 기록합니다.

Dual Kawase는 공유 down 체인에서 강도별 결과를 up 재구성하고, device sigma에 맞춰 분산을 보간하는
명시적 근사 모드입니다. 두 device pixel 이하에서는 Gaussian을 사용해 선명한 끝부분을 유지합니다.
Clamp·회전/반사/균일 scale·최대 약 124 device sigma를 지원하며 다른 설정은 Gaussian으로 대체합니다.
`VariableBlurKernel.dualKawase`는 자체 해상도 피라미드를 사용하므로 `resolutionScale`과 독립적입니다.
공개 `ImageFilter.variableBlur`와 `ImageFilterConfig.CreateVariableBlur` 및 샘플의 초기 선택은
**Fast adaptive**입니다. 기본 조합은 `resolutionScale: 0.25`, `adaptiveResolution: true`,
`kernel: VariableBlurKernel.fastGaussian`입니다. 약한 구간의 원본 해상도와 선명한 끝부분을 보존합니다.
원본 해상도 Gaussian은 `resolutionScale: 1, kernel: VariableBlurKernel.gaussian`으로 명시할 수 있습니다.
Fixed와 Dual Kawase도 개별 옵션으로 선택할 수 있습니다.
이전 [Fixed 검증](../../history/26-10-03/works/results/2026-10-01-variable-blur-fixed-adoption.md)은 당시 기본 선택의 기록이며,
[현재 iOS 프레임 연결 결과](../../history/26-10-03/works/results/2026-10-02-ios-frame-loop.md)와 구분합니다.

`--intermediate`는 Adaptive의 중간 출력 합성을, `--full-capture`는 전체 캡처를 강제하는 같은 바이너리 A/B 옵션입니다.
`--owned-subtrees`는 셰이더 없는 형제 scope도 별도 Surface로 처리하며,
`--full-stages`는 Kawase 강도 결과를 전체 캡처에 재구성합니다. 기본 실행은 native 형제 scope와 필요한 띠만 사용합니다.
`--full-bands` / `--full-detail`은 Gaussian 띠·선명한 구간의 기존 전체 크기 backing을 유지하는 A/B 옵션입니다.
iOS는 새 shader-only 장면에 최대 2개 GPU frame과 비동기 표시를 사용하는 C 경로가 기본입니다.
native·회전·replay는 직렬 admission과 필요한 transaction 표시를 유지합니다.
프레임 정책 선택 옵션은 제거했습니다. iOS 수집기는 설정 없이 C를 측정하며,
과거 A/B 선택과 legacy presentation 설정은 사용할 수 없습니다.
프레임 준비·표시·복귀 수정과 이후 실기기 결과는 [프레임 연결 결과](../../history/26-10-03/works/results/2026-10-02-ios-frame-loop.md)를 참조합니다.
수집기도 별도 정책 옵션이 없으면 C를 측정합니다.
`--sigma 32`로 최대 강도를 측정할 수 있습니다.
수집기의 평균 FPS는 warm 구간의 실제 표시 간격으로 계산하며 CPU stage나 GPU 실행 시간에서 추정하지 않습니다.

진단에는 캡처 fallback 사유, 작업 해상도·draw bounds, Surface pool hit/miss/temporary,
CPU stage p50/p95/p99, 실제 표시 간격과 Metal 할당량을 포함합니다.
CPU stage는 최대 최근 4,096회 호출이며 초기 준비를 포함하고, `filter-total`은 내부 stage와 중첩됩니다.
Surface 정보는 마지막 프레임의 scene filter 전체를 포함합니다. RGBA 추정 바이트를 합산해 live/peak VRAM으로
해석하지 않습니다. UIKit command buffer 카운터는 terminal marker만 세며 Skia 내부 제출 횟수는 아닙니다.
GPU 구간 시간과 peak 메모리는 별도 Metal System Trace가 필요합니다.
[작업 결과·미검증 범위](../../history/26-10-03/works/results/2026-09-30-variable-blur.md)를 참고하세요.

2026-10-01 후속: 축소 격자가 정렬 가능한 축만 부분 캡처하도록 개선했습니다.
`--variable-blur-capture`는 실제 embedded SkSL의 1·1/2·1/4 레벨을 래스터에서 실행하여
전체 도메인과 부분 캡처의 픽셀을 비교합니다. Graphite GPU 픽셀 비교나 Adaptive 마스크 검증을 대신하지 않습니다.
실기기 측정 스크립트의 `--full-capture` 옵션은 `DOROTI_VARIABLE_BLUR_DISABLE_CROP=1`로
같은 바이너리의 전체 도메인 기준을 실행합니다. 기본 실행과 각각 새 출력 폴더로 비교하세요.
[부분 캡처 후속 결과](../../history/26-10-03/works/results/2026-10-01-variable-blur-per-axis.md)에 실행 범위와 미검증 사항을 기록합니다.


## 웹폰트 비교

**Fonts** 탭에서 Roboto/Galmuri11/SUITE Variable을 전환합니다. 300/400/500/700/900
행과 연속 wght 슬라이더, 한글·영문·숫자, 편집 가능한 여러 줄 입력을 제공합니다.
Galmuri는 일반 HTML CSS 링크, SUITE는 원본 로컬 CSS/WOFF2 등록 예제입니다.
일반 모드의 디코더만 로컬로 묶으려면 `-p:DorotiBundleWoff2Decoder=true`를 사용합니다.
`Assets` 모드는 기본 폰트뿐 아니라 CSS와 디코더도 외부 요청 없이 제공합니다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:DorotiSampleWebFontSource=Assets -o Doroti/artifacts/sample2-fonts
```

`wwwroot`를 COOP/COEP 헤더가 있는 서버로 제공하세요. `/sample/` 배포에서는 HTML의
base href도 `/sample/`로 바꿉니다. 디코더 CSP와 CSS 지원 범위는
[폰트 사용 문서](../../Doroti/src/Doroti.Host.Web/Fonts/README.md#css-links-and-variable-fonts)를 참고하세요.
검증 절차는 [css-fonts](../../history/26-09-28/web-fonts-summary.md)에 있습니다.


## 공통 assets 폴더의 웹폰트

폰트 원본은 `web/wwwroot`가 아니라 **`assets/fonts/`**에 둡니다.
웹 프로젝트의 `Content` + `Link` 매핑이 개발 서버와 publish에 같은 웹 경로를 만듭니다.
Doroti 웹 SDK가 링크된 파일의 실제 원본 폴더를 개발용 정적 에셋 manifest에 기록합니다.
예를 들어 아래 파일은 웹에서 `fonts/SUITE/SUITE-Variable.css`로 접근합니다.

```text
DorotiSampleApp2/
  assets/fonts/SUITE/
    SUITE-Variable.css
    SUITE-Variable.woff2
    LICENSE
  assets/fonts/Galmuri/
    galmuri-local.css
    Galmuri11.woff2
    Galmuri11-Bold.woff2
    Galmuri11-Condensed.woff2
    LICENSE.Galmuri
```

```xml
<!-- web/DorotiSampleApp2.Web.csproj; Include는 프로젝트 파일 기준 상대 경로 -->
<Content Include="../assets/fonts/**/*"
         Link="wwwroot/fonts/%(RecursiveDir)%(Filename)%(Extension)"
         CopyToOutputDirectory="PreserveNewest"
         CopyToPublishDirectory="PreserveNewest" />
```

새 WOFF/WOFF2와 CSS도 `assets/fonts/원하는폴더/`에 함께 넣으면 같은 규칙으로 포함됩니다.
CSS 안의 `url('./MyFont.woff2')`는 배포 후에도 같은 폴더를 가리킵니다.
다른 원본 디렉터리를 쓰려면 `Include`만 해당 경로로 바꾸면 됩니다.

```html
<link rel="stylesheet" href="fonts/SUITE/SUITE-Variable.css">
```

CSS 없이 등록할 때도 실제 파일 시스템 경로가 아니라 웹 URL을 사용합니다.

```csharp
new BrowserFontAsset("SUITE Variable", "fonts/SUITE/SUITE-Variable.woff2")
```

샘플의 Galmuri는 `DorotiSampleWebFontSource=Assets`일 때만 로컬 파일을 포함합니다.
기본 모드는 기존 CSS CDN 링크를 사용합니다. CupertinoIcons는 공통 앱 DLL의 내장
리소스로 이미 등록하므로 웹 정적 파일 목록에서 제외합니다. `bin`/`obj` 안의 파일은
빌드 산출물이므로 직접 수정하지 않습니다.

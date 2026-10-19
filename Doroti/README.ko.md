# Doroti 프레임워크

[English](README.md) | **한국어** · [프로젝트 소개](../README.ko.md)

### XAML 없이 C#으로 만드는 크로스 플랫폼 UI

Doroti는 데스크톱·모바일·웹에서 공유하는 위젯, 레이아웃, 페인팅, 시맨틱스, 렌더링 파이프라인을 제공합니다. Flutter를 참고한 API를 `Doroti.Framework.*`의 C# 코드로 직접 개발하고 유지보수합니다.

이 문서는 프레임워크 구성, 개발 환경, 앱 시작 방법과 빌드 도구를 안내합니다. 실행 가능한 예제와 플랫폼별 실행 명령은 [샘플 앱 가이드](../samples/DorotiTestbedApp/README.ko.md)에서 확인할 수 있습니다.

> [!WARNING]
> Doroti는 실험적 프로젝트입니다. API와 프로젝트 구조는 변경될 수 있으며, 플랫폼별 구현과 검증 수준에는 차이가 있습니다.

[요구 사항](#요구-사항) · [빌드와 실행](#빌드와-실행) · [앱 개발](#앱-개발) · [플랫폼 설정](#플랫폼-설정) · [프레임워크 구성](#프레임워크-구성)

## 제공하는 기능

- C#으로 작성하는 Material·Cupertino 위젯, 레이아웃, 상태, 페인팅, 시맨틱스.
- 플랫폼 중립 앱 라이브러리와 플랫폼별 네이티브·웹 실행 프로젝트.
- Skia Graphite 렌더링: Windows·Android·Linux의 Vulkan, Apple 플랫폼의 Metal. Web은 Android 외 플랫폼에서 Ganesh/WebGL2를 기본으로 사용하고 Android에서 Graphite/Dawn/WebGPU를 우선합니다.
- Windows App SDK, 선택적 Windows MAUI, Android, UIKit, AppKit, Mac Catalyst, WebAssembly, Qt 호스트.
- 공통 진입점, 리소스, 네이티브 바인딩, 템플릿과 빌드 도구.

각 플랫폼의 호스트와 렌더러는 [플랫폼별 구현 표](../README.ko.md#플랫폼별-구현)에 정리되어 있습니다.

## 요구 사항

Release 앱 DLL에는 ReadyToRun 게시를 포함해 Obfuscar 난독화를 선택적으로 적용할 수 있습니다. NativeAOT·Mono/WASM AOT 경로는 제외합니다.
Windows와 Linux Qt에서는 `PublishSingleFile=true`로 단일 파일을 게시할 수 있습니다.
트리밍하지 않은 JIT 또는 개별 ReadyToRun 게시를 지원합니다. [캐러셀 게시 예제](../samples/DorotiCarouselApp/README.md#데스크톱-단일-실행-파일-게시)를 참고하세요.

### 공통 도구와 SDK 선택

- Windows·macOS·Linux에서 [PowerShell 7](https://learn.microsoft.com/ko-kr/powershell/scripting/install/install-powershell?view=powershell-7.6)을 설치해 `eng/doroti.ps1` 등 저장소 스크립트 실행에 사용합니다. 아래 명령은 **저장소 루트 `DorotiLab`**에서 PowerShell로 시작합니다.
- **iOS 외 플랫폼: .NET SDK 10.0.400** 또는 같은 기능 밴드의 호환 패치 버전. [루트 global.json](../global.json)과 [Doroti/global.json](global.json)이 선택합니다.
- **iOS Testbed: .NET SDK 11.0.100-rc.2.26504.105** 또는 호환 패치 버전. [iOS global.json](../samples/DorotiTestbedApp/ios/global.json)이 선택합니다. .NET 10과 함께 설치합니다.
- 선택한 SDK에 맞는 플랫폼 워크로드와 NuGet 패키지 복원이 필요합니다. .NET 10 경로는 프로젝트가 지정한 10.0.11 런타임 팩을 사용하며, 필요한 팩은 대상별로 복원합니다.

`dotnet`은 **현재 작업 폴더**에서 상위로 `global.json`을 찾습니다. `--project` 경로만 iOS로 지정해도 SDK가 바뀌지는 않습니다. iOS 직접 실행은 `samples/DorotiTestbedApp/ios`에서 하고, 다른 플랫폼은 저장소 루트에서 실행합니다. workspace CLI의 `build/run/publish -App ./samples/DorotiTestbedApp -Platform ios`는 iOS 폴더를 자동으로 사용합니다.

SDK 선택과 대상 프레임워크는 별개입니다. Testbed를 직접 빌드하는 iOS 실기기 Release는 NativeAOT가 기본이며, `dotnet build/publish`에 `-p:DorotiCompilationMode=Mono -p:PublishAot=false`를 전달해 Mono 프로필을 선택합니다. NativeAOT의 기본값은 `net11.0-ios`와 MAUI `11.0.0-rc.2.26505.5`이고 Debug·시뮬레이터는 `net10.0-ios`를 유지합니다. workspace CLI는 manifest의 TFM/RID를 사용하며 현재 Testbed의 `ios` 별칭은 `net10.0-ios` / `iossimulator-x64`입니다. 다른 시뮬레이터·서명된 실기기·SDK 선택은 [iOS 샘플 안내](../samples/DorotiTestbedApp/README.ko.md#ios-샘플)를 참고하세요.

**.NET 11 RC2 + Xcode 27:** RC2는 Xcode 27.0과 워크로드 집합 `11.0.100-rc.2.26509.2`(iOS `27.0.12212-net11-rc.2`)를 사용합니다. Xcode 버전 검사는 활성화한 상태로 빌드합니다. [SampleApp2 빌드 예제](../samples/DorotiSampleApp2/README.md#net-11-rc2--xcode-27-native-aot), [RC2 공식 요구 사항](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc2-12212)을 참고하세요.

### 플랫폼별 구성

선택한 플랫폼 행의 도구를 준비합니다. workload 이름은 설치 항목이며, 네이티브 SDK와 시스템 라이브러리는 별도로 준비해야 합니다.

| 플랫폼 / RID | 빌드 호스트 | .NET SDK / workload | 추가 도구와 실행 조건 |
| --- | --- | --- | --- |
| Windows App SDK (기본) / `win-x64` | Windows x64 | 10 / 별도 MAUI workload 없음 | Visual Studio MSBuild, MSVC **v145** C++ toolset, Windows SDK **10.0.26100.0**. 기본 Vulkan presenter는 Vulkan 1.2 드라이버, D3D12 외부 메모리 공유와 DXGI/DirectComposition 지원이 필요합니다. Acrylic은 Windows 11 24H2 이상을 사용합니다. |
| Windows MAUI (선택) / `win-x64` | Windows x64 | 10 / `maui-windows` | Windows SDK와 MAUI Windows 빌드 도구. workspace CLI에서 샘플의 `-Platform windows-maui` 별칭을 선택합니다. |
| macOS AppKit / `osx-arm64` | Apple Silicon Mac | 10 / `macos` | **macOS 14 이상**, workload와 호환되는 전체 Xcode 설치 및 Metal 지원. 앱 소유 Swift/Objective-C binding도 Xcode로 빌드합니다. |
| Mac Catalyst / `maccatalyst-arm64` | Apple Silicon Mac | 10 / `maui-maccatalyst` | workload와 호환되는 전체 Xcode, Mac Catalyst SDK와 Metal 지원. AppKit과 별도 runner입니다. |
| Android / `android-arm64`, `android-x64` | Windows 또는 macOS | 10 / `maui-android` | Android SDK Platforms·Build Tools·Platform Tools(`adb`), **OpenJDK 17–21**. .NET workload가 요구하는 SDK와 native bridge의 **API 34**를 준비합니다. Android 7.0/API 24 이상 기기 또는 해당 ABI 에뮬레이터와 Vulkan 1.2 지원이 필요합니다. |
| iOS / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode | 11 / `maui-ios` | workload와 호환되는 전체 Xcode와 iOS SDK. 시뮬레이터는 해당 Simulator runtime, 실기기는 **iOS 15 이상**, 코드 서명 인증서와 provisioning profile이 필요합니다. Release NativeAOT는 `ios-arm64`용입니다. |
| Linux Qt / `linux-x64` | Linux x64 | 10 / 별도 MAUI workload 없음 | 기본 Quick은 **Qt 6.6 이상** Core/Gui/Widgets/Quick/Qml/QuickControls2와 런타임 QML 모듈이 필요합니다. Testbed WebView에는 **Qt 6.8 이상** WebEngineQuick/WebChannel도 필요합니다. 공통으로 **CMake 3.24 이상**, C/C++20 compiler, `pkg-config`, Wayland client 개발 파일, `wayland-scanner`, Vulkan 헤더, fontconfig와 `wayland` 또는 `xcb` QPA, Vulkan 1.2가 필요합니다. 비교 백엔드는 [네이티브 호스트 안내](../packages/platforms/qt/native/README.md)를 참고하세요. |
| Web / `browser-wasm` | Windows, macOS 또는 Linux | 10 / `wasm-tools` | Android는 사용 가능한 WebGPU를 우선하고, 나머지 플랫폼은 WebGL2를 사용합니다. Testbed의 메인 소유 런타임에는 WASM threads와 COOP/COEP 격리도 필요합니다. `worker-direct-webgpu`를 명시적으로 선택하면 하드웨어 WebGPU 어댑터가 필요합니다. |

Windows App SDK 2.4는 target의 NuGet 복원·배포에 포함됩니다. 별도의 machine-wide Windows App Runtime 설치는 요구하지 않습니다. Android native bridge는 저장소의 Gradle wrapper 8.10.2/AGP 8.6.1을 사용합니다. `JAVA_HOME`으로 지원 JDK를 지정하고 `adb`를 `PATH`에 추가합니다. Apple은 `xcode-select -p`와 `xcodebuild -version`으로 선택한 Xcode를 확인합니다.

Linux는 API 조건을 충족하는 llvmpipe 같은 소프트웨어 Vulkan 장치도 허용합니다. Web의 `Microsoft.TypeScript.MSBuild` 7.0.0은 Web runner가 복원하며, 앱 빌드에 Node/npm/Bun 설치는 필요하지 않습니다.

### 설치 상태 확인과 워크로드 복원

```powershell
# 저장소 루트: SDK 10 확인
dotnet --version
dotnet workload list
pwsh -File ./Doroti/eng/doroti.ps1 doctor -App ./samples/DorotiTestbedApp -Platform windows -Scope target

# macOS AppKit 예시: 현재 호스트에서 사용할 runner 경로로 변경
dotnet workload restore ./samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj

# iOS: SDK 11을 선택한 위치에서 확인 및 복원
Push-Location ./samples/DorotiTestbedApp/ios
dotnet --version
dotnet workload list
dotnet workload restore ./DorotiTestbedApp.iOS.csproj
Pop-Location
```

`workload restore`는 선택한 SDK의 .NET workload를 준비하며 외부 도구는 별도로 설치합니다. provider doctor에는 `-App`과 `-Platform`이 필요하고, `-Scope managed|target|native|tools|full`로 준비 조건의 검사 범위를 선택합니다. FAIL/PARTIAL은 nonzero exit입니다. 준비 조건 PASS는 빌드·기기 수락을 대신하지 않습니다.

플랫폼별 실행 명령은 [Testbed 실행 안내](../samples/DorotiTestbedApp/README.ko.md#material-샘플-모드)에 있습니다. `reference/flutter-master` checkout은 명시적인 Flutter 비교에만 필요하며, 필요할 때 `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`로 준비합니다.

## 빌드와 실행

플랫폼에 필요한 도구를 준비한 뒤 **저장소 루트 `DorotiLab`**에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor -App ./samples/DorotiTestbedApp -Platform windows -Scope target

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiTestbedApp -Platform windows
```

`run`은 빌드한 뒤 앱을 실행합니다. Windows는 Windows App SDK/`HwndExactCpp`가 기본이며, 독립 MAUI 실행 프로젝트는 `-Platform windows-maui`로 선택합니다. 다른 대상은 `-Platform`에 `android`, `ios`, `macos`, `maccatalyst`, `linux`, `web`을 지정합니다. 준비 사항과 기기 옵션은 [샘플 앱 가이드](../samples/DorotiTestbedApp/README.ko.md#플랫폼별-실행)를 참고하세요.

### 명령 안내

`describe -App <workspace> -Platform all`은 선언된 provider 별칭, 현재 호스트에서 가능한 작업과 개발 전송 방식을 보여줍니다. `build`, `run`, `publish`, `dev`에는 선언된 별칭과 provider 지원이 필요하며 `dev`는 Debug 전용입니다. 설정·기기·템플릿 서비스는 확장이 제공하고 미지원 서비스는 명시적으로 실패합니다. 기본 템플릿은 `dotnet new`로 설치하며 `--design widgets|material|cupertino`를 지원합니다.

이전 `-WindowsBackend`, `-Rid`, `-LastSuccessful`, `-NoBuild`, `-NoRestore`, `native`와 고정 템플릿 명령 스위치는 현재 managed provider CLI에 없습니다. TFM/RID/backend/profile은 `doroti-workspace.json`에서 선택하며 다른 지원 빌드 프로필은 직접 runner/MSBuild 명령으로 사용합니다. Android Debug Mono 개발은 provider의 Python/ADB adapter를 사용합니다. Apple Debug 개발은 패키지에 포함된 Python adapter로 AppKit·Catalyst·iOS 시뮬레이터와 선언된 iPhone Mono LAN/CoreCLR USB profile을 연결합니다. 현재 Apple 실행 수락은 미검증입니다. 개발 대상은 [VS Code 안내](../tools/vscode-doroti/README.md)를 참고하세요.

저장소 유지보수 명령은 `validate -ValidationSuite <suite>`, `audit`, `release`입니다. `release`는 Release 검증 suite를 실행하며 패키지나 GitHub Release를 생성하지 않습니다. 로컬 패키지 후보는 [release-candidate.py](eng/release-candidate.py)로 따로 생성합니다.

### 로컬 릴리스 후보

Windows 빌드 호스트에서 독립 core/provider 패키지와 격리된 NuGet 전용 템플릿 consumer를 생성합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/release-candidate.py --targets windows web --design widgets
```

기본 버전은 core `0.4.1-beta`, Material·Cupertino `1.0.0-alpha.1`, 선택한 provider manifest의 버전입니다. `--core-version`, `--material-version`, `--cupertino-version`, 반복 가능한 `--provider-version PROVIDER=VERSION`으로 지정합니다. `--workspace`는 workspace v2 manifest를 선택하며, 대상은 현재 호스트에서 게시 가능한 선언된 별칭이어야 합니다. provider마다 한 실행에 한 profile만 선택할 수 있습니다.

기본 출력은 새 `Doroti/artifacts/release/<version>-<id>/`이며 `--output`으로 변경할 수 있습니다. `candidate.json`에 평가된 버전, 패키지·게시 파일 해시, 게시 profile과 runtime 결과를 기록합니다. `--build-root`는 `Doroti/artifacts` 아래여야 하고, 조사 로그·consumer는 `temp/testing/release-candidate/`에 생성합니다. 비어 있지 않은 후보 폴더는 덮어쓰지 않습니다. 로컬 출력은 [저장 공간 정책](eng/LOCAL_STORAGE.md)에 따른 삭제 가능 산출물입니다.

runtime 검사는 `--runtime-targets`로 명시하며 Android에는 `--device <serial>`도 필요합니다. 생략하면 runtime은 `notVerified`입니다. 서명·깨끗한 컴퓨터 설치·물리 입력은 별도 수락 범위입니다. 스크립트는 NuGet이나 GitHub에 게시하지 않고, `doroti release`도 이 후보를 생성하지 않습니다.

## 앱 개발

### 시작점

앱의 루트 위젯으로 진입점을 생성합니다.

```csharp
new Doroti.Framework.DorotiWidgetEntrypoint(() => new MyApp())
```

`MyApp`은 앱에서 정의한 루트 위젯입니다. 진입점이 루트를 연결하고 첫 프레임을 자동으로 요청하므로 `scheduleFrameCallback`이나 `scheduleForcedFrame`을 직접 호출할 필요가 없습니다. 선택적인 두 번째 `Func<Task>` 인자로 폰트 같은 리소스를 준비한 뒤 루트를 생성할 수 있습니다. 완료 처리는 뷰의 이벤트 루프로 돌아오며, 준비 중 뷰가 분리되거나 종료되면 루트를 연결하지 않습니다.

### 테마

Material은 Material 3를 사용합니다. `ThemeData` 생성자, 팩터리, `copyWith`에서 제거된 `useMaterial3` 인자를 생략하세요. 타이포그래피는 `Typography.Create` 또는 `CreateMaterial2021`을 사용합니다.

시스템 다크 모드를 따르려면 `MaterialApp`에 `theme`, `darkTheme`, `themeMode: ThemeMode.system`을 지정합니다. `ColorScheme.CreateFromSeed`와 `Brightness.light`/`Brightness.dark`로 팔레트를 만들고, 위젯에서는 `Theme.of(context).colorScheme`으로 읽습니다. [전체 테마 예제](../samples/DorotiTestbedApp/README.ko.md#시스템-다크-모드와-색-팔레트)를 참고하세요.

### 앱 아이콘

샘플과 새 템플릿은 Doroti 앱 아이콘을 기본으로 사용합니다. 앱별 교체 방법과 기본값 비활성화는 [앱 아이콘 안내](branding/README.md#앱의-기본-아이콘)를 참고하세요.

### 네이티브 바인딩

Android, iOS, AppKit macOS, Mac Catalyst 실행 프로젝트는 각각 앱 소유 네이티브 바인딩을 참조합니다. Android는 `AndroidGradleProject`, Apple은 `XcodeProject`를 사용하며, .NET 실행 프로젝트가 선택한 profile로 바인딩과 최종 앱을 빌드합니다. Android native workspace는 Android Studio, Apple workspace는 Xcode에서 엽니다. provider CLI에는 `native doctor/build/open/add` 명령이 없습니다.

## 플랫폼 설정

### Windows

| 설정 | 동작 |
| --- | --- |
| `DOROTI_WINDOWS_PRESENTER` | 기본값은 `Vulkan` |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | 기본값은 `NoPreference`, `LowPowerPreference`·`HighPerformancePreference`는 Vulkan에 적용 |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | 정확한 장치 이름 또는 유일한 이름 일부로 Vulkan 장치 선택 |

기본 경로는 Graphite/Vulkan으로 렌더링하고 같은 GPU의 D3D12/DXGI DirectComposition으로 화면에 표시합니다. 자동 출력 경로 전환은 없습니다. 네이티브 PlatformView의 래스터 조각은 D3D11 그리기 API를 유지합니다.

### Web

| 렌더러 | 요구 사항 |
| --- | --- |
| `worker-direct-webgl` (Android 외 기본) | Ganesh/WebGL2, 독립 Worker 런타임도 지원 |
| `worker-direct-webgpu` (Android 우선) | Graphite/Dawn, `runtimeLocation: "main"`, WASM threads, COOP/COEP 격리, 하드웨어 WebGPU 어댑터 |

옵션 생략, `auto`, 알 수 없는 렌더러 값은 Android에서 WebGPU를, Windows·iOS/iPadOS·macOS 등에서는 WebGL2를 우선합니다. Android 자동 선택은 시작 전에 런타임·출처 조건과 WebGPU API·어댑터를 확인하고 사용할 수 없으면 WebGL2를 선택합니다. 명시적인 렌더러 지정은 유지하며, 사전 확인 이후 장치·context 초기화 실패 시에는 다른 백엔드로 재시도하지 않습니다. 두 경로 모두 표시할 canvas를 한 번 이전합니다. 로더의 `started`는 런타임·GPU 준비 완료를 뜻하며, 첫 콘텐츠가 표시되었다는 의미는 아닙니다.

앱의 부팅 코드는 `web/src/**/*.ts`, 프레임워크 웹 코드는 `../packages/platforms/web/Doroti.Host.Web/Web/*.ts`에 있습니다. `Microsoft.TypeScript.MSBuild`가 실행 프로젝트의 `obj`에 JavaScript를 만들고, 배포에는 결과 파일을 포함합니다. Node, npm, Bun, 번들러는 필요하지 않습니다. Testbed와 템플릿은 같은 출처의 대체 폰트를 미리 읽고 import map의 `dotnet.js`를 사용합니다. [웹 렌더러 옵션](../samples/DorotiTestbedApp/README.ko.md#web-렌더러와-측정-옵션)을 참고하세요.

### 창 외형

`DorotiViewConfiguration.appearance`에서 배경 효과와 상단바를 독립적으로 선택합니다. 상단바 기본값은 `unified`이며, `solid`는 분리형 상단바입니다. Windows App SDK·Qt에는 Acrylic을, macOS에는 별도 설정으로 Acrylic이나 Liquid Glass를 지정할 수 있습니다.

Windows 11 24H2 이상에서는 실험 플래그 없이 `new WindowBackdropOptions(WindowBackdropMode.acrylic)`으로 Acrylic을 요청합니다. Material 샘플은 이미 이 설정을 사용합니다. 효과가 보이려면 앱 콘텐츠도 투명하거나 반투명해야 하며, 배경 옵션 미지정과 `system`은 불투명 창을 유지합니다.

<details>
<summary>이전 Acrylic 설정과의 호환성</summary>

`experimentalAcrylic`은 같은 Acrylic 구현을 사용합니다. `DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1`은 이전 샘플 모드를 재현합니다. 실행 중 kind/theme/tint/luminosity 변경과 상태 조회는 호환성을 위해 `doroti/windows/experimental-acrylic` 플랫폼 채널을 유지합니다.

</details>

## 프레임워크 구성

| 모듈 | 역할 |
| --- | --- |
| `Doroti.Framework.*` | 기반 기능, 스케줄링, 서비스, 애니메이션, 제스처, 레이아웃, 페인팅, 시맨틱스, 위젯, Material·Cupertino |
| `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting` | 런타임 지원과 공통 시작·빌더·뷰 계약 |
| `Doroti.App.Sdk` | 플랫폼 중립 `net10.0` 앱 어셈블리와 공용 리소스 |
| `Doroti.Runner.Sdk` | 실행 프로젝트 검증, 네이티브·웹 부팅, 플러그인 등록 |
| `Doroti.Skia.Rendering`, `Doroti.Skia.RuntimeEffects` | 공용 GPU 렌더링, 장면, 텍스트, 이미지, 효과, SkSL 컴파일 |
| `Doroti.Host.WindowsAppSdk` + `.Native` | 관리 코드의 프레임워크 연동과 네이티브 C++ 창·입력 처리 |
| `Doroti.Host.Maui` | Android, UIKit, AppKit, Mac Catalyst, 선택적 Windows MAUI 어댑터 |
| `Doroti.Host.Web` | Worker·canvas 시작, 입력, 접근성, 리소스 |
| `Doroti.Host.Qt` | Qt 6 `QQuickWindow`(기본), C ABI 브리지, 입력, IME, 데스크톱 서비스, 접근성 |
| `Doroti.Target.*` | 플랫폼별 패키지 구성과 배포 |

기능 추가와 수정은 해당 기능을 담당하는 프레임워크, 런타임, 렌더러, 호스트 프로젝트에서 직접 수행합니다. 공통 계약이 바뀌면 사용하는 쪽도 함께 수정합니다. Dart-to-C# 컴파일러와 고정 Flutter 소스는 선택적인 소스 가져오기·동작 비교 도구이며, 유지보수 중인 프레임워크 소스를 덮어쓰지 않고 일반 빌드에도 필요하지 않습니다.

## 플랫폼 지원 범위

호스트, 렌더러와 빌드 모드의 지원 범위는 플랫폼마다 다릅니다. 서명, 스토어 배포, 입력기, 접근성과 성능은 사용할 기기와 구성에서 확인해야 합니다.

네이티브 컨트롤과 WebView에는 호스트별 합성 제약이 있습니다. 프레임 텍스처는 CPU RGBA, Android Surface/AHB, Windows D3D11 입력을 지원합니다. 웹 텍스처는 WebGPU 또는 명시적인 WebGL에서 Canvas·비디오·프레임 GPU 입력을 받습니다. iOS/macOS Metal·Linux DMA-BUF 카메라 어댑터는 사용 전에 실행 검증이 필요합니다.

## 개발과 디렉터리 구성

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 공통 프레임워크, 런타임, 렌더링, 도구, SDK |
| [`../packages/`](../packages/README.md) | Material·Cupertino 구현과 플랫폼 host/target provider |
| [`templates/`](templates/) | `doroti-app` 앱 작업 공간 템플릿 |
| [`eng/`](eng/) | 빌드, 실행, 검증, 패키징, 진단 스크립트 |
| [`../tools/`](../tools/) | 저장소 개발 도구 |
| [`tests/`](tests/) | 유지보수하는 회귀 테스트 소스 |
| [`../tools/Doroti.DartToCSharp/`](../tools/Doroti.DartToCSharp/) | 선택적 Dart·Flutter 소스 변환 컴파일러 |

도구 캐시는 `.doroti/`, 삭제 가능한 빌드·게시 결과는 `Doroti/artifacts/`에 둡니다. 새 검증 소스·임시 프로젝트·로그·캡처는 저장소 루트의 `temp/testing/<task>/<run>/`을 사용합니다. 유지보수 suite는 `eng/validate.ps1`의 20분 제한으로 실행하며 성공 시 요약 후 해당 run을 정리합니다. 직접 실행한 검사는 [검증 안내](tests/README.md)에 따라 결과를 기록하고 정리합니다. 보존할 단계별 기록은 `../history/`에 둡니다. 컴파일러 소유 `.g.cs` 파일은 `src/Doroti.Framework.*`에 포함하여 컴파일하지 않습니다. 저장소 JSON 처리는 `System.Text.Json`을 사용합니다.

## 라이선스

Doroti는 [BSD 3-Clause 라이선스](../LICENSE)를 따릅니다. 외부 소스와 패키지의 저작권 표기는 [서드파티 고지](THIRD-PARTY-NOTICES.md)를 참고하세요.

### Linux Debian 패키지

Debian 13 amd64의 Linux Qt 게시에서 `-PackageFormat deb -Output <디렉터리>`를
사용할 수 있습니다. Release의 self-contained, untrimmed JIT 게시 결과에
Qt 6.12 런타임을 앱 전용 경로에 동봉하고 launcher, 메뉴, 아이콘, 라이선스 문서를 포함합니다.
WebEngine 6.140도 기본 동봉하며, 분리할 때는 엔진·앱 `.deb` 두 개를 APT로 함께 설치할 수 있습니다.
자체 저장소는 필수가 아닙니다. SDK 경로를 명시하면 런타임 파일을 자동 수집합니다.
`doroti-sdk.json`은 선택 사항이며, 있으면 지정한 배포 목록과 소스·고지 자료를 검증합니다.
[런타임 안내](../packages/platforms/qt/qt-runtime.md)를 참고하세요.
Linux runner에 고유한 `DorotiDebPackageId`, maintainer, description, copyright 파일을
설정합니다. [Qt 패키징 안내](../packages/platforms/qt/README.md#debian-packages)에
설정·버전 규칙·환경 요구사항이 있습니다. 옵션을 생략하면 기존 폴더 게시를 유지합니다.
`sudo apt install ./<app>.deb`로 설치·업데이트하고, `sudo apt remove <package-id>` 또는
`sudo apt purge <package-id>`로 제거합니다. 사용자 XDG 데이터는 보존합니다.
로컬 `.deb` 배포만으로 자동 업데이트를 제공하지 않습니다. 설치와 GUI 실행 검증은
[테스트 안내](tests/README.md)에서 별도로 기록합니다.
launcher는 세션에 따라 네이티브 Wayland/xcb를 선택하며 명시한 설정을 보존합니다.
Qt 6.12/Debian 13의 실제 Wayland·xcb 검증은 릴리스 완료 조건으로 남아 있습니다.

# Doroti 프레임워크

[English](README.md) | **한국어** · [프로젝트 소개](../README.ko.md)

### XAML 없이 C#으로 만드는 크로스 플랫폼 UI

Doroti는 데스크톱·모바일·웹에서 공유하는 위젯, 레이아웃, 페인팅, 시맨틱스, 렌더링 파이프라인을 제공합니다. Flutter 프레임워크 소스를 C#으로 변환하는 데서 시작했으며, 현재는 `Doroti.Framework.*`의 C# 코드를 직접 개발하고 유지보수합니다.

이 문서는 프레임워크 구성, 개발 환경, 앱 시작 방법과 빌드 도구를 안내합니다. 실행 가능한 예제와 플랫폼별 실행 명령은 [샘플 앱 가이드](../DorotiTestbedApp/README.ko.md)에서 확인할 수 있습니다.

> [!WARNING]
> Doroti는 실험적 프로젝트입니다. API와 프로젝트 구조는 변경될 수 있으며, 플랫폼별 구현과 검증 수준에는 차이가 있습니다.

[요구 사항](#요구-사항) · [빌드와 실행](#빌드와-실행) · [앱 개발](#앱-개발) · [플랫폼 설정](#플랫폼-설정) · [프레임워크 구성](#프레임워크-구성)

## 제공하는 기능

- C#으로 작성하는 Material·Cupertino 위젯, 레이아웃, 상태, 페인팅, 시맨틱스.
- 플랫폼 중립 앱 라이브러리와 플랫폼별 네이티브·웹 실행 프로젝트.
- Skia Graphite 렌더링: Windows·Android·Linux의 Vulkan, Apple 플랫폼의 Metal, Web의 Dawn/WebGPU.
- Windows App SDK, 선택적 Windows MAUI, Android, UIKit, AppKit, Mac Catalyst, WebAssembly, Qt 호스트.
- 공통 진입점, 리소스, 네이티브 바인딩, 템플릿과 빌드 도구.

각 플랫폼의 호스트와 렌더러는 [플랫폼별 구현 표](../README.ko.md#플랫폼별-구현)에 정리되어 있습니다.

## 요구 사항

### 공통 도구와 SDK 선택

- Windows·macOS·Linux에서 [PowerShell 7](https://learn.microsoft.com/ko-kr/powershell/scripting/install/install-powershell?view=powershell-7.6)을 설치해 `eng/doroti.ps1` 등 저장소 스크립트 실행에 사용합니다. 아래 명령은 **저장소 루트 `DorotiLab`**에서 PowerShell로 시작합니다.
- **iOS 외 플랫폼: .NET SDK 10.0.400** 또는 같은 기능 밴드의 호환 패치 버전. [루트 global.json](../global.json)과 [Doroti/global.json](global.json)이 선택합니다.
- **iOS Testbed: .NET SDK 11.0.100-rc.1.26425.128** 또는 호환 패치 버전. [iOS global.json](../DorotiTestbedApp/ios/global.json)이 선택합니다. .NET 10과 함께 설치합니다.
- 선택한 SDK에 맞는 플랫폼 워크로드와 NuGet 패키지 복원이 필요합니다. .NET 10 경로는 프로젝트가 지정한 10.0.11 런타임 팩을 사용하며, 필요한 팩은 대상별로 복원합니다.

`dotnet`은 **현재 작업 폴더**에서 상위로 `global.json`을 찾습니다. `--project` 경로만 iOS로 지정해도 SDK가 바뀌지는 않습니다. iOS 직접 실행은 `DorotiTestbedApp/ios`에서 하고, 다른 플랫폼은 저장소 루트에서 실행합니다. workspace CLI의 `build/run/publish -App ./DorotiTestbedApp -Platform ios`는 iOS 폴더를 자동으로 사용합니다.

SDK 선택과 대상 프레임워크는 별개입니다. iOS 실기기 Release는 NativeAOT가 기본이며, `-CompilationMode Mono`로 복구 프로필을 선택합니다. NativeAOT는 `net11.0-ios`와 MAUI `11.0.0-rc.1.26451.6`을 사용합니다. 현재 Debug·시뮬레이터·명시적 Mono 프로필은 `net10.0-ios`를 유지합니다. 서명된 실기기 앱은 `publish`로 생성하며, 자세한 절차는 [iOS 샘플 안내](../DorotiTestbedApp/README.ko.md#ios-샘플)를 참고하세요.

### 플랫폼별 구성

선택한 플랫폼 행의 도구를 준비합니다. workload 이름은 설치 항목이며, 네이티브 SDK와 시스템 라이브러리는 별도로 준비해야 합니다.

| 플랫폼 / RID | 빌드 호스트 | .NET SDK / workload | 추가 도구와 실행 조건 |
| --- | --- | --- | --- |
| Windows App SDK (기본) / `win-x64` | Windows x64 | 10 / 별도 MAUI workload 없음 | Visual Studio MSBuild, MSVC **v145** C++ toolset, Windows SDK **10.0.26100.0**. 기본 Vulkan presenter는 Vulkan 1.2 드라이버, D3D12 외부 메모리 공유와 DXGI/DirectComposition 지원이 필요합니다. Acrylic은 Windows 11 24H2 이상을 사용합니다. |
| Windows MAUI (선택) / `win-x64` | Windows x64 | 10 / `maui-windows` | Windows SDK와 MAUI Windows 빌드 도구. workspace CLI에서 `-WindowsBackend Maui`로 선택합니다. |
| macOS AppKit / `osx-arm64` | Apple Silicon Mac | 10 / `macos` | **macOS 14 이상**, workload와 호환되는 전체 Xcode 설치 및 Metal 지원. 앱 소유 Swift/Objective-C binding도 Xcode로 빌드합니다. |
| Mac Catalyst / `maccatalyst-arm64` | Apple Silicon Mac | 10 / `maui-maccatalyst` | workload와 호환되는 전체 Xcode, Mac Catalyst SDK와 Metal 지원. AppKit과 별도 runner입니다. |
| Android / `android-arm64`, `android-x64` | Windows 또는 macOS | 10 / `maui-android` | Android SDK Platforms·Build Tools·Platform Tools(`adb`), **OpenJDK 17–21**. .NET workload가 요구하는 SDK와 native bridge의 **API 34**를 준비합니다. Android 7.0/API 24 이상 기기 또는 해당 ABI 에뮬레이터와 Vulkan 1.2 지원이 필요합니다. |
| iOS / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode | 11 / `maui-ios` | workload와 호환되는 전체 Xcode와 iOS SDK. 시뮬레이터는 해당 Simulator runtime, 실기기는 **iOS 15 이상**, 코드 서명 인증서와 provisioning profile이 필요합니다. Release NativeAOT는 `ios-arm64`용입니다. |
| Linux Qt / `linux-x64` | Linux x64 | 10 / 별도 MAUI workload 없음 | **Qt 6.5 이상** Core/Gui/Widgets/OpenGL/OpenGLWidgets 개발 파일, **CMake 3.24 이상**, C/C++20 compiler, `pkg-config`, Wayland client 개발 파일, `wayland-scanner`, Vulkan 개발 헤더, fontconfig. 실행 시 `wayland` 또는 `xcb` QPA plugin과 Vulkan 1.2 드라이버가 필요합니다. |
| Web / `browser-wasm` | Windows, macOS 또는 Linux | 10 / `wasm-tools` | 기본 렌더러는 하드웨어 WebGL2를 사용합니다. Testbed의 메인 소유 런타임에는 WASM threads와 COOP/COEP 격리도 필요합니다. `worker-direct-webgpu`를 명시적으로 선택하면 하드웨어 WebGPU 어댑터가 필요합니다. |

Windows App SDK 2.4는 target의 NuGet 복원·배포에 포함됩니다. 별도의 machine-wide Windows App Runtime 설치는 요구하지 않습니다. Android native bridge는 저장소의 Gradle wrapper 8.10.2/AGP 8.6.1을 사용합니다. `JAVA_HOME`으로 지원 JDK를 지정하고 `adb`를 `PATH`에 추가합니다. Apple은 `xcode-select -p`와 `xcodebuild -version`으로 선택한 Xcode를 확인합니다.

Linux는 API 조건을 충족하는 llvmpipe 같은 소프트웨어 Vulkan 장치도 허용합니다. Web의 `Microsoft.TypeScript.MSBuild` 7.0.0은 Web runner가 복원하며, 앱 빌드에 Node/npm/Bun 설치는 필요하지 않습니다.

### 설치 상태 확인과 워크로드 복원

```powershell
# 저장소 루트: SDK 10 확인
dotnet --version
dotnet workload list
pwsh -File ./Doroti/eng/doroti.ps1 doctor

# macOS AppKit 예시: 현재 호스트에서 사용할 runner 경로로 변경
dotnet workload restore ./DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj

# iOS: SDK 11을 선택한 위치에서 확인 및 복원
Push-Location ./DorotiTestbedApp/ios
dotnet --version
dotnet workload list
dotnet workload restore ./DorotiTestbedApp.iOS.csproj
Pop-Location
```

`workload restore`는 해당 SDK의 .NET workload를 준비합니다. 표에 있는 Xcode, Android SDK/JDK, MSVC, Qt 같은 외부 도구 설치까지 수행하지는 않습니다. `doctor`는 공통 도구 확인이며, 각 플랫폼의 전체 빌드·기기 실행 검증을 대신하지 않습니다.

플랫폼별 실행 명령은 [Testbed 실행 안내](../DorotiTestbedApp/README.ko.md#material-샘플-모드)에 있습니다. `reference/flutter-master` checkout은 명시적인 Flutter 비교에만 필요하며, 필요할 때 `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`로 준비합니다.

## 빌드와 실행

플랫폼에 필요한 도구를 준비한 뒤 **저장소 루트 `DorotiLab`**에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
```

`run`은 빌드한 뒤 앱을 실행합니다. Windows는 Windows App SDK/`HwndExactCpp`가 기본이며, 독립 MAUI 실행 프로젝트는 `-WindowsBackend Maui`로 선택합니다. 다른 대상은 `-Platform`에 `android`, `ios`, `macos`, `maccatalyst`, `linux`, `web`을 지정합니다. 준비 사항과 기기 옵션은 [샘플 앱 가이드](../DorotiTestbedApp/README.ko.md#플랫폼별-실행)를 참고하세요.

### 빌드 결과 재사용

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform web -Configuration Release
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release -LastSuccessful
```

`-LastSuccessful`과 `-NoBuild`는 입력, 의존성, 도구 버전과 출력 해시가 일치하는 기존 성공 빌드만 재사용합니다. 기록이 없거나 현재 구성과 다르면 이 옵션 없이 다시 실행해야 합니다. `-NoRestore`는 복원만 생략하며 빌드와 의존성 확인은 수행합니다. 이 옵션들은 `run`에만 적용됩니다.

<details>
<summary>빌드 재사용 검증 방식</summary>

CLI는 `doroti.launch-state/v4` 형식으로 실행 프로젝트, 구성, RID, 컴파일 모드, 소스·리소스·네이티브 입력, 상속 설정, 평가된 프로젝트 항목, 복원한 의존성 내용, SDK·워크로드·도구 정보와 출력 해시를 기록합니다. 정적 웹 리소스도 포함하며, iOS 컴파일 모드별 출력과 캐시는 분리합니다.

일반 빌드는 복원 후 의존성 정보를 수집합니다. 의존성이나 도구가 바뀌었거나 이전에 기록되지 않았다면 다시 빌드하고, 오래된 형식의 기록은 재사용하지 않습니다. 컴파일러는 새 프로세스에서 실행하며, 빌드 중 의존성이 바뀌면 성공 결과를 기록하지 않습니다. 사용자 정의 네이티브 타깃은 외부 의존성을 `DorotiLaunchDependency`로 선언합니다.

Qt는 CMake 의존성 검사를 유지하고 파일 크기와 시간이 같더라도 선택한 네이티브 라이브러리를 복사합니다. Windows C++/WinRT 생성은 생성기, SDK·패키지, 헤더 정보를 검사합니다. 이 검사는 빌드 일관성을 확인하며 실제 기기 배포나 실행 검증을 대신하지 않습니다.

</details>

### 명령 안내

| 명령 | 용도 |
| --- | --- |
| `doctor` | 필수 .NET·PowerShell 도구와 선택적 참조 소스 상태 확인 |
| `build` | `Doroti.Product.slnx` 빌드 |
| `build/run/publish -App <path> -Platform <alias>` | `doroti-workspace.json`에서 대상 실행 프로젝트를 찾아 빌드·실행·배포 |
| `native doctor\|build\|open\|add -App <path> -Platform android\|ios\|macos\|maccatalyst` | 기본 네이티브 바인딩 작업 공간 진단, 빌드, 위치 출력, 확장 |
| `validate -ValidationSuite <suite>` | 통합 검증 실행(`Developer`가 기본값) |
| `audit` | 로컬 저장소 정책과 소스 검증 |
| `release` | Release 검증·점검 후 제품 패키지 생성 |
| `clean` | Doroti 빌드 산출물과 임시 상태 제거 |

앱을 지정하지 않은 `build`는 여러 운영체제의 빌드 도구가 필요한 제품 솔루션을 대상으로 합니다. 한 플랫폼만 빌드하려면 `-App`과 `-Platform`을 지정하세요. `release` 명령은 로컬 산출물을 검증하고 패키징하며 GitHub Release를 게시하지 않습니다.

## 앱 개발

### 시작점

앱의 루트 위젯으로 진입점을 생성합니다.

```csharp
new Doroti.Framework.DorotiWidgetEntrypoint(() => new MyApp())
```

`MyApp`은 앱에서 정의한 루트 위젯입니다. 진입점이 루트를 연결하고 첫 프레임을 자동으로 요청하므로 `scheduleFrameCallback`이나 `scheduleForcedFrame`을 직접 호출할 필요가 없습니다. 선택적인 두 번째 `Func<Task>` 인자로 폰트 같은 리소스를 준비한 뒤 루트를 생성할 수 있습니다. 완료 처리는 뷰의 이벤트 루프로 돌아오며, 준비 중 뷰가 분리되거나 종료되면 루트를 연결하지 않습니다.

### 테마

Material은 Material 3를 사용합니다. `ThemeData` 생성자, 팩터리, `copyWith`에서 제거된 `useMaterial3` 인자를 생략하세요. 타이포그래피는 `Typography.Create` 또는 `CreateMaterial2021`을 사용합니다.

시스템 다크 모드를 따르려면 `MaterialApp`에 `theme`, `darkTheme`, `themeMode: ThemeMode.system`을 지정합니다. `ColorScheme.CreateFromSeed`와 `Brightness.light`/`Brightness.dark`로 팔레트를 만들고, 위젯에서는 `Theme.of(context).colorScheme`으로 읽습니다. [전체 테마 예제](../DorotiTestbedApp/README.ko.md#시스템-다크-모드와-색-팔레트)를 참고하세요.

### 네이티브 바인딩

Android, iOS, AppKit macOS, Mac Catalyst 실행 프로젝트는 각각 앱 소유 네이티브 바인딩을 참조합니다. Android는 `AndroidGradleProject`, Apple은 `XcodeProject`를 사용하며, 최종 앱은 .NET 실행 프로젝트에서 만듭니다. `native` 명령으로 해당 작업 공간을 진단하고 빌드하거나 위치를 확인하고 확장할 수 있습니다.

## 플랫폼 설정

### Windows

| 설정 | 동작 |
| --- | --- |
| `DOROTI_WINDOWS_PRESENTER` | 기본값은 `Vulkan` |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | 기본값은 `NoPreference`, `LowPowerPreference`·`HighPerformancePreference`는 Vulkan에 적용 |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | 정확한 장치 이름 또는 유일한 이름 일부로 Vulkan 장치 선택 |

기본 경로는 Graphite/Vulkan으로 렌더링하고 같은 GPU의 D3D12/DXGI DirectComposition으로 화면에 표시합니다. 자동 출력 경로 전환은 없습니다. 네이티브 PlatformView의 래스터 조각은 D3D11 그리기 API를 유지합니다. 동기화와 창 크기 변경에 관한 세부 사항은 [D3D12 출력 보고서](docs/validation/windows-d3d12-output-2026-09-14.md)에 있습니다.

### Web

| 렌더러 | 요구 사항 |
| --- | --- |
| `worker-direct-webgl` (기본) | Ganesh/WebGL2, 독립 Worker 런타임도 지원 |
| `worker-direct-webgpu` (명시적 선택) | Graphite/Dawn, `runtimeLocation: "main"`, WASM threads, COOP/COEP 격리, 하드웨어 WebGPU 어댑터 |

`auto`와 알 수 없는 렌더러 값은 WebGL2를 선택합니다. GPU 초기화 실패 시 자동으로 다른 렌더러로 전환하지 않습니다. 두 경로 모두 표시할 canvas를 한 번 이전합니다. 로더의 `started`는 런타임·GPU 준비 완료를 뜻하며, 첫 콘텐츠가 표시되었다는 의미는 아닙니다.

앱의 부팅 코드는 `web/src/**/*.ts`, 프레임워크 웹 코드는 `src/Doroti.Host.Web/Web/*.ts`에 있습니다. `Microsoft.TypeScript.MSBuild`가 실행 프로젝트의 `obj`에 JavaScript를 만들고, 배포에는 결과 파일을 포함합니다. Node, npm, Bun, 번들러는 필요하지 않습니다. Testbed와 템플릿은 같은 출처의 대체 폰트를 미리 읽고 import map의 `dotnet.js`를 사용합니다. [웹 렌더러 옵션](../DorotiTestbedApp/README.ko.md#web-렌더러와-측정-옵션)을 참고하세요.

### 창 외형

`DorotiViewConfiguration.appearance`에서 배경 효과와 상단바를 독립적으로 선택합니다. 상단바 기본값은 `unified`이며, `solid`는 분리형 상단바입니다. Windows App SDK·Qt에는 Acrylic을, macOS에는 별도 설정으로 Acrylic이나 Liquid Glass를 지정할 수 있습니다. [창 외형 API](docs/window-appearance.md)를 참고하세요.

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
| `Doroti.Host.Qt` | Qt 6 `QWindow`, C ABI 브리지, 입력, IME, 데스크톱 서비스, 접근성 |
| `Doroti.Target.*` | 플랫폼별 패키지 구성과 배포 |

기능 추가와 수정은 해당 기능을 담당하는 프레임워크, 런타임, 렌더러, 호스트 프로젝트에서 직접 수행합니다. 공통 계약이 바뀌면 사용하는 쪽도 함께 수정합니다. Dart-to-C# 컴파일러와 고정 Flutter 소스는 선택적인 소스 가져오기·동작 비교 도구이며, 유지보수 중인 프레임워크 소스를 덮어쓰지 않고 일반 빌드에도 필요하지 않습니다.

## 플랫폼별 검증 범위

빌드, 네이티브 실행, 브라우저 실행, 실제 기기 동작, 접근성, 플랫폼 간 일관성은 각각 별도로 확인합니다. 빌드 성공이 서명, 스토어 배포 준비, 기기 동작, 성능을 보장하지 않습니다. 실행하지 않은 조합은 `notVerified`로 남깁니다.

| 영역 | 범위와 관련 기록 |
| --- | --- |
| Windows | 일부 실제 창 크기 변경·혼합 DPI 조건은 사용자 확인을 받았으며, 자동화된 픽셀·프레임 간격 검사 실패는 별도로 유지합니다. [D3D12 출력 보고서](docs/validation/windows-d3d12-output-2026-09-14.md)의 검증 범위를 참고하세요. |
| Linux | 과거 VMware Wayland/XWayland의 OpenGL 실행 결과가 현재 Vulkan 경로나 실제 하드웨어를 검증하지는 않습니다. [Linux Qt 기록](../history/26-08-20/linux-qt-backend-summary.md)을 참고하세요. IME, Orca, 컨텍스트 복구, 장기 성능은 별도 검증이 필요합니다. |
| AppKit | 네이티브 실행 범위와 남은 조건은 [AppKit 요약](../history/26-08-20/macos-appkit-dual-backend-summary.md)에 기록되어 있습니다. |
| iOS | NativeAOT 빌드·배포·기기 관측 결과는 [NativeAOT 요약](../history/26-09-10/nativeaot-work2-summary.md)에 있으며, 문서에 기록된 구성에 한정됩니다. |
| 네이티브 컨트롤·WebView | [PlatformView 지원표](docs/platform-views/support-matrix.md)에서 구현과 검증 범위를 확인하세요. |
| Texture | [프레임 텍스처](docs/textures.md): CPU RGBA, Android Surface/AHB, Windows D3D11 GPU 입력. Windows 카메라와 AMD/NVIDIA 합성 검증 완료. iOS/macOS Metal·Linux DMA-BUF 카메라 어댑터는 코드 구성 완료, 실행 미검증. |

[개발 이력](../history/)에는 특정 실행의 결과를 보관합니다. 모든 현재 기기와 구성에 대한 보장을 뜻하지 않습니다.

## 개발과 디렉터리 구성

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 프레임워크, 런타임, 렌더링, 호스트, 타깃, SDK |
| [`templates/`](templates/) | `doroti-app` 앱 작업 공간 템플릿 |
| [`eng/`](eng/) | 빌드, 실행, 검증, 패키징, 진단 스크립트 |
| [`tools/`](tools/) | 프레임워크 도구 |
| [`validation/`](validation/) | 검증 소스와 테스트용 자료 |
| [`docs/`](docs/) | API 문서와 범위를 명시한 검증 보고서 |
| [`../tools/Doroti.DartToCSharp/`](../tools/Doroti.DartToCSharp/) | 선택적 Dart·Flutter 소스 변환 컴파일러 |

도구와 검증이 생성한 결과는 `.doroti/`나 `artifacts/`에 두고, 검증 산출물은 `artifacts/validation/`을 사용합니다. 보존할 단계별 기록은 `../history/`에 둡니다. 컴파일러 소유 `.g.cs` 파일은 `src/Doroti.Framework.*`에 포함하여 컴파일하지 않습니다. 저장소 JSON 처리는 `System.Text.Json`을 사용합니다.

## 라이선스

Doroti는 [BSD 3-Clause 라이선스](../LICENSE)를 따릅니다. 외부 소스와 패키지의 저작권 표기는 [서드파티 고지](THIRD-PARTY-NOTICES.md)를 참고하세요.

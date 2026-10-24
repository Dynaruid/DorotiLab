# Doroti Custom Carousel Sample

[gskinnerTeam/flutter_custom_carousel](https://github.com/gskinnerTeam/flutter_custom_carousel)의 예제 앱을 Doroti C#으로 옮긴 독립 예제 앱입니다.
재사용 캐러셀은 [packages/Doroti.CustomCarousel](../../packages/Doroti.CustomCarousel/README.md),
화면 코드는 [src/App.cs](src/App.cs)에 있습니다.

## 사용법

홈의 미리보기를 세로로 드래그하거나 ↑/↓ 또는 하단 선택 표시로 선택한 뒤 **Open**을 누릅니다.
밝은 오프화이트 배경과 큰 실제 이미지 미리보기, 간결한 타이포그래피로 구성하며 작은 화면에서는 제목과 조작부를 조정합니다.
각 데모에서 터치·마우스 드래그와 해당 축의 스크롤 입력을 사용합니다.
가로 데모는 일반 세로 휠, 수평 wheel/trackpad 입력 또는 Shift+wheel을 사용할 수 있습니다.

| 데모 | 동작 |
| --- | --- |
| Cover Slider | 세 개의 가로 루프 목록. 사진을 누르면 상세 보기 |
| Circular Menu | 원형 배치, 거리별 scale/blur. 선택된 토핑을 눌러 버거에서 추가·제거 |
| Card Deck | 비루프 카드 덱, sticky snapping, Previous/Next와 Shuffle |
| Digital Wallet | 역방향 가로 입력과 카드 겹침·fade/blur, 선택 계좌 잔액 표시 |
| Record Box | 세로 루프, 깊이 방향으로 이어지는 9장 앨범과 원근 기울기. 드러난 앨범 가장자리를 눌러 선택 |

## 실행

저장소 루트 `DorotiLab`에서 실행합니다.
[global.json](../../global.json)에 지정된 .NET SDK와 대상 플랫폼의 빌드 도구가 필요합니다.
WebAssembly 빌드에는 `wasm-tools` 워크로드가 필요합니다.

```powershell
# Windows App SDK
pwsh -File Doroti/eng/doroti.ps1 run --app samples/DorotiCarouselApp --platform windows

# Web 개발 서버
dotnet run --project samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Debug
```

### Web Release 게시

Web Release publish는 저장소 기본 WASM AOT 설정을 사용합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Release -o temp/testing/carousel-web
python Doroti/eng/serve-isolated-web.py temp/testing/carousel-web/wwwroot --port 5226
```

브라우저에서 `http://127.0.0.1:5226`을 엽니다. WASM threads에 필요한 COOP/COEP 헤더를 제공하는 서버입니다.

워크스페이스는 `windows`, `web`, `android`, `ios`, `linux`, `macos`, `maccatalyst`를 선언합니다.
다른 플랫폼은 해당 호스트와 SDK에서 같은 CLI의 `--platform` 값을 바꿔 실행합니다.
예: `pwsh -File Doroti/eng/doroti.ps1 run --app samples/DorotiCarouselApp --platform android`.

## 플랫폼별 빌드와 옵션

아래는 **저장소 루트의 PowerShell**에서 실행하는 직접 .NET 명령입니다. 대상 OS와 빌드 케이스에 맞는 명령을 선택하세요. `build`는 컴파일, `publish`는 배포 산출물 생성입니다. 기기 설치·실행은 별도이며 공용 앱 `.csproj` 대신 플랫폼 러너를 지정합니다.

[플랫폼 도구 준비](../../Doroti/README.ko.md)를 먼저 확인하세요. 루트 `global.json`은 .NET 10을 선택합니다. .NET 11 예제는 아래 지정 폴더에서 SDK를 선택합니다. 프로젝트 경로만 바꿔도 SDK가 바뀌지는 않습니다. 런타임·난독화 같은 MSBuild 옵션은 아래 `dotnet` 명령에 전달합니다.

### 기본 빌드와 Release 기본값

| 플랫폼 | 빌드 호스트 / RID | Debug 빌드 | 직접 빌드의 Release 기본값 |
| --- | --- | --- | --- |
| Windows App SDK | Windows / `win-x64` | `dotnet build ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Debug` | CoreCLR JIT |
| Android | Windows, macOS / `android-arm64`, `android-x64` | `dotnet build ./samples/DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Debug -r android-arm64` | Mono profiled AOT (ARM64) |
| iOS | macOS + Xcode / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | `dotnet build ./samples/DorotiCarouselApp/ios/DorotiCarouselApp.iOS.csproj -c Debug -r iossimulator-arm64 -p:DorotiCompilationMode=Mono -p:PublishAot=false` | Mono / .NET 10 / `iossimulator-arm64` |
| Linux Qt | Linux x64 + Qt / `linux-x64` | `dotnet build ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Debug -r linux-x64` | CoreCLR JIT |
| macOS AppKit | Apple Silicon Mac + Xcode / `osx-arm64` | `dotnet build ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacOS.csproj -c Debug -r osx-arm64` | CoreCLR JIT |
| Mac Catalyst | Apple Silicon Mac + Xcode / `maccatalyst-arm64` | `dotnet build ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacCatalyst.csproj -c Debug -r maccatalyst-arm64` | Mono AOT |
| Web | Windows, macOS, Linux + `wasm-tools` / `browser-wasm` | `dotnet build ./samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Debug` | Mono WASM AOT (`publish`) |

이 샘플에는 Windows MAUI 러너가 없습니다. Windows는 Windows App SDK를 사용합니다.

Android x64 에뮬레이터는 `-r android-x64`, Intel Mac의 iOS 시뮬레이터는 `-r iossimulator-x64`로 바꿉니다. iOS Release NativeAOT에는 아래 별도의 .NET 11 **publish** 절차를 사용하세요.

### 데스크톱 단일 실행 파일 게시

Windows App SDK는 `PublishSingleFile=true`를 지원합니다. Linux Qt 6.12 번들은
교체 가능한 공유 라이브러리를 포함하는 폴더 게시를 사용합니다. 다음 명령 전에
Qt SDK와 재배포 inventory를 구성하세요.

```powershell
# Windows: single EXE, .NET/Windows App SDK self-contained
dotnet publish ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishSingleFile=true -o ./temp/publish/carousel-windows
# Linux: Qt publication requires a reviewed Qt 6.12 SDK runtime; use directory output
dotnet publish ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release --self-contained true -p:DorotiBuildQtNative=true -o ./temp/publish/carousel-linux
```

단일 실행 파일은 실행 시 DLL과 콘텐츠를 추출합니다. ReadyToRun/난독화 옵션도 함께
사용할 수 있으며, 트리밍·NativeAOT는 지원하지 않습니다. macOS AppKit·Mac Catalyst는
현재 .NET Apple SDK의 제한으로 `.app` 게시를 사용합니다.

### 데스크톱 JIT, ReadyToRun, 난독화

대상 플랫폼에서 원하는 게시 케이스 하나를 실행합니다. 각 러너에 선언된 RID를 사용합니다.

#### Windows App SDK

```powershell
# Release JIT
dotnet publish ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=false
# Release JIT + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=true
# Release ReadyToRun
dotnet publish ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=false
# Release ReadyToRun + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=true
```

#### Linux Qt

```powershell
# Release JIT
dotnet publish ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=false
# Release JIT + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=true
# Release ReadyToRun
dotnet publish ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=false
# Release ReadyToRun + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=true
```

#### macOS AppKit

```powershell
# Release JIT
dotnet publish ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacOS.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=false
# Release JIT + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacOS.csproj -c Release -p:PublishReadyToRun=false -p:DorotiObfuscate=true
# Release ReadyToRun
dotnet publish ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacOS.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=false
# Release ReadyToRun + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacOS.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=true
```

Mac Catalyst의 Release Mono AOT는 `dotnet build ./samples/DorotiCarouselApp/macos/DorotiCarouselApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64`로 빌드합니다. 이 Mono 경로는 Obfuscar 적용 대상에서 제외합니다. 이 저장소의 NativeAOT 프로필은 iOS 실기기용이므로 위 데스크톱 러너에 `PublishAot=true`를 추가하지 않습니다.

### Android Mono, JIT, AOT, CoreCLR

.NET 10 기본 ARM64 Release는 Mono profiled AOT입니다. 아래 JIT/R2R 케이스는 Mono AOT와 profiled AOT를 **모두** 끕니다. `DorotiCompilationMode=Mono`는 Android의 공용 프로필 이름이며, 실제 런타임은 `UseMonoRuntime`으로 선택합니다.

```powershell
# .NET 10 / Mono profiled AOT: Obfuscar is skipped
dotnet build ./samples/DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Release -r android-arm64 `
    -p:UseMonoRuntime=true -p:RunAOTCompilation=true `
    -p:AndroidEnableProfiledAot=true -p:PublishReadyToRun=false `
    -p:AndroidPackageFormats=apk

# .NET 10 / Mono JIT + Obfuscar
dotnet build ./samples/DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Release -r android-arm64 `
    -p:DorotiCompilationMode=Mono -p:PublishAot=false `
    -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false `
    -p:AndroidPackageFormats=apk -p:UseMonoRuntime=true `
    -p:PublishReadyToRun=false -p:DorotiObfuscate=true
# .NET 10 / experimental CoreCLR JIT + Obfuscar
dotnet build ./samples/DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Release -r android-arm64 `
    -p:DorotiCompilationMode=Mono -p:PublishAot=false `
    -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false `
    -p:AndroidPackageFormats=apk -p:UseMonoRuntime=false `
    -p:PublishReadyToRun=false -p:DorotiObfuscate=true
```

.NET 11 CoreCLR JIT/R2R는 해당 SDK의 Android/MAUI 워크로드를 설치한 뒤 사용합니다. 모든 샘플에서 저장소의 공용 SDK 선택 폴더를 사용하며, 아래 프로젝트 경로는 이동한 폴더 기준입니다. SDK 선택과 원래 폴더 복귀를 위해 `Push-Location`/`try`/`finally` 블록 전체를 복사합니다. 아래 두 빌드 중 필요한 케이스를 선택하고, 난독화를 끄려면 `DorotiObfuscate=false`로 바꿉니다.

```powershell
Push-Location ./samples/DorotiSampleApp2/android/sdk/net11
try {
    dotnet --version
    # .NET 11 / CoreCLR JIT + Obfuscar
    dotnet build ../../../../DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Release -r android-arm64 `
        -p:DorotiCompilationMode=Mono -p:PublishAot=false `
        -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false `
        -p:AndroidPackageFormats=apk -p:DorotiAndroidTargetFramework=net11.0-android `
        -p:DorotiAndroidMauiVersion=11.0.0-rc.2.26505.5 -p:MauiVersion=11.0.0-rc.2.26505.5 `
        -p:UseMonoRuntime=false -p:PublishReadyToRun=false `
        -p:DorotiObfuscate=true
    # .NET 11 / CoreCLR ReadyToRun + Obfuscar
    dotnet build ../../../../DorotiCarouselApp/android/DorotiCarouselApp.Android.csproj -c Release -r android-arm64 `
        -p:DorotiCompilationMode=Mono -p:PublishAot=false `
        -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false `
        -p:AndroidPackageFormats=apk -p:DorotiAndroidTargetFramework=net11.0-android `
        -p:DorotiAndroidMauiVersion=11.0.0-rc.2.26505.5 -p:MauiVersion=11.0.0-rc.2.26505.5 `
        -p:UseMonoRuntime=false -p:PublishReadyToRun=true `
        -p:DorotiObfuscate=true
} finally { Pop-Location }
```

기기 선택·설치는 [공용 배포 도구](../../helpers/deploy-helper/README.md)를 참고하세요. 이 도구의 `-App`은 `Testbed`와 `Sample2`만 지원하므로 Carousel은 직접 .NET 빌드와 기기 설치를 사용합니다.

### iOS 시뮬레이터, Mono 실기기, NativeAOT

Mono 예제는 루트의 .NET 10 SDK에서 실행합니다. 시뮬레이터 Debug/Release와 실기기 Mono Release는 난독화에서 제외합니다. 실기기는 로컬 개발 인증서·프로비저닝 프로필 값으로 placeholder를 바꿉니다.

```powershell
# .NET 10 / Mono simulator Debug
dotnet build ./samples/DorotiCarouselApp/ios/DorotiCarouselApp.iOS.csproj -c Debug -r iossimulator-arm64 -p:DorotiCompilationMode=Mono -p:PublishAot=false -p:DorotiIosTargetFramework=net10.0-ios
# .NET 10 / Mono simulator Release
dotnet build ./samples/DorotiCarouselApp/ios/DorotiCarouselApp.iOS.csproj -c Release -r iossimulator-arm64 -p:DorotiCompilationMode=Mono -p:PublishAot=false -p:DorotiIosTargetFramework=net10.0-ios
# .NET 10 / Mono physical device Release
dotnet build ./samples/DorotiCarouselApp/ios/DorotiCarouselApp.iOS.csproj -c Release -r ios-arm64 -p:DorotiCompilationMode=Mono -p:PublishAot=false -p:DorotiIosTargetFramework=net10.0-ios '-p:CodesignKey=YOUR_CERTIFICATE_SHA1' '-p:CodesignProvision=YOUR_PROFILE_UUID'
```

.NET 11 NativeAOT는 아래처럼 SDK 선택 폴더로 이동하여 **publish**합니다. `build`나 `run`으로 대체하지 않습니다. NativeAOT에는 Obfuscar를 적용하지 않습니다. 호환되는 Xcode를 선택하고, Xcode 버전 검사 우회나 .NET 10 NativeAOT가 필요한 경우 [SDK별 상세 설정](../DorotiSampleApp2/docs/ios-nativeaot.md)을 참고하세요.

```powershell
Push-Location ./samples/DorotiTestbedApp/ios
try {
    dotnet --version
    dotnet publish ../../DorotiCarouselApp/ios/DorotiCarouselApp.iOS.csproj -c Release -r ios-arm64 `
        -p:DorotiCompilationMode=NativeAot -p:DorotiIosTargetFramework=net11.0-ios `
        -p:DorotiIosMauiVersion=11.0.0-rc.2.26505.5 -p:Registrar=managed-static `
        -p:_UseDynamicDependenciesForMarkNSObjects=false `
        '-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true' `
        -p:PrepareAssemblies=false -p:PostProcessAssemblies=false `
        --artifacts-path ../../DorotiCarouselApp/.doroti/publish/ios-nativeaot-net11 `
        '-p:CodesignKey=YOUR_CERTIFICATE_SHA1' '-p:CodesignProvision=YOUR_PROFILE_UUID'
} finally { Pop-Location }
```

Testbed의 직접 iOS Release 기본값은 NativeAOT이지만, SampleApp2와 Carousel은 Mono가 기본입니다. 위 예제는 이 차이에 의존하지 않도록 모드·TFM을 명시합니다. 현재 iOS CoreCLR은 개발 세션용이며 일반 Release JIT/R2R 게시 케이스로 안내하지 않습니다.

### Web AOT, 비 AOT, 난독화, HTTPS 공유

아래 게시 명령 중 하나를 선택합니다. `dotnet run -c Release`는 WASM AOT 게시를 대신하지 않습니다. 게시물을 제공할 때는 WASM threads에 필요한 COOP/COEP 헤더를 설정하는 서버를 사용합니다.

```powershell
# Release WASM AOT: Obfuscar is skipped
dotnet publish ./samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Release -p:RunAOTCompilation=true -o ./samples/DorotiCarouselApp/.doroti/publish/web
# Release without WASM AOT or obfuscation
dotnet publish ./samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Release -p:RunAOTCompilation=false -p:DorotiObfuscate=false -o ./samples/DorotiCarouselApp/.doroti/publish/web
# Release without WASM AOT + Obfuscar
dotnet publish ./samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Release -p:RunAOTCompilation=false -p:DorotiObfuscate=true -o ./samples/DorotiCarouselApp/.doroti/publish/web
python Doroti/eng/serve-isolated-web.py ./samples/DorotiCarouselApp/.doroti/publish/web/wwwroot --port 5226
```

브라우저에서 `http://127.0.0.1:5226`을 엽니다. Docker Desktop이 준비되어 있으면 [Quick Tunnel](../../tools/doroti-cloudflared/README.md)로 HTTPS 공유도 가능합니다.

```powershell
# Default Release WASM AOT
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start -App ./samples/DorotiCarouselApp
# Non-AOT Release
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start -App ./samples/DorotiCarouselApp -DisableAot
# Non-AOT Release + Obfuscar
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start -App ./samples/DorotiCarouselApp -DisableAot -Obfuscate
```

`-DisableAot`·`-Obfuscate`는 `-SkipPublish`와 함께 사용할 수 없습니다.

### 옵션 조합과 난독화 범위

| 케이스 | Obfuscar |
| --- | --- |
| Debug / Hot Reload | 항상 제외 |
| Release JIT / 비 AOT Web | `DorotiObfuscate=true`일 때 적용 |
| Release ReadyToRun | IL 난독화 후 R2R 컴파일 |
| Mono AOT / profiled AOT / WASM AOT / iOS NativeAOT / Mac Catalyst Mono | 제외 |

난독화의 기본값은 꺼짐이며, 공용 앱 DLL의 내부 이름만 변경하고 공개 API는 유지합니다. `-p:DorotiObfuscarHideStrings=true`로 문자열 숨김을 추가하고 `-p:DorotiObfuscarRules=obfuscar-rules.xml`로 공용 앱 폴더 기준 예외 파일을 선택할 수 있습니다. 난독화 게시에 `--no-build`는 사용할 수 없습니다.

서로 다른 SDK·RID·런타임의 산출물을 따로 보관하려면 명령에 `--artifacts-path ./Doroti/artifacts/<고유-케이스-이름>`을 추가합니다. `mapping.xml`과 원본 PDB는 내부 분석 자료로 보관하고 배포하지 않습니다.

## 소스와 라이선스

원본 사진과 카드 이미지는 `assets/images/`에 있으며 공통 앱 DLL에 내장됩니다.
탐색·셔플 버튼은 [Doroti.Icons.Material](../../packages/Doroti.Icons.Material/README.md)의 아이콘을 사용합니다.
`Doroti.Material`이 아이콘 패키지를 참조하며, 폰트는 각 View에서 처음 사용할 때 자동 등록됩니다.
Release 빌드·게시에서는 사용한 글리프만 남기고, 아이콘 패키지의 라이선스도 출력에 포함합니다.
원본 BSD 3-Clause 라이선스는
[assets/LICENSE.flutter_custom_carousel](assets/LICENSE.flutter_custom_carousel)에 있습니다.
앱과 각 참조 라이브러리가 등록한 고지는 빌드·게시 출력의 `licenses/`에 포함됩니다.
Qt 고지는 Linux Qt 러너에서만 추가합니다. 단일 파일 게시도 실행 파일과 `licenses/`를
함께 배포합니다.
애니메이션 효과는 Doroti의 Transform/Opacity/ImageFiltered 조합을 사용합니다.
원본 shimmer/tint, Hero 전환과 지갑 거래 목록은 이 예제에 포함하지 않습니다.

Linux runtime publication uses an explicit Qt 6.12.0 Framework SDK and independent
WebEngine 6.140.0 SDK for this WebView-enabled sample. Set the SDK prefixes and
provide reviewed source/notice inventories as described in the
[Qt runtime guide](../../packages/platforms/qt/qt-runtime.md). WebEngine is bundled
by default. A separate engine/app `.deb` pair can be installed together with APT
without operating an engine repository. Single-file Qt publication is explicitly
unsupported by this bundle profile.

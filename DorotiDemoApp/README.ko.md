# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 플랫폼 workspace 계약을 직접 사용하는 dogfood 앱입니다. 루트 `DorotiDemoApp.csproj`는 플랫폼 중립 `net10.0` 라이브러리이고, 여섯 소문자 플랫폼 폴더가 각각 독립적으로 build/run 가능한 runner 프로젝트를 소유합니다.

## 구조

- `Program.cs`, `src/`, `assets/`: 공용 startup, widget tree, 앱 asset
- `doroti-workspace.json`: `android`, `ios`, `linux`, `macos`, `web`, `windows` alias와 runner 경로
- `windows/`: WinUI/MAUI runner와 package identity
- `web/`: Blazor WebAssembly runner, TypeScript source, `wwwroot`
- `android/`: .NET Android/MAUI runner와 기본 Gradle AAR/.NET binding
- `ios/`: .NET iOS/MAUI runner와 독립 Xcode framework/binding
- `macos/`: Mac Catalyst runner와 독립 Xcode framework/binding이며 AppKit macOS를 뜻하지 않음
- `linux/`: managed runner와 앱 소유 CMake/Qt 6 C ABI shim

생성 bootstrap과 plugin registration은 각 runner의 `obj/<rid>/Doroti.Generated` 아래에만 만들어집니다. 플랫폼 icon, splash, manifest, entitlement, native source, output, lock file은 해당 플랫폼 폴더가 소유합니다.

## 명령

저장소 루트에서 실행합니다. workspace CLI는 `doroti-workspace.json`에서 runner를 선택합니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 doctor -App ./DorotiDemoApp -Platform all
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 publish -App ./DorotiDemoApp -Platform web
```

### 플랫폼별 실행

아래 명령은 모두 저장소 루트에서 실행합니다.

```powershell
# Windows (Windows 호스트)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Web
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web

# Android x64 에뮬레이터
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform android -Rid android-x64

# iOS x64 시뮬레이터 (macOS + Xcode)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform ios -Rid iossimulator-x64

# Mac Catalyst arm64 (Apple Silicon macOS + Xcode)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform macos -Rid maccatalyst-arm64

# Linux x64 (Qt 6 + CMake)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform linux -Rid linux-x64
```

Linux 데모는 `WindowBackdropMode.acrylic`을 요청합니다. Wayland compositor가
`ext-background-effect-v1` 또는 구형 KDE blur 프로토콜을 제공하면 native blur를
사용하고, 제공하지 않으면 투명 배경으로 폴백합니다. `backgroundColor`와
`darkBackgroundColor`의 alpha가 아크릴 tint 강도를 결정합니다.

Web runner가 시작되면 브라우저에서 `http://127.0.0.1:5088`을 엽니다. Android와 iOS는 각각 실행 중인 에뮬레이터 또는 시뮬레이터가 필요합니다. Android arm64 기기와 iOS arm64 기기에서 실행하려면 각각 `-Rid android-arm64`, `-Rid ios-arm64`로 바꾸며, iOS 실제 기기는 별도의 코드 서명 설정이 필요합니다.

runner를 직접 지정해도 됩니다.

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release
dotnet run --project ./DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj
dotnet run --project ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj
dotnet build ./DorotiDemoApp/android/DorotiDemoApp.Android.csproj -c Release -r android-x64
dotnet build ./DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj -c Release -r iossimulator-x64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacCatalyst.csproj -c Release
dotnet publish ./DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj -c Release -r linux-x64
```

예전 `dotnet run --project DorotiDemoApp.csproj -p:DorotiTarget=...` 경로는 `DOROTIAPP100`으로 실패하면서 새 runner 경로를 안내합니다.

## 기본 native bridge

새 앱에는 Android, iOS, Mac Catalyst native library와 binding project가 기본으로 포함됩니다. Native library는 최종 앱 runner를 대체하지 않습니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiDemoApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiDemoApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiDemoApp -Platform ios
```

`native open`은 Android Studio/Xcode project 경로만 출력하며 실제 IDE 실행은 `-Launch`를 명시했을 때만 합니다. 기본 ABI는 `platformInfo`, `echo`, UI-thread callback을 제공합니다.

## 지원과 evidence 상태

플랫폼 중립 앱, Windows, Web, Android arm64/x64, Mac Catalyst 교차 빌드, iOS device/simulator 교차 빌드, Linux managed runner의 자동 Release 빌드를 검증합니다. Android Gradle 및 iOS Xcode binding scaffold도 현재 교차 빌드 환경에서 컴파일합니다.

이 결과는 native launch, GPU 표시, browser interaction, physical device, accessibility, signing/store, Linux X11/Wayland 동작을 증명하지 않습니다. 실행하지 않은 gate는 workspace evidence에서 `notVerified`로 유지하며, Mac Catalyst 결과를 native AppKit macOS 결과로 바꾸지 않습니다.

[ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md)과 [workspace evidence](../Doroti/validation/evidence/app-targets-evidence.json)를 참고하세요.

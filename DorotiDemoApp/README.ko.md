# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 플랫폼 workspace 계약을 직접 사용하는 dogfood 앱입니다. 루트 project는 플랫폼 중립이며, `macos`와 `maccatalyst`를 별도 정식 제품으로 둔 7개 runner alias가 있습니다.

## 빠르게 실행하기

.NET 10 SDK와 PowerShell 7을 설치한 뒤 저장소 루트에서 다음 명령을 실행합니다. 첫 실행은 필요한 package를 복원하고 runner를 빌드하므로 시간이 걸릴 수 있습니다.

```powershell
# Windows 기본 backend로 실행
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Web으로 실행
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web
```

Web runner가 시작되면 브라우저에서 `http://127.0.0.1:5088`을 엽니다. renderer backend를 직접 비교하려면 다음 주소를 사용할 수 있습니다.

- 기본 자동 선택: `http://127.0.0.1:5088`
- document WebGL2: `http://127.0.0.1:5088/?dorotiRenderer=document-webgl`
- 같은 thread OffscreenCanvas: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-bitmap`
- persistent .NET Worker: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-worker`

실행을 종료하려면 명령을 실행한 terminal에서 `Ctrl+C`를 누릅니다. Android, iOS, macOS, Linux와 Windows MAUI 실행 명령은 아래의 [플랫폼별 실행](#플랫폼별-실행)을 참고하세요.

## 구조

- `Program.cs`, `src/`, `assets/`: 공용 startup, widget tree, 앱 asset
- `doroti-workspace.json`: `macos`(AppKit)와 `maccatalyst`(UIKit)를 구분하는 runner 경로
- `windowsappsdk/`: Windows App SDK 2.4 `HwndExactCpp` child-HWND runner와 managed ANGLE/EGL-D3D11 Skia presentation 경로입니다.
- `windows/`: 정식 MAUI backend runner와 package identity
- `web/`: Blazor WebAssembly runner, TypeScript source, `wwwroot`
- `android/`: .NET Android/MAUI runner와 기본 Gradle AAR/.NET binding
- `ios/`: .NET iOS/MAUI runner와 독립 Xcode framework/binding
- `macos/`: native AppKit/osx-arm64와 Mac Catalyst/maccatalyst-arm64 runner, binding, manifest, lock file
- `linux/`: managed runner와 앱 소유 CMake/Qt 6 C ABI shim

생성 bootstrap과 plugin registration은 각 runner의 `obj/<rid>/Doroti.Generated` 아래에만 만들어집니다. 플랫폼 icon, splash, manifest, entitlement, native source, output, lock file은 해당 플랫폼 폴더가 소유합니다.

## 명령

저장소 루트에서 실행합니다. workspace CLI는 `doroti-workspace.json`에서 runner를 선택합니다.
`build`, `run`, `publish`는 기본적으로 Release 구성을 사용하며, 디버깅이 필요할 때만 `-Configuration Debug`를 명시합니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 doctor -App ./DorotiDemoApp -Platform all
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 publish -App ./DorotiDemoApp -Platform web
```

### 플랫폼별 실행

아래 명령은 모두 저장소 루트에서 실행합니다. Linux runner 명령은 native shim을 함께 빌드하므로 Linux x64 호스트에서 실행해야 합니다.

```powershell
# Windows App SDK HwndExactCpp backend (현재 기본값)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# 독립 Windows MAUI backend
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -WindowsBackend Maui

# Web
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web

# Android x64 에뮬레이터
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform android -Rid android-x64

# iOS arm64 시뮬레이터 (Apple Silicon macOS + Xcode)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform ios -Rid iossimulator-arm64

# Native AppKit arm64 (실험 backend, macOS 14+, Apple Silicon)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform macos -Rid osx-arm64

# Mac Catalyst arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform maccatalyst -Rid maccatalyst-arm64

# Linux x64 (Qt 6 + CMake)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform linux -Rid linux-x64
```

### Android 실기기와 에뮬레이터 연결

Android SDK Platform Tools의 `adb`가 필요합니다. Android Studio를 설치했다면 일반적으로 `%LOCALAPPDATA%\Android\Sdk\platform-tools`에 있습니다. `adb`가 `PATH`에 없으면 아래처럼 경로를 지정할 수 있습니다.

```powershell
$adb = "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
& $adb version
& $adb devices -l
```

목록의 두 번째 열이 `device`이면 실행할 준비가 된 상태입니다. 여러 기기나 에뮬레이터가 연결되어 있으면 첫 번째 열의 serial을 `-Device`에 전달합니다. 연결 대상이 하나뿐이면 `-Device`를 생략해도 됩니다.

#### USB 실기기

1. Android 설정에서 **휴대전화 정보 > 소프트웨어 정보 > 빌드 번호**를 여러 번 눌러 개발자 옵션을 활성화합니다.
2. **개발자 옵션 > USB 디버깅**을 켜고 PC에 USB로 연결합니다.
3. 기기에 표시되는 RSA 디버깅 허용 창을 승인한 뒤 `adb devices -l`로 serial을 확인합니다.
4. 일반적인 arm64 기기는 `android-arm64` RID로 실행합니다.

```powershell
# ABI 확인
& $adb -s <device-serial> shell getprop ro.product.cpu.abi

# Release + arm64 AOT 빌드, 설치, 실행
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-arm64 `
  -Configuration Release `
  -Device <device-serial>

# 빠른 개발용 Debug 실행
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-arm64 `
  -Configuration Debug `
  -Device <device-serial>
```

`<device-serial>`은 꺾쇠까지 포함한 문자열이 아니라 `adb devices -l`에 나온 값으로 바꿉니다. 예를 들어 serial이 `R3CY30KZA4B`이면 `-Device R3CY30KZA4B`로 지정합니다.

Android 11 이상에서는 같은 네트워크에서 무선 디버깅도 사용할 수 있습니다. 기기의 **개발자 옵션 > 무선 디버깅 > 페어링 코드로 기기 페어링** 화면에 나온 주소와 포트를 사용합니다. 페어링 포트와 연결 포트는 서로 다를 수 있습니다.

```powershell
& $adb pair <device-ip>:<pairing-port>
& $adb connect <device-ip>:<debug-port>
& $adb devices -l
```

#### Android 에뮬레이터

Android Studio의 **Device Manager**에서 x86_64 시스템 이미지로 가상 기기를 만들고 먼저 시작합니다. 부팅이 끝나면 보통 `emulator-5554`와 같은 serial로 표시됩니다.

```powershell
& $adb devices -l

pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-x64 `
  -Device emulator-5554
```

현재 `android-x64` 에뮬레이터 Release는 Mono AOT 시작 문제를 피하기 위해 JIT/인터프리터 호환 경로를 사용합니다. 실제 arm64 Release AOT 동작은 `android-arm64` 실기기에서 확인합니다.

#### 연결 문제 해결

- `unauthorized`: 기기 잠금을 해제하고 RSA 허용 창을 승인한 뒤 USB를 다시 연결합니다.
- `offline`: `& $adb kill-server`와 `& $adb start-server`를 실행하고 기기 또는 에뮬레이터를 다시 연결합니다.
- 목록에 없음: USB 연결 모드, USB 디버깅, 케이블의 데이터 지원 여부와 Windows 제조사 USB 드라이버를 확인합니다.
- 대상이 둘 이상이라는 오류: `adb devices -l`의 정확한 serial을 `-Device`에 지정합니다.
- 기존 설치와 서명이 충돌함: 필요한 앱 데이터가 없는지 확인한 다음 `& $adb -s <device-serial> uninstall dev.doroti.demo`로 기존 앱을 제거하고 다시 실행합니다.

Web runner가 시작되면 브라우저에서 `http://127.0.0.1:5088`을 엽니다. Android와 iOS는 각각 실행 중인 에뮬레이터 또는 시뮬레이터가 필요합니다. Android arm64 기기와 iOS arm64 기기에서 실행하려면 각각 `-Rid android-arm64`, `-Rid ios-arm64`로 바꾸며, iOS 실제 기기는 별도의 코드 서명 설정이 필요합니다.

Linux에는 .NET SDK와 PowerShell 7 외에 Qt 6.5 이상 Core/Gui/Widgets/OpenGL, CMake, C++ compiler, `pkg-config`, Wayland client 개발 파일, `wayland-scanner`, `wayland` 또는 `xcb` QPA plugin이 필요합니다.

runner를 직접 지정해도 됩니다.

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release
dotnet run --project ./DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Release
dotnet run --project ./DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj # MAUI backend
dotnet run --project ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj
dotnet build ./DorotiDemoApp/android/DorotiDemoApp.Android.csproj -c Release -r android-x64
dotnet build ./DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj -c Release -r iossimulator-arm64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacOS.csproj -c Release -r osx-arm64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64
dotnet publish ./DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj -c Release -r linux-x64
```

예전 `dotnet run --project DorotiDemoApp.csproj -p:DorotiTarget=...` 경로는 `DOROTIAPP100`으로 실패하면서 새 runner 경로를 안내합니다.

## 기본 native bridge

새 앱에는 Android, iOS, native AppKit macOS, Mac Catalyst native library와 binding project가 기본으로 포함됩니다. Native library는 최종 앱 runner를 대체하지 않습니다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiDemoApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiDemoApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiDemoApp -Platform ios
```

`native open`은 Android Studio/Xcode project 경로만 출력하며 실제 IDE 실행은 `-Launch`를 명시했을 때만 합니다. 기본 ABI는 `platformInfo`, `echo`, UI-thread callback을 제공합니다.

## 시스템 다크 모드와 색 팔레트

데모는 `MaterialApp`에 light/dark `ThemeData`와 `ThemeMode.system`을 전달합니다. 두 palette는 `ColorScheme.CreateFromSeed`로 만들며 현재 widget은 `Theme.of(context).colorScheme`의 role을 사용합니다. Window configuration의 `backgroundColor`와 `darkBackgroundColor`도 같은 light/dark 전환을 따릅니다.

Linux 데모는 `WindowBackdropMode.acrylic`과 `WindowBackdropFallback.transparent`를 요청합니다. Wayland compositor가 `ext-background-effect-v1` 또는 구형 KDE blur protocol을 제공하면 전체 client surface에 native blur를 요청하고, 제공하지 않으면 설정한 transparent fallback을 사용합니다. 두 background color의 alpha는 acrylic tint 강도를 결정합니다. 이 protocol 경로는 구현되어 있지만 실제 compositor blur의 시각 acceptance는 아직 `notVerified`입니다.

## 지원과 evidence 상태

Windows 기본 경로는 self-contained Windows App SDK 2.4 `HwndExactCpp` child-HWND host입니다. Native C++은 top-level/child/task HWND와 input/lifecycle ingress를, managed code는 Doroti scene 생성과 hardware-D3D11 ANGLE/EGL/Skia raster/presentation을 소유합니다. Exact-size GPU backing을 `EGL_FIXED_SIZE_ANGLE` window surface로 blit하며 새 surface는 첫 swap 뒤 initial show 또는 resize 완료 전에 `DwmFlush`합니다. 8 ms refresh는 interactive move가 두 monitor에 걸쳐 있고 pending/in-flight render가 없을 때만 동작합니다.

확인한 Windows 실제 resize와 mixed-DPI 경계 동작은 사용자 acceptance를 받았습니다. Strict synthetic input qualification과 pixel/cadence FAIL은 acceptance로 재분류하지 않고 유지합니다. 자동 input은 PASS했고 IME/UIA 및 lifecycle/device는 automated partial 범위입니다. 실제 한글 IME 후보창/caret, Narrator/Accessibility Insights, 미실행 edge/speed/DPI/monitor 조합, device removal, installer/MSIX, 각 wait 지점 shutdown은 `notVerified`입니다. 명시적 MAUI backend는 별도 evidence 경계를 가지며 silent fallback으로 사용하지 않습니다. 마지막 Windows 전체 product solution Release 실행은 Windows target 통과 뒤 macOS project가 없는 `sips`를 호출해 실패했으므로 Windows 작업에는 위 target-scoped 명령을 사용합니다.

native AppKit과 Mac Catalyst를 서로 독립적으로 포함한 자동 build를 검증합니다. AppKit backend는 실험적이며 Microsoft 지원 대상이 아니고 최소 macOS 14, 첫 RID는 `osx-arm64`입니다.

Archive한 AppKit 기록은 native launch, 화면에 표시된 Material gallery, Metal completion 기반 present, CPU readback/full-frame copy 0건, AppKit-only bundle, native bridge 3개 operation을 증명합니다. Pointer/keyboard/IME, accessibility, resize/fullscreen/scale 이동, Release live, signing/notarization/store gate는 계속 `notVerified`입니다. Mac Catalyst 결과를 native AppKit macOS 결과로 바꾸지 않습니다.

Linux Qt는 Kubuntu 26.04 VMware에서 실제 Material gallery의 Wayland rendering, XWayland/xcb input smoke, swap terminal ACK, 20/30회 resize, semantics tree, framework-dependent/self-contained publish를 확인했습니다. 물리 Linux, 실제 X11 session, 한글 IME/Orca, context 강제 재생성, acrylic blur 시각 acceptance와 장기 성능은 `notVerified`입니다.

[ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md), [ADR-025](../Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md), archive한 [platform-runner workspace 요약](../history/26-08-19/platform-runner-workspace-summary.md), [AppKit dual-backend 요약](../history/26-08-20/macos-appkit-dual-backend-summary.md), [Linux Qt backend 요약](../history/26-08-20/linux-qt-backend-summary.md)을 참고하세요.

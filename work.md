# Doroti Android(MAUI) 및 전 플랫폼 custom shader 지원

작성일: 2026-08-17
상태: 구현 완료, 자동 검증 통과(수동/타 OS acceptance는 아래 경계 유지)
대상: `Doroti.App.Sdk`, `Doroti.Host.Maui`, `Doroti.Host.Web`, 공용 Skia runtime effects, 신규 Android target package, `DorotiDemoApp`, `doroti-app` 템플릿, 통합 검증

## 1. 결론

Android는 기존 MAUI 단일 프로젝트 구조에 네 번째 `DorotiTarget`으로 추가한다.

구현 결과 Windows, Mac Catalyst, Android, Web의 현재 네 제품 target이 같은 `FragmentProgram`/`FragmentShader` 계약과 Skia `SKRuntimeEffect` compiler를 사용한다. Inline SkSL과 manifest에 등록한 `.sksl` asset, float uniform, image sampler, `Paint.shader`, `ShaderMask`, gradient/image shader 및 일반 filter 합성을 공통 backend에 연결했다. 현재 SkiaSharp가 filtered child를 implicit texture로 바인딩하는 runtime image-filter API를 노출하지 않으므로 `ImageFilter.shader`는 지원한다고 광고하지 않고 fail-closed하며, Flutter의 Android stretch는 Flutter 원본의 matrix fallback을 사용한다.

검증 결과:

- 네 target graph와 Release build가 통과했다. Android는 signed APK/AAB와 `arm64-v8a` native asset을 만든다.
- 외부 `dotnet new doroti-app` package-only consumer도 Android arm64 build가 통과했다.
- Windows `MauiSKSwapChainPanel` live frame/replay가 통과했다.
- Android 16/API 36 arm64 실기기(SM-S931N)에서 `MauiSKGLTextureView`, OpenGL ES Skia, custom SkSL을 포함해 13 presented frames, replay 1, failed 0, software fallback 0을 확인했다.
- Android 스크롤 흰 화면의 근본 원인은 `_RenderCustomClip<T>`가 값 형식 `Rect`의 미계산 캐시를 `null`로 표현하려 했던 C# 제네릭 nullable 포팅 오류였다. `Rect.zero`가 유효한 캐시처럼 남아 Flutter의 `StretchingOverscrollIndicator`가 내부/외부 viewport를 `0x0`으로 자르던 경로를 명시적인 cache-validity 계약으로 고쳤다. 같은 계약을 `Doroti.DartToCSharp`의 field/method lowering과 fail-closed 생성물 검사에도 반영해 프레임워크 재생성 시 되돌아가지 않게 했다. 동시에 `Matrix4.translateByDouble`의 post-multiply 의미와 paint 중 들어온 다음 frame invalidation 보존도 복구했다.
- 같은 101-file compiler selection을 clean output에 재생성해 `NetworkImage` callback typedef, iterable `first`/`last`, nullable map key, `ImageCache.onError`, diagnostics optional parameter, promoted mixin, `FlutterView -> DorotiView`, collection extension/getter, enum member, member/local scope, `CustomClipper<T>` 계약 오류도 공용 lowerer에서 복구했다. 재생성된 Painting과 Semantics 프로젝트는 Release 컴파일이 통과했다. 전체 Rendering clean compile은 이 점검으로 더 깊은 기존 lowering 부채 51건이 드러나 아직 `notVerified`이며, 이번 스크롤/custom-shader 회귀의 통과 주장과 분리한다.
- Android 13/API 33 x86_64 에뮬레이터에서 강화된 `AndroidLive`를 다시 실행해 46 presented frames, replay 6, failed 0, native pointer events 52, software fallback 0을 확인했다. 고정 AppBar/FAB를 제외한 1920x1200 본문 ROI를 시작, 위 4회/아래 2회 각 스와이프 직후, 안정화 후에 모두 판정했으며 non-light 9.29%~13.25%, colored 7.86%~10.66%로 흰 화면 회귀가 없었다.
- Web WebAssembly compile/link와 Mac Catalyst Windows-host cross-build는 통과했다.
- 새 custom shader의 Mac Catalyst native presentation과 Web browser-live presentation, Android 화면의 사용자 육안 지속 표시, IME/TalkBack/stylus/mouse acceptance는 `notVerified`로 유지한다.

- 앱 프로젝트는 계속 하나만 둔다. `DorotiDemoApp.csproj`나 템플릿을 Android 전용 프로젝트로 분리하지 않는다.
- 선택 값은 `DorotiTarget=Android`, 제품 RID는 실기기용 `android-arm64` 또는 에뮬레이터용 `android-x64`, TFM은 `net10.0-android`로 고정한다.
- 루트 `Program.cs`와 `src/App.cs`는 수정 없이 target-neutral 상태를 유지한다.
- 앱 소유 네이티브 셸은 `Platforms/Android`에만 둔다. `Platforms/Maui` 또는 `Platforms/Doroti`는 다시 만들지 않는다.
- Doroti 필수 초기화, GPU surface, 입력 및 접근성 구현은 `Doroti.Host.Maui`가 소유한다.
- Android 조합 루트는 `Doroti.Target.Android.Maui.android-arm64` 및 `Doroti.Target.Android.Maui.android-x64` 제품 패키지로 제공한다.
- Android arm64 실기기와 Android x64 에뮬레이터를 지원한다. Play Store 서명/배포는 후속 범위로 둔다.

이 방향이면 현재의 `Windows`, `MacCatalyst`, `Web`과 동일하게 다음 명령 형태를 사용할 수 있다.

```powershell
dotnet build .\DorotiDemoApp\DorotiDemoApp.csproj -c Release `
  -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64
```

단, 지금 상태에서 TFM과 descriptor만 추가해서는 안 된다. 공용 MAUI 호스트가 여러 곳에서 `WINDOWS`가 아니면 Mac Catalyst/UIKit이라고 가정하므로 Android 컴파일이 깨진다. Android 지원은 아래의 호스트 분리 작업을 포함해야 한다.

## 2. 현재 구조 검토

### 이미 재사용할 수 있는 기반

- `Doroti.App.Sdk`가 선택한 `DorotiTarget`에 따라 TFM, RID, `UseMaui`, compile item, package reference를 결정한다.
- 앱의 중간 산출물, 출력물, publish 경로와 lock 파일은 `obj/<target>`, `bin/<target>`, `packages.<target>.lock.json`으로 격리된다.
- `Program.cs`는 `IDorotiApplicationStartup`, `src/App.cs`는 실제 UI를 소유한다.
- `Platforms/<Target>`만 조건부 컴파일되므로 Android 네이티브 셸을 같은 규칙에 넣을 수 있다.
- `Doroti.Host.Maui`는 `SKGLView`, 외부 소유 `SKSurface`/`GRContext`, dispatcher invalidation, retained-scene replay, MAUI 기반 IME/semantics의 공통 뼈대를 이미 제공한다.
- 이 PC에는 `android` workload가 설치되어 있어 Android restore/build 기반 검증을 수행할 수 있다.

### Android 추가 전에 고쳐야 할 경계

1. `Doroti.Host.Maui.csproj`는 Windows와 Mac Catalyst TFM만 평가한다.
2. `DorotiMauiPlatformApplications.cs`에는 WinUI와 Mac Catalyst delegate만 있고 Android `MauiApplication` 조합 루트가 없다.
3. `MauiFrameworkHost.cs`의 비-Windows 분기는 Android에서도 Mac Catalyst RID와 `SKMetalView` backend를 보고한다.
4. `MauiNativeInput.cs`의 비-Windows 분기는 UIKit API를 사용하므로 Android에서 컴파일할 수 없다.
5. `DorotiMauiSurface.Diagnostics`도 비-Windows bootstrap을 Mac Catalyst로 기록한다.
6. SDK descriptor와 bootstrap 생성기는 `WinUI-Xaml`, `UIKit-Main`, `Managed-Main`만 안다.
7. DemoApp, 템플릿, product solution, target manifest, lock 파일 및 validator 어디에도 Android producer/consumer 계약이 없다.
8. 구현 전 라이브 증거는 Windows native presentation뿐이었다. 이번 구현에서 Android의 실제 `SKGLView.Handler.PlatformView`와 OpenGL ES backend/replay는 확인했으며, 별도 context-loss acceptance는 `notVerified`로 유지한다.

## 3. 소유권과 목표 파일 구조

| 소유자 | 책임 |
| --- | --- |
| 앱 `Program.cs`, `src/App.cs` | 플랫폼 중립 startup 및 UI |
| 앱 `Platforms/Android` | Android manifest, `MainActivity`, `MainApplication`, 앱별 native hook |
| `Doroti.App.Sdk` | `Android -> net10.0-android/android-arm64` 선택, item/descriptor/package/bootstrap 생성 |
| `Doroti.Host.Maui` | 공통 MAUI 앱, GPU surface, Android lifecycle/input/IME/cursor/accessibility adapter |
| `Doroti.Target.Android.Maui.android-arm64`, `Doroti.Target.Android.Maui.android-x64` | Android 실기기/에뮬레이터 지원 선언, backend identity, target manifest, package boundary |
| `Doroti/eng` | graph/build/package/live/physical evidence와 회귀 검증 |

목표 앱 구조는 다음과 같다.

```text
DorotiDemoApp/
  DorotiDemoApp.csproj
  Program.cs
  src/App.cs
  Resources/
  Platforms/
    Android/
      AndroidManifest.xml
      MainActivity.cs
      MainApplication.cs
      application-manifest.json
      Resources/                 # Android에 정말 필요한 최소 리소스만
    Windows/
    MacCatalyst/
    Web/
```

Android 셸은 생성된 `Doroti.Generated.DorotiBootstrap.Create(...)`를 호출하고 앱별 hook만 등록한다. 공통 `MauiAppBuilder`, `UseSkiaSharp`, Doroti service 등록을 앱 폴더에 복제하지 않는다.

## 4. 구현 단계

### A0. Android GPU/SDK 기준선 확정

목표: 제품 코드를 넓게 수정하기 전에 로컬 .NET 10/MAUI 10/SkiaSharp 조합의 실제 Android 계약을 확인한다.

- 설치된 Android workload, SDK, JDK, adb, 사용 가능한 arm64 실기기 또는 x86_64 에뮬레이터를 기록한다.
- `SKGLView.Handler.PlatformView`의 실제 Android 타입과 GPU backend를 작은 validation probe에서 수집한다.
- `PaintSurface`에서 `SKSurface`, `GRContext`, framebuffer 크기가 유효한지 확인한다.
- Android 최소 API(`SupportedOSPlatformVersion`)는 설치된 MAUI pack과 사용할 실제 기기를 기준으로 명시한다. 조사 없이 임의의 API level을 고정하지 않는다.
- `android-arm64` restore/build가 Windows에서 가능한지 먼저 확인한다.
- probe는 제품 앱이나 템플릿에 임시 코드를 남기지 않고 `Doroti/validation/app-bootstrap` 아래의 검증 자산으로 제한한다.

완료 게이트:

- 실제 native view type과 backend identity가 기록됨.
- software surface가 아닌 GPU `SKSurface`/`GRContext`가 확인됨.
- 최소 API와 테스트 기기/에뮬레이터 조건이 문서화됨.
- 이 단계에서 live 실행을 못 했다면 이후 항목은 `notVerified`로 유지함.

### A1. `Doroti.App.Sdk`에 Android target 계약 추가

대상: `Sdk.props`, `Sdk.targets`.

- RID 역추론에 `android-arm64 -> Android`를 추가한다.
- `DorotiTarget=Android`에 아래 속성을 부여한다.
  - `TargetFramework=net10.0-android`
  - `RuntimeIdentifier=android-arm64`
  - `UseMaui=true`
  - `ANDROID;DOROTI_MAUI` compile constants
  - A0에서 확정한 `SupportedOSPlatformVersion`
- `DorotiTargetDescriptor`에 `Android`, `HostKind=Maui`, `NativeEntryKind=Android-Application`, 신규 target package를 등록한다.
- Android는 CLR `Main`을 생성하지 않는다. Android runtime이 `MainApplication`/`MainActivity` attribute를 발견하고, 그 셸이 generated descriptor를 사용하도록 bootstrap 생성을 분리한다.
- `Platforms/Android/**/*.cs`만 Android 빌드에 들어가고 다른 플랫폼 소스 및 XAML은 들어가지 않는 graph contract를 추가한다.
- `AndroidManifest.xml`과 Android resource item이 `EnableDefault*Items=false`에서도 확실히 평가되는지 확인하고, 필요한 item만 SDK가 명시적으로 포함한다.
- arm64의 `obj/android`, `bin/android`, `packages.android.lock.json`과 x64의 `obj/android-x64`, `bin/android-x64`, `packages.android-x64.lock.json`을 분리해 target/RID 간 충돌을 막는다.
- 잘못된 `DorotiTarget=Android`/RID 조합은 `DOROTIAPP003`/`DOROTIAPP004`로 fail-closed 해야 한다.

완료 게이트:

- `WriteDorotiTargetGraph`가 Android TFM/RID/descriptor/startup/platform sources를 정확히 한 번씩 출력함.
- Windows/Mac Catalyst/Web source leakage가 0임.
- Android에 `MauiXaml` 및 Windows `ApplicationDefinition`이 0임.
- `Program.cs`, `src/App.cs`, generated bootstrap 경로가 기존 target과 동일한 계약을 유지함.

### A2. Android target package와 MAUI 호스트 3분기화

신규 제품 프로젝트:

```text
Doroti/src/Doroti.Target.Android.Maui.android-arm64/
  Doroti.Target.Android.Maui.android-arm64.csproj
  AndroidMauiTarget.cs
  doroti-target-manifest.json
  buildTransitive/Doroti.Target.Android.Maui.android-arm64.targets
```

- target package는 `net10.0-android`, `android-arm64`, `UseMaui=true`를 사용하고 `Doroti.Host.Maui`를 참조한다.
- manifest는 A0에서 확인한 native view type/backend와 `softwareFallback=false`를 선언한다.
- `EnsureSupported()`는 Android arm64 프로세스만 허용한다.
- `Doroti.Product.slnx`, pack/release metadata, product package 검증 목록에 신규 프로젝트를 등록한다.

`Doroti.Host.Maui` 변경:

- `net10.0-android`의 arm64/x64 평가와 RID별 `obj/android`, `obj/android-x64` 격리를 추가한다.
- 모든 `#if WINDOWS ... #else UIKit ...` 형태를 `WINDOWS`, `MACCATALYST`, `ANDROID`의 명시적 분기로 바꾼다. 새 플랫폼이 잘못 Mac으로 취급되는 fallback을 남기지 않는다.
- Android용 `DorotiMauiAndroidApplication`을 추가해 generated application descriptor로 `MauiApp`을 만든다.
- target identity, runtime RID, native bootstrap source 문자열을 한 곳에서 계산해 diagnostics의 Mac 하드코딩을 없앤다.
- 공통 MAUI surface/retained scene/semantics 구현은 공유하고 Android에서만 필요한 native adapter만 조건부 파일 또는 조건부 블록으로 격리한다.

완료 게이트:

- `Doroti.Host.Maui`의 Windows, Mac Catalyst, Android 세 TFM이 각각 독립 restore/build 됨.
- Android build 산출물에 UIKit/WinUI 참조가 없고 기존 두 target의 API/diagnostics identity가 변하지 않음.
- product package 안에 target manifest와 buildTransitive descriptor가 포함됨.

### A3. 앱과 템플릿의 얇은 Android 셸 추가

`DorotiDemoApp`와 `Doroti.Templates/content/doroti-app`를 동시에 갱신한다.

- `MainApplication.cs`
  - Android `[Application]` entry를 제공한다.
  - `DorotiMauiAndroidApplication`을 상속한다.
  - `Doroti.Generated.DorotiBootstrap.Create(...)`로 descriptor를 공급한다.
  - 앱별 `ConfigurePlatform(MauiAppBuilder)` hook만 노출한다.
- `MainActivity.cs`
  - `[Activity]`와 `MainLauncher=true`를 선언한다.
  - MAUI 기본 configuration-change 집합을 명시해 회전/화면 크기/밀도 변경에서 불필요한 중복 host/session 생성을 막는다.
  - launch mode와 exported 값은 Android manifest/최소 API 정책에 맞춘다.
- `AndroidManifest.xml`
  - `ApplicationId`, 버전, label/icon은 csproj/shared resource와 중복된 상수로 갈라지지 않게 한다.
  - 권한은 실제 기능에 필요한 최소 집합만 선언한다. 네트워크, 저장소, 센서 권한을 관성적으로 추가하지 않는다.
- `application-manifest.json`
  - target RID와 요구 capability를 Android로 동기화한다.
- DemoApp에 repository `ProjectReference`, 템플릿에는 package 기반 자동 참조가 정확히 선택되도록 한다.
- `packages.android.lock.json`과 `packages.android-x64.lock.json`은 DemoApp과 생성 템플릿 consumer 각각의 target/RID-specific app lock으로 관리한다. `Doroti/src/**/packages.lock.json`은 만들거나 추적하지 않는다.

완료 게이트:

- DemoApp과 `dotnet new doroti-app` 결과가 같은 Android 파일/manifest 계약을 가짐.
- 두 앱 모두 Android restore/build/package가 됨.
- root `Program.cs`, `src/App.cs`에 `#if ANDROID`, `Android.*`, `Maui*` 참조가 없음.
- 체크인된 `Platforms/Maui`, `Platforms/Doroti`, generated bootstrap이 없음.

### A4. Android 렌더링과 lifecycle 완성

- 기존 `SKGLView { HasRenderLoop=false }`와 dispatcher-driven invalidation을 유지한다.
- Android OpenGL ES context 생성/소실, Activity pause/resume, background/foreground, 화면 회전 후에도 Doroti scene을 다시 그릴 수 있게 한다.
- 새 back buffer/native paint에서는 retained scene을 replay한다. 첫 프레임 `Presented`만으로 성공 판정하지 않는다.
- physical pixel size, logical size, density 변경을 올바르게 host metrics로 전달한다.
- Activity/Window 종료 시 session, semantics layer, hidden IME view, `GRContext` 관련 참조를 중복 dispose하지 않는다.
- Android의 화면 꺼짐/재개나 surface 재생성 후 software fallback으로 조용히 전환하지 않고 strict GPU 위반을 진단한다.

자동 live 게이트 최소 조건:

- 앱 실행과 첫 화면 표시.
- `Presented > 0`, `Failed = 0`, `SoftwareFallbackFrames = 0`.
- 강제 native repaint 또는 회전/재개 뒤 `Replayed > 0`.
- native view/backend가 A0에서 확정한 Android identity와 일치.
- background/foreground와 2회 회전 후 화면이 계속 보이고 프로세스 crash가 없음.

### A5. Android 입력, IME, cursor, 접근성 완성

컴파일 성공을 Android 사용자 상호작용 지원으로 간주하지 않는다. capability별로 구현과 증거를 분리한다.

- touch: finger down/move/up/cancel, multi-touch pointer id, pressure를 확인한다.
- pen/mouse: stylus pressure/buttons, hover, wheel/scroll을 지원 가능한 기기에서 확인한다.
- keyboard: Android native key event를 Doroti `KeyData`로 변환하고 repeat/modifier/Back 처리 경계를 정한다.
- IME: hidden MAUI `Entry`/`Editor`가 soft keyboard, composing text, selection, multiline action, obscure text를 올바르게 왕복하는지 확인한다.
- cursor: Android `PointerIcon` 지원 API에서는 Doroti cursor kind를 매핑하고, touch-only 기기에서는 안전한 no-op으로 둔다.
- clipboard/platform services: Android 앱 lifecycle 및 main-thread 규칙을 지키는지 확인한다.
- accessibility: semantics overlay가 TalkBack에 label, role, focus, tap, setText를 노출하고 invisible duplicate focus를 만들지 않는지 확인한다.
- Android system Back은 우선 Doroti navigation/route 처리 계약을 정의한 뒤 Activity 종료로 위임한다.

증거 구분:

- emulator 자동화: touch, text input, Back, rotation의 반복 가능한 기본 계약.
- physical Android: stylus/mouse/IME/TalkBack/GPU context-loss 및 실제 표시 확인.
- 수행하지 않은 capability는 evidence에서 `implemented-not-live-verified` 또는 `notVerified`로 남긴다.

### A6. 검증 스크립트와 evidence 통합

`Doroti/eng/validate-app-targets.ps1`을 중심으로 짧은 대표 게이트를 유지한다.

1. `Graph`
   - Windows, Android, Mac Catalyst, Web descriptor와 source isolation.
   - invalid Android RID/TFM negative test.
2. `Build`
   - `Windows -> Android -> Web -> MacCatalyst -> Windows --no-restore` 순서.
   - target 전환 후 assets/generated attributes/lock/output 오염이 없는지 확인.
   - Android APK 또는 AAB에 arm64 native assets와 최종 manifest가 포함되는지 검사.
3. `WindowsLive`
   - 현재 Windows presentation/replay 증거를 회귀 게이트로 유지.
4. `AndroidLive`
   - adb로 명시된 emulator/device에 설치/실행하고 A4의 diagnostics를 회수.
   - PC 환경변수 경로를 Android 앱이 직접 볼 수 있다고 가정하지 않는다. debug intent extra + app-private cache + `run-as`/adb pull 또는 구조화 logcat 중 하나를 계약으로 정한다.
5. `AndroidPhysical`
   - 자동 suite에 암묵적으로 포함하지 않는 명시적 acceptance gate.
6. `Evidence`
   - schema를 올리고 `androidBuild`, `androidEmulatorLive`, `androidPhysical`을 별도 상태로 기록.

기본 Developer suite는 기기 없이 재현 가능한 graph/build/package와 기존 Windows live까지만 수행한다. Android emulator/physical 결과는 장치 serial을 명시한 별도 명령으로 실행하며, 장치가 없다는 이유로 PASS를 합성하지 않는다.

최종 대표 명령은 다음 형태로 정리한다.

```powershell
pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer
pwsh -NoProfile -File .\Doroti\eng\validate-app-targets.ps1 -Shard AndroidLive -AndroidSerial <serial>
pwsh -NoProfile -File .\Doroti\eng\validate-app-targets.ps1 -Shard AndroidPhysical -AndroidSerial <serial>
```

저장소 지침에 따라 test 성격의 명령 timeout은 20분으로 둔다.

### A7. 문서와 릴리스 경계 갱신

- DemoApp 영문/한글 README에 Android restore/build/install/run 명령과 SDK/JDK/adb 전제조건을 추가한다.
- `Doroti.App.Sdk`, `Doroti.Host.Maui`, Android target package의 package description/release notes를 동기화한다.
- 템플릿 생성 후 Android 실행 예제를 문서화한다.
- evidence에는 다음 결과를 섞지 않는다.
  - Windows live GPU proof
  - Android compile/package proof
  - Android emulator presentation/interaction proof
  - Android physical presentation/IME/TalkBack proof
  - Play Store 서명/배포 proof

## 5. 완료 정의

다음을 모두 만족해야 “Android 지원 구현 완료”로 닫는다.

- 단일 app `.csproj`에서 `DorotiTarget=Android`, `android-arm64`, `net10.0-android`가 선택됨.
- DemoApp과 생성 템플릿 consumer가 restore/build/package 됨.
- `Doroti.Target.Android.Maui.android-arm64`가 제품 solution에서 build/pack되고 manifest 계약이 검증됨.
- Windows/Mac Catalyst/Web 기존 graph/build 및 Windows live replay가 회귀 없이 통과함.
- Android emulator 또는 실제 arm64 기기에서 strict GPU frame, retained replay, lifecycle resume가 확인됨.
- touch 및 soft-keyboard 입력의 기본 경로가 live 확인됨.
- 실제 arm64 기기에서 사용자에게 화면이 지속적으로 보이는 것이 확인됨.
- TalkBack, stylus/mouse, physical IME 등 실행하지 않은 항목은 완료로 과장하지 않고 `notVerified`로 남음.
- 템플릿, docs, lock files, validators, evidence가 제품 소스와 같은 계약을 사용함.

## 6. 이번 범위에서 제외

- Android별 별도 앱 프로젝트 또는 solution 생성.
- `Program.cs`/`src/App.cs`의 Android 조건부 분기.
- iOS 지원 동시 추가.
- `android-x86`, `android-arm` target package 동시 출시.
- Play Console 업로드, signing key 생성/보관, store 정책 대응.
- Firebase, deep link, push notification, background service, camera/location/storage 권한.
- software raster fallback을 성공 경로로 허용하는 변경.
- Android 지원을 이유로 Windows의 유일한 초기화용 `App.xaml`을 제거하는 작업.

## 7. 권장 실행 순서

```text
A0 실제 Android GPU 계약 확인
  -> A1 SDK target/graph
  -> A2 Host + target package
  -> A3 DemoApp/template shell
  -> A4 GPU/lifecycle
  -> A5 input/IME/accessibility
  -> A6 compact validation/evidence
  -> A7 docs/release boundary
```

A0의 실제 GPU surface가 strict 조건을 만족하지 못하면 A4 이후를 진행하기 전에 `SKGLView` 유지 여부와 Android 전용 native Skia view 도입 여부를 다시 결정한다. 그 경우에도 앱/SDK/target 소유권 경계는 유지하고, 제품 화면 코드를 플랫폼 프로젝트로 분리하지 않는다.

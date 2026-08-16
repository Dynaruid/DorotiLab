# Doroti application bootstrap 개편

## 결론

Flutter처럼 **공통 애플리케이션 진입점과 네이티브 플랫폼 셸을 분리**한다. 다만 Doroti의 예약 초기화 코드를 사용자가 보는 `Platforms/Doroti` 또는 `Platforms/Maui` 소스 폴더에 두지는 않는다.

- `Program.cs`는 Windows, MacCatalyst, Web, 이후 Linux/Qt가 공유하는 유일한 **논리적 애플리케이션 진입점**이다.
- `Platforms/<Target>`에는 OS가 요구하는 물리적 진입점, manifest/resource, 사용자 커스터마이징 훅만 둔다.
- Doroti 세션, view, surface, application boundary, resource/plugin 연결은 `Doroti.Hosting`과 각 `Doroti.Host.*`/`Doroti.Target.*` 패키지가 소유한다.
- 대상별로 반복되는 접착 코드는 `Doroti.App.Sdk`가 `obj/<target>/Doroti.Generated/`에 생성한다. 생성 코드는 앱 템플릿의 소스가 아니며 수정 대상도 아니다.
- `Platforms/Maui`는 제거한다. MAUI는 Windows와 MacCatalyst가 현재 사용하는 호스트 구현일 뿐 애플리케이션 플랫폼이 아니다.
- 런타임 reflection으로 `App`을 찾지 않는다. SDK가 `Program`의 타입을 컴파일 시점에 강하게 연결하고, 잘못된 계약은 빌드 오류로 닫는다.

즉 Flutter와 대응시키면 `Program.cs`가 `lib/main.dart`, `Platforms/Windows`·`Platforms/MacCatalyst`·향후 `Platforms/Linux`가 runner shell, SDK가 생성한 `obj/**/Doroti.Generated`가 tool-managed bootstrap 역할을 맡는다.

## 현행 구조에서 확인된 문제

- 루트 `Program.cs`는 Web과 MacCatalyst만 조건부 호출하며 Windows의 실질적인 시작 경로가 아니다. 공통 진입점처럼 보이지만 대상별 의미가 다르다.
- Windows와 MacCatalyst가 각각 `MauiApp.CreateBuilder`, toolkit, Skia, `Application`, `Window`, `DorotiMauiSurface` 연결을 다시 구현한다.
- `Platforms/Maui`의 `MauiProgram`, `DorotiMauiApplication`, `DorotiMauiPage`는 MAUI 대상에 함께 컴파일되지만 현재 Windows/MacCatalyst 시작 경로는 각 플랫폼의 별도 bootstrap application을 사용한다. 공통 구현이 아니라 중복된 제3 경로가 되어 있다.
- Windows는 문자열과 reflection으로 `DorotiApp.App`을 찾고 MacCatalyst는 정적 타입을 직접 참조한다. rename, trimming/AOT, 진단 시점이 서로 다르다.
- Web bootstrap은 `DorotiWebApplication`과 JavaScript plugin descriptor를 직접 조립한다. 프로젝트의 `DorotiJavaScriptPlugin` MSBuild item과 등록 정보가 중복될 수 있다.
- `DorotiMauiSurface`는 entrypoint factory와 view configuration을 따로 받고, Web은 application assembly와 plugin descriptor까지 포함한 별도 record를 받는다. 동일 앱을 호스트하는 공통 계약이 아직 없다.
- SDK의 대상 allowlist와 compile item 규칙이 Windows/MacCatalyst/Web을 직접 열거하므로 Linux/Qt를 추가할 때 앱 템플릿, SDK, validator를 동시에 조건 분기로 늘리게 된다.

## 소유권 경계

| 영역 | 소유자 | 내용 |
|---|---|---|
| 공통 앱 구성 | 앱 개발자 | `Program.cs`, root widget, 공통 서비스와 앱 옵션 |
| 네이티브 셸 커스터마이징 | 앱 개발자 | activation, native window 옵션, URL/notification lifecycle, entitlement/manifest |
| Doroti 불변 초기화 | Doroti | session/view/application boundary 생성, surface 연결, resource/plugin 등록, dispose 순서 |
| 대상 선택과 접착 코드 | `Doroti.App.Sdk` | 선택된 target package, 생성 bootstrap, compile/content item, 계약 진단 |
| 렌더링/입력 호스트 | `Doroti.Host.Maui`, `Doroti.Host.Web`, 향후 `Doroti.Host.Qt` | 각 UI toolkit과 Doroti capability 사이의 adapter |
| OS/RID 구현 | `Doroti.Target.*` | Windows, MacCatalyst, Web, Linux의 native backend와 지원 조건 |

플랫폼 파일을 사용자가 수정할 수 있다는 것과 Doroti 초기화 순서를 사용자가 소유한다는 것은 분리한다. 사용자는 안정된 hook을 통해 builder와 native lifecycle을 확장할 수 있지만, 필수 등록을 삭제하거나 순서를 바꾸지는 못하게 한다.

## 제안하는 공통 계약

### 1. `Program.cs`는 대상 중립 startup 타입으로 만든다

초기 API 모양은 다음처럼 단순하게 유지한다. 이름은 구현 단계에서 조정할 수 있지만 역할은 분리하지 않는다.

```csharp
using Doroti.Hosting;

namespace DorotiApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(App.Definition)
        .UseView(App.ViewConfiguration);
}
```

- `Program.cs`에는 `#if DOROTI_BROWSER`, `MACCATALYST`, MAUI, Blazor, Qt 참조가 없어야 한다.
- `IDorotiApplicationStartup`과 `DorotiApplicationBuilder`는 target-neutral `Doroti.Hosting`에 둔다.
- builder 결과는 최소한 entrypoint factory, application assembly, view configuration, normalized launch context를 가진 불변 `DorotiApplicationDescriptor`가 된다.
- command-line, WinUI activation, UIKit lifecycle, browser base URI처럼 모양이 다른 입력은 `DorotiLaunchContext`로 정규화한다. 공통 `Program`이 OS 타입을 직접 받지 않는다.
- 기본 startup 타입은 `$(RootNamespace).Program`이며 필요하면 `<DorotiApplicationType>...</DorotiApplicationType>`으로 명시한다.
- SDK 생성 코드는 `DorotiApplicationType`을 generic/type reference로 직접 사용한다. 현재 Windows reflection 경로는 제거한다.

`Program`은 모든 OS에서 반드시 CLR `Main` 자체일 필요는 없다. WinUI와 UIKit은 프레임워크가 요구하는 실제 process entry를 유지하되, 어느 대상이든 앱 정의를 얻는 유일한 경로가 `Program`이어야 한다.

### 2. 호스트가 공통 descriptor를 소비한다

- `DorotiMauiSurface`는 `(Func<IDorotiViewEntrypoint>, DorotiViewConfiguration)` 대신 `DorotiApplicationDescriptor` 또는 descriptor에서 만든 scoped runtime을 받는다.
- `DorotiWebApplication`에 들어 있는 공통 필드는 descriptor로 올리고, browser-only plugin adapter만 Web 계층에 남긴다.
- application manifest load, resource capability, plugin capability, session start, view attach/show, dispose 순서를 한 composition service에서 통일한다.
- 한 프로세스에서 descriptor는 하나지만 view/session은 lifecycle에 맞게 생성될 수 있어야 한다. 향후 multi-window를 막는 singleton surface 설계는 피한다.
- platform plugin 등록은 `Program.cs`의 조건부 코드가 아니라 MSBuild item과 생성된 target registration에서 descriptor로 합쳐진다.

### 3. MAUI 불변 구현을 `Doroti.Host.Maui`로 이동한다

`Platforms/Maui/MauiProgram.cs`, `DorotiMauiApplication.cs`, `DorotiMauiPage.cs`의 역할은 앱 템플릿이 아니라 `Doroti.Host.Maui`가 소유한다.

호스트 패키지는 다음과 같은 base/extension 계약을 제공한다.

- `UseDorotiApplication<TStartup>()`: toolkit, Skia, Doroti descriptor/runtime, application/page를 필수 순서로 등록한다.
- `DorotiMauiApplication`: 기본 window/page와 scoped surface를 만든다.
- Windows용 `DorotiMauiWinUIApplication<TStartup>`과 MacCatalyst용 `DorotiMauiUIApplicationDelegate<TStartup>`: `CreateMauiApp`의 불변 구현을 소유한다.
- `ConfigurePlatform(MauiAppBuilder builder)` 같은 명시적 hook: 사용자 서비스와 native handler 추가에만 사용한다.
- 필수 Doroti 등록은 hook 전후에 검증하고, 빠졌거나 교체되었으면 시작 후 예외가 아니라 build/startup 진단으로 실패시킨다.

### 4. Web도 동일한 startup을 사용한다

현재 `Platforms/Web/PlatformBootstrap.cs`는 제거하고 SDK 생성 bootstrap이 다음 역할을 한다.

1. `Program`으로 공통 descriptor를 만든다.
2. `WebAssemblyHostBuilder`와 `DorotiRoot`를 구성한다.
3. target package와 `DorotiJavaScriptPlugin` item에서 생성한 browser plugin registration을 연결한다.
4. `DorotiWebRunner.RunAsync`를 호출한다.

사용자 Web 커스터마이징이 필요하면 선택적인 `Platforms/Web/WebPlatform.cs`에 `ConfigureWebHost` hook만 둔다. `wwwroot`, `index.html`, PWA manifest 같은 Web 자산은 계속 사용자 영역이다.

### 5. Linux/Qt는 MAUI의 예외 분기가 아니라 새 host/target으로 추가한다

향후 구성은 다음 경계를 따른다.

- `Doroti.Host.Qt`: Qt event loop, window, GPU surface, pointer/keyboard/IME/clipboard/accessibility adapter.
- `Doroti.Target.Linux.Qt.linux-x64`와 필요한 추가 RID package: native Qt/Skia 자산과 지원 조건.
- `Doroti.App.Sdk`: `DorotiTarget=Linux`, RID/TFM, package, 생성 process entry 선택.
- `Platforms/Linux`: icon, desktop file, native window option과 사용자 `ConfigureQt` hook.

Qt runner 역시 같은 `Program : IDorotiApplicationStartup`을 소비한다. 공통 앱 코드는 Qt 타입을 참조하지 않고, MAUI를 일반 desktop 추상화로 승격시키지도 않는다.

## 개편 후 템플릿 구조

```text
doroti-app/
├─ DorotiApp.csproj
├─ Program.cs                         # 모든 target의 공통 논리적 진입점
├─ src/
│  └─ App.cs
├─ Platforms/
│  ├─ Windows/
│  │  ├─ App.xaml
│  │  ├─ App.xaml.cs                  # native shell + 사용자 hook
│  │  └─ manifests...
│  ├─ MacCatalyst/
│  │  ├─ AppDelegate.cs               # native shell + 사용자 hook
│  │  └─ plist/entitlements...
│  ├─ Web/
│  │  ├─ WebPlatform.cs               # 선택 사항
│  │  └─ wwwroot/...
│  └─ Linux/                          # Qt target 도입 시 추가
│     └─ LinuxPlatform.cs              # native 옵션/hook
└─ Resources/...

obj/<target>/Doroti.Generated/
├─ DorotiBootstrap.g.cs               # SDK 소유, 수정 금지
└─ DorotiPluginRegistration.g.cs       # SDK 소유, 수정 금지
```

`Platforms/Maui`와 `Platforms/Doroti`는 만들지 않는다. 예약 코드를 source tree 안에 둘 경우 사용자 수정, 템플릿 버전 drift, regenerate 충돌을 다시 만들기 때문이다.

## 플랫폼별 얇은 셸의 형태

Windows와 MacCatalyst 파일은 삭제 대상이 아니라 **사용자 소유 adapter**로 축소한다.

```csharp
// Platforms/Windows/App.xaml.cs
public sealed partial class App : DorotiMauiWinUIApplication<Program>
{
    public App() => InitializeComponent();

    protected override void ConfigurePlatform(MauiAppBuilder builder)
    {
        // 사용자 native service/handler 등록 지점
    }
}
```

```csharp
// Platforms/MacCatalyst/AppDelegate.cs
[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate<Program>
{
    protected override void ConfigurePlatform(MauiAppBuilder builder)
    {
        // 사용자 UIKit/MAUI 확장 지점
    }
}
```

base class가 `CreateMauiApp`을 소유하고 필수 초기화를 수행한다. 사용자는 native lifecycle override와 제공된 hook을 사용할 수 있지만 `DorotiMauiSurface`를 직접 조립할 필요는 없다.

## SDK/진단 계약 변경

- `Sdk.targets`의 `Compile Include="Platforms\Maui\**\*.cs"`를 제거한다.
- 대상별 bootstrap 파일을 앱 source에서 찾지 말고 `DorotiApplicationType`을 입력으로 생성한다.
- Windows/MacCatalyst/Web 직접 allowlist는 target descriptor item으로 바꾼다. 새 target은 SDK core 조건문을 복제하기보다 target package가 TFM, RID, host kind, native entry kind를 제공하게 한다.
- 각 빌드는 정확히 하나의 `DorotiTargetDescriptor`와 하나의 generated bootstrap을 가져야 한다.
- `DOROTIAPP` 진단을 추가/개편한다.
  - startup 타입 누락, 비공개, 중복 또는 interface 미구현
  - target descriptor 0개/복수
  - 선택되지 않은 플랫폼 source 유입
  - 앱 source의 금지된 `Platforms/Maui` 및 legacy `PlatformBootstrap`
  - Web plugin metadata 누락/중복 및 생성 registration 불일치
- 생성 파일 목록과 선택된 startup/host/target을 `WriteDorotiTargetGraph`에 기록해 구조 검증이 가능한 상태로 만든다.

## 구현 순서

### B1. 공통 startup/descriptor 계약

- `Doroti.Hosting`에 startup, builder, descriptor, launch context를 추가한다.
- MAUI와 Web이 descriptor를 소비하도록 application boundary와 lifecycle 조립을 공통화한다.
- 같은 descriptor로 두 host를 구성하는 unit/contract test를 먼저 만든다.

완료 조건:

- 공통 계약이 MAUI, Blazor, Qt 타입을 참조하지 않는다.
- entrypoint, assembly, configuration, resource/plugin boundary가 한 descriptor에서 유실 없이 전달된다.
- reflection 및 문자열 기반 앱 타입 조회가 0건이다.

### B2. MAUI host 소유권 이동

- 공통 MAUI application/page/builder를 `Doroti.Host.Maui`로 이동한다.
- Windows/MacCatalyst base entry와 사용자 hook을 추가한다.
- 템플릿과 `DorotiDemoApp`의 `Platforms/Maui`를 삭제하고 각 platform shell을 축소한다.

완료 조건:

- Windows와 MacCatalyst에 앱 정의/surface 조립 중복이 없다.
- Windows build/publish/live GPU presentation이 기존 backend identity와 frame/resource 계약을 유지한다.
- MacCatalyst는 Windows cross-build와 실제 Apple Silicon live 결과를 분리 기록한다.

### B3. SDK generated bootstrap과 Web 통합

- `Program` startup을 대상별 native entry에 강하게 연결하는 생성 코드를 추가한다.
- Web `PlatformBootstrap.cs`를 제거하고 `DorotiWebRunner`로 이전한다.
- `DorotiJavaScriptPlugin`에서 browser registration을 생성하여 수기 descriptor 목록을 제거한다.
- target graph validator를 새 구조에 맞춘다.

완료 조건:

- 세 대상의 `Program.cs` compile item과 내용이 동일하고 조건부 컴파일이 없다.
- external `dotnet new doroti-app` 소비자가 Windows/Web build와 Web publish를 통과한다.
- WebGL2 presentation과 pointer smoke를 다시 확인하며 native live나 physical acceptance로 확대 해석하지 않는다.

### B4. Linux/Qt 확장점 증명

- 실제 Qt 구현 전 synthetic target descriptor로 SDK가 네 번째 host kind를 수용하는지 검증한다.
- 이후 `Doroti.Host.Qt`와 Linux target package를 연결하고 platform hook을 추가한다.
- Linux build/package, Qt live presentation, input/IME/accessibility, physical/cross-target 검증은 각각 별도 evidence로 남긴다.

완료 조건:

- Linux 추가를 위해 `Program.cs`, `src/App.cs`, MAUI/Web host를 수정하지 않는다.
- 새 target 추가 변경이 target package, SDK descriptor registration, Linux platform assets/validator에 한정된다.

## 검증 게이트

구조 변경은 compile 성공만으로 닫지 않는다.

- Structural: legacy `Platforms/Maui`와 `PlatformBootstrap` 0건, reflection 0건, 대상별 source leakage 0건, generated bootstrap 정확히 1개.
- Contract: Windows/MacCatalyst/Web descriptor가 같은 startup과 application assembly를 사용하고 target별 plugin/resource registration만 달라지는지 검사.
- Build: Release Windows, Web, MacCatalyst cross-build와 반복 build; lock/package graph identity 확인.
- Package consumer: 저장소 밖 임시 디렉터리에서 template install/create/restore/build/Web publish.
- Native live: Windows MAUI GPU presented/replayed/failed/resource evidence를 재수집.
- Web live: Blazor mount, WebGL2 presentation, pointer/keyboard/IME/resize/plugin을 Web 범위로 재검증.
- Target-specific: MacCatalyst 실제 장비와 Linux/Qt는 실행 가능한 환경에서만 PASS로 기록하고, 그 전에는 `notVerified`를 유지.
- Regression: platform hook에서 사용자 서비스/handler를 하나 추가한 template fixture와, 필수 Doroti 등록을 훼손하려는 negative fixture를 함께 검사.

테스트 명령은 기존 진입점을 유지하되 새 bootstrap 구조 검사를 `validate-app-targets.ps1`에 통합한다. 테스트 실행 시 repository 지침에 따라 20분 timeout을 사용한다.

## 이번 개편에서 하지 않을 것

- MAUI 위에 Qt를 추상화하거나 Qt를 MAUI target으로 취급하지 않는다.
- 사용자가 편집하는 `Platforms` 아래에 Doroti 생성 코드를 체크인하지 않는다.
- 기존 reflection bootstrap 또는 `Platforms/Maui`를 호환 모드로 장기간 병행하지 않는다. beta template이므로 producer, template, DemoApp, validators, docs를 한 번에 전환한다.
- Windows build/live 결과를 MacCatalyst, Web, Linux의 실제 동작 증거로 사용하지 않는다.

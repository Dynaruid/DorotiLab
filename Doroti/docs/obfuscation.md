# Obfuscar 앱 DLL 난독화

플랫폼 러너를 Release로 빌드하거나 게시할 때 `DorotiObfuscate=true`를 전달하면
공용 `Doroti.App.Sdk` 앱 DLL에 Obfuscar **2.2.50**을 실행합니다. 기본값은 꺼짐입니다.
Obfuscar는 SDK에 포함된 로컬 도구 manifest로 버전을 고정하며, 최초 사용 시
NuGet 접속이 필요합니다. 전역 도구 설치와 PowerShell은 난독화 단계에 필요하지 않습니다.

```powershell
# Windows App SDK: 일반 managed Release
dotnet publish samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release -p:DorotiObfuscate=true

# Windows MAUI: 기본 ReadyToRun을 유지한 Release
dotnet publish samples/DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:PublishReadyToRun=true -p:DorotiObfuscate=true

# Web: WASM AOT를 명시적으로 끈 Release
dotnet publish samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -p:RunAOTCompilation=false -p:DorotiObfuscate=true

# Android: Mono AOT와 profiled AOT를 모두 끈 Release
dotnet build samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false -p:DorotiObfuscate=true
```

ReadyToRun에서도 난독화를 사용할 수 있습니다. 공용 앱의 IL을 먼저 난독화한 뒤
러너의 게시 단계에서 crossgen2가 R2R 코드를 생성합니다. `PublishReadyToRun=false`로
바꿀 필요는 없습니다. 이미 R2R로 생성된 최종 DLL을 Obfuscar로 다시 수정하지 않습니다.

여러 러너에서 공통으로 켜려면 앱 폴더의 `Directory.Build.props`에 아래 설정을 추가합니다.
앱 프로젝트 파일에만 설정하지 말고 러너에서도 읽을 수 있는 위치에 둡니다.

```xml
<PropertyGroup>
  <DorotiObfuscate Condition="'$(Configuration)' == 'Release'">true</DorotiObfuscate>
</PropertyGroup>
```

## 적용 경로

모든 플랫폼은 동일한 러너 정책을 사용합니다. **컴파일 모드를 자동으로 바꾸지는 않습니다.**
`PublishAot`, `DorotiCompilationMode=NativeAot`, `RunAOTCompilation`,
`AndroidEnableProfiledAot`, `AotAssemblies`, `_RunAotCompiler` 중
하나라도 NativeAOT·Mono/WASM AOT를 선택하면 건너뜁니다. Debug·개발 빌드도 건너뜁니다.
`PublishReadyToRun=true`는 허용하며, NativeAOT 등 제외 조건과 함께 지정하면 여전히 제외합니다.
선택한 러너의 판정이 앱 `ProjectReference`에 전달되므로 전역 opt-in이 AOT 앱에 새지 않습니다.

| 플랫폼 | 난독화 대상 |
| --- | --- |
| Windows App SDK / Windows MAUI | managed Release 및 ReadyToRun. NativeAOT 제외 |
| Android | Mono/CoreCLR JIT 및 CoreCLR ReadyToRun Release. Mono AOT·profiled AOT 제외 |
| Web | `RunAOTCompilation=false`인 Release |
| Linux Qt | managed Release 및 ReadyToRun. NativeAOT 제외 |
| macOS AppKit | managed Release 및 ReadyToRun. NativeAOT 제외 |
| iOS / Mac Catalyst | AOT가 없는 CoreCLR 경로만 대상. Mono는 혼합 interpreter/AOT를 포함해 보수적으로 제외 |

iOS 실기기의 기본 NativeAOT Release는 계속 난독화를 건너뜁니다. 이 설정은
플랫폼에 없던 JIT 배포 경로를 새로 제공하지 않습니다.

컴파일 → 앱 DLL 난독화 → 러너 참조 복사 → 플랫폼 트리밍 → 선택적 ReadyToRun → 패키징 순서입니다.
R2R에는 IL과 네이티브 코드가 함께 포함됩니다. 이 구성은 난독화된 IL에서 둘을 생성합니다.
[ReadyToRun 공식 설명](https://learn.microsoft.com/en-us/dotnet/core/deploying/ready-to-run).
R2R 입력 DLL의 경로·SHA-256과 난독화 모드를 기록하여, 옵션을 켰다 꺼도 이전
네이티브 이미지가 재사용되지 않도록 합니다. 같은 게시 폴더에 다시 난독화하여
게시할 때는 이전 일반 게시의 앱 PDB도 제거합니다.
플랫폼 호스트, 바인딩 DLL, Doroti 프레임워크, 외부 패키지, JavaScript는 변경하지 않습니다.
이름 변경은 공개 API를 유지하며 내부 타입·메서드·필드를 대상으로 합니다.
프로퍼티·이벤트 이름 변경과 문자열 숨김은 기본적으로 끕니다.

앱의 난독화 빌드 출력은 일반 출력 아래 `obfuscated` 경로에 분리됩니다.
예를 들어 `bin/Release/obfuscated/net10.0/`에 앱 DLL이 생깁니다.
원본 컴파일 DLL은 `obj/Release/obfuscated/net10.0/`에 유지하고, 난독화된 중간 DLL과
`mapping.xml`, 실제 실행한 `obfuscar.xml`은 그 아래 `Doroti.Obfuscar/`에 생성합니다.
매핑 파일은 오류 분석용 내부 자료이며 배포하지 않습니다. 원본 PDB도 난독화 DLL과
일치하지 않으므로 복사하지 않습니다. 옵션 변경 후에는 반드시 빌드하여 게시합니다.
난독화 게시에서 `--no-build`는 오류로 중단합니다. `--artifacts-path`를 사용하면
앱 출력 pivot에 `_obfuscated`를 붙이고 원래 artifacts 루트를 유지합니다.

## 예외 규칙

문자열로 찾는 타입·메서드, 리플렉션 기반 JSON 모델, 이름에 의존하는 바인딩에는
예외가 필요합니다. 공개 API 유지가 이러한 동작을 모두 보장하지는 않습니다.
JSON source-generation context는 기본 예외에 포함됩니다. 공용 앱 DLL에 JS/native
import·export가 있다면 해당 타입·메서드도 이름을 유지하도록 예외를 지정합니다.
코드에서는 표준 특성을 사용할 수 있습니다.

```csharp
[System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
internal sealed class ReflectedModel
{
    public string Name { get; set; } = "";
}
```

앱 외부 규칙 파일을 사용할 수도 있습니다. 경로는 공용 앱 프로젝트 폴더 기준이며,
아래 파일은 `ObfuscarRules` 루트와 `Skip*` 요소만 허용합니다.

```xml
<!-- obfuscar-rules.xml -->
<ObfuscarRules>
  <SkipType name="MyApp.Contracts.*" skipMethods="true" skipFields="true"
            skipProperties="true" skipEvents="true" />
</ObfuscarRules>
```

```xml
<PropertyGroup>
  <DorotiObfuscarRules>obfuscar-rules.xml</DorotiObfuscarRules>
  <!-- 선택 사항: 문자열 숨김도 적용할 경우 -->
  <DorotiObfuscarHideStrings>true</DorotiObfuscarHideStrings>
</PropertyGroup>
```

강력한 이름으로 서명한 앱 DLL은 재서명이 필요하므로 이 통합에서는 오류로 중단합니다.
도구 복원·실행·규칙 검증에 실패해도 빌드를 중단합니다. 난독화는 역분석 비용을
높이는 수단이며 비밀키 보호 수단은 아닙니다.

## 검증

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/obfuscar_contract.py
```

실제 Obfuscar를 실행해 내부 이름 변경, 공개 API 호출, 리플렉션 예외, 반복 빌드,
옵션 해제, 실제 R2R 게시·헤더·실행, NativeAOT/Mono/WASM AOT 제외 및 잘못된 규칙의
실패 전파를 확인합니다. 플랫폼 정책 평가는
네이티브 패키징·서명·실기기 실행의 증거와 별개입니다.

2026-10-13 검증 결과:

- **PASS**: 실제 Testbed 공용 앱 Release 빌드와 Obfuscar 실행. 플랫폼 UI 실행 검증은 제외.
- **PASS**: 테스트 앱의 내부 타입·메서드 이름 변경, 공개 API·리플렉션 예외 실행,
  반복 빌드·옵션 해제, artifacts 출력 분리, `--no-build` 및 잘못된 규칙의 오류 전파.
- **PASS**: 테스트 앱 게시 결과 실행. 원본 앱 PDB·매핑·설정 파일이 게시 폴더에 없음을 확인.
- **PASS**: 실제 8개 러너의 Release 설정 평가와 앱 참조 속성 전달.
  Web·Android의 명시적 non-AOT 경로도 평가.
- **PASS**: Windows MAUI의 기본 `PublishReadyToRun=true`에서 난독화 활성화와 앱 참조 전달 평가.
- **PASS**: `win-x64` 테스트 앱의 실제 R2R 게시. 최종 앱 DLL의 R2R 서명과 내부 이름 변경,
  공개 API·리플렉션 예외 실행을 확인. 난독화 켬 → 끔 → 켬을 같은 게시 폴더에서
  수행하여 캐시 갱신과 원본 앱 PDB 제거도 확인.
- **PASS**: App/Runner SDK 패키지 생성과 난독화 파일 포함, 터널 인자 전달과 옵션 충돌 검증.
  터널 검증의 Docker 명령은 모의 실행.
- **notVerified**: 실제 플랫폼 앱의 난독화 배포 패키징·서명·실기기·브라우저 UI 실행,
  Composite R2R 및 Windows x64 외 R2R 실행.

공식 자료: [Obfuscar](https://github.com/obfuscar/obfuscar),
[설정과 예외](https://docs.lextudio.com/obfuscar/getting-started/configuration),
[고정 버전](https://www.nuget.org/packages/Obfuscar.GlobalTool/2.2.50).

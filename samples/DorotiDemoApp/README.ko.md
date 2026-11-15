# DorotiDemoApp

[English](README.md) | **한국어**

`samples/DorotiTestbedApp`의 **Components** 페이지를 복사한 독립 Material 데모 앱입니다.
앱을 열면 컴포넌트 갤러리가 바로 표시됩니다. 라이트/다크 전환, 화면 폭에 따른 1열·2열 배치,
다이얼로그, 드로어, 입력 컨트롤, 로컬·URL 이미지 데모를 포함합니다.

저장소 루트의 PowerShell에서 실행합니다.

```powershell
# Windows App SDK
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiDemoApp -Platform windows

# Web
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiDemoApp -Platform web
```

Android, iOS, Linux Qt, macOS AppKit, Mac Catalyst 러너도 포함합니다.
해당 플랫폼의 빌드 환경에서 `-Platform`에 `android`, `ios`, `linux`, `macos`,
`maccatalyst`를 지정합니다. 플랫폼별 준비 사항은 [프레임워크 가이드](../../Doroti/README.ko.md)를 참고하세요.

Web 게시의 기본값은 net10/threaded입니다. `dotnet publish`에
`-p:DorotiWebExecutionMode=auto`를 추가하면 두 variant 전체를, `compatible`을
지정하면 비격리용 단일 배포를 만듭니다. 모드 변경 후에는 run/watch를 다시 시작하고
페이지를 새로 불러옵니다. compatible Hot Reload는 미인수입니다.
[Web 실행 프로필과 마이그레이션](../../packages/platforms/web/README.md#execution-profiles-opt-in)을 확인하세요.

Android·iOS의 빌드·설치·실행은 [공용 배포 도구](../../helpers/deploy-helper/README.md)에서
`-App Demo`로 선택할 수도 있습니다.

공용 앱 또는 Windows 러너만 빌드하려면 다음 명령을 사용합니다.

```powershell
dotnet build ./samples/DorotiDemoApp/DorotiDemoApp.csproj -c Debug
dotnet build ./samples/DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Debug
```

`src/Components/{Components,Selection,Decorations,ImageDemo}.cs`와 Roboto 폰트·로컬 이미지는
Testbed 페이지와 리소스를 독립적으로 복사했습니다. 플랫폼 실행 구조는 `DorotiCarouselApp`을 따릅니다.
Flutter 라이선스는 `src/Components/LICENSE.flutter`, Roboto 라이선스는
`assets/fonts/LICENSE.roboto.txt`에 있습니다.

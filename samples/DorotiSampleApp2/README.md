# DorotiSampleApp2

Cupertino 스타일의 독립 Doroti 샘플 앱입니다. 공통 C# UI와 Windows / Web 실행 프로젝트로 구성됩니다.

- **Components**: 카운터 버튼, 스위치, 슬라이더, 활동 표시기, 다이얼로그
- **Profile**: 이름 입력과 인사말, 탭 전환 시 입력 상태 유지
- **Settings**: 시스템 / 라이트 / 다크 테마 선택
- **Variable Blur**: 60개 항목의 ListView 위에 상단 고정 VariableBlur 오버레이, 강도 조절과 켜기/끄기

Variable Blur 페이지는 리스트 상단 180px에 `BackdropFilter`와
`ImageFilterConfig.CreateVariableBlur(startSigma: 강도, endSigma: 0, resolutionScale: 0.25)`를 적용합니다.
라디오 버튼으로 다음 네 가지 모드를 비교할 수 있습니다.

- **Full quality**: 전체 구간을 원본 해상도로 처리합니다.
- **Adaptive** (기본): 약한 블러는 원본 해상도, 강한 블러는 1/2·1/4 해상도로 처리하고 경계를 혼합합니다.
- **Fast adaptive**: Adaptive에 7회 샘플링 근사 커널을 적용합니다. 강한 블러에서 품질이 낮아질 수 있습니다.
- **Fixed 1/4**: 전체 구간을 1/4 해상도로 처리합니다. 선명한 구간도 저해상도가 됩니다.

별도의 스위치로 블러 전체를 켜고 끕니다.
위쪽은 흐리고 아래쪽은 선명하며, 블러 영역에서도 리스트를 스크롤할 수 있습니다.
구현은 [src/VariableBlurPage.cs](src/VariableBlurPage.cs)에 있습니다.
VariableBlur에는 GPU 렌더러가 필요하며, 기본 CPU 래스터 검증에는 이 탭의 블러 렌더링이 포함되지 않습니다.

설정과 입력값은 앱 실행 중에만 유지됩니다. Cupertino Icons 1.0.9 폰트와 해당 라이선스는 `assets/fonts`에 포함되어 있습니다.

저장소 루트 `DorotiLab`에서 실행합니다. 루트 `global.json`에 지정된 .NET SDK와 플랫폼별 빌드 도구가 필요합니다.

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

워크스페이스 CLI에서도 `-App ./samples/DorotiSampleApp2 -Platform windows` 또는 `-Platform web`으로 선택할 수 있습니다.

화면 구현은 [src/App.cs](src/App.cs), 공통 진입점은 [Program.cs](Program.cs)에 있습니다.

## 검증

```powershell
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --portrait
```

실제 위젯과 Skia 렌더러로 720×840 / 400×800 화면을 그려 카운터, 탭 간 상태 유지,
입력 콜백, 다이얼로그 배치, 테마 전환, 활동 표시기 애니메이션을 검증합니다.
탭 전환은 좌표 기반 마우스 / 터치 포인터 이벤트를 위젯 입력 경로로 전달해
히트 테스트, 선택된 탭 번호, 표시된 페이지와 같은 탭 재선택까지 확인합니다.
PNG는 검증 프로젝트의 `bin/Release/net10.0/snapshots`에 생성됩니다.
이 검증에는 실제 OS 입력 및 Windows / 브라우저 화면 표시 확인은 포함되지 않습니다.

Variable Blur는 Vulkan GPU가 있는 환경에서 별도로 검증합니다.

```powershell
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release -- --variable-blur
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --portrait
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --high-dpi --oversized-backing
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --high-dpi --oversized-backing --2560x1600 --frame-benchmark
```

블러 영역에서 합성 트랙패드 pan/zoom을 시작해 종료 이벤트 전의 리스트 위치와
GPU 렌더링 픽셀 변화를 확인합니다. 실제 물리 트랙패드 입력 및 화면 표시 지연은 별도 확인 대상입니다.
`--frame-benchmark`는 프레임 구성과 GPU 완료까지 측정하며 창 표시 FPS가 아닙니다.
실제 Windows 창을 PowerShell로 조작하는 측정 절차와 결과는
[VariableBlur 성능 기록](../../history/26-09-26/wgsl-gpu-effects-summary.md)에 있습니다.


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

# Web 게시와 폰트 설정

DorotiSampleApp2의 Web 배포, 렌더러 비교, 로컬 폰트 추가 방법입니다.
명령은 저장소 루트 `DorotiLab`에서 실행합니다.

## WASM AOT 비교 실행

Mono WASM AOT는 Release `publish`의 기본값입니다. 저장소 루트에서 비교용 산출물을
분리해 게시하고, 아래 서버를 실행한 터미널을 열어 둡니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --artifacts-path ./temp/testing/sample2-web-aot
python Doroti/eng/serve-isolated-web.py ./temp/testing/sample2-web-aot/publish/DorotiSampleApp2.Web/release/wwwroot --port 5218
```

[AOT WebGL2](http://127.0.0.1:5218/?dorotiRenderer=worker-direct-webgl) 또는
[AOT WebGPU](http://127.0.0.1:5218/?dorotiRenderer=worker-direct-webgpu)를 엽니다.
서버는 WASM threads에 필요한 COOP/COEP 헤더와 `.mjs`/`.wasm` MIME을 제공합니다.

다른 터미널에서 비-AOT 비교 버전을 게시·실행할 수 있습니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:RunAOTCompilation=false --artifacts-path ./temp/testing/sample2-web-baseline
python Doroti/eng/serve-isolated-web.py ./temp/testing/sample2-web-baseline/publish/DorotiSampleApp2.Web/release/wwwroot --port 5219
```

[비-AOT WebGL2](http://127.0.0.1:5219/?dorotiRenderer=worker-direct-webgl)와 같은 화면에서 비교합니다.
두 명령 모두 기본 CDN 폰트 모드입니다. 로컬 폰트 비교가 필요하면 두 publish 명령에 모두
`-p:DorotiSampleWebFontSource=Assets`를 추가합니다. AOT는 배포 파일 크기를 늘립니다.

## Web 배포와 폰트

Docker 기반 익명 HTTPS 터널로 다른 기기에서 열려면 다음을 실행합니다.

```powershell
pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 -App ./samples/DorotiSampleApp2 -OpenBrowser
```

Release 게시 후 접속 주소가 출력되고 브라우저가 열립니다. 종료는
`pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 stop`을 사용합니다.
필수 도구와 관리 명령은 [터널 안내](../../../tools/doroti-cloudflared/README.md)를 참고하세요.

웹 글꼴은 `web/WebFonts.cs`의 `PreloadLanguages = ["ko", "en"]` 힌트에 따라 첫 화면 전에
CDN에서 Roboto와 전체 Noto Sans KR을 로드합니다. 한글 자모와 11,172개 음절을 입력 전에
준비하므로 새로운 조합마다 폰트를 받느라 잠깐 사각형으로 표시되는 현상을 방지합니다.
그 외 문자권과 컬러 이모지는 필요한 Noto 폰트 조각을 자동 다운로드합니다.
힌트를 비우면 Roboto만 미리 로드하는 기본 동작으로 돌아갑니다.
Flutter의 CanvasKit/Skwasm처럼 엔진에 폰트 파일을
등록합니다. CSS 링크의 @font-face도 바이트로 읽어 등록하며, DOM body의 font-family는 상속하지 않습니다. 폰트가 추가되면 글자 폭과
레이아웃도 자동 갱신됩니다. 네이티브의 기본/미해결 Cupertino 폰트는 플랫폼 UI 폰트로
연결합니다. CDN 변경·다운로드 비활성화와 지원 범위는
[자동 웹 폰트 안내](../../../packages/platforms/web/Doroti.Host.Web/Fonts/README.md)에 있습니다.
기본 CDN 모드는 Roboto/Noto와 Galmuri를 CDN에서 읽고, 비교용 SUITE는 로컬 CSS/WOFF2로 포함합니다. 네이티브 빌드는 Roboto를 CDN에서
받아 DLL에 포함하므로 실행 시에는 폰트 다운로드가 필요 없습니다.

웹에서도 선택적으로 기본 폰트를 앱 에셋에 포함할 수 있습니다:

```powershell
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:DorotiSampleWebFontSource=Assets
```

이 모드는 기존 샘플의 Roboto 3종과 라이선스를 웹 DLL에 포함하고, 기본 폰트 CDN 및
언어·이모지 자동 다운로드를 끕니다. 설정은 [web/WebFonts.cs](../web/WebFonts.cs)에 있습니다.
Fonts 탭의 Galmuri/SUITE와 디코더도 로컬로 포함하므로 이 화면은 외부 폰트 요청 없이 동작합니다. SUITE에 없는 한글은 Galmuri로 폴백합니다. 다른 문자/이모지가 필요하면 그 폰트도 직접 포함하세요.
옵션을 생략하면 기존 CDN 모드로 실행됩니다. 모드를 바꿀 때는 개발 서버를 재시작합니다.

## 웹폰트 비교

**Fonts** 탭에서 Roboto/Galmuri11/SUITE Variable을 전환합니다. 300/400/500/700/900
행과 연속 wght 슬라이더, 한글·영문·숫자, 편집 가능한 여러 줄 입력을 제공합니다.
Galmuri는 일반 HTML CSS 링크, SUITE는 원본 로컬 CSS/WOFF2 등록 예제입니다.
일반 모드의 디코더만 로컬로 묶으려면 `-p:DorotiBundleWoff2Decoder=true`를 사용합니다.
`Assets` 모드는 기본 폰트뿐 아니라 CSS와 디코더도 외부 요청 없이 제공합니다.

여러 줄 입력창은 첫 포커스와 재포커스 시 실제 입력 위치와 표시 커서를 일치시킵니다.
Flutter와 같이 iOS 터치는 단어 경계, Android·데스크톱은 누른 글자 위치를 선택합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:DorotiSampleWebFontSource=Assets -o Doroti/artifacts/sample2-fonts
```

`wwwroot`를 COOP/COEP 헤더가 있는 서버로 제공하세요. `/sample/` 배포에서는 HTML의
base href도 `/sample/`로 바꿉니다. 디코더 CSP와 CSS 지원 범위는
[폰트 사용 문서](../../../packages/platforms/web/Doroti.Host.Web/Fonts/README.md#css-links-and-variable-fonts)를 참고하세요.

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

# Web 개발 서버 설정

앱 루트(`doroti-workspace.json`과 같은 디렉터리)에 `web_dev_config.jsonc`를
만들면 Doroti Web 개발 서버가 읽습니다. SampleApp2와 `doroti-app` 템플릿에는
비어 있는 프록시 목록과 주석 예제가 들어 있습니다.
[Flutter 개발 서버 설정](https://docs.flutter.dev/platform-integration/web/web-dev-config-file)의
`server.proxy` 구조를 JSONC로 표현합니다. `//`와 `/* */` 주석 및 마지막
쉼표를 지원하며 .NET의 `System.Text.Json`으로 읽습니다.

```jsonc
{
  "server": {
    "proxy": [
      // /api/items -> http://localhost:5000/items
      { "target": "http://localhost:5000/", "prefix": "/api/", "replace": "/" },
      { "target": "http://localhost:3000/", "prefix": "/users/" },
      {
        "target": "http://localhost:5000/",
        "regex": "^/versioned/(v\\d+)/(.*)",
        "replace": "/$2?apiVersion=$1"
      },
    ],
  },
}
```

위 설정에서 `/api/items?page=2`는 `http://localhost:5000/items?page=2`로,
`/users/123`은 `http://localhost:3000/users/123`으로 전달됩니다.
`/versioned/v2/items?page=2`는
`http://localhost:5000/items?apiVersion=v2&page=2`로 전달됩니다.
앱에서는 개발 서버와 같은 origin의 `/api/...` URL을 사용합니다.
프록시는 서버에서 백엔드에 연결하므로 브라우저의 교차 origin 요청이 발생하지 않습니다.

규칙은 위에서부터 검사하며 처음 매칭된 규칙 하나를 사용합니다.
`prefix`는 대소문자를 구분하는 문자열 prefix이고, `regex`는 .NET 정규식입니다.
각 규칙은 둘 중 하나를 지정해야 합니다. `replace`를 생략하면 경로를 유지하고,
`replace: ""`는 매칭된 부분을 제거합니다. 정규식 치환에는 `$1`, `$2`를
사용하며, JSONC 문자열의 백슬래시는 위처럼 `\\`로 이스케이프합니다.
대상 URL의 base path와 원래 쿼리도 보존합니다.

HTTP 메서드, 요청 본문, 요청·응답 헤더, 쿠키, 상태 코드 및 WebSocket을
전달합니다. 백엔드의 redirect 응답은 그대로 브라우저에 전달합니다.
`Location`이나 `Set-Cookie`의 Domain/Path를 치환하지 않으므로 백엔드는
브라우저에 공개되는 경로를 기준으로 설정해야 합니다.
매칭되지 않은 요청은 정적 파일과 SPA fallback으로 이어집니다.
`/_framework`, `/_content`, `/_vs`, `/_blazor`는 런타임·디버거·Hot Reload가
소유하는 경로이므로 프록시보다 우선합니다.

## 실행

```powershell
dotnet run --project samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug
dotnet watch --project samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj run --configuration Debug
```

workspace CLI의 `run`/`dev`와 VS Code Run/Hot Reload도 같은 runner 실행
설정을 사용합니다. 설정 변경 후에는 개발 서버를 재시작합니다.
설정 파일이 없으면 기존 Blazor 개발 서버를 사용합니다.
기존 프로젝트에서 패키지 방식으로 쓰려면 이 기능이 포함된 `Doroti.Tool.Web`을
Web runner에 참조해야 합니다. 저장소의 Web bootstrap은 소스 구현을 연결합니다.

다른 파일을 선택하려면 runner 디렉터리를 기준으로 한 경로 또는 절대 경로를
`DorotiWebDevConfig`에 지정합니다. 명시한 파일이 없거나 설정이 잘못되면
실행을 실패시키고 원인을 출력합니다.

```powershell
dotnet run --project samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug -p:DorotiWebDevConfig=../web_dev_config.local.jsonc
```

`-p:DorotiWebDevServer=false`로 기존 개발 서버를 선택할 수도 있습니다.
`Doroti.Tool.Web`은 개발 서버의 `.csproj`와 C# 소스만 포함합니다.
runner 실행 시 서버의 일반 NuGet `PackageReference`를 복원하고,
앱의 `.doroti/cache/web-devserver/<SDK 버전>`에 서버를 로컬 빌드합니다.
첫 실행에는 복원·컴파일 시간이 추가되고 이후에는 증분 빌드를 사용합니다.
외부 DLL을 Doroti 패키지에 다시 묶지 않으며, NuGet 패키지 디렉터리에는
빌드 산출물을 쓰지 않습니다. `DorotiWebDevServerCache`로 캐시 위치를
바꿀 수도 있습니다.
개발 서버와 의존성은 앱의 browser-wasm 런타임 참조나
`dotnet publish`의 정적 사이트에 포함하지 않습니다.
게시한 사이트의 프록시는 실제 운영 서버에서 별도로 설정합니다.

## 주소, HTTPS 및 응답 헤더

```jsonc
{
  "server": {
    "host": "127.0.0.1",
    "port": 8080,
    "https": {
      "certPath": "certs/localhost.pem",
      "certKeyPath": "certs/localhost-key.pem"
    },
    "headers": [{ "name": "X-Development", "value": "Doroti" }],
    "proxy": [],
  },
}
```

PEM 인증서와 키 경로는 설정 파일 디렉터리를 기준으로 해석합니다.
주소 우선순위는 명령행 `--urls` → JSONC host/port/https →
`launchSettings.json`의 `applicationUrl`/`ASPNETCORE_URLS` → 기본 주소
`http://localhost:5000`입니다. JSONC에서 생략한 host/port는 기본 주소의 값을
사용합니다. 명령행 주소는 다음처럼 runner 인자 뒤에 전달합니다.

```powershell
dotnet run --project samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug -- --urls http://127.0.0.1:8081
```

`WasmEnableThreads=true`이면 COOP `same-origin`과 COEP `require-corp`를
정적 파일과 프록시 응답에 적용합니다. 이 두 값은 사용자 헤더보다 우선하여
WASM 스레드의 cross-origin isolation을 유지합니다.

## 검증

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/platforms/web/tests/Doroti.Web.DevServer.Tests/Doroti.Web.DevServer.Tests.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_dev_proxy.py
```

첫 테스트는 설정 검증과 실제 HTTP/WebSocket 전달을 확인합니다.
두 번째는 서버 소스만 포함하는 도구 패키지, 앱별 로컬 서버 빌드,
소스 및 격리된 NuGet 소비 앱의 실행, Blazor 정적 manifest,
SPA fallback, 헤더, 주소 우선순위, 기본 서버 선택, PEM HTTPS와 SDK
browser-refresh 스크립트 주입을 확인합니다.
Developer/Release 검증에도 포함됩니다.

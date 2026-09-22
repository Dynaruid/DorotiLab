# Doroti 웹 HTTPS Quick Tunnel

DorotiTestbedApp을 Release로 게시하고 Docker의 Nginx와 Cloudflare 익명
Quick Tunnel로 제공합니다. 다른 기기는 출력된 HTTPS 주소를 열면 됩니다.
Cloudflare 계정, 도메인, 토큰, 공유기 포트 포워딩, 기기별 인증서 설치는 필요 없습니다.

참고: `C:\Users\parti\Labo\my_sudoku_v2\habitat-cloudflared`의 익명 터널 실행과
로그 기반 URL 추출 방식. 여기서는 Docker Compose가 두 서비스를 직접 관리합니다.

## 시작

필수: PowerShell 7, Docker Desktop의 Linux containers, 기존 Doroti 웹 빌드용
.NET SDK 및 WASM 도구. 빌드는 호스트에서 기존 `Doroti/eng/doroti.ps1 publish`로
수행하고, Docker는 게시된 정적 파일과 터널을 실행합니다.

저장소 루트에서 실행합니다.

```powershell
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start
```

스크립트는 게시 → Nginx 상태 확인 → 익명 터널 연결 → 공개 HTTPS 페이지와
COOP/COEP 헤더 및 워커/WASM MIME 확인 후 주소를 출력합니다. 주소는 이 폴더의
`generated-url.txt`에도 저장됩니다. 컨테이너는 명령이 끝나도 실행됩니다.
시작이 실패하면 이 도구의 컨테이너를 정리합니다.

```powershell
# 이미 게시된 Release 파일을 재사용 (소스 변경을 반영하지 않음)
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 start -SkipPublish

# 현재 주소 / 컨테이너 상태 / 실시간 로그
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 url
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 status
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 logs

# 공유 종료: 웹 서버, 터널, 전용 네트워크와 URL 파일 정리
pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 stop
```

소스 수정 후에는 `start`를 다시 실행합니다. 기존 미리보기를 중지하고 다시
게시하므로 잠시 연결이 끊기며 새 터널 주소가 발급됩니다. `-SkipPublish`는
기존 컨테이너가 정상 실행 중이면 그대로 재사용합니다.

## 구성

```text
다른 기기의 브라우저
  → https://<임의 이름>.trycloudflare.com
  → Docker cloudflared
  → Docker Nginx:80
  → DorotiTestbedApp/web/bin/Release/net10.0/publish/wwwroot
```

- Nginx는 게시 결과를 읽기 전용으로 마운트합니다.
- `Cross-Origin-Opener-Policy: same-origin` 및
  `Cross-Origin-Embedder-Policy: require-corp`를 페이지·JS·WASM·워커 응답에
  붙입니다. HTTPS와 함께 Doroti의 WASM 스레드/`SharedArrayBuffer` 조건을 충족합니다.
- `.wasm` MIME은 Nginx 기본 `mime.types`의 `application/wasm`을 사용합니다.
  `.mjs` 워커는 별도로 `application/javascript`를 지정합니다. 게시된 `.gz`
  파일은 요청의 `Accept-Encoding`에 따라 제공됩니다.
- 로컬 확인 주소는 `http://localhost:5089`입니다. 기존 `doroti.ps1 run`의
  5088 포트 및 `launchSettings.json`과 독립적입니다.
- 호스트 포트 충돌 시 `$env:DOROTI_PREVIEW_PORT = '5090'`을 설정하고 실행합니다.
  같은 환경에서 후속 관리 명령을 실행하세요.
- 이미지: `nginx:1.29-alpine`, `cloudflare/cloudflared:2026.8.2`.
  cloudflared는 UDP 통신 환경에 영향을 덜 받도록 HTTP/2를 사용합니다.
- 시작 시 공개 HTTP 응답만 자동 확인합니다. 실제 기기의 렌더링·터치·브라우저
  WebGPU 지원 여부는 그 기기에서 별도로 확인해야 합니다.

Quick Tunnel은 인터넷에 공개되는 임시 테스트 URL이며 주소를 아는 사람은
접속할 수 있습니다. 같은 공유기에 있지 않아도 접속됩니다. 터널 재생성 시 주소가
바뀌고, 가용성 보장 없이 동시 진행 요청 200개 제한 및 SSE 미지원 제약이 있습니다.
테스트 후에는 `stop`으로 공유를 종료하세요.

공식 문서: [Cloudflare Quick Tunnels](https://developers.cloudflare.com/cloudflare-one/networks/connectors/cloudflare-tunnel/do-more-with-tunnels/trycloudflare/),
[SharedArrayBuffer 조건](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/SharedArrayBuffer).

# WASM threads 활성화 검증

날짜: 2026-09-08. 범위: work3.md 작성과 Testbed Web의 WasmEnableThreads 활성화.
병렬 레이아웃 엔진 구현이나 성능 수용 기록은 아니다.

## 결과

- 설정: `DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj`에 `WasmEnableThreads=true`.
- Release build: PASS, 2:00.23, warnings/errors 0.
- evaluated: WasmBuildNative=true, WasmEnableThreads=true, WasmEnableSIMD=true,
  RunAOTCompilation 미설정, `_SkiaSharpNativeBinaryType=mt,simd`.
- runtime pack: Microsoft.NETCore.App.Runtime.Mono.multithread.browser-wasm 10.0.11.
- Skia native: 4.152.0-rc.1.26426.14, `3.1.56/mt,simd/libSkiaSharp.a`.
- dotnet.native.worker.mjs 생성과 native build의 `--enable-threads` 확인.
- COOP same-origin, COEP require-corp, document crossOriginIsolated=true: PASS.
- Chromium 151.0.7922.34 bootstrap: **FAIL**. 원본 오류는 동봉 JSON에 보존.
- first content 미도달; resize/wheel, runtime API shared heap, managed 계산 중첩,
  실제 레이아웃 병렬화/성능 및 물리 기기 수용: **notVerified**.

최초 오류는 Doroti protocol version `undefined`와 .NET의
`mono_wasm_pthread_on_pthread_attached`에서 undefined `dispatchEvent` 접근이다.
120초 first-content 대기가 만료됐다. pthread 제어 메시지와 host protocol의 경계,
Worker 안에서 root runtime을 시작하는 초기화 경로를 별도로 조사해야 한다.
메시지 envelope를 실측하지 않았으므로 이것을 확정된 단일 원인으로 기록하지 않는다.
work3.md T0에 최소 재현·메시지 분리·runtime 호환성·재검증 순서를 기록했다.

## 실행 ledger와 재현

이번 검증 실행은 build 1회 + browser 1회 = **2회**, 자동 retry 0.
두 실행 모두 `invoke-work2-check.ps1`의 외부 20분 timeout을 사용했다.
browser 내부 first-content 대기는 120초다. 아래 명령은 저장소 루트 기준이다.

```powershell
& ./Doroti/eng/invoke-work2-check.ps1 -Label work3-threads-build -Command @(
  'dotnet', 'build', 'DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj',
  '-c', 'Release', '--artifacts-path', '.doroti/work3-threads/artifacts', '--nologo')
```

성공한 build의 정적 자산을 freeze하고 별도 터미널에서 로컬 헤더 서버를 실행한다.

```powershell
python Doroti/validation/web-playwright/freeze-build.py .doroti/work3-threads/artifacts/obj/DorotiTestbedApp.Web/release/staticwebassets.build.json .doroti/work3-threads/wwwroot
python Doroti/eng/serve-isolated-web.py .doroti/work3-threads/wwwroot --port 5192
```

브라우저 검증은 서버 실행 후 별도로 실행한다. 재실행 시 기존 label을 덮어쓰지 않는다.

```powershell
$env:DOROTI_WEB_BASE_URL = 'http://127.0.0.1:5192'
& ./Doroti/eng/invoke-work2-check.ps1 -Label work3-threads-browser -Directory Doroti/validation/web-playwright -Command @(
  'node', 'probe-threaded-bootstrap.mjs', 'enabled-1')
```

실행 당시 로컬 자료:

- `.doroti/work3-threads/evaluated.json`: evaluated 설정과 native archive 경로.
- `.doroti/work3-threads/wwwroot-manifest.json`: freeze한 564 endpoints의 자산 기록.
- `Doroti/artifacts/framework-web-work2/work3-threads-build.log` 및 `.err.log`.
- `Doroti/artifacts/framework-web-work2/work3-threads-browser.log` 및 `.err.log`.
- `Doroti/validation/web-playwright/artifacts/threads-bootstrap/enabled-1.json`.
- [원본 browser FAIL 사본](wasm-threads-bootstrap-failure.json).

원본과 사본의 SHA-256 일치 확인:
`5AA55B7730263605B43C4B0C030947654DE22BE459C1A02087D5EF91265DE40A`.
검증 종료 후 이번 임시 서버를 종료하고 5192 listener가 없음을 확인했다.

기존 단일 스레드 S0는 `.doroti/c234/before-wwwroot`에 보존했다.
이번 threading 빌드를 기존 정상 비교 서버에 덮어쓰지 않았다.
flag는 사용자 요청대로 true이며, 현재 새 threaded build의 부팅 실패를 숨기거나
렌더러 변경·generated runtime 패치·flag false로 우회하지 않았다.

# DorotiSampleApp2 Mono WASM AOT 확인 — 2026-10-05

후속 변경: 사용자 요청으로 Web Release 게시 기본값을 AOT로 바꿨다.
[기본값 변경 검증](2026-10-05-web-aot-default.md)을 참고한다. 아래는 선택 실행으로 검증한 당시의 기록이다.

**AOT·비-AOT Release 게시 PASS. AOT WebGL/WebGPU와 비-AOT WebGL의 기본 상호작용 PASS.**
SampleApp2를 직접 비교하기 위해 별도 게시물과 loopback 서버를 만들었다.
이 샘플의 성능 개선율·표시 FPS는 측정하지 않았다. 앞선 Testbed 수치를 적용하지 않는다.
정확한 source/payload hash와 browser 결과는 [receipt](2026-10-05-sample2-web-wasm-aot.json)에 있다.

## 실행 구성

- source HEAD: `1d1f398b579dd6a75c9d9ddde686c1b5c48d3c1f`. 기존 Testbed 실험의 문서·probe 변경은 보존했다.
  제품 소스, Web 기본 AOT 설정, 폰트 설정은 변경하지 않았다.
- SDK 10.0.400, Mono/Emscripten/AOT pack 10.0.11, Windows x64, Chrome 154.0.8037.93 headless.
- 두 버전 모두 Release trimmed, native relink, threads/SIMD enabled, 기본 **CDN 폰트 모드**다.
  AOT만 `RunAOTCompilation=true`; 비-AOT는 명시적 `false`. artifacts directory를 분리했다.
- 수정하지 않은 stock compiler로 게시했다. `driver-gen.c`에서 SampleApp2 앱과 Web 진입점,
  Cupertino·Framework를 포함한 **53개 AOT module 등록**을 확인했다.

## 검증

720×840, DPR 1의 새 browser context를 각 조합에 한 번씩 사용했다.

| 조합 | 결과 |
| --- | --- |
| 비-AOT / WebGL2 | PASS |
| AOT / WebGL2 | PASS |
| AOT / WebGPU | PASS |
| 비-AOT / WebGPU | notVerified |

세 실행에서 bootstrap 완료, 선택 renderer, cross-origin isolation와 실제 shared WASM heap을
확인했다. 실제 canvas pointer 입력으로 `Tap me` 카운터를 1로 올리고, Components·Profile·
Fonts·Variable Blur·Upload·Settings 여섯 탭의 화면 진입을 확인했다.
Profile에서 실제 `doroti-ime` 포커스를 기다린 뒤 `AOT 한글`을 합성 입력하고 인사말 반영,
Done 후 blur, 다른 탭을 방문한 뒤 입력과 카운터 상태 유지를 확인했다.
모든 실행의 page/console error 및 request failure는 0개였다.

AOT WebGL Components·Fonts와 AOT WebGPU Variable Blur 캡처를 시각적으로 확인했다.
SUITE의 한글·영문·굵기 비교와 스크롤 후 blur 화면이 표시됐다. 정확한 픽셀 동등성,
폰트 전체 범위 또는 shader 품질 비교 판정은 아니다. Upload는 페이지 진입만 확인했고
실제 file picker/drop/read는 이번 범위에서 검증하지 않았다.

## 게시 크기

| native WASM, decimal MB | 비-AOT | AOT |
| --- | ---: | ---: |
| raw | 9.66 | 40.37 |
| Brotli 압축본 | 2.70 | 8.44 |
| gzip 압축본 | 3.62 | 12.98 |

이는 `dotnet.native.*.wasm` 한 파일의 크기다. 전체 초기 다운로드 크기가 아니며,
기본 CDN 폰트 요청도 별도로 존재한다. 검증 서버는 raw 파일을 제공한다.
CPU/GPU 시간, 실제 표시 FPS, 네트워크 cold start, 물리 IME·보조공학·모바일·장기 안정성은
이번 샘플에서 notMeasured/notVerified다.

## 재현과 원시 자료

[SampleApp2 README의 WASM AOT 실행 방법](../../../samples/DorotiSampleApp2/README.md#wasm-aot-비교-실행)을 따른다.
이번 게시 명령은 다음과 같으며 모두 **1,200초 process-tree timeout**으로 실행했다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/web-aot/2026-10-05/sample2-aot -p:RunAOTCompilation=true
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/web-aot/2026-10-05/sample2-baseline -p:RunAOTCompilation=false
```

`serve-isolated-web.py`로 AOT는 5218, 비-AOT는 5219에 제공했다. 실제 검증은 Playwright 환경의
`temp/testing/platform-audit/all/venv/Scripts/python.exe`와 저장된 `sample2_smoke.py`로 실행했다.
각 게시와 smoke 1회, 자동 retry 0이다. 원시 자료는 삭제 가능한
`temp/testing/web-aot/2026-10-05/`의 `sample2-aot.log`, `sample2-baseline.log`,
`sample2-payload.json`, `sample2-*-manifest.json`, `sample2_smoke.py`, `sample2-smoke-r1/`에 있다.
이 문서와 receipt는 원시 자료를 정리해도 남는다.

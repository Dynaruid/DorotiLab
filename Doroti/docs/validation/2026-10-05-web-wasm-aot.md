# Web Mono WASM AOT 실험 — 2026-10-05

후속 변경: 사용자 요청으로 Web Release 게시 기본값을 AOT로 바꿨다.
[기본값 변경 검증](2026-10-05-web-aot-default.md)을 참고한다. 아래는 기본값 변경 전의 실험 기록이다.

**Release AOT 게시·실제 WebGL/WebGPU 실행 PASS. 제한된 WebGL CPU 비교에서 개선 관측.**
DorotiTestbedApp의 일반 실행 기본값은 유지하고 AOT/비-AOT 게시물을 분리했다.
전체 성능·모바일·물리 입력 수락은 아니다. 정확한 bytes, hash, 개별 trial과
실패 이력은 [receipt](2026-10-05-web-wasm-aot.json)에 보존한다.

## 게시와 실행

- source HEAD: `1d1f398b579dd6a75c9d9ddde686c1b5c48d3c1f`. 빌드 당시 working tree는 clean이었다.
  이번 변경은 실행 방법·검증 스크립트·기록이며 제품 코드나 기본 AOT 설정 변경은 없다.
- Windows x64, Ryzen 7 8845HS, SDK **10.0.400**, Mono/Emscripten runtime pack **10.0.11**,
  Chrome **154.0.8037.93** headless. main-owned threaded/shared-runtime render Worker.
- AOT/비-AOT 모두 Release trimmed, `WasmBuildNative=true`, threads/SIMD enabled.
  차이는 명시적 `RunAOTCompilation=true` / `false`이며 각각 별도 artifacts directory다.
- AOT는 SDK 기본 `WasmStripILAfterAOT=true`. compiler override와 partial AOT는 사용하지 않았다.
  stock compiler의 stack reserve **8MiB**를 수정하지 않고 게시가 완료됐다.
- `driver-gen.c`의 **55개 AOT module 등록**, Material과 Framework의 compiled-method token
  목록을 확인했다. native relink만으로 AOT라고 판정하지 않았다. 이는 Mono WASM AOT이며
  모든 인터프리터 경로가 제거되는 CoreCLR NativeAOT와 같지 않다.

9월의 [CanvasKit/AOT 실패 기록](../../../history/26-09-05/web-canvaskit-redesign-v2-results.md)은
다른 소스·토폴로지의 결과다. 이번 stock full-eligible AOT 게시와 현재 SkiaSharp Web host
실행 성공이 그 과거 실패를 원복하거나 원인을 확정하지는 않는다.

| 검증 | 결과와 범위 |
| --- | --- |
| AOT Release publish | PASS, stock compiler, 55/55 assembly AOT 및 native link 완료 |
| 비-AOT Release publish | PASS, 같은 source와 SDK, 별도 산출물 |
| AOT WebGL / WebGPU | PASS, 실제 Material 화면, cross-origin isolation와 shared WASM heap 확인 |
| 스크롤·크기 변경 | PASS, 합성 wheel 후 semantics 위치/노드 변화와 logical viewport·navigation 위치 변경 확인 |
| 텍스트·선택 | PASS, 실제 `doroti-ime` 포커스 후 `AOT 한글` 입력 및 Home/Shift+Right 선택 범위 확인 |
| warm progress CPU | PASS 실행, 3쌍 AB/BA/AB, 각 trial numeric ring drop 0 및 browser error 0 |
| 실제 표시 FPS·물리 IME·모바일 | notMeasured / notVerified |

## 제한된 CPU 비교

두 게시물을 고정한 상태에서 1000×800, DPR 1, WebGL2의 Components → Communication →
Progress indicators를 같은 위치에 놓고 재생했다. 2.5초 warm-up 후 6초 구간을
세 쌍 AB/BA/AB로 측정했다. 최초 screenshot에서 동일한 화면 배치와 재생 상태를 확인했다.
메인/Worker의 기존 opt-in `dorotiFrameCost=1` 숫자 ring을 사용하며 측정 중 build는 없었다.

아래 값은 **trial별 p95의 중앙값**이다. 프레임 콜백에는 동기 WebGL raster가 중첩되므로
두 행을 합하면 안 된다. 합계 CPU는 interval union으로 별도 보존했다.

| CPU 구간 | 비-AOT | AOT | 변화 |
| --- | ---: | ---: | ---: |
| Framework frame callback, 동기 raster 포함 | 20.10ms | 11.25ms | −44.0% |
| managed Skia raster/submit | 10.10ms | 4.91ms | −51.4% |

각 쌍의 콜백 p95는 **20.10→11.25**, **20.19→11.45**, **19.31→11.16ms**였다.
이 화면에서 managed 실행 비용을 줄일 가능성이 충분히 보이므로 AOT를 비교 후보로 유지한다.
정확히 같은 logical target/tick/animation phase를 대응한 결정적 비교는 아니다.
스크롤·전체 화면 재빌드·WebGPU 성능·GPU 완료 지연에 대한 개선율로 확대하지 않는다.

6초 구간의 callback 수는 비-AOT **333–345**, AOT **659–675**로 달랐다.
headless의 callback/submit 처리량이며 표시 FPS가 아니다. owner CPU union은 양쪽 모두
약 **5.7초**였다. 동일 시간의 owner 할당은 AOT가 더 많았지만 callback당 약 **403–404KB**로
유사했다. AOT가 할당 구조나 총 CPU 사용량을 개선했다고 주장하지 않는다.
WASM capacity는 약 **208–243MB** 범위였으며 process resident/GPU resident bytes나
모바일 메모리 수락을 의미하지 않는다.

## 배포 크기와 선택

| 파일 크기, decimal MB | 비-AOT | AOT |
| --- | ---: | ---: |
| `dotnet.native.*.wasm`, raw | 9.66 | 48.33 |
| 같은 파일의 Brotli 압축본 | 2.70 | 9.85 |
| 같은 파일의 gzip 압축본 | 3.62 | 15.36 |

native WASM은 raw 약 **5.00배**, Brotli 약 **3.65배**다. 전체 publish 디렉터리의 raw
파일 합계는 receipt에 별도 보존하며, source map과 기타 파일을 포함해 실제 초기 다운로드와
같지 않다. 로컬 테스트 서버는 raw 파일을 제공하므로 네트워크 첫 방문 시간 비교도 아니다.
실서비스는 `.br`/`.gz` 파일 존재만으로 압축 전송되지 않으며 호스팅의 content encoding 설정이 필요하다.

현재 판단은 **AOT 선택 실행을 제공하고 일반 기본값은 유지**다. 배포 크기와 모바일 첫 방문,
추가 화면의 CPU·메모리, 실제 기기 체감을 확인한 뒤 기본 채택을 판단한다.
[Microsoft AOT 안내](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0)와
[IL stripping 안내](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-10.0)의 publish/크기/fallback 범위도 참고한다.

## 실패 이력과 검증 경계

- `smoke-r1`: native 텍스트 endpoint 포커스를 기다리지 않고 입력해 timeout. fixture에
  실제 focus 대기를 추가한 `smoke-r2` PASS. `smoke-r3`은 desktop backing canvas를 logical
  viewport 너비와 같다고 가정한 강화 fixture가 timeout. 제품은 더 큰 backing을 보존하고
  root로 clip하므로 logical root·navigation 위치로 확인을 수정했다. 최종 `smoke-r4`는
  양 renderer의 스크롤 위치·logical 크기와 텍스트 선택 범위까지 PASS. 초기 실패 자료를 덮어쓰지 않았다.
- `compare-r1`: tooltip을 accessible name으로 찾다가 workload 진입 timeout. DOM은
  tooltip을 `aria-description`에 담는다. 해당 속성을 사용하는 `compare-r2`의 6개 trial PASS.
  r1에는 완료된 측정 trial이 없으며 성능 근거로 사용하지 않았다.
- 별도의 [semantics Debug 반복 시작/서비스 PARTIAL](2026-10-05-web-semantics.md)에서 기록한
  Mono startup 오류의 해결을 이번 Release AOT smoke로 확정하지 않는다.
- clean-machine 배포, 모든 service/plugin/texture/font 경로, screen reader, 물리 한글 IME,
  Safari/Firefox/모바일, 실제 표시 FPS·장기 메모리는 이번 범위에서 미검증이다.

## 재현과 원시 자료

모든 publish/probe는 **1,200초 process-tree timeout**, 자동 retry 0으로 실행했다.
프로젝트나 workload 기본값을 바꾸지 않고 다음 명령으로 각각 게시한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/testbed-web-aot -p:RunAOTCompilation=true
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/testbed-web-baseline -p:RunAOTCompilation=false
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_wasm_aot.py --aot ./temp/testing/testbed-web-aot/publish/DorotiTestbedApp.Web/release/wwwroot --baseline ./temp/testing/testbed-web-baseline/publish/DorotiTestbedApp.Web/release/wwwroot --output ./temp/testing/web-aot/new
```

브라우저 명령에는 Playwright 환경과 설치된 Chrome이 필요하다. 이번 환경은
`temp/testing/platform-audit/all/venv/Scripts/python.exe`였다. 실제 raw 자료는 삭제 가능한
`temp/testing/web-aot/2026-10-05/`의 `testbed-stock.log`, `testbed-baseline.log`,
`payload.json`, `smoke-r1/`, `smoke-r2/`, `smoke-r3/`, `smoke-r4/`, `inspect-progress/`,
`compare-r1/`, `compare-r2/`에 있다. 원시 결과와 payload를 정리해도 이 문서/receipt는 남는다.
실행 방법은 [샘플 README](../../../samples/DorotiTestbedApp/README.ko.md#wasm-aot-게시비교)에 있다.

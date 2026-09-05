# Playwright Chromium C0/C1 메모리 비교

2026-09-05. **측정 완료 / C1의 전체 메모리 절감은 확인되지 않음.** C1 제품 선택은 유지한다. 기존 CLR 임시 할당 4,560,000→0 bytes 결과는 유효하지만 브라우저 전체 사용량 감소로 확대하지 않는다. 기존 latency FAIL / PARTIAL · notQualified 상태도 유지한다.

## 조건과 범위

- Playwright Chromium **151.0.7922.34**, Windows `chrome-headless-shell.exe`, headless, Radeon 780M / ANGLE D3D11 / WebGL2 hardware, fallback 없음. 화면이 표시되는 일반 Chrome의 수치로 일반화하지 않는다.
- C0와 C1 각각 독립된 새 browser/process tree **4회**, 총 8회. 한 번에 하나씩 실행했다. 순서: C0→C1, C1→C0, C1→C0, C0→C1. 실행은 13:40 UTC부터 약 3분, 20분 timeout 적용.
- viewport 960×640, DPR 1. 매번 800×640 → 800×720 → 960×720 → 960×640을 반복했다. warmup 20회 + 측정 120회. 모든 전환에서 logical 크기, front generation 일치, queue 0, context 정상, unpaired request 0을 기다렸다. **native drag 0회 추가**, 기존 7/10 ledger 유지.
- 옵션: `dorotiResizeDiagnostics=1`, `dorotiRenderer=worker-canvaskit-webgl`, `dorotiResizeScheduling=display`, `dorotiCopyOwnership=owned`, `dorotiMetricsCoalescing=frame`, `dorotiEncodingCache=1`. CanvasKit stage trace, 영상, 스크린샷, Playwright trace 수집 없음.
- warmup 후 및 40/80/120회 전환 후 snapshot, 최종 자연 GC 상태 5회 snapshot을 수집했다. 아래 주 결과는 **각 실행 최종 5회 중앙값을 구한 뒤, 4개 실행의 중앙값**이다. snapshot 사이에는 300ms 대기와 프로세스 조회 시간이 있다. 전환 후 checkpoint 대기는 500ms다.
- 메인·UI·Raster의 CDP isolate ID 3개가 서로 다른지 확인했다. 브라우저 PID의 자식 트리만 Windows에서 조회했고 CDP가 열거한 모든 PID의 포함을 검증했다. 사용자 브라우저·static server·Node/PowerShell 측정 도구 메모리는 합계에서 제외했다.
- 모든 주 결과를 수집한 후에만 세 isolate에 `HeapProfiler.collectGarbage`를 요청하고 별도 snapshot을 수집했다. 이는 **V8 GC**이며 .NET managed GC를 강제로 수행한 결과가 아니다.

## 주 결과

단위 MiB = 1,048,576 bytes. 괄호는 4개 실행값의 최소–최대다.

| 항목 | C0 | C1 |
| --- | ---: | ---: |
| Chromium 프로세스 트리 private bytes 합계 | 978.97 (975.84–989.07) | 986.31 (964.84–993.30) |
| Working set 합계 | 808.47 (798.23–810.76) | 815.82 (789.61–831.44) |
| Renderer process private bytes | 670.58 (666.04–674.86) | 668.96 (663.65–669.46) |
| GPU process private bytes | 253.36 (248.19–256.47) | 259.31 (243.61–265.73) |
| 메인+UI+Raster V8 used heap | 20.48 (16.92–28.03) | 19.15 (15.76–21.10) |
| UI .NET WASM linear-memory capacity | **138.00 (모두 동일)** | **138.00 (모두 동일)** |

private bytes의 paired C1−C0는 **+0.87, +16.01, +15.27, −24.23MiB**였다. paired 차이의 중앙값은 +8.07MiB다. 한 방향의 전체 메모리 절감이 재현되지 않았고 범위도 겹친다. 각 variant 중앙값의 차이 +7.34MiB와 paired 차이 중앙값은 계산 방식이 다르다. 4개 pair만으로 통계적 동등성이나 C1의 확정적 메모리 퇴행을 주장하지 않는다.

## V8 GC 요청 후 별도 관측

| 항목 | C0 중앙값 | C1 중앙값 |
| --- | ---: | ---: |
| Chromium process private bytes 합계 | 873.99 | 866.68 |
| Working set 합계 | 708.24 | 708.36 |
| Renderer process private bytes | 591.04 | 590.95 |
| 메인+UI+Raster V8 used heap | 11.112 | 11.064 |
| UI .NET WASM linear-memory capacity | 138.00 | 138.00 |

GC 후 private bytes의 paired C1−C0 범위는 −15.64~+9.40MiB, 중앙값 −3.19MiB다. 자연 GC 결과와 방향이 달라지므로 이 보조 표만 골라 C1이 RAM을 절감한다고 결론내리지 않는다. 두 버전의 GC 후 renderer와 JS heap은 매우 근접했다.

## 해석 한계와 선택

`PrivateMemorySize64`는 프로세스 전용 commit, `WorkingSet64`는 resident working set이다. working set을 프로세스별로 합하면 shared page가 중복될 수 있으며 둘을 같은 RAM 지표로 취급하지 않는다. GPU process private bytes는 GPU VRAM 사용량이 아니다.

CDP `Runtime.getHeapUsage`는 해당 **V8 isolate의 JS heap**이다. [CDP Runtime 문서](https://chromedevtools.github.io/devtools-protocol/tot/Runtime/#method-getHeapUsage)의 `backingStorageSize`도 array buffer/external string 지표이므로 .NET managed live heap으로 취급하지 않는다. .NET WASM 값은 실제 UI Worker의 `getDotnetRuntime(0).Module.HEAPU8.buffer.byteLength`를 읽었다. 이는 선형 메모리 용량으로, 살아 있는 C# 객체량이나 누적 할당량이 아니다. 각 지표는 순차 조회이며 원자적 전체 snapshot 또는 peak 측정이 아니다.

사용자가 육안 FPS를 비슷하다고 평가했고, C1은 불필요한 HashSet 복사와 boxing을 제거한다. **C1 유지 근거는 입증된 임시 할당 제거이며, 이번 Chromium 측정에서 총 메모리 사용량 감소는 확인하지 못했다.** 제품 소스는 이번 측정을 위해 변경하지 않았다. .NET live heap, WASM 누적 managed 할당량/GC 횟수, GPU VRAM, 장기 사용 및 physical presentation은 이번 범위 밖이다.

## 증거와 재현

- [원시 실행 결과](../../Doroti/validation/web-playwright/artifacts/memory-c0-c1-r1/runs.json), [사전 측정 조건](../../Doroti/validation/web-playwright/artifacts/memory-c0-c1-r1/plan.json), [계산 결과](../../Doroti/validation/web-playwright/artifacts/memory-c0-c1-r1/summary.json).
- [측정 harness](../../Doroti/validation/web-playwright/compare-canvaskit-memory.mjs), [집계 스크립트](../../Doroti/validation/web-playwright/summarize-canvaskit-memory.py). 측정 harness SHA-256 `cb40e08025489914c4769d3a4b3ca9d57b222bb36f208cabf81d62a1247990c9`; 측정 폴더에 사본 보존.
- 실행: `Doroti/validation/web-playwright`에서 C0/C1 static server를 유지한 상태로 `node compare-canvaskit-memory.mjs`. 기존 결과를 덮어쓰지 않도록 `DOROTI_MEMORY_OUTPUT`을 새 경로로 지정한다. 집계: `python summarize-canvaskit-memory.py artifacts/memory-c0-c1-r1/runs.json`.
- C0 실제 응답 `Doroti.Framework.Widgets.2rn4wg03sq.wasm`, SHA-256 `3e71e02f03cfab0c12d011de1ed414c5f545497c81b8b99e1529bd55fc47d4fe`; C1 `Doroti.Framework.Widgets.ed9iv0a1r5.wasm`, SHA-256 `0ea1e4630886de4bc9e1e9c6f7695e88e4e780f60f25321402736003864c2933`. 8회 모두 응답 body를 대조했다. serve root는 각각 `.doroti/publish/n1-c0/wwwroot`, `.doroti/publish/n2-c1/wwwroot`다.
- 사전 API probe는 `artifacts/memory-c0-c1/probe.json`. 첫 시도는 OS 조회를 `chrome.exe`로 한정해 headless shell PID를 찾지 못했고 warmup 뒤 **FAIL**, 비교 통계에서 제외했다. `artifacts/memory-c0-c1/runs.json`에 실패를 보존했다. 실행 파일명 필터를 제거하고 CDP browser PID의 자식 트리를 조회하도록 고친 `r1`이 최종 8회다.
- 최종 8회 수집·hardware·settle·응답 identity 검사 **PASS**, browser pageerror 0. 측정용 Chromium PID 전부 종료 확인. 수동 비교용 서버 5188/5189는 유지한다. 수집 PASS가 메모리 절감 PASS를 뜻하지 않는다.

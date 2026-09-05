# Web CanvasKit 연속 resize 구현·실험 기록

2026-09-05. 상태: **PARTIAL / latency gate FAIL / 사용자 border drag notVerified**.

## 사용자 지적에 따른 시험 조건 정정

기존 작은 폭 CDP 루프는 Windows 빠른 drag와 속도가 달랐다. 아래 기존 결과는 **느린 corpus의 이력**이며 빠른 창 조절의 개선·기각 근거로 사용할 수 없다. 새로운 제품 최적화 전에 `-FastResize`로 R0를 다시 확정한다.

- 입력은 `validate-windows-vulkan-live-resize.ps1`와 같은 `Doroti.WindowsResizeCapture.exe`의 QPC deadline 기반 SendInput이다. 공유 테스트 드라이버에 선택적 시작 크기만 추가했고 Windows 기본값 520×300은 유지했다. Chrome은 DPI에 맞춘 640×400 logical 시작 크기를 사용한다. 제품 Windows/Vulkan 소스는 변경하지 않았다.
- 기본 matrix: Right/Bottom/Left/TopLeft × expand/shrink/reverse × 600px/150ms 및 600px/600ms × 3회. 매 step에 Playwright 호출·스크린샷·settle·16ms sleep을 끼우지 않는다.
- 드라이버는 log-only로 사용해 입력/실제 window rect를 기록한다. WGC frame 분석을 포함하는 Windows 결과와 GPU 비용까지 동등하다고 주장하지 않는다.
- 빠른 테스트에서는 Playwright video/trace 녹화를 끈다. 실패 후 스크린샷만 허용하고 stage ring과 native rect/QPC 증거를 저장한다.
- 입력 QPC span은 요청 시간의 -10~+25ms, 실제 native excursion은 Windows 드라이버와 같은 요청량 80%(480px) 이상이어야 자극이 유효하다. 속도를 늦춰 PASS시키지 않는다. 실제 peak도 그대로 기록한다.
- reverse는 150/600ms 안에서 600px 왕복 후 기존 Windows 드라이버처럼 원점에서 100ms를 유지한다. 입력 span과 전체 mouse-held 시간을 분리한다.
- QPC와 epoch clock의 변환 오차·drift를 기록하고, 드라이버의 시작 크기 준비 동작은 latency 집계에서 제외한다.
- 초기 calibrated probe(Left/reverse/150ms, 기본 30fps): 실제 입력 149.98ms, native excursion 567px, motion 중 front 1회, target→caught-up-front p95 155.6ms, settle 80.2ms. **자극 유효 / 추종 성능 FAIL**이다. 이 probe는 Playwright video/trace가 켜져 있었으므로 녹화를 끈 최종 corpus 및 이전 느린 corpus와 합치지 않는다.
- 실행: `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -FastResize -Port 5188`.
- 초기 `artifacts/resize-fast-native-baseline/`은 Chrome 200% DPI 최소 추적 너비 때문에 shrink 입력이 이동량을 충족하지 못해 중단했다. 실패 자료를 보존하고 시작 크기를 보완했다. 최종 주 corpus는 `artifacts/resize-fast-native-v2/`이다. 기존 `canvaskit-resize-latency.spec.ts`는 명시적으로 legacy slow CDP 진단으로 이름을 바꿨다.

### 빠른 native baseline 결과

- 환경: headed Chromium, 물리 모니터 2560×1600 / 165Hz, Windows DPI 192(200%). 기본 30fps scheduling. 제안 성능 gate 33.3ms는 그대로 유지한다.
- 전체 matrix: **23개 조건 PASS / 1개 조건 native 입력 FAIL**. Left/150ms/expand 첫 실행이 요청 이동량을 충족하지 못했다. 속도·시작 크기·드라이버 binary를 바꾸지 않은 별도 재실행(`resize-fast-left-expand-repeat`)은 **3회 유효**였다. 최초 실패 원인은 미확정이며 전체 matrix를 clean PASS로 재분류하지 않는다.
- 두 실행에서 24개 조건별 3회, 총 **72회 유효한 baseline 표본**을 확보했다. 모든 표본에서 ring drop 0, timing/이동량/최종 buffer 계약을 확인했다. TopLeft의 가로·세로 이동량도 각각 480px 이상인지 원본 native rect로 재검증했다.
- 150ms 조건의 실제 입력 시간은 약 150ms다. 중간 step에 browser round trip 또는 settle을 넣지 않았다. **72회 모두 추종 성능 gate FAIL**이다.

| 요청 시간 | trial별 target→caught-up-front p95 범위 | motion 중 새 generation front 수 | front interval p95 범위 | settle 범위 |
| --- | --- | --- | --- | --- |
| 150ms | 58.7–79.0ms | 3–4 | 35.4–60.4ms | 39.2–64.8ms |
| 600ms | 58.5–111.8ms | 10–17 | 37.1–76.6ms | 25.7–60.9ms |

이는 trial별 통계의 최솟값/최댓값이며 pooled p95가 아니다. target 관측 이후 main front 통지까지 측정하며, native 창 변화→browser observer 전달 지연과 물리 scan-out은 이 p95에 포함하지 않는다. 원본 및 종합: `artifacts/resize-fast-final/baseline-summary.json`.

`-FastResize`는 전용 opt-in이며 일반 headed 회귀에는 이 matrix를 추가하지 않는다. 기본 실행은 baseline 수집과 입력/계약 검증이다. `-RequireLatencyGate`를 함께 주면 추종 FAIL도 테스트 실패로 반환한다. 각 프로세스 timeout은 20분이다.

### 빠른 TopLeft 반전의 R1 교대 A/B

같은 600px/150ms 입력에서 baseline → display를 3쌍 교대로 실행했다. 새 제품 변경 없이 기존 opt-in 후보를 비교했다. 입력/계약 검증은 6회 모두 PASS, 추종 gate는 6회 모두 FAIL이다.

| 쌍 | baseline p95 | display p95 | baseline / display UI dispatch busy proxy |
| --- | --- | --- | --- |
| 1 | 64.9ms | 58.7ms | 66.8% / 73.2% |
| 2 | 77.9ms | 63.9ms | 69.4% / 71.9% |
| 3 | 60.8ms | 76.2ms | 65.5% / 75.0% |

일관된 개선이 없으므로 기본 30fps 정책은 유지한다. UI busy proxy는 native motion 시간과 겹친 managed frame dispatch 구간의 비율이며 OS CPU 사용률이 아니다. 이 한 조건의 결과로 나머지 후보 전체 matrix 또는 input/CPU 회귀를 PASS 처리하지 않는다. `artifacts/resize-fast-ab-{1,2,3}-{baseline,display}/`와 `resize-fast-final/ab-summary.json`에 근거를 저장했다.

최종 TypeScript check, PowerShell parser, native C++ configure/build 및 `git diff --check` PASS. 실행한 owned 서버와 입력 드라이버는 모두 종료했다. 다음 구현 판단에는 이 빠른 corpus를 사용하며, 전체 계획 완료 및 사용자 체감 PASS는 아직 아니다.

다음 절들은 이 정정 전에 수행한 느린 corpus의 변경·측정 기록이다.

## 적용 범위

- `dorotiResizeScheduling=display`: resize 전용 30fps gate만 해제하는 opt-in 후보. 기존 single Worker rAF, callback 교체, 최신 metrics, mailbox와 buffer/terminal 계약을 유지한다. 기본 scheduling은 기존 baseline이다.
- DisplayList encoder의 byte enum 변환에서 `Convert.ToByte` boxing을 `Unsafe.BitCast<TEnum, byte>`로 제거했다. `Enum.IsDefined`와 기존 오류 경계는 유지한다. managed/Cross-language golden은 byte-for-byte 동일하다.
- main/UI/Raster마다 최대 8,192개 stage record를 저장한다. `dorotiCanvasKitTrace=1`일 때만 수집하며, collection 명령으로 가져온다. per-scene diagnostics에 ring 전체를 실어 보내지 않는다. 시간은 `performance.timeOrigin + performance.now()`로 정규화한다.
- 관측 epoch, UI 수신/apply/frame, mapping/encoding/paragraph/copy, scene send, Raster decode/replay/submit, terminal sent/handled, main commit을 기록한다. managed mapping/encoding은 frame callback ID로 scene과 연결한다.
- Playwright wrapper에 owned server `-Port`, 여러 `-TestFile`을 추가했다. Flutter wrapper에는 두 포트, renderer 선택, `-Resize`, `-SkipBuild`를 추가했다. 점유 포트의 다른 프로세스를 종료하지 않는다.
- Windows/Vulkan 제품 코드, `auto=document-webgl`, 과거 CK5/CK7 FAIL 및 CK9 notVerified는 변경하지 않았다.

## 환경과 재현 한계

- checkout HEAD: `e31bb15f8d92b70d2bcb7b49b72863f51494f461` + 이 작업의 dirty diff. 커밋을 생성하지 않았다.
- Release / net10.0 / `WasmBuildNative=true`; 평가된 `RunAOTCompilation`은 빈 값이다. AOT publish 결과라고 주장하지 않는다.
- Playwright bundled Chromium `151.0.7922.34`, hardware WebGL2, AMD Radeon 780M / ANGLE D3D11. 주요 성능 corpus는 headless DPR 1의 현재 demo다.
- 1280×800에서 시작하여 120개 크기를 확대·축소·반전한다. 각 `setViewportSize` 뒤에는 renderer settle을 기다리지 않는다. 마지막에만 settle한다. 요청 크기와 실제 command/observer 시간은 JSON에 남긴다. CDP 및 IPC 비용 때문에 실제 통지 간격이 정확히 16ms인 것은 아니다.
- 60Hz의 제안 gate를 유지한다. 물리 모니터 주사율, scan-out, mouse border drag를 headless 수치로 증명하지 않는다.
- Flutter는 SDK `56b8e1a851a594b1a154f8ea93270807dab22b9a`, release CanvasKit을 loader에서 강제하고 실제 wasm asset 로드를 확인했다. 3회 연속 resize와 최종 metrics 일치는 PASS다. 두 demo 내용과 Flutter post-frame/Doroti submit endpoint가 달라 상대 latency는 **notComparable**이다.
- 초기 idle 상태의 오래된 epoch 때문에 전체 content-age proxy max가 수 초가 될 수 있다. 이를 재제출 시각으로 초기화하지 않았다. 후속 보고서의 `steadyContentAgeProxy`는 첫 active front 이후만 따로 표시한다.
- clock 정밀도 calibration, 동일한 단순 Flutter fixture, DPR 1.25/1.5, 모니터 이동, 120Hz, CPU/GPU 메모리, 전체 build/layout/paint 세부 분해는 미완료다.

## 측정 및 결정

아래 p95는 각 trial의 epoch→GPU submit p95(ms)다. trial별 p95 평균은 참고값이며 pooled p95가 아니다. 개선 목표를 사후 완화하지 않았다.

| 실험 | baseline 3회 | 후보 3회 | 결정 |
| --- | --- | --- | --- |
| R0 30fps baseline | 79.8 / 64.8 / 76.8 | — | 계측·연속성 근거 확보 |
| R1 gate 해제 교대 A/B | 79.3 / 76.8 / 73.1 | 70.3 / 67.9 / 68.7 | 평균 약 9.7% 개선, 채택 gate FAIL; opt-in 유지 |
| R2 snapshot 단일 apply A/B, gate 해제 공통 | 66.4 / 68.2 / 66.5 | 67.2 / 66.1 / 67.1 | 일관된 개선 없음, rollback |
| R2 enum boxing 제거 후 gate 해제 | 직전 gate 해제 corpus 참조 | 67.3 / 62.9 / 65.8 | wire 동일성 PASS, encoder p95 감소; 전체 목표 미달 |
| R4 Raster terminal 대기 시 rAF defer A/B | 65.7 / 69.4 / 69.1 | 63.9 / 82.5 / 70.2 | latency 퇴행, rollback |

R1 baseline의 UI dispatch busy proxy는 약 58~66%, gate 해제는 약 91%였다. 이는 UI callback 소요시간/active 기간이며 OS CPU 사용률 측정값은 아니다. 기본 적용을 보류한 이유다. R4는 약 70~73%로 낮췄지만 p95 퇴행을 동반했다.

R2의 mapping p95는 약 3.4~3.7ms, encoding p95는 약 8.4~9.3ms였다. boxing 제거 이후 encoding p95는 7.6 / 7.8 / 7.9ms, UI frame p95는 28.8 / 29.0 / 28.6ms였다. paragraph layout은 이 warm resize corpus에서 호출되지 않아 cache 최적화를 추가하지 않았다.

R2 후보의 front interval p95는 62.3 / 60.0 / 64.1ms, settle은 39.8 / 60.9 / 58.4ms다. epoch p95 ≤33.3ms, interval p95 ≤33.3ms, settle ≤50ms를 충족하지 않는다. 개선 전/후 trial variance와 서로 다른 binary 단계도 감안해야 하므로 합산 개선을 채택 PASS로 사용하지 않는다.

R3의 `terminal handled → next send`에는 다음 scene을 만드는 시간이 포함된다. 이 값만으로 ACK 병목을 단정하지 않았다. 실제 `scene encoded → send` p95는 대체로 0.1~4.8ms로 UI 전체 작업보다 작고 main submit 통지 p95도 대개 1ms 미만이었다. credit 2는 현재 `notNeeded`로 남긴다. bitmap 표시(R5)는 GPU 제출 자체의 목표가 미달하여 비교 진입을 보류한다. 이 판단으로 물리 표시 지연이 없다고 결론내리지는 않는다.

## 검증

- 모든 테스트 프로세스에 repository 규칙인 20분 timeout을 적용했다.
- Release build, TypeScript check PASS.
- managed DisplayList contract PASS: 6,330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42`.
- gate 해제 후보 headless 회귀: **11 PASS / 2 renderer 전용 skip**. topology, cold-start input ordering, Raster 100ms stall, grow-only capacity 증가/재사용, DPR2, 3회 restart, malformed protocol, input endpoint, cross-language golden과 buffer/lease terminal을 포함한다.
- 초기 R0/R1/R2/R4 성능 harness는 누락/중복 scene terminal, outstanding buffer, generation/geometry 계약을 함께 검사했고 통과했다. 성능 목표 달성을 뜻하지 않는다.
- trace on/off 교대 3쌍: 모두 correctness PASS. trace on p95 66.3 / 67.7 / 65.6ms, off 65.5 / 67.7 / 68.4ms. 작은 차이는 trial variance 범위로 보며 계측 off로 목표가 달성됐다고 하지 않는다.
- headed 후보 회귀: **5 PASS / 2 다른 renderer용 skip**. TextField 공백·strut caret, native selection overlay, status hit-test, editable context menu, 181-step native HWND edge resize를 포함한다. TextField 최종 screenshot도 확인했다.
- native HWND 회귀의 target→caught-up-front p95는 84.0ms, max 107.3ms다. 기존 연속성 기준 PASS이며 이 작업의 실시간 추종 목표 PASS가 아니다.
- `auto` input/resize/zoom/DPR2 회귀: **4 PASS / 1 renderer용 skip**. `worker-direct-webgl`: **5 PASS**.
- 기본 30fps CanvasKit 경로의 최종 Worker/input/DPR2 회귀: **9 PASS**. 위 선택 회귀 합계는 **34 PASS / 5 skip**이며 별도의 성능·Flutter harness PASS를 합산하지 않았다.
- 사용자 직접 drag, IME 실제 조합 입력, 물리 scan-out: **notVerified**.

## 산출물과 실행

`Doroti/validation/web-playwright/artifacts/` 아래:

- `resize-r0-baseline/`, `resize-r1-ab/`, `resize-r2-atomic-ab/`, `resize-r2-encode/`, `resize-r4-pressure-ab/`: `test-results/*/resize-run-N.json` 및 `resize-summary.json`.
- `flutter-differential/test-results/*/flutter-resize.json`: 실제 renderer asset, SDK SHA, isolation, 양쪽 trajectory/sample, 최종 screenshot.
- `resize-r6-candidate/`: headless correctness report.
- `resize-r6-trace-ab/`, `resize-r6-headed/`, `resize-r6-auto/`, `resize-r6-worker-direct-webgl/`: 최종 후속 검증. native 수치는 `resize-r6-headed/test-results/*/native-hwnd-resize-report.json`.
- `resize-r6-default/`: 기본 CanvasKit 경로 최종 회귀.
- `resize-final/source.patch`, `resize-final/source-manifest.json`: 빠른 테스트 보완을 포함한 최종 제품·harness 변경 18개 파일의 패치와 SHA-256. 현재 working tree에 대한 `git apply --reverse --check` 통과. 당시 기각된 임시 정책을 포함하는 패치는 아니다.
- `wrapper/<동일 label>/`: 빌드·서버·Playwright stdout/stderr.

느린 corpus의 후속 benchmark JSON에는 실제 제공된 host/UI/Raster/trace JavaScript의 SHA-256도 포함한다. 빠른 native JSON은 입력 드라이버 SHA-256을 기록하며 같은 웹 build를 사용했다. 최초 R0/R1 binary에는 이 fingerprint 수집이 없으므로 HEAD만으로 당시 dirty 구현 전체가 식별된다고 주장하지 않는다.

```powershell
# 기존 baseline과 gate 해제의 교대 3쌍 비교
$env:DOROTI_RESIZE_AB = '1'
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -HeadlessOnly -TestFile tests/canvaskit-resize-latency.spec.ts -Port 5188 -ArtifactLabel resize-local-ab
Remove-Item Env:DOROTI_RESIZE_AB

# 현재 후보로 일반 회귀. 상세 stage trace는 기본 off.
$env:DOROTI_RESIZE_EXPERIMENT_QUERY = '&dorotiResizeScheduling=display'
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -HeadlessOnly -TestFile tests/canvaskit-worker.spec.ts -Port 5188 -ArtifactLabel resize-local-candidate
Remove-Item Env:DOROTI_RESIZE_EXPERIMENT_QUERY

pwsh -NoProfile -File ./Doroti/eng/run-web-flutter-differential.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -FlutterRenderer canvaskit -Resize -DorotiPort 5188 -FlutterPort 5189
```

개발 서버 실행 후 수동 후보 URL: `http://127.0.0.1:5088/?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display`.

다음 성능 작업은 남은 managed build/layout/paint 비용 분해와 동일 workload fixture 구축부터 진행해야 한다. 실패한 snapshot/terminal 대기 정책을 기본 경로로 다시 적용하거나 제출 수치를 실제 화면 PASS로 바꾸지 않는다.

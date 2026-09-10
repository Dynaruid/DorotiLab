# Web direct 기본 전환 실행 결과

실행일: 2026-09-07. 시작 HEAD: `c9b9eb21` / clean working tree.
계획의 과거 검토 HEAD `207010a4` 이후 변경이 이미 커밋된 상태를 확인하고 시작했다.
범위는 [work2.md](web-direct-work2-executed-plan.md)의 P0~P4이며 기존 `work.md`와 과거 실패 기록은 보존했다.

**최종 상태: PARTIAL. 기본값 전환 및 CSS/캐시 수정은 구현했다. live resize와 시작 지연의 성능 gate는 FAIL이며 전체 완료가 아니다.**
사용자 최종 관찰: **“일부 개선됐지만 지연이 남음”**. 자동 검사와 별도의 사용자 증거다.

## 변경 내용

- loader/target manifest의 생략·auto·미인식 선택을 `worker-direct-webgl`로 통일했다.
  명시적 CanvasKit/document/offscreen 선택을 유지했고 Worker diagnostics의 requestedMode 고정을 제거했다.
  direct 초기화 실패를 다른 renderer로 숨기는 fallback은 추가하지 않았다.
- direct backing은 Worker가 소유하고 main은 완료 front의 capacity/DPR CSS 배율을 유지한다.
  observer가 viewport 크기로 CSS를 다시 써서 배율을 바꾸던 충돌을 제거했다. root가 viewport를 clip한다.
  epoch/정확한 scene 검사와 최신 admission/backpressure, 제한된 resize wake는 유지했다.
- 공용 Skia picture 승격은 **frame당 최대 2개, 총 4M pixels, 누적 2ms 도달 뒤 추가 승격 중단**이다.
  예산 때문에 밀린 picture도 그 frame에서 정상 replay한다. 두 번째 사용이라는 기존 warm-up 의미를 유지한다.
  개별 승격은 중단할 수 없으므로 2ms는 hard task deadline이 아니다. 실제 단일 승격의 긴 비용은 여전히 남는다.
- warm-up metadata는 128개, 미사용 120 raster frames로 제한했다. dictionary와 linked list를 함께 정리해
  hit/eviction은 O(1)이며, 만료는 오래된 항목부터 처리한다. 성공 승격·surface/context 해제에서 제거한다.
  기존 image cache 24 entries / 16M pixels, 개별 4M pixels, transform/translation/WillChange 의미를 유지한다.
- paragraph·promotion 누적 시간/최악값과 cache 수명을 노출했다. opt-in direct 진단에서 framework callback
  시작 시각·지속시간·callback ID·resize generation을 남기고 기존 bounded framework trace를 on-demand 수집한다.
  Worker epoch와 main epoch를 연결하고 raster request ID/input sequence로 클릭과 새 scene을 연결한다.
  callback 시간에는 내부에서 동기 실행되는 raster가 포함되므로 surface 시간을 다시 합산하지 않는다.
- 측정 도구에 `--onset`, `--restart`, `--no-diagnostics`를 추가했다. 클릭 전부터 첫 100/500/1000ms, 이후 5초를 기록한다.
  클릭 pointerup/wheel sequence와 새 scene의 request ID를 연결한다. `--no-trace`는 기존 CanvasKit stage flag이며
  direct 상세 계측을 끄려면 `--no-diagnostics`를 함께 사용한다.
- 기본 선택, capacity CSS/연속 resize, GPU cache replay 픽셀 일치·translation 재사용·metadata 만료/해제 회귀를 추가했다.
  admission burst는 9개 변경 + 최종 변경으로 제한했다. README 한국어/영문, ADR, 활성 검증 문서를 갱신했다.
  기존 renderer A/B 보고서는 explicit document/direct 실험과 현재 제품 기본 정책을 구분한다.

## 환경과 증거 범위

Windows 11 Pro 10.0.26200, .NET SDK 10.0.400, Release interpreter build.
Playwright 설치 Chromium, hardware ANGLE / AMD Radeon 780M / D3D11, sample 1280×900 DPR 1.
resize는 별도 context에서 DPR 1/2, 최초 viewport 1000×720, 8회 양방향/breakpoint 변경이다.
실제 물리 refresh/scan-out은 검증하지 않았다. 60Hz 목표를 낮추지 않았다.
측정 당시 일반 사용자 환경에서 사용자 체감 확인도 요청했으므로 독점 GPU benchmark로 간주하지 않는다.

`Doroti/artifacts/direct-default/measurement-summary.json`에 최종 runtime source SHA-256과 요약을 기록했다.
원본 per-run JSON은 `Doroti/validation/web-playwright/artifacts/sample-perf/`에 있다.
publish asset/build identity는 `artifacts/wrapper/direct-default-publish/build-manifest.json`과 build/publish 로그에 있다.
모든 테스트/측정 프로세스는 **20분 timeout**, retry 0으로 실행했다.

## P0와 중간 실패 보존

- 초기 `direct-baseline-onset-1..3`: commit gap max 117/139/154.3ms. 일부가 계측 빌드와 겹쳐
  controlled before/after 비교에서는 제외했다. 원본을 삭제하거나 PASS로 대체하지 않는다.
- 첫 계측 재시도는 이전 devserver asset manifest가 새 파일명과 달라 404로 종료했다.
  이 시도는 측정 실패이며 제품 성능 표본이 아니다. 해당 Doroti server만 확인 후 재시작했다.
- 단독 `direct-instrumented-onset-valid`: callback max 636.7ms, 관측 경계→첫 commit 589ms,
  단일 캐시 승격 max 37.5ms, warm-up metadata 370개.
- recording clock을 켠 `direct-p2-phases`: 첫 조작 frame의 build가 약 511.5ms,
  semantics 약 99.8ms였다. 처음 앱 mount의 build/layout 비용과 첫 조작은 분리해 분석했다.
  첫 조작에서 여러 animation start가 build 구간에 몰렸지만 이것만으로 특정 위젯을 원인으로 확정하지 않는다.
- 최초 DPR 1/2 capacity 테스트는 CSS 배율 불일치를 재현했다. CSS 수정 뒤에는 style mutation 수가 줄어
  “샘플 수 >2”가 실패했다. root ResizeObserver에서도 수집하도록 보완했고 이 중간 실패도 보존했다.
- 이후 CSS 배율은 맞았지만 active 구간에 완료 front가 1개뿐이어서 연속성 assertion이 실패했다.
  테스트 기준을 낮추지 않았고 최종에도 실패로 남긴다.
- 공용 sliver의 동적 호출을 정적 호출로 바꾸는 실험은 첫 빌드에서 override 반환형 CS1715 실패,
  수정 후 build/FCR-5 PASS였다. 성능 개선이 확인되지 않아 **최종 source에서 모두 되돌렸다**.
  `direct-final-*`은 이 중간 실험의 표본이며 최종 `direct-verified-*`으로 대체하거나 숨기지 않는다.
- metadata의 최초 bounded 구현은 eviction 때 선형 탐색했다. 최종 구현은 linked list로 이를 제거했다.

## 최종 자동 검증

| 검사 | 결과 / 증거 |
| --- | --- |
| Web Release build | PASS, 0 warnings/errors, `verified-build.log` |
| TypeScript | PASS, `typescript.log`, `final-typescript.log` |
| WindowsAppSdk host build | PASS, `windows-build.log` |
| Scheduler / resize 계약 | PASS, `scheduler-contract.log`, `resize-contract.log` |
| native Material 전체 / Windows sample 계약 | PASS, `material-native.log`, `native-sample.log` |
| GPU cache 예산/픽셀 일치/translation/128개 상한/120-frame 만료/context 정리 | PASS, `verified-raster-budget.log`, `raster-budget-expiry.log` |
| source 선택 및 Material sample 주요 동작 | 15 PASS, 2 strict resize FAIL, 1 explicit-mode skip; `default-functional.log` |
| 최종 progress pixel-change/stop, selection/text, wheel, admission, DPR2, pinch, restart/protocol | 14 PASS; `verified-functional.log` |
| 실제 WEBGL_lose_context 후 latest exact 복구 | 1 PASS; `verified-context.log` |
| Release publish 기본/auto/오타/모든 explicit override | 8 PASS; `artifacts/wrapper/direct-default-publish/` |
| 최종 빠른 resize | DPR 1/2 모두 FAIL; `verified-geometry.log`, `verified-resize-summary.json` |

첫 source suite의 sample restart skip은 최종 forced-direct suite에서 PASS로 실행했다.
pixel-marker 검사는 decoder 단위검증이며 실제 resize 화면의 marker 추종 검증을 대신하지 않는다.
sample screenshot 한 장을 확인했고 기능 시험은 실제 포인터 좌표/PNG 변화도 검사했지만 Flutter/native 시각 동등성은 검증하지 않았다.

## 최종 성능: FAIL

다른 agent/build/browser test와 겹치지 않게 최종 측정 명령을 순차 실행했다.
각 값은 commit notification/CPU evidence이며 표시 FPS가 아니다.

| cold progress run | input→새 scene commit ms | callback max ms | 이후 5초 commit p95 / max ms | warm-up metadata |
| --- | ---: | ---: | ---: | ---: |
| 1 | 714.9 | 773.8 | 31.3 / 67.0 | 120 |
| 2 | 755.8 | 819.6 | 20.6 / 36.9 | 120 |
| 3 | 694.8 | 755.4 | 20.1 / 32.4 | 120 |

세 run 모두 첫 1초에 50ms 초과 callback이 있었다. 100ms onset 목표와 warm p95≤20ms/max≤33.4ms를
모든 run에서 만족하지 못했다. 관측 경계가 아니라 실제 pointerup sequence와 연결한 새 scene 수치다.
캐시 예산만으로 시작 지연을 해결했다거나 안정적인 warm 성능 무회귀를 입증했다고 주장하지 않는다.

- 5초 idle 후 첫 progress: `direct-verified-idle-onset.json`. 이것은 이미 실행한 progress를 stop/restart한
  시나리오와 다르므로 별도로 `--restart`를 추가해 start→stop→5초 idle→restart를 3회 측정했다.
  `direct-verified-restart-1..3`의 input→새 scene commit은 **570.9 / 598.2 / 632.5ms**, callback max는
  **646.1 / 680.2 / 713.7ms**였다. 초기 실행에만 나타나는 지연이 아니며 restart 목표도 FAIL이다.
  이후 5초 commit p95/max는 **18.5/30.9, 19.8/33.0, 20.8/31.0ms**로 마지막 run의 warm p95도 목표를 넘었다.
- cold new/visited section sweep: `direct-verified-sweep.json`, callback p95 96.0ms / max 497.5ms,
  surface max 107.9ms. 첫 입력 이후 1초 내 긴 callback 6개. 이동 방향/column/sequence 원본을 보존했다.
  신규 section과 visited 구간의 독립적인 3회 표본 통계는 미완료다. 경계의 무변화 시간도 포함된 sweep의
  전체 commit max를 순수 render 지연이나 display FPS로 해석하지 않는다.
- 진단 OFF 대조군: `direct-verified-off`, 6842.8ms 동안 front request advance 342, 오류 0.
  상세 trace가 없어 onset/percentile은 **notMeasured**다. 첫 raw 결과의 trace-derived zero는 관측이 없다는 뜻이며
  zero latency/zero long tasks가 아니다. 도구 표시도 이후 null로 바로잡았다. warm-up이나 OFF count로 onset을 대체하지 않는다.

최종 CDP 빠른 resize:

| DPR | CSS scale 오류 / samples | active front 수 | boundary gap max ms | observer 후 최종 exact ms | 판정 |
| --- | ---: | ---: | ---: | ---: | --- |
| 1 | 0 / 10 | 1 | 354.9 | 1531.9 | FAIL |
| 2 | 0 / 10 | 1 | 366.0 | 1459.7 | FAIL |

각 run 8 targets 중 4개는 superseded, unreached 0이다. 모든 target이 실제 exact로 그려진 것은 아니다.
raw JSON에는 시간 가중 geometry 오차, idle age/active age, observer별 catch-up을 보존했다.
이것은 native drag가 아니다. capacity 초과 target도 빠르게 지나가므로 특정 target의 allocation 완료를
이 결과만으로 보증하지 않는다. 실제 grow/DPR 전환/context resource 수명 전체 matrix는 미완료다.

## 실행 명령과 남은 범위

저장소 build/native 명령에는 임시 Python `subprocess.run(..., timeout=1200)` wrapper를 적용해
`Doroti/artifacts/direct-default/*.log`에 stdout/stderr를 보존했다. 주요 명령:

```powershell
dotnet build DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --nologo
dotnet build Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj -c Release --nologo
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --raster-budget
dotnet run --project Doroti/validation/fcr7-material-widget -c Release
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --windows-sample .doroti/evidence/direct-default-native
pwsh -File Doroti/eng/run-web-playwright.ps1 -BuildMode Publish -Port 5092 -TestFile tests/default-renderer.spec.ts -ArtifactLabel direct-default-publish -HeadlessOnly
# web-playwright 폴더에서, 각 명령 자체 20분 timeout:
$env:DOROTI_PERF_QUERY='&dorotiRenderer=worker-direct-webgl'
node measure-material-sample.mjs measured-onset --progress --onset --cold --no-trace --no-screenshot
node measure-material-sample.mjs measured-restart --progress --onset --restart --no-trace --no-screenshot
node measure-material-sample.mjs measured-off --progress --onset --cold --no-trace --no-diagnostics --no-screenshot
```

동일 조건의 cold run은 기본 3회, 기타 기능/수명은 1회부터 실행했다. 원인/조건 변경과 harness 실패는 위에 분리했다.
100회 반복이나 성공할 때까지 retry는 하지 않았다. 조작 cycle과 frame/event/data 표본 수는 구분한다.

**남은 구현/검증**: first rebuild와 첫 section 구성의 큰 동기 작업을 줄이는 공용 framework 수정,
resize 중 주기적인 새 레이아웃과 <100ms exact settle, 새/방문 section 독립 비교,
slow/fast/animated native drag, 실제 browser page zoom/monitor DPR 전환, 120Hz, 한글 IME/caret/접근성 물리 검증,
별도 NuGet package 소비 앱 검증 및 전체 장기 resource/GC matrix.
기존 scene을 최신 epoch로 바꾸거나 prefetch/초기 warm-up에 비용을 숨기는 수정은 하지 않았다.
현재 실패가 재현되므로 더 많은 동일 재실행으로 PASS를 만들지 않고 실패 원본과 미완료 gate를 유지한다.
5088 source server는 최종 빌드로 실행 중이고, 5092 publish 검증 server는 wrapper가 종료했다.

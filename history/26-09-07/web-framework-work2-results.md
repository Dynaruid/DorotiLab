# Web framework work2 실행 결과

2026-09-07. 시작 HEAD `4759326fc25c1e067aada082c735dfe3be0d3172`, clean worktree.
현재 사용자의 전체 실행 요청에 따라 [조사 당시 계획](web-framework-work2-plan.md)의 구현·대조·검증을 진행했다.
기존 계획의 “계획 작성만” 문장은 당시 조사 범위를 설명하며 이번 실행을 제한하지 않는다.

**상태: PARTIAL. progress 상태 범위와 Web semantics 직렬화를 개선했다. 전체 성능 수용은 완료되지 않았다.**
기존 [direct FAIL](web-direct-default-execution.md), 사용자 “일부 개선됐지만 지연이 남음”을 유지한다.
이 실행에 대한 새 사용자/물리 장치 관찰은 없다.

## 구현

- direct Worker runtime 생성 전에 `DOROTI_STAGE_TRACE`를 설정한다. `dorotiResizeDiagnostics=1`에서
  작업량과 numeric type ID 집계, postprocessing 이름표, managed allocation/heap/GC 횟수를 캡처한다.
  OFF에서는 type lookup, 문자열/stack/tree 참조 수집을 하지 않는다.
- work ring은 8,192 boundaries, type/kind table은 512, 중첩 scope는 512, frame별 상위 5개 type은
  512 build frames로 제한한다. 각 overflow를 별도 보고한다. ThreadStatic 격리와 예외 unwind를 검사했다.
  inclusive time은 중첩을 포함하며 합산하지 않는다. self time은 계측한 자식 scope만 제외한다.
- rebuild/forced/dirty/sort 외에 delegate/retained child/tween check/restart를 계측했다.
  restart owner는 numeric type ID와 kind 6이다. `finalizeTree` 시작/끝 phase를 추가했다.
- main pointerup sequence와 direct Worker present request의 input sequence, exact-rendered commit을 연결한다.
  `dorotiInputMarkers=1`은 상세 진단 OFF에서도 commit marker만 opt-in 수집한다. 표시/scan-out 증거가 아니다.
- `measure-material-sample.mjs`는 첫 조작 직후 trace를 보존한다. 6초 후 ring만 읽으면 초기 지연 frame이
  사라져 뒤 frame을 원인으로 오인할 수 있었다. early/late snapshot을 분리하고 분석기는 같은 build 안에서만 span을 찾는다.
  원본 초기 `work2-broad-profile`은 causal frame 누락으로 비용 귀속 corpus에서 제외한다.
- progress를 전용 `ProgressIndicatorsState`로 분리했다. 기존 broad 상태 경로는
  `dorotiProgressScope=broad`로 유지하며, 기본 local과 같은 layout/input을 사용한다.
  안정된 GlobalKey와 기존 visited-section KeepAlive를 사용한다. 나머지 control의 broad 갱신은 남아 있다.
- Web semantics 전체 snapshot 프로토콜을 유지한다. 노드 JSON cache는 2 MiB / 2,048 nodes,
  node당 16 KiB로 제한하며 삭제/clear/dispose에서 정리한다. geometry/contentUnchanged 상태를 포함해
  완전히 같은 payload만 재사용한다. 변경 노드와 full→compact 전환은 다시 직렬화한다.
  명시적 delta/baseline 프로토콜은 도입하지 않았다. 전체 traversal/전송 bytes는 여전히 O(n)이다.

## P0/P1 원인 대조

source Release, Chromium hardware ANGLE / AMD Radeon 780M / D3D11, 1280×900 DPR 1.
Flutter pinned revision `6b182d2c7585eba26d4edce0f97630effd256c33`의 progress State와 local 범위를 대조했다.
공식 [Flutter build 비용 지침](https://docs.flutter.dev/perf/best-practices#control-build-cost)도 같은 상태 국소화를 설명한다.

| 상세 진단 원본 | 입력→commit | build | rebuild / forced | delegate / retained visit | tween check / restart |
| --- | ---: | ---: | ---: | ---: | ---: |
| work2-broad-causal | 535.6ms | 469.4ms | 2796 / 2627 | 3 / 23 | 217 / 13 |
| work2-local-causal | 80.3ms | 19.0ms | 56 / 53 | 0 / 0 | 5 / 1 |

broad의 상위 self type: Text 36.1ms, Theme 35.2ms, Material 31.3ms,
TextButton 27.3ms, RawGestureDetector 25.4ms. 여러 type에 작업이 분산되어 있다.
local의 Theme restart 1개는 선택 상태 변경과 함께 발생하며 임의로 중지하지 않았다.
전체 invalidation에서 발생하는 정당한 child update를 공용 dirty flag 삭제나 keep-alive skip으로 제거하지 않았다.
기존 identity skip, dirty 중복 방지, sliver key reconciliation은 pinned Flutter와 같은 기본 계약을 가진다.
P2의 별도 불필요한 dependency/forced rebuild 결함은 이 증거에서 확정하지 못했다.

Web host semantics 원인 run: local 직렬화 10.6ms → cache run 5.2ms, host 전체 12.5ms → 8.6ms.
전체 semantics flush는 20.6ms → 17.3ms였다. 단일 대조이며 장기 percentile 증거는 아니다.
semantics는 해당 frame의 raster 뒤에 있으므로 첫 commit에 더하거나 개선분을 onset에서 차감하지 않는다.

## 실행 조건과 한계

- 각 test/benchmark/compiler process는 20분 timeout. 성능과 build를 겹치지 않았고 retry는 0이다.
- SDK 10.0.400 / runtime 10.0.11, Release trimmed publish. `RunAOTCompilation`은 빈 값,
  `WasmBuildNative=true`, `WasmEnableSIMD=true`, `PublishTrimmed=true`를 msbuild로 확인했다.
  Jiterpreter property의 빈 값만으로 비활성이라고 결론내리지 않는다.
- [Microsoft runtime 문서](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-10.0)
  및 실제 relink/wasm-opt 로그를 대조했다. 기존 활성 SIMD/relink를 새 개선으로 주장하지 않는다.
- main LongTask/LoAF와 Worker callback은 별도 원본이다. managed heap/할당/GC 횟수는 runtime 관측치다.
  GC 정지시간, 전체 Wasm/native heap, GPU driver allocation, 물리 Hz/전원 고정은 미검증이다.
- P5: AOT/UI-raster Worker 분리는 보류. CPU work가 남지만 동일 작업량 native/Web 순수 runtime 비율은
  아직 분리하지 못했고, raster 병렬화로 broad build 자체를 없앨 근거도 없다.
  과거 full AOT compiler stack overflow와 partial AOT browser stack overflow를 재시도/덮어쓰기하지 않았다.

## 초기 실패와 수정

- background 서버 명령은 자동 승인 검토에서 `blocked by policy`로 거부되어 실행 세션 서버로 전환했다.
- 첫 profile ring은 2,048 boundaries여서 onset을 잃었다. 8,192로 확대하고 early capture를 추가했다.
- 최초 progress scroll 회귀는 왕복 wheel 값이 실제 offset 복귀를 보장한다고 가정해 실패했다.
  DOM에서 실제 progress 위치를 재탐색한 뒤 상태/픽셀/정지/column reversal을 확인했다.
- native framework allocation: 기본 tiering에서 128 bytes가 나와 기존 zero-allocation assertion FAIL.
  `DOTNET_TieredCompilation=0`의 별도 조건에서는 0 bytes/PASS. 기본 FAIL을 지우거나 Web 결과로 대체하지 않는다.
- package consumer 최초 빌드는 저장소의 artifacts compile 제외 규칙으로 Main이 포함되지 않아 실패했다.
  소비 프로젝트에서 Program.cs를 명시한 후 실제 package framework contract가 통과했다.
- 마지막 source runner smoke는 `libSkiaSharp` 로딩 오류로 첫 화면 전에 실패했다 (`source-final.log`).
  `dotnet build DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --no-restore -p:WasmBuildNative=true`로
  source용 native 라이브러리를 다시 링크하고 서버를 재시작했다. 이후 같은 progress/state/column 검사는
  1 PASS였다 (`source-native-relink.log`, `source-relinked.log`). 최초 실패는 보존하며 publish corpus에 합치지 않는다.

## 결과와 원본

원본:
`Doroti/validation/web-playwright/artifacts/sample-perf/work2-*` 및
`Doroti/artifacts/framework-web-work2/` (로그, source/package fingerprint, publish, trace 분석).
source-runner와 publish의 결과를 같은 corpus로 합치지 않는다.

물리 창 테두리/trackpad/page zoom/monitor DPR/120Hz, 한글 IME/caret/screen reader,
Flutter/native 픽셀 동등성과 패키지 소비 Web 앱의 실제 browser 실행은 `notVerified`이다.

### 최종 publish 성능

최종 managed scope는 clock=0도 정상 unwind하도록 보완했다. 이 최종 publish의 독립 corpus가
`work2-final-*`, `work2-detail-*`, `work2-sweep-*`이다. 각 performance run은 별도 browser로 순차 실행했다.

| 최소 marker 조건 | 입력→새 scene 3회 ms | 이후 steady p95 3회 ms | steady max 3회 ms | 판정 |
| --- | --- | --- | --- | --- |
| broad A | 429.0 / 368.1 / 355.0 | 20.2 / 20.0 / 19.6 | 25.4 / 29.0 / 24.3 | onset FAIL |
| local B | 53.4 / 49.1 / 47.1 | 19.9 / 19.9 / 20.0 | 25.0 / 27.6 / 23.7 | onset p95 53.4ms, FAIL |
| local stop→5초 idle→restart | 20.4 / 23.6 / 23.2 | 19.3 / 20.3 / 19.5 | 24.8 / 27.3 / 24.5 | onset 단기 3/3 통과, steady 한 run FAIL |

각 조건의 pooled steady p95/max는 A 20.0/29.0ms (895 intervals), B 19.9/27.6ms (895),
restart 19.6/27.3ms (896)다. pooled 결과로 개별 run FAIL을 덮지 않는다.
같은 publish에서 A/B onset median은 368.1→49.1ms다. native/Web 또는 Flutter 언어 성능 비율은 아니다.

상세 진단 ON 3회 onset은 60.6/47.3/52.5ms, callback max는 46.7/30.6/35.8ms이며
첫 1초의 >50ms callback은 모두 0개다. 최소 계측에서는 callback 자체를 측정하지 않았다.
ON/OFF onset median 차이는 52.5−49.1=3.4ms지만 작은 비짝지은 표본이므로 정확한 observer overhead로 일반화하지 않는다.
early diagnostics export는 이후 cadence에도 영향을 줄 수 있어 최종 steady gate에는 최소 marker corpus를 사용한다.

최종 상세 run의 첫 build는 17.4/9.6/10.6ms, semantics flush는 11.1/9.6/10.9ms.
최초 cache 대조에서는 보유 120 nodes 중 37개만 재직렬화했다. 기존 방식은 모든 120개를 직렬화했다.
최종 publish에서도 36~37개만 재직렬화하며 source-runner 대비 trim/relink의 차이는 별도로 둔다.
DOM은 전체 snapshot을 받아 content 3개 갱신을 유지한다. JSON 전송량/DOM 전체 순회가 delta가 된 것은 아니다.

| sweep callback p95 / max ms | run 1 | run 2 | run 3 |
| --- | --- | --- | --- |
| 왼쪽 새 구간 | 49.7 / 66.2 | 51.9 / 67.9 | 46.3 / 59.2 |
| 왼쪽 재방문 | 19.6 / 34.6 | 15.7 / 32.6 | 19.8 / 33.6 |
| 오른쪽 새 구간 | 133.6 / 222.3 | 137.6 / 224.7 | 112.3 / 258.2 |
| 오른쪽 재방문 | 15.5 / 44.7 | 14.5 / 35.4 | 13.3 / 32.2 |

신규/방문을 독립 구간으로 기록했고 무변화 경계의 전체 commit 간격을 render stall로 해석하지 않았다.
entry snapshots 및 retained frame별 type/counter는 각 `sweepSegments`에 보존했다.
`retainedTraceBuild` 통계는 구간 전 frame도 포함할 수 있으므로 구간 필터를 거친 callback과 혼동하지 않는다.
중복 paragraph 또는 개별 promotion이 이 지연을 지배한다고 입증하지 못해 추가 cache 확대/queue 정책 변경은 하지 않았다.

최종 strict resize는 DPR 1/2 모두 FAIL. 첫 기능 corpus에서는 active front 2개, exact settle 456.7/491.7ms였지만
최종 bounded corpus에서는 active front 1개, boundary max 270.2/265.5ms, target p95 936.1/1224.9ms,
latest exact 822.6/1174.2ms였다. 변동성과 실패를 모두 보존한다.
capacity grow를 superseded target으로 대신 판정하지 않고 grow 완료 뒤 복귀한 별도 검사는 두 DPR 모두 PASS다.
CSS 배율 검사 통과와 latency FAIL은 별개다. strict 시간 assertion을 추가했으며 기존 >1 front assertion도 유지했다.

### Flutter 및 CPU 관측

실제 reference `lib/main.dart`를 Release CanvasKit/JS로 빌드하고 1280×900 DPR1, semantics ON에서 3회 실행했다.
첫 progress 위치는 DOM에서 Communication group의 실제 button으로 확인했다.
Chrome rAF callback p95/max는 6.582/73.326, 6.073/69.484, 6.098/69.554ms.
pointerup→ARIA 갱신은 271.4/290.1/269.8ms이며 이 endpoint는 Doroti scene commit과 다르다.
Flutter는 group lazy 경계, Doroti는 세부 section 경계이며 방문 child 수가 동일하지 않다. 직접 성능 비율/시각 동등성은 미판정이다.

`work2-cpu-profile.cpu-trace.json`에 별도 Chrome CPU trace를 보존했다.
Wasm stack은 다수 `wasm-function[...]`로 남아 managed 함수명을 복원할 수 없었다.
`FrameworkWorkProfile`의 제한된 type span 이상으로 attribution을 추측하지 않았다.
trace의 V8 GC 이벤트를 managed GC 정지로 해석하지 않는다.

### 최종 자동 검증

| 검사 | 결과 |
| --- | --- |
| Web Release build / trimmed publish / TypeScript | PASS |
| native Material 전체, Windows sample 회귀, semantics, scroll, scheduler | PASS |
| framework work ring/type cap/thread/exception 계약 | tiering OFF 조건 PASS; 기본 allocation 128-byte FAIL 별도 보존 |
| publish 기본/auto/오타 및 모든 explicit renderer, progress pixels/stop/column/state, selection/text, protocol | 초기 16 PASS / 1 skip; skip한 sample restart는 최종 forced-direct에서 PASS |
| 최종 grow 완료/복귀 DPR1/2, 실제 WebGL context-loss 복구, sample worker restart, malformed protocol | 6 PASS |
| 최종 strict resize DPR1/2 | 2 FAIL |
| DPR2 실제 pointer로 sheet 열기/닫기 | 1 PASS |
| 최종 source runner native relink 후 progress/state/column | 1 PASS; relink 전 bootstrap FAIL 별도 보존 |
| 17개 package 의존성 local feed, 별도 소비 앱에서 framework 계약 실행, Ui/Widgets/Web DLL fingerprint | PASS; 최종 version `0.2.0-work2-final` |

기존 장시간 flicker suite는 반복 횟수가 고정되지 않은 30초 resize 루프를 확인해 중단했다.
그 실행의 앞선 2 grow PASS / 2 resize FAIL은 로그에 보존했고, 최종 suite에서는 해당 장시간 시험을 제외했다.
최종 bounded suite에는 8개 시험만 실행했으며 retry 0이었다. 중단한 시험은 PASS가 아니다.

분석 재현: `python Doroti/validation/web-playwright/summarize-work2.py`.
`summary.json`은 per-run/pooled 통계와 missing endpoint를 구분한다.
원본계획과 현재 work2의 미완료 체크는 유지하며 물리 수용이나 모든 성능 gate를 충족한 것으로 표시하지 않는다.

검증용 5089/5090/5091 서버는 종료했다. 기존 5088 source runner는 최신 빌드로 재시작했다.

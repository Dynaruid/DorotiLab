# C1 이후 최적화 실행

2026-09-05 사용자 요청: 세 후보를 순서대로 끝까지 검토한다. 시작 기준은 채택된 C1이다. 기존 실패와 native 7/10 ledger를 유지한다.

## 실행 전 조건

1. Encoder: 동일 wire bytes/validation을 보존하며 resource/string 임시 버퍼와 최종 복사를 제거하는 단일 변경을 먼저 평가한다. CLR prepared fixture의 byte allocation과 시간, 실제 CanvasKit stationary phase를 비교한다. 전송 중인 최종 배열의 소유권은 유지하며 새 전역 cache/pool을 추가하지 않는다.
2. Mapper: 현재 실제 할당과 복사 수명 확인 후 독립 후보로 판단한다. immutable document의 방어적 복사를 근거 없이 없애지 않는다. 기대 비용이 작거나 안전한 재사용 근거가 없으면 미적용으로 기록한다.
3. UI Worker 대기: 기존 trace에서 UI 실행과 ACK 대기 구간을 겹쳐 본다. transport와 CPU 점유를 구분하고, 원인이 확인되지 않은 scheduling 변경은 적용하지 않는다.

검증은 단일 변경별 deterministic wire/golden, malformed input, cache validation, browser hardware/correctness 및 stationary 비교다. 모든 build/test는 20분 timeout. native 실험은 자동 확대하지 않는다. C1 source/publish는 `.doroti/publish/n2-c1`과 `.doroti/checkpoints/c1-next`에 보존한다. 결과 수치 없이 FPS/GC pause/전체 RAM 개선을 주장하지 않는다.

## 구현 및 후보별 판정

- **Encoder 적용**: resource/string table 크기를 계산해 최종 owned byte 배열에 직접 쓴다. 임시 writer 2개와 확장·복사를 제거했다. 이 변경만의 CLR 절감은 호출당 약 624 bytes였다(`measure-c2`).
- **Encoder 크기 힌트 적용**: 이미 producer마다 있는 opt-in encoding cache에 이전 성공한 command byte length 정수만 기록한다. 다음 writer의 시작 capacity로 사용하며 256 bytes~1MiB로 제한한다. 프레임마다 새 buffer를 할당하므로 반환/전송 중인 배열을 재사용하지 않는다. 성공한 작은 scene은 즉시 힌트를 줄이고 `Clear()`도 초기화한다. 풀이나 전역 buffer 보관은 추가하지 않았다. CLR label `c2b`와 browser publish `c1-next-c2`는 두 encoder 변경을 포함한다.
- **Mapper 적용**: path의 argument 개수를 먼저 계산해 List capacity를 정하고, 각 command의 LINQ iterator를 index 순회로 바꿨다. 최종 `DisplayPath` 및 `DisplayListDocument`의 방어적 복사는 유지한다. 공용 document 소유권 변경과 picture/resource 캐시 확장은 하지 않았다. browser `c1-next-c3`는 C1+encoder+mapper다.
- **Scheduling 변경 미적용**: 아래 ACK 분석에서 기록된 동기 UI 점유가 대기 대부분과 겹친다. 별도의 전송 병목이나 불필요한 rAF 대기를 확정하지 못했으므로 scheduling을 변경하지 않는다. 이것은 조사 완료 후 미적용 판정이다.

## CLR 할당 결과

최종 비교는 Release, `DOTNET_TieredCompilation=0`, 준비된 입력과 warm cache, 5 batch 중앙값이다. Encoder는 batch당 200회, mapper는 10,000회다. 초기 tiered-JIT 결과도 보존했지만 최종 시간 수치와 섞지 않는다. 아래 값은 byte/call이며 소수점은 batch 평균 후 중앙값 계산에서 나온다.

| prepared fixture | C1 | 최종 | 감소 |
| --- | ---: | ---: | ---: |
| encoder, 33 commands, cache on | 29,732.56 | 18,776.00 | 36.85% |
| encoder, 528 commands, cache on | 404,015.84 | 236,222.72 | 41.53% |
| path mapper, 4 line segments + curve/close | 1,312 | 424 | 67.68% |
| path mapper, 128 line segments + curve/close | 17,408 | 2,664 | 84.70% |

cache off의 encoder 개선은 약 624 bytes/call로 작다. 크기 힌트 이득은 기존 `dorotiEncodingCache=1` 경로에서 얻는다. 입력 길이 급증/급감 시의 transient allocation은 여전히 있으며, 최종 buffer를 없앤 것은 아니다.

CLR 시간은 encoder cache-on 33 commands 17.59→17.46µs, 528 commands 248.37→231.14µs였다. cache-off 큰 fixture는 244.87→256.69µs로 증가한 표본도 있다. mapper는 4 segments 0.60→0.24µs, 128 segments 8.51→2.58µs였다. 작은 CLR fixture timing을 WASM/native FPS 개선으로 환산하지 않는다.

## 실제 브라우저 비교

보존한 C1 → encoder C2 → encoder+mapper C3를 실행한 뒤 C3 → C2 → C1 순서로 반복했다. 각각 2회 standalone stationary test, 매 test trace off/on/on/off, 동일 viewport 전환과 옵션을 사용했다. Chromium 151.0.7922.34 / Radeon 780M ANGLE D3D11 / headless / DPR 1, non-AOT다. native drag는 0회다.

trace-on에서 `MediaDependentCheck > 0`인 실제 metrics 변경 frame 16개/후보를 골라 비교한 중앙값이다. trace off와 섞지 않으며 단계별 중앙값을 합하지 않는다.

| 후보 | UI frame ms | build ms | mapping ms | encoding ms |
| --- | ---: | ---: | ---: | ---: |
| C1 | 17.25 | 4.25 | 1.40 | 4.00 |
| C2 (encoder) | 18.15 | 4.10 | 1.65 | 4.05 |
| C3 (encoder+mapper) | 17.50 | 4.05 | 1.30 | 4.20 |

**브라우저 전체 frame/encoding의 일관된 속도 개선은 확인되지 않았다.** mapper 감소도 이 작은 corpus만으로 통계적 개선이라고 주장하지 않는다. 동일 동작과 반복되는 임시 할당 감소를 근거로 두 국소 변경을 유지한다. C3의 첫 trace-on 마지막 전환은 settle polling 시 1 frame만 관측돼 다른 전환의 6~8 frames와 달랐다. 표본을 숨기거나 일정한 frame 수로 간주하지 않았고, 원시 결과와 역순 반복을 모두 보존했다. native p95, GC pause, 전체 RAM/VRAM 및 physical scan-out은 이번에 측정하지 않았다.

## UI Worker ACK 대기 조사

기존 `n1l` native trace에서 **terminal 전송 시각이 motion 안에 있는** 5 scene을 선택했다. 과거 표의 6 UI callback 기준 표본과 구간 정의가 달라 중앙값도 다르다.

- terminal→UI 수신 대기 합계 **97.50ms**.
- UI frame + resize apply의 합집합과 겹치는 시간 **94.00ms (96.4%)**.
- 대기 중앙값 17.80ms, 기록된 busy overlap 중앙값 17.10ms, 미귀속 중앙값 0.70ms.
- scene별 대기/overlap(ms): 36=16.90/16.10, 37=17.80/17.10, 38=23.40/22.80, 39=21.70/20.90, 40=17.70/17.10.

동기 managed frame 실행 중에는 UI Worker가 ACK handler를 처리할 수 없다는 해석과 일치한다. 미귀속 구간을 전송 비용이라고 단정하지 않으며, 알려진 UI interval만 사용하므로 완전한 event-loop profile은 아니다. 교차 Worker timestamp 정밀도 한계도 유지한다. 새 native 측정 없이 CPU 작업 감소를 우선할 근거를 확인했다.

## 검증 및 재현 자료

- managed wire golden은 **6330 bytes**, SHA-256 `66412ccb5e02519bbd8c11ecab5e63ce914e2db745f6d51110bbd03f89ccbe42` 그대로다. deterministic roundtrip, malformed/resource/Unicode/fuzz, cache eviction, 큰 scene↔작은 scene 및 이전 output 불변 검사를 통과했다.
- mapper fixture는 실제 내부 `ToPath`에 delegate를 연결한다. 빈 path, verb/좌표 순서, 원본 `reset()` 후 snapshot 보존을 확인했다. 4/128 segments의 출력 hash는 전후 동일하다.
- [집계 결과](../../Doroti/validation/web-playwright/artifacts/c1-next-summary.json), [집계 도구](../../Doroti/validation/web-playwright/summarize-c1-followup.py), [ACK 분석 도구](../../Doroti/validation/web-playwright/analyze-ui-terminal-wait.py). ACK 원시 결과는 `.doroti/checkpoints/c1-next/ack-n1.json`이다.
- managed baseline/candidate/final binaries와 `*measure*.stdout.log`, 20분 timeout runner는 `.doroti/checkpoints/c1-next/`에 있다. Encoder fixture: `dotnet run --project Doroti/validation/display-list-contract -c Release -- --measure-encoder`; mapper: `dotnet run --project Doroti/validation/browser-mapper -c Release`. 실제 실행에서는 timed runner를 사용했다.
- 각 browser 원시 `stationary-work.json`/served hash/source patch는 `artifacts/c1-next-{baseline,c2,c3,c3-r2,c2-r2,c1-r2}/test-results/`에 있다. 비교한 app은 해당 immutable publish root와 실제 served WASM hash로 판별한다.
- 최종 기본 `.doroti/publish/web-Publish-Release`는 실제 Release non-AOT publish로 갱신했다. 비교한 C3와 기본 publish의 Widgets/Host.Web/DisplayList WASM hash 3개 일치(`publish-identity.json`). Widgets는 기존 C1 hash `0ea1e463...`, Host.Web `54a1f108ed7e2a8338daadf05ac5f29e98e46d0c0a1f16e1d92962df6df0a05e`, DisplayList `d88c7520476689a7149fbfd5fe45993de83849956cf7db29323daac0a1d1814e`다.
- 최종 기본 publish의 CanvasKit 회귀 **20 PASS** (`artifacts/wrapper/c1-next-final/playwright.stdout.log`): all-opcode golden, transfer/lease terminal, F0/F1/F2 resize pixels, DPR 1/1.25/1.5/2, synthetic composition/input, focus 순서, split ownership, stall, continuous backing/front, bounded restart/resource replay, malformed protocol, lifecycle 회복, F3 raster wrap 및 별도 DPR2 project 검사다. 6개의 순차 stationary 비교도 모두 PASS. 실제 OS IME와 native physical acceptance는 해당하지 않는다.
- 직접 확인용 최신 서버: `http://127.0.0.1:5190/?dorotiResizeDiagnostics=1&dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1`. PID/serve root는 `.doroti/manual/c1-next/session.json`; 핵심 3개 WASM 실제 HTTP 응답 hash 확인은 `verified-assets.json`이다. 기존 5188=C0, 5189=C1 원본 비교 서버는 그대로 보존한다.
- 기본 `auto=document-webgl`에서도 golden/terminal **2 PASS**, 기존 raster wrap clipping **expected failure 1개 재현** (`c1-next-default`). Playwright의 `3 passed` 표시는 expected failure를 포함하므로 기능 3 PASS로 기록하지 않는다. 이 기존 결함은 이번 변경으로 수정됐다고 주장하지 않는다.
- 종료 확인: 자동 검증용 5191 listener와 wrapper browser는 종료됐다. 수동 비교 서버 5188/5189/5190만 유지한다. `git diff --check` PASS. 제품 변경 patch와 최종 4개 source hash는 `optimization.patch`, `final-source-identity.json`에 보존했다. C1 MediaQuery source hash `5866fd4b...`도 그대로다.

최종 선택은 **C1 + encoder + path mapper 국소 최적화**다. 세 번째 후보는 조사 후 scheduling 미변경으로 종료했다. 기존 자동 latency FAIL, capturedPresentation 미검증/과거 FAIL, native 7/10 ledger, PARTIAL · notQualified는 그대로다. 이번 개선의 입증 범위는 동일 데이터·정확성 보존과 prepared CLR 임시 할당 감소다.

# Web CanvasKit 렌더링 경로 재설계 — 작업계획 2부

작성·중단일: 2026-09-05
상태: **사용자 요청으로 이번 구현·실행 종료 / 후속 작업 계획만 작성 / PARTIAL · notQualified**

사용자가 반복 측정을 줄이도록 요청했고, 이후 외출을 위해 이번 작업을 여기서 마무리하도록 요청했다. 이 문서는 다음 작업의 계획이며 지금 계속 실행하라는 지시가 아니다. 다음 재개도 대규모 조건별 3회 matrix를 자동 실행하지 않는다.

- [1부 계획·진행 상태 보관본](history/26-09-05/web-canvaskit-redesign-v2-plan-part1.md)
- [이번 구현·실험 결과와 실패 이력](history/26-09-05/web-canvaskit-redesign-v2-results.md)
- `auto=document-webgl` 유지. CanvasKit은 experimental opt-in. Windows/Vulkan 기본 경로를 변경하지 않았다.

## 1. 이번에 남긴 구현과 판정

| 범위 | 현재 상태 |
| --- | --- |
| W0 계측 | native→observer 매칭/모호성, exact/caught-up/미도달, 첫·끝 포함 공백, 시간 가중 geometry/age, source·served asset hash, F0~F2와 Flutter 동등 fixture, GPU 픽셀 marker 구현 |
| managed phase clock | 기존 causal timestamp는 보존하고 실제 phase 작업 시간을 위한 opt-in `RecordedAtMicroseconds` 추가. FCR-3 contract PASS. F3 native에서 새 clock으로 phase 비용을 분해하는 실행은 남음 |
| W1 실행·표시 | 실제 Release non-AOT publish 격리 경로 정상. P0 direct, P1 Raster rAF, P2 bitmap exact/crop 및 main rAF 소비 옵션 구현·기능 검증. AOT는 아래 실패 경계로 기각 |
| W2 소유권·metrics | managed→JS fresh copy 이양, UI 중복 slice 제거, pool/replay journal 유지. input/focus/IME/lifecycle 전에 pending metrics를 적용하는 frame 병합 옵션 구현 |
| W3a | immutable Picture identity, bounded CPU mapping cache, 독립적인 validated command encoding cache 구현. resource/paragraph/가변 그래프를 무리하게 캐시하지 않음. golden/자원 오류/상한/해제 contract PASS |
| W3b | versioned retained wire/Raster picture registry **미구현·확대 기각**. F3 재사용률과 현재 mapping/encoding 비용으로 확대 효과를 입증하지 못함 |
| W4 | 옵션별 비교 완료. 실험 후보는 direct + display scheduling + owned copy + frame metrics 병합 + encoding cache. 기본값 승격은 보류. transport credit 2와 단일 Worker topology는 미구현 |
| W5 | 기능 회귀와 일부 native/capture 수행. **성능 목표 미달, 전체 환경·사용자 직접 수용 미검증** |

현재 실험 후보 URL 옵션:

```text
?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1
```

동작 확인에 사용한 산출물은 `.doroti/publish/web-Publish-Release/wwwroot`이다. 코드 변경 후에는 이전 산출물을 그대로 사용하지 말고 실제 non-AOT publish를 갱신한다. 이번 종료 시 테스트용 5188/5189 서버와 native capture 실행은 종료했다.

### 성능·캡처의 현재 결론

환경: Chromium 151.0.7922.34, AMD Radeon 780M / ANGLE D3D11, Windows DPI 192(200%), **165Hz**. 60Hz 측정으로 바꾸어 표기하지 않는다.

- TopLeft/150ms/reverse 독립 3회 비교: baseline p95 중앙값 **64.0ms**, 후보 **46.9ms**. 약 27% 개선이지만 목표 33.3ms 미달.
- 확대 matrix는 사용자 요청으로 **54회에서 중단**했다. Right의 6조건과 Bottom/150ms의 3조건에서 baseline/후보 각 27회 확보. 전체 24조건을 끝냈다고 쓰지 않는다.
- 이 54회는 자극 54 PASS, legacy/v2 성능 54 FAIL. trial p95 중앙값 baseline **61.3ms**, 후보 **47.1ms**. 단순 전체 중앙값은 약 23% 개선이며 조건별 비교를 대신하지 않는다. active 100ms 초과 공백은 없었다.
- F3 picture cache는 active frame의 대표 재사용이 **1/29 pictures(약 3.4%)**였다. 별도 비교에서 picture cache 및 Raster/main rAF, bitmap 경로가 F3 추종을 일관되게 개선하지 못했다. encoding cache의 단독 추가 이득도 20%로 입증한 것은 아니다.
- Flutter 공통 WGC endpoint: F0/F1/F2를 framework별 **1회씩, 총 6회** 실행. 마커 decode·자극 및 사전에 정한 width/height·gap 상대 기준은 이 표본에서 PASS. 물리 scan-out이나 반복 재현성 PASS가 아니다. **F2 center 오차는 Doroti가 더 컸으므로 표시 품질 전체 PASS로 확대하지 않는다.**
- 추가 F0 캡처 P1 rAF / P2 crop / P2 exact 각 1회는 완료됐다. notification p95는 각각 **19.7 / 14.0 / 43.5ms**. 활성 캡처 마커는 각각 **16/16, 15/15, 14/14** decode. 단일 진단 표본이며 F3 채택 근거가 아니다.
- 마지막 F3 후보 trace+capture는 실행하지 않았다. 사용자 횟수 축소 요청 이후 완료된 추가 native drag는 총 **9회**다. 캡처 경로 길이 오류 3건은 자극 전 setup 실패로 별도 보존했다.

### AOT·검증 실패 경계

- full AOT: Material/aot-instances compiler stack overflow로 실패. workspace compiler 복사본의 stack 확대 실험도 메모리 압박으로 중지했다. 설치 SDK는 수정하지 않았다.
- isolated partial AOT: publish는 성공했지만 F0/F1/F2 모두 browser `Maximum call stack size exceeded`로 시작 실패. 정상 AOT 성능 후보가 아니다.
- 공용 bin/obj 혼용에 따른 interpreter assertion은 빌드 모드별 `--artifacts-path` 격리로 non-AOT를 복구했다. AOT 실패 자체가 해결된 것은 아니다.
- 컴파일과 겹친 warm input 256.2ms > 250ms FAIL은 남아 있다. 단독 재실행은 아직 하지 않았다.
- viewport 요청 직후 이전 observer의 settled를 읽던 테스트 race와 embedded TextField assertion은 수정·해당 회귀 PASS. native 자극을 느리게 만들지 않았다.
- 최신 `canvaskit-resize-lifecycle.spec.ts`의 main/UI 100ms stall 및 최대화/복원/background 테스트는 **작성·TypeScript 확인만 했고 미실행**이다.
- 마지막 idle baseline/active motion age 분리 회귀와 selected encoding 옵션의 DPR/embedded 재검증은 **미실행**이다. 기존 절대 contentAge 결과와 새 active age를 섞지 않는다.

## 2. 다음 작업의 실행 예산과 중단 규칙

1. **한 작업 라운드의 native 드래그는 재실행까지 합쳐 최대 10회**, 기본 조건당 1회. 이전 54회·focus 비교·captured 원시 자료를 먼저 재분석한다.
2. full matrix와 조건별 3회 반복을 자동 재개하지 않는다. 아래 단계가 실패하거나 자료가 부족하면 남은 횟수 안에서 원인을 하나만 분리하고 결과를 기록한다.
3. 빌드와 CPU 부하가 큰 기능 테스트를 native 성능 측정과 동시에 실행하지 않는다. trace/capture on은 진단 corpus로 분리한다.
4. `.github/copilot-instructions.md`에 따라 모든 test/build 프로세스는 20분 제한. 새 코드 변경·실패·미해결 우려가 없으면 이미 PASS한 검사를 반복하지 않는다.
5. 기존 dirty 변경, 원시 FAIL, 이번 중단 기록을 보존한다. 자동 PASS와 사용자 수용을 분리한다.

## 3. 2부 실행 순서

### B0. 재개 기준 고정과 미검증 변경 정리 — native 0회

- 현재 Git diff와 non-AOT publish/served hash를 확인한다. 이번 중단 이후 변경 여부를 먼저 구분한다.
- 새 `resize-following.spec.ts` idle/active age 사례와 `resize-pixel-marker.spec.ts`를 실행한다. 기존 절대 age·FAIL은 보존한다. 필요하면 저장된 trace를 재계산한 별도 sidecar를 남기고 드래그는 반복하지 않는다.
- `canvaskit-resize-lifecycle.spec.ts`와 선택 후보의 `canvaskit-resize-input.spec.ts`를 필수 범위만 실행한다. UI Worker 식별 및 maximize/restore의 새 epoch 대기를 먼저 확인한다.
- warm input 실패 항목 `input-regression.spec.ts:21`을 컴파일 없이 단독 확인한다. synthetic composition PASS를 OS IME PASS로 쓰지 않는다.
- correctness FAIL이면 해당 원인부터 고치고 성능 후보 기본값 채택을 보류한다.

### B1. 실제 F3 phase 병목 분해 — native 최대 1회

- TopLeft / 150ms / reverse, 기존 후보, trace on / capture off로 한 번만 측정한다.
- 새 managed `RecordedAtMicroseconds`와 scene sequence를 연결해 build, layout+compositing, paint, scene 구성, map/encode/interop, semantics 시간을 분리한다. causal timestamp를 작업 시간으로 사용하지 않는다.
- scene-encoded→send, UI→Raster 전송, replay→submit, terminal→UI 처리 지연을 구분한다. UI 동기 작업으로 늦어진 ACK를 전송 자체의 비용으로 계산하지 않는다.
- 기존 진단의 send→Raster median 약 0.1ms, Raster replay→submit median 약 3.4ms, ACK 처리 median 약 18.2ms는 단서다. 기존 값과 새 실행을 같은 표본으로 합치지 않는다.

예시: 현재 코드와 산출물이 같을 때만 `-SkipBuild`를 사용한다.

```powershell
$env:DOROTI_FAST_RESIZE_EDGE='TopLeft'
$env:DOROTI_FAST_RESIZE_MS='150'
$env:DOROTI_FAST_RESIZE_MOTION='reverse'
$env:DOROTI_FAST_RESIZE_RUNS='1'
$env:DOROTI_FAST_RESIZE_TRACE='1'
$env:DOROTI_FAST_RESIZE_CAPTURE='0'
$env:DOROTI_RESIZE_EXPERIMENT_QUERY='&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1'
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -SkipBuild -Configuration Release -BuildMode Publish -FastResize -Port 5188 -ArtifactLabel v2p2-trace
```

### B2. 측정된 지배 비용 하나만 개선 — native 0회

- build/layout/paint가 지배하면 공통 framework의 dirty 전파·constraint 변경·재사용 가능 subtree를 확인한다. 실제 필요한 relayout/줄바꿈을 생략하거나 demo를 단순화해 성능을 통과시키지 않는다.
- scene/encoding이 지배하면 현재 encoding cache의 hit/bytes/allocation과 자원 포함 command 비용을 확인한다. F1만의 재사용 이득으로 W3b를 부활시키지 않는다.
- 전송 대기가 지배할 때만 credit 2 prototype을 검토한다. in-flight/pending 상한, exactly-once terminal, resource generation, rebind/restart journal 계약을 먼저 설계한다. 단순 ACK 대기 완화로 오래된 scene을 쌓지 않는다.
- 두 Worker 경계가 주원인이라는 증거가 없으면 단일 Worker topology를 구현하지 않는다. AOT도 재현 가능한 compiler/runtime 원인과 해결 가설이 생기기 전에는 대형 재빌드하지 않는다.
- 기존 baseline 30fps는 비교용으로 보존한다. 고정 FPS 제한을 새 해결책으로 삼지 않는다.

### B3. 최소 대응 성능 비교 — native 최대 6회

다음 3조건에서 baseline/수정 후보를 한 번씩 교대로 비교한다. 조건을 더 늘리지 않는다.

| 조건 | 목적 | 횟수 |
| --- | --- | ---: |
| TopLeft / 150ms / reverse | 기존 focus 자료와 비교, moving origin·빠른 반전 | 2 |
| Left / 150ms / reverse | 이번 확대 matrix에서 미실행한 방향 | 2 |
| Bottom / 600ms / expand | 이번 확대 matrix에서 미실행한 지속 자극 | 2 |

- 같은 binary의 옵션 A/B가 가능하면 우선 사용한다. source와 실제 served asset hash를 남긴다.
- p95 ≤ 33.3ms, interval ≤ 33.3ms, 정확한 settle ≤ 50ms, active >100ms 공백 0 기준을 유지한다.
- 첫 프레임·geometry 적분·age·p99/max도 본다. 단일 표본은 탐색 근거로만 사용하며 안정적 20% 개선을 입증했다고 쓰지 않는다.
- 실패하면 미달 단계와 원인을 기록한다. 남은 방향을 모두 채우기 위한 반복을 시작하지 않는다.

### B4. F3 캡처와 품질 경계 — native 최대 2회

- 아직 없는 F3 baseline/수정 후보의 같은 WGC endpoint 캡처를 각각 한 번 수행한다.
- F6-R PNG는 전체 모니터 캡처다. native window rect와 창 이동을 보정해 root/edge/center를 계산한다. PNG 전체 크기를 client 크기로 쓰지 않는다.
- 픽셀 marker decode, 오래된 frame·origin 노출, width/height/center와 wrap landmark를 확인한다. F2의 center 상대 오차가 컸던 기존 원시 PNG도 우선 재분석한다.
- 추가 center/wrap gate가 필요하면 새 후보 실행 **전에** 정의한다. 기존 width/height 상대 PASS를 center/wrap PASS로 확대하지 않는다.
- 남은 최대 1회는 자극/캡처가 불성립한 경우에만 재실행에 사용한다. 이로써 B1+B3+B4+예비 합계 최대 10회다.

### B5. 기록·사용자 수용 — 자동 드래그 추가 없음

- correctness, stimulus, latency, capturedPresentation, Flutter comparability, manualAcceptance를 나누어 보고한다.
- 60Hz, monitor 이동, 실제 OS IME, 물리 scan-out·직접 드래그는 현재 notVerified다. 환경과 사용자가 준비됐을 때 별도 범위로 확인하며 지금 자동 측정을 늘리지 않는다.
- 사용자가 Right/Bottom/Left/TopLeft·반전·줄바꿈 추종이 충분하다고 확인하기 전에는 W5 완료 또는 기본 경로 승격으로 표시하지 않는다.
- 결과 보고서와 이 문서를 갱신하고 실행한 소유 서버/드라이버를 종료한다.

## 4. 재개 체크리스트

- [x] 이번 실행 종료, 기존 1부 계획·실패 이력 보존
- [x] 반복 예산 축소 및 2부 계획 작성
- [ ] B0 최신 미실행 기능 회귀와 warm input 단독 확인
- [ ] B1 실제 F3 managed phase 진단 1회
- [ ] B2 지배 비용에 근거한 필요한 수정 하나
- [ ] B3 최소 대응 비교 6회 이내
- [ ] B4 F3 captured baseline/후보 2회 이내
- [ ] B5 사용자 직접 수용 및 최종 판정

**이번 작업 종료는 성능 목표 달성이나 사용자 수용 완료를 뜻하지 않는다. 다음 실행은 이 2부 범위와 축소된 횟수 예산으로 재개한다.**

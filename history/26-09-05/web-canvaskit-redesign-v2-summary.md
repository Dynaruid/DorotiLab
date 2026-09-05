# Web CanvasKit 렌더링 경로 재설계 v2 — work.md 요약

정리일: 2026-09-05. 루트 `work.md`의 1부 진행 상태와 2부 실행 결과를 요약한 보관 문서다.

**최종 상태: 2부 자동 작업·기록 완료 / latency FAIL / capturedPresentation FAIL·partial / 사용자 수용 notVerified / PARTIAL·notQualified.** W5 성능 목표와 사용자 직접 수용은 미완료다. `auto=document-webgl`을 유지하며 CanvasKit은 experimental opt-in이다. Windows/Vulkan 기본 경로는 변경하지 않았다.

## 구현 및 수정 사항

- W0: native→observer 매칭, exact/caught-up/미도달, 첫·끝 프레임 포함 공백, 시간 가중 geometry/age, source·served asset hash, F0~F2/Flutter 동등 fixture, GPU 픽셀 marker 계측을 구현했다. managed phase는 기존 causal timestamp와 별도로 opt-in `RecordedAtMicroseconds`를 사용한다.
- W1~W2: 격리된 Release non-AOT publish와 direct/Raster rAF/bitmap exact·crop 경로를 검증했다. managed→JS fresh copy 이양, UI 중복 slice 제거, pool/replay journal 유지, 입력·focus·IME·lifecycle 전에 pending metrics를 적용하는 frame 병합 옵션을 구현했다.
- W3a~W4: immutable Picture identity, bounded CPU mapping cache, validated command encoding cache를 구현했다. 실험 후보는 direct + display scheduling + owned copy + frame metrics 병합 + encoding cache다. retained wire/Raster picture registry(W3b)는 효과 근거 부족으로 확대를 기각했으며 transport credit 2와 단일 Worker topology도 미구현이다.
- 2부 B2: DisplayList 문자열의 중복 Unicode 검사·UTF-8 변환을 unique value당 strict 변환 1회로 통합하고 bytes를 정렬과 출력에 공유했다. wire bytes와 자원·command 검증은 유지했다.
- 2부 검증 도구: 실제 background 검증을 막던 Playwright focus emulation, capture-only에서도 실행되던 Windows 화면 분석, 첫 setup PNG를 사용해 8px 오차가 생기던 chrome offset calibration을 수정했다. calibration은 마지막 pre-motion decoded frame을 사용한다.

실험 후보 옵션:

```text
?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1
```

검증 산출물은 `.doroti/publish/web-Publish-Release/wwwroot`이다. 코드 변경 후에는 실제 non-AOT publish를 갱신하고 source·served asset hash를 확인해야 한다.

## 2부 B0~B5 결과

| 단계 | 결과 |
| --- | --- |
| B0 기능 확인·기존 자료 재분석 | 브라우저 14 PASS. warm input 단독 141.8ms <250ms PASS. 기존 54회 age를 sidecar로 재분석하고 원시 FAIL을 보존 |
| B1 F3 phase 진단 | native 1회. UI frame 중앙값 19.2ms, scene 구성+map/encode/interop 6.3ms, encode 3.2ms. UI→Raster 0.1ms, Raster replay→submit 3.3ms, terminal→UI 처리 17.5ms. ACK 처리 지연은 전송 비용으로 계산하지 않음 |
| B2 중복 UTF-8 변환 제거 | managed wire 계약·TypeScript PASS, 브라우저 5 PASS. encoder 수정 자체의 독립 성능 개선량은 미입증 |
| B3 최소 성능 비교 | 3조건 baseline/후보 각 1회, native 6회. 자극 6 PASS / latency 6 FAIL |
| B4 F3 WGC 캡처 | 첫 수집 실패 1회 + 수정 후 유효 baseline/후보 2회. 수집·decode 정상, 상대 gap·center gate FAIL |
| B5 기록·종료 | 자동 판정·보고서 완료, 소유 서버·드라이버·브라우저 종료 기록. 사용자 직접 수용은 notVerified |

2부 native 실행은 실패 재실행을 포함해 **총 10회**로 종료했다. correctness는 브라우저 총 **19 PASS**, managed wire 계약·TypeScript PASS다. 초기 harness FAIL은 별도 보존한다.

측정 환경은 Chromium 151.0.7922.34, AMD Radeon 780M / ANGLE D3D11, Windows DPI 192(200%), **165Hz**다.

| B3 조건 | baseline p95 | 후보 p95 | 33.3ms 목표 |
| --- | ---: | ---: | --- |
| TopLeft / 150ms / reverse | 63.6ms | 45.2ms | FAIL |
| Left / 150ms / reverse | 63.8ms | 61.0ms | FAIL |
| Bottom / 600ms / expand | 56.8ms | 41.3ms | FAIL |

모든 B3 실행에서 미도달·geometry uncovered·active >100ms gap은 0이다. 후보 Left는 interval p95도 34.2ms로 미달하고 active age·settle이 퇴행했으며 첫 front는 세 조건 모두 늦어졌다. 비교는 수정된 동일 binary의 옵션 A/B이므로 encoder 수정 전후 비교나 안정적인 20% 개선을 입증하지 않는다.

F3 유효 WGC 캡처는 baseline/후보 모두 drop·error 0, active marker 23/23·24/24 decode였다. width/height 상대 gate는 PASS지만 boundary gap p95 52.08→58.94ms와 center X/Y p95 200.5/201.5→283/284 physical px는 상대 gate FAIL이다. 후보 PNG에서 작은 이전 프레임과 오른쪽·아래 공백이 보였고 최종 크기는 맞아졌다. marker geometry, 일부 PNG의 2줄 유지, WGC callback은 dynamic wrap 전체 수용이나 물리 scan-out을 증명하지 않는다. 번역 popup이 가린 부위의 품질도 미판정이다.

## 보존할 1부 이력과 실패 경계

- TopLeft/150ms/reverse 독립 3회에서 p95 중앙값은 64.0→46.9ms로 개선됐지만 목표 미달이었다.
- 확대 matrix는 사용자 요청으로 54회에서 중단했다. Right 6조건과 Bottom/150ms 3조건에서 baseline/후보 각 27회를 확보했으며 전체 24조건을 완료하지 않았다. 자극 54 PASS / legacy·v2 성능 54 FAIL, trial p95 중앙값 61.3→47.1ms였다. 단순 전체 중앙값은 조건별 비교를 대신하지 않는다.
- F3 picture 재사용은 대표 active frame에서 1/29(약 3.4%)였다. picture cache, Raster/main rAF, bitmap 경로가 추종을 일관되게 개선하지 못했고 encoding cache 단독 20% 이득도 입증하지 못했다.
- Flutter F0/F1/F2 공통 WGC endpoint는 framework별 1회씩 총 6회였다. 해당 표본의 marker·자극·width/height·gap 상대 gate만 PASS다. F2 center 오차는 Doroti 262.5px / Flutter 178.5px로 Doroti가 더 컸다. F3 Flutter 비교와 반복 재현성·전체 표시 품질 수용은 미검증이다.
- 추가 F0 P1 rAF/P2 crop/P2 exact 캡처 각 1회의 notification p95는 19.7/14.0/43.5ms였다. 단일 진단 표본이며 F3 채택 근거가 아니다. 1부의 횟수 축소 이후 추가 native 9회 및 자극 전 경로 길이 setup 실패 3건은 2부 10회와 별도 이력이다.
- full AOT는 Material/aot-instances compiler stack overflow로 실패했다. workspace compiler stack 확대도 메모리 압박으로 중지했으며 설치 SDK는 수정하지 않았다. partial AOT는 publish 성공 후 F0/F1/F2 모두 browser `Maximum call stack size exceeded`로 시작 실패했다.
- 공용 bin/obj 혼용 interpreter assertion은 `--artifacts-path` 격리로 non-AOT를 복구했지만 AOT 실패는 해결하지 못했다.
- 컴파일과 겹친 warm input 256.2ms >250ms FAIL은 이후 단독 PASS와 함께 보존한다. 1부 당시 미실행이던 lifecycle·idle/active age·선택 후보 회귀와 phase 진단의 후속 결과는 2부 보고서를 따른다.

## 남은 과제와 후속 작업 제약

- W5 목표인 p95·interval ≤33.3ms, exact settle ≤50ms, active >100ms 공백 0을 유지한다. 현재 latency와 capturedPresentation 실패 원인을 해결해야 한다.
- 60Hz, monitor 이동, 실제 OS IME, 물리 scan-out, 사용자 직접 Right/Bottom/Left/TopLeft 드래그·반전·줄바꿈 수용은 **notVerified**다. 자동 PASS나 synthetic composition을 사용자 수용으로 확대하지 않는다.
- 이번 2부 자동 실행은 종료됐으며 예산 10회를 모두 사용했다. full matrix·조건별 3회 반복을 자동 재개하지 않는다. 후속 라운드도 재실행 포함 native 최대 10회, 기본 조건당 1회를 지키고 기존 원시 자료를 먼저 분석한다.
- 측정된 지배 비용 하나를 분리해 개선한다. 전송 병목 근거 없이 credit 2/단일 Worker로 확대하거나 해결 가설 없이 AOT 대형 재빌드를 반복하지 않는다. 실제 relayout·줄바꿈을 생략하거나 demo 단순화·고정 FPS 제한으로 통과시키지 않는다.
- build/CPU 기능 검사와 native 성능 측정을 겹치지 않는다. trace/capture-on 진단 corpus를 성능 corpus와 분리하고 build/test는 20분 제한을 적용한다. 기존 dirty 변경과 원시 FAIL을 보존한다.
- 사용자 직접 수용 전에는 W5 완료나 기본 경로 승격으로 표시하지 않는다.

## 상세 기록과 증거 위치

- [2부 실행 결과·최종 판정·build identity](web-canvaskit-redesign-v2-part2-results.md)
- [1부 계획·진행 상태 보관본](web-canvaskit-redesign-v2-plan-part1.md)
- [1부 구현·실험 결과와 실패 이력](web-canvaskit-redesign-v2-results.md)
- 원시 자료: `Doroti/validation/web-playwright/artifacts/<label>/test-results/`, 실행 로그: `Doroti/validation/web-playwright/artifacts/wrapper/<label>/`.
- 주요 분석: `artifacts/p2-summary.json`, `artifacts/p2-capture-comparison.json`, `artifacts/p2b0/previous-age-sidecar.json`, `artifacts/p2b1/phases.json`, 각 캡처의 `capture-sidecar.json`. 교정 전 `capture-first-reference.json`은 수용 근거에서 제외한다. 상세 보고서에 측정 시 source·served asset·driver hash를 보존했다.

이 문서는 기존 기록의 요약이다. 보관 작업에서 제품 테스트나 native 측정을 새로 실행하지 않았다.

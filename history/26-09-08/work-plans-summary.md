# work.md · work2.md 작업 요약 및 보관

보관일: 2026-09-08. 사용자의 명시적 요청에 따라 두 문서를 요약하고 루트에서
삭제한다. **문서 정리는 작업 전체 완료나 기존 FAIL/PARTIAL의 PASS 전환을 뜻하지 않는다.**
아래는 삭제 직전 문서의 기록을 요약한 것으로, 이번 보관 중 앱 빌드·성능·물리
검증을 새로 실행한 결과가 아니다. 원문의 과거 결론과 후속 변경을 구분한다.

| 자료 | 내용 |
| --- | --- |
| [work.md 원본](work.original.md) | 구조 개편, 동일 작업 실행 비용 연구, HAMT + indexed 단일화, C2/C3/C4 실험의 최종 기록 |
| [work2.md 원본](work2.original.md) | framework 병목 조사, 상태 범위·semantics·이미지·열 복귀 개선 기록 |
| [보관 manifest](work-plans-archive-manifest.json) | 삭제 전 원본의 byte 길이·SHA-256·보관 경로 |
| [후속 구성 계획](../../work3.md) | WASM 의존성 기반 병렬 계산 계획 및 threads 부팅 선행 과제 |

원본은 byte 단위로 보존했다. **원본 안의 상대 경로는 작성 당시 저장소 루트
기준**이므로 보관 디렉터리 기준으로 해석하지 않는다. 아래 요약의 링크는
보관 위치에 맞게 구성했다. 원문의 localhost 주소는 당시 실행 기록이며 현재
서버의 가동 상태를 보증하지 않는다.

## 1. work.md — 재빌드·레이아웃 구조와 동일 작업 실행 비용

### 목적과 최종 구현 상태

긴 resize·열 전환·section 진입 callback을 줄이되 공용 Widget/Element/
RenderObject의 State 수명과 동작을 유지하는 작업이다. 초기 P0–P6에서는
상태 범위와 section viewport를 개선했고, 후속 Q0–Q4에서는 Flutter와 같은
논리 대상·callback·처리 시점을 유지하면서 복사·할당·간접 호출 비용을 줄였다.

**마지막 기록의 제품 기준은 HAMT + indexed다.** 초기 opt-in/기본 승격 보류
결론 이후 사용자가 단일화를 명시적으로 요청했다. 이 요청으로 Dictionary
복사와 sample eager/일반 SliverList 선택 분기, HAMT 빌드 플래그 및 viewport
옵션을 제거했다. 다른 위젯이 사용하는 공용 SliverList는 유지했다.
이는 성능 gate 통과에 따른 승격과 구분한다.

### 구현·실험 결과

| 작업 | 결과와 한계 |
| --- | --- |
| P0–P2 비용 측정·상태 소유권 | 독립 baseline corpus, navigation과 안정적인 본문 분리, section 직접 builder·상태/소유권 계약 및 disposal 검사 수행 |
| P3 section viewport | SectionExtentIndex/SectionList/SectionFocusCoordinator, lazy 생성·End·anchor·Tab·열 이동 State 및 동적 재정렬 검사 통과. 화면 밖 접근성·dependency/IME/overlay 전체 계약은 미완료 |
| P4/P5 실행 비용·budget | indexed prefix/measurement 비교, build/layout 중 yield와 speculative cache를 도입하지 않음. 당시 새 Worker/AOT는 미채택 |
| 카로셀·스크롤 | 공용 Skia 그림 명령 재사용과 임시 path 해제, fractional 위치·무효화·cache 상한 적용. 사용자도 카로셀 개선을 관찰. 새 section의 긴 callback은 남음 |
| 반응형 열 구성 | Scaffold의 MediaQuery 변환을 슬롯으로 이동, 오른쪽 scroll owner 유지·SectionList 재소유 및 안정된 키로 깊은 열 복귀 State/anchor 보존 |
| C1 HAMT | 맵의 전체 복사를 경로 복사·구조 공유로 변경. map snapshot/충돌/삭제/수명, C# 20노드·155이벤트, 고정 Flutter 17 callback 계약 통과. 전체 sample trace 동등성 증명은 아님 |
| HAMT + indexed 단일화 | 사용자의 명시적 채택 요청으로 적용. 맵·section·State·focus·anchor, Web build/TypeScript 및 browser 회귀 PASS. 전체 성능·메모리 수용은 별도 |
| C2/C3/C4 | cache closure/adapter, typed sliver bridge, 비교자·dirty 병합·snapshot 처리 후보를 함께 구현·검증. 반복 가능한 WASM 개선 미확인으로 **미채택·제품 원복**, 패치와 실패 보존 |

### 주요 수치와 해석

- 초기 indexed 비교의 callback p95 실행별 중앙값: 같은 열 **341→159.1ms**,
  열 전환 **1699.8→351.0ms**. 상대 구조 개선과 최종 latency 통과는 별개다.
- 그림 재사용 후 callback 중앙값: 일반 카로셀 **11.7→7.6ms**, snapping
  **11.6→6.8ms**, 세로 스크롤 **10.15→6.45ms**. commit 간격은 약
  16.5–17ms이며 물리 FPS 상승을 입증하지 않는다. 신규 section에서
  158.6/247.8ms의 긴 callback이 남았다.
- 열 구성 후속 상세 비교 2회의 최대 callback은 약 **45.5–49.0% 감소**.
  최소 계측 한 쌍의 최종 반영 지연은 열 전환 **686.5→149.7ms**, 같은 열
  **51.5→51.6ms**. 모든 resize나 실제 창 drag의 개선으로 일반화하지 않는다.
- C1 eager 비교: 맵 할당 **1,400,800→396,992 bytes(-71.7%)**.
  초기 맵 put 시간은 전체 callback의 약 0.47%로, 전체 지연 해결의 근거는
  부족했다. 결과는 `componentOnly/PARTIAL`.
- HAMT indexed 추가 3쌍: 같은 열 정착 **350.3→396.4ms**로 악화했고 열 전환
  결과도 쌍별 방향이 달랐다. 맵 할당 **20,400→5,760 bytes** 절감은 확인했으나
  전체 heap·프로세스 메모리 또는 처리시간 개선은 입증되지 않았다.
- C2/C3/C4 같은 열 최대 callback 중앙값 **320.8→330.4ms**.
  C2 CLR cache-hit 할당 절감은 있었지만 해당 Web resize의 cache 호출은 0회다.
  열 전환 2·3번 쌍은 LayoutWork **1208/1295**로 달라 `notComparable`이다.
  이 결과를 코드 자체의 확정적 지연 악화나 전체 Flutter 불일치로 단정하지 않는다.

### 남은 수용과 증거

P3/P6 및 Q0/Q1/Q3/Q4의 전체 종료 조건은 미완료다. strict latency,
전체 logical target/semantics trace, GC pause·live/transient memory,
화면 밖 screen-reader 탐색·IME/overlay·물리 표시/입력 수용이 남았다.
Windows/Web/MAUI/Android/Linux 빌드와 사용 가능한 자동 검사, Linux Qt xcb
실행 기록은 있으나 WSLg Wayland는 이전 publish에서도 재현되는 protocol
오류로 FAIL이었다. Android 기기·Apple 실행 등은 `notVerified`를 유지한다.

성능 실행 ledger는 실험별로 분리한다. 카로셀 20회, 반응형 후속 19회,
C1 초기 20회, indexed 추가 6회, 단일화 검증 7개 명령, C2/C3/C4 20개 명령의
기록을 하나의 반복 corpus로 합치지 않는다. 최초 실패·재시도·복구 빌드도 보존했다.

상세 기록:

- [구조 개편](wasm-section-structure.md), [카로셀·스크롤](carousel-scroll-performance.md),
  [반응형 후속](responsive-layout-followup.md)
- [동일 작업/HAMT 실행](wasm-same-work-execution.md),
  [indexed HAMT 비교](indexed-hamt-comparison.md), [단일화](hamt-indexed-unification.md)
- [C2/C3/C4 실행](wasm-c234-execution.md), [집계](wasm-c234-summary.json),
  [미채택 패치](wasm-c234-candidate.patch)

## 2. work2.md — framework 병목 조사와 국소 개선

### 목적과 적용 결과

첫 progress·idle restart·section 진입·resize에서 build/layout/semantics/
raster/queue 비용을 분리하고, 샘플의 넓은 상태 갱신과 공용 framework 비용을
구분하는 작업이다. **최종 상태는 개선 적용 및 자동 검증 수행 / 전체 수용 PARTIAL**이다.
초기의 조사 전용 설명·미체크 항목보다 원본 7–10절의 후속 실행 기록을 함께 읽어야 한다.

| 작업 | 적용·검증 결과 | 남은 항목 |
| --- | --- | --- |
| P0 계측 | direct work/profile/GC 관측, early causal capture, 최소 OFF marker, main/Worker 분리와 Chrome CPU trace | 정확한 managed symbol·GC pause·WASM heap 귀속, 모든 span 연결과 cold boot end-to-end |
| P1 상태 범위 | broad/local fixture에서 rebuild **2796→56**, delegate **3→0**. local progress 채택 및 scroll/column/state/pixel 회귀 | Flutter timeline 3회와 비교했으나 commit/pixel endpoint·lazy 단위가 달라 엄밀한 동등성 미완료 |
| P2 공용 rebuild 조사 | pinned source·상위 hot type·implicit owner 검토/계측 | 별도의 불필요한 공용 rebuild 결함은 미확정; dirty/key/keep-alive 계약 우회 미채택 |
| P3 semantics | 동일 노드 JSON 재사용, 2MiB/2048개 상한, full snapshot 및 content/geometry/listener 계약 보존 | tree projection·전체 순회/전송량 O(n), delta 도입과 물리 접근성 검증 보류 |
| P4 section·resize | 새/방문 section 각 3회, DPR1/2 grow 후 복귀, strict resize 실패 보존, context recovery | 신규 section CPU와 resize latency 미달; paragraph/promotion을 단독 원인으로 확정하지 않음 |
| P5 runtime | SDK/runtime 평가값과 공식 자료 검토 | 동일 작업 native/Web 비용 비율 미분리. 당시 AOT/Worker 분리 보류 및 이전 실패 보존 |
| P6 통합 | source/Release publish/native/package 자동 검증, 한국어·영어 실행 안내와 fingerprint 보존 | 별도 package Web 앱의 실제 browser 수용, 물리 입력/IME/120Hz 미검증 |

### 사용자 관찰 이후 수정

1. 사용자는 progress 시작 버벅임이 체감상 해소됐다고 확인했다. 이어서 선택/
   버튼 State 범위를 줄이고 responsive 내용·delegate와 navigation을 재사용했다.
   선택 버튼 대조는 **508.1→80.5ms**, 최종 최소 계측 3회는
   **78.9/80.9/78.3ms**였다. 이 관찰을 전체 onset 성능 PASS로 확장하지 않는다.
2. 같은 열 resize의 최종 exact는 **51.2/69.6/59.7ms**지만 active tracking
   p95는 **122.7/126.8/131.5ms**로 전체 실시간 수용은 PARTIAL이다.
3. 이미지 구간 진입과 정지 후 scroll 재시작 문제에 대해 공용 target-size decode,
   ResizeImage 키·종횡비·전달을 수정하고 sample 표시 픽셀 수 및 geometry-only
   semantics 임시 객체를 줄였다. 전체 section 진입·restart·resize 수용은 PARTIAL이다.
4. 깊은 스크롤 후 2열→1열→2열에서 오른쪽이 회색으로 남는 오류를 재현하고
   공용 `Element.activate`의 nullable dependency 정리를 수정했다. State/inherited
   유지 및 오른쪽 실제 픽셀 검증을 추가했다. correctness 수정은 기존 성능 FAIL을
   해소한 것으로 처리하지 않는다.

상세 기록:

- [framework 실행 결과](../26-09-07/web-framework-work2-results.md)
- [상태·resize 후속](../26-09-07/web-state-resize-followup.md)
- [이미지·scroll 후속](../26-09-07/web-image-scroll-followup.md)
- [열 복귀 수정](../26-09-07/web-column-return-fix.md)

## 3. 보관 이후 이어갈 조건

두 문서의 기존 30% 구조 개선 기준 및 16.7/33.3/50/100ms 계열 latency 목표를
완화하지 않는다. 자동 commit 간격·build 통과와 물리 FPS·표시/접근성 수용을
구분하며, 기능 PASS로 성능 FAIL이나 `notVerified`를 덮어쓰지 않는다.
테스트는 명령당 20분 timeout, 실행·retry·warm-up을 포함하는 명시적 예산을 따른다.

후속 [work3.md](../../work3.md)는 부모/형제 의존성이 풀린 순수 계산부터 실행하는
작업 그래프와 2개 계산 스레드, UI owner의 검증·반영을 계획한다. 이 문서들의
과거 “Worker 미채택”은 당시 판단으로 보존하며 새로운 사용자 요청과 구분한다.
현재 thread 활성화 기록은 [bootstrap 보고](wasm-threads-bootstrap.md)를 따른다.
설정·빌드는 통과했으나 Worker runtime 부팅이 FAIL이며, 병렬 레이아웃 구현·성능
검증의 선행 과제다. 이를 이전 두 문서의 완료 성과로 포함하지 않는다.

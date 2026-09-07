# 상태 변경과 resize 후속 개선

2026-09-07. 이전 work2 미커밋 변경을 유지한 상태에서 추가 구현했다.

## 사용자 관찰과 범위

사용자는 이전 progress 시작 직전 버벅임이 체감상 없어졌다고 확인했다.
버튼 상태 변경 직전 지연과 창 크기 조절 중 추종 지연은 남아 있다는 후속 요청이다.
이 관찰은 이전 progress 변경에 대한 사용자 확인이며, 이번 추가 변경의 물리 수용은 아직 없다.

## 원인과 변경

- 기존 Week 선택은 ComponentsScreen 전체 setState를 호출했다. 상세 기준선에서
  첫 새 scene commit 508.1ms, build 446.8ms, rebuild 2,538 / forced 2,348,
  delegate rebuild 3회였다. layout은 5ms로, 해당 동작의 주원인은 layout보다 넓은 build 범위였다.
- 각 예제에 표준 StatefulBuilder를 두어 해당 예제의 redraw만 요청한다.
  단일/다중 segmented 선택도 서로 독립된 redraw를 사용한다. 관련 값과 controller 수명은
  화면 State에서 유지하고, async 완료는 예제 context가 mounted일 때만 갱신한다.
- 유한한 예제 목록의 Widget과 column별 scroll view/delegate를 재사용한다.
  stable GlobalKey로 한 열/두 열 사이 이동 시 State를 보존한다. 실제 render constraints와
  layout은 계속 적용되며, dirty flag를 지우거나 resize event를 지연하지 않는다.
- SampleHome은 같은 theme 값과 breakpoint/설정/선택 상태에서 navigation Widget을 재사용한다.
  theme의 값 동등성을 사용한다. localization cache가 반환한 새 객체의 참조 차이만으로
  전체 navigation을 다시 만들지 않는다. AnimatedBuilder의 breakpoint 애니메이션은 유지한다.
- 공용 Web host에서는 contentUnchanged semantics 노드에 대해 기존 wire 계약과 같은
  id/children/rect/표시 필드만 직렬화한다. null인 전체 content DTO를 매번 생성/방문하지 않는다.
  기존 cache 상한, full snapshot, 순서, listener, 변경된 content의 full payload는 유지한다.

## 탐색 결과

동일 source Release, Chromium hardware, 1280×900 DPR 1, 상세 계측 ON의 독립 실행이다.
각 중간 후보는 원인 확인용 한 번씩 실행했으며 장기 percentile 수용으로 해석하지 않는다.

| 후보 | Week 입력→새 scene | 최대 callback |
| --- | ---: | ---: |
| 기존 전체 화면 갱신 | 508.1ms | 541.3ms |
| 예제 단위 갱신 | 129.2ms | 118.8ms |
| single/multiple 선택 독립 | 80.5ms | 64.2ms |

예제 단위 후보의 rebuild는 287회, delegate rebuild는 0회였다.

| 같은 열 구성 resize, 8단계 | 최대 callback | active 새 epoch front | 경계 포함 최대 공백 | 추종 p95 | 마지막 exact |
| --- | ---: | ---: | ---: | ---: | ---: |
| 이전 | 660.0ms | 0 | 500.3ms | 1485.4ms | 1361.7ms |
| 최종 source 탐색 | 131.8ms | 6 | 109.3ms | 136.6ms | 69.5ms |

최종 탐색에서 ComponentsScreen의 추가 build는 0회다. 위 수치는 CDP viewport 단계 및
scene commit 알림이며 물리 창 테두리 조작/FPS/scan-out을 측정한 것이 아니다.

## 실패 보존

- 최초 Week 측정은 role=button으로 탐색해 실패했다. 실제 role=radio에 맞춰 수정했다.
- 다음 측정은 첫 scene보다 먼저 캡처해서 onset=null이었다. 이를 PASS나 0ms로 세지 않고
  input sequence에 대응하는 exact-rendered commit을 기다리도록 수정했다.
- 첫 컴파일의 잘못된 호출 텍스트 치환과 GlobalKey 형식 인수 누락을 수정했다. 원본 로그는 유지한다.
- 최종 source suite는 6 PASS / strict resize 2 FAIL이다. 두 FAIL은 max gap
  158.4/164.2ms로 <100ms 기준 미달이다. assertion과 기준을 낮추지 않았다.
- 별도 상태/테마/semantics native DOM identity 회귀는 1 PASS다.

## 재현 및 산출물

원본 JSON: `Doroti/validation/web-playwright/artifacts/state-resize/`.
실행 로그: `Doroti/artifacts/framework-web-work2/state-*.log`, `resize-wide-*.log`.
기존 work2 원본과 이전 publish 결과는 그대로 유지한다.

`Doroti/validation/web-playwright`에서:

```powershell
node measure-state-resize.mjs sample-button button
node measure-state-resize.mjs sample-wide resize-wide
node measure-state-resize.mjs sample-cross resize-cross
node summarize-state-resize.mjs
```

각 benchmark/test는 20분 timeout, 순차 실행, retry 0이다.
최종 반복 수치와 추가 검증은 아래에 기록한다.


## 최종 source 반복

| 지표 | run 1 | run 2 | run 3 |
| --- | ---: | ---: | ---: |
| Week onset, 상세 OFF + 최소 input marker | 78.9ms | 80.9ms | 78.3ms |
| 같은 열 resize, 상세 ON callback max | 129.3ms | 129.4ms | 124.9ms |
| active 새 epoch front 수 | 7 | 6 | 7 |
| 경계 포함 최대 공백 | 102.6ms | 103.1ms | 112.8ms |
| target 추종 p95 | 122.7ms | 126.8ms | 131.5ms |
| 마지막 observer→latest exact | 51.2ms | 69.6ms | 59.7ms |

OFF 조건의 callback 비용은 notMeasured다. 앞선 detailed 기준선과 OFF 결과를 같은
percentile corpus로 합치지 않는다. source와 trimmed publish도 별도 산출물이다.
마지막 exact는 3/3 단기 통과하지만 onset ≤50ms, active gap <100ms, tracking p95 ≤50ms는
미달이다. 이번 변경은 개선/PARTIAL이며 완전한 실시간 수용 PASS가 아니다.


## 통합 검증

- Web Release build 및 trimmed publish PASS, TypeScript PASS.
- source 전체 선택/입력/picker/progress/resize 전환 + DPR1/2 grow 완료 검사: 6 PASS,
  strict resize 2 FAIL. 별도 geometry DOM identity/선택/테마 회귀: 1 PASS.
- trimmed publish에서 progress/resize 전환/선택/테마 회귀: 3 PASS.
- native Windows sample: picker, text raster, app bar, drawer, image 회귀 PASS.
- 최종 source build는 WasmBuildNative=true로 native relink까지 마치고 5088에 반영했다.
  임시 5091 publish 서버는 종료했다.
- 별도 package 재배포, 물리 창 테두리/120Hz/IME/접근성 장치와 이번 변경에 대한
  사용자 체감 재확인은 notVerified다. 기존 package 검증을 이번 변경의 결과로 재분류하지 않는다.

최종 source 및 publish fingerprint는 `Doroti/artifacts/framework-web-work2/state-resize-fingerprint.json`.


최종 5088 source 재시작 뒤 상태/테마 회귀도 1 PASS다.
급격한 breakpoint/용량 변경을 포함한 strict resize의 최종 exact는 DPR1 158.9ms,
DPR2 188.4ms였다. 같은 열 구성의 51~70ms 결과를 모든 resize 시나리오에 일반화하지 않는다.

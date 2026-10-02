# iOS 프레임 생성·GPU 제출·표시 연결 작업 요약 — work4.md

원문 작성·실행일: **2026-10-02** · 보관일: **2026-10-03**
대상: SampleApp2 / iOS Graphite-Metal / iPhone 12 / 60Hz

**iOS 구현·CPU/Metal 회귀·39회 유효 비교·native 생성/회전·lifecycle 수정과 Fast smoke를 완료하고, 사용자 확인·지시로 iOS C 및 공개 API/Sample2 Fast 기본값을 채택했다. 전체 재반복·정량 입력 지연·10분 사용은 미검증**이라는 원문의 결론을 보존한다. 보관 자체는 새 제품 검증이 아니다. 이후 전체 native 공통화와 A/B 제거는 [work5 요약](work5-summary.md)·[work6 요약](work6-summary.md)에 별도로 기록한다.

## 1. 문제와 구현 계약

기존 iOS Draw는 새 shader scene을 조회하고 GPU admission을 검사한 뒤, Paint/BeginPaint에서 framework callback을 실행했다. GPU pending으로 조기에 반환하면 다음 장면 자체를 준비하지 못했다. 이전 pipeline smoke의 새 프레임 350개 중 overlap은 0이어서 실제 pipeline 효과를 검증한 결과가 아니었다.

framework callback을 drawable/acquire·GPU gate 앞의 owner pulse로 분리하고 immutable viewport/DPR/context generation을 게시했다. callback 재요청은 다음 pulse로 남기며 최신 pending scene 하나를 유지한다. fresh shader-only는 최대 **2개 GPU 프레임·비동기 표시**, native/shield·replay·resize/rotation은 drain 후 직렬 및 필요한 transaction 표시를 사용한다.

transaction 여부는 coordinator 설정의 존재가 아닌 실제 scene/native 상태로 판단한다. 실제 GPU 완료·owner 처리·drawable 표시를 구분하고 마지막 소비자 완료까지 scene/recording/Surface/snapshot/drawable/native lease를 보존한다. terminal은 한 번만 처리하며 역순 완료가 최신 replay source를 덮지 않게 한다.

## 2. 당시 A/B/C 비교와 검사

| 정책 | GPU 상한 | shader-only 표시 | 당시 목적 |
| --- | ---: | --- | --- |
| A | 1 | 기존 transaction | 직렬 기준 |
| B | 1 | 비동기 | 표시 대기 제거 효과 분리 |
| C | 2 | 비동기 | 준비 분리와 실제 overlap 확인 |

같은 payload/runtime/source·장면/DPR/sigma에서 A/C Off·Fixed·Adaptive × σ20/32 × 3회, 36회와 B 3회를 교차 비교했다. 모드별 60Hz 기준은 ≥58 FPS, p95 ≤17.5ms, p99 ≤34.2ms, >25ms ≤2%였다. 반복 성능 개선과 input→scene→표시 지연은 별도 판정한다. Safari 비교는 사용자 지시로 제외했다.

callback에서 처음 scene 생성, native 전환·상한 2·재요청·generation/replay·역순/실패/종료 회귀, 공통 CPU·macOS Metal 픽셀, 같은 recorder/cache의 두 async recording을 통과했다. 두 해상도·각 두 프레임 픽셀 오차는 **0/255**, Python collector 검사는 **8개** PASS였다.

## 3. 반복 측정과 실패 이력

[iOS 실행 결과](../26-10-03/works/results/2026-10-02-ios-frame-loop.md)와 [정책별 JSON](../26-10-03/works/results/2026-10-02-ios-frame-loop-summary.json)에 payload/source identity·CPU 단계·실패·재시도를 보존했다.

| 수정된 동일 NativeAOT 앱 | C 평균 표시 FPS 3회 | 실제 overlap 3회 |
| --- | --- | --- |
| Adaptive σ20 | 59.756 / 59.762 / 59.623 | 153 / 159 / 247 |
| Adaptive σ32 | 59.688 / 59.588 / 59.624 | 182 / 162 / 226 |

C의 GPU pending 최대는 **2**, Adaptive 평균 개선율은 같은 앱 A 대비 σ20/32 **59.1% / 60.4%**다. C 핵심 18회는 표시 기준을 통과했다. A Fixed σ32 첫 실행 **56.785 FPS** 미달과 C3 첫 시작 timeout·원인 미확정 및 같은 payload 재실행 **59.660 FPS**를 별도로 남겼다.

초기 표시 callback 기반 admission credit 후보는 실제 화면 갱신 중단으로 제거했다. drawable 2개 후보는 **29.63 FPS·overlap 0·지연 개선 없음**으로 제거했다. 최종 반복 후보는 display link·latest scene 하나·GPU 두 프레임·idle pause를 사용했다. callback→표시는 A 약 **32.7ms**, C 약 **49.9ms**여서 당시 기본 채택을 보류했다. 이를 정량 물리 입력 지연으로 해석하지 않는다.

이전 Mono 기준 **33.759 FPS / p95 33.442ms / 오류 0/0**는 이전 payload의 결과로 보존했다. 수정 전 clean NativeAOT 계측 기준은 미확보이며 Mono/NativeAOT를 같은 바이너리 개선율로 합치지 않았다. Full/Fast/Kawase 추가 36회는 실시하지 않아 모든 모드 60Hz를 주장하지 않았다.

실제 iPhone A/C editor/WebView 생성·재생성 각 4회, C native editor 회전, 일반 Sample2 첫 진입·동일 PID 복귀를 자동 확인했다. native pending 최대 **1**, 오류 **0/0**, 종료 pending 0·terminal 전량 완료였다. 물리 IME/native 버튼·손가락 스크롤·수동 회전 UX·10분 사용과 구분한다.

## 4. Sample2 멈춤 보고와 lifecycle 수정

사용자 멈춤 보고 후 초기 복귀 64프레임의 정상 preflight 판정을 철회했다. pause 때 SetNeedsDisplay 모드를 복구하고 resume/reattachment의 실제 pulse를 재시작하며, active scene에서만 continuous pulse를 시작하고 suspend를 animator 종료보다 먼저 게시했다.

수정본 동일 PID의 복귀 3회에서 완료 수는 **596→917 / 1074→1395 / 1556→1882**, 오류 **0/0**이었다. evidence는 임시 파일 완성 후 교체하도록 수정하고 live 복사 실패도 보존했다. 새 Adaptive σ20 A/B/C 1회는 **39.91 / 37.21 / 59.73 FPS**, C p95 약 16.72ms·overlap 178/1791·pending 최대 2였다. 새 payload의 smoke를 이전 39회나 전체 σ32/native 재검증으로 합치지 않았다.

자동 스크롤 검사는 실행당 40초 뒤 멈추고 종료되므로 benchmark 없는 일반 앱과 구분했다. 사용자 재문의 뒤 일반 Sample2 수정본으로 다시 실행했다.

## 5. 사용자 지시에 따른 Fast 및 iOS C 기본 채택

Fixed의 세부 품질·Kawase 형태에 대한 사용자 판단과 공개 API 적용 범위 확인으로 다음 기본값을 적용했다.

- `ImageFilter.variableBlur`, `ImageFilterConfig.CreateVariableBlur`: **resolutionScale=0.25 / adaptiveResolution=true / kernel=fastGaussian**.
- Sample2 초기 및 Off 복귀 선택은 `fast`. Full Gaussian은 `resolutionScale=1, kernel=gaussian`로 명시한다. Fixed/Kawase 선택은 유지한다.
- 공통 CPU·API/config snapshot·명시 Full Gaussian·Metal 픽셀·collector 검사 PASS. 실제 iPhone C Fast σ20/32 각 1회는 **59.829 / 59.827 FPS**, p95/p99 약 16.72ms, >25ms 0%, pending 최대 2, 오류 0/0. [Fast 기본값 집계](../26-10-03/works/results/2026-10-02-fast-adaptive-default-summary.json)

이어 사용자 아이폰 정상 동작 확인과 기본 적용 지시로 **iOS Graphite/Metal 미설정 정책을 C**로 바꿨다. 새 NativeAOT 앱의 무설정 C, 진단만 켠 PID 6912 복귀 3회 **584→904 / 1060→1383 / 1543→1867**, pending 최대 2·오류 0/0을 확인했다. 진단/benchmark를 제거한 일반 앱 PID 6920과 강제 종료 후 재실행도 C였다. [C 기본 적용 집계](../26-10-03/works/results/2026-10-02-ios-default-C-summary.json)

당시 A/B 복귀 옵션과 8가지 선택 검사는 이력으로 보존한다. **이후 work6에서 A/B 옵션을 제거했으므로 현재 실행 안내로 사용하지 않는다.** 사용자 채택은 미수행 정량 입력·최종 payload 전체 반복·10분 Off 대조의 통과를 뜻하지 않는다. [work3의 Fixed 후보 결과](../26-10-01/work3-summary.md)도 별도 역사로 유지한다.

## 6. 남은 인수와 증거 보존

최종 payload의 전체 반복, Full/Fast/Kawase의 조건별 반복, 실제 손가락/IME/슬라이더·품질 전환, 정량 input→display, 같은 조건 Off 대비 10분 사용은 남았다. 사용자 요청에 따른 양호한 기기 상태 가정은 정량 밝기·열 통제로 확대하지 않는다. GPU/표시/CPU 시간과 단일 종료 allocation·peak VRAM은 구분한다.

재현 검사는 [ShaderFramePipelineRegression](../../Doroti/tests/Doroti.Tests/ShaderFramePipelineRegression.cs), [GPU 회귀](../../Doroti/tests/Doroti.Tests/VariableBlurGpuRegression.cs), [기기 수집기](../../Doroti/tests/variable_blur_device.py)를 따른다. raw는 `temp/testing/variable-blur/ios-frame-loop-*`에 기록했으나 현재 존재를 보장하지 않는다. 추적되는 실행 결과와 JSON을 보관 근거로 사용하며, 이번 작업은 문서 요약·링크 정리다.

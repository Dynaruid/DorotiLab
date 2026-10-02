# Variable Blur Fixed 1/4 우선 채택 작업 요약 — work3.md

원문 수정일: **2026-10-01** · 보관일: **2026-10-03**
대상: SampleApp2 / 공통 Skia renderer / iPhone 12 Graphite-Metal / DPR 3 / 60Hz

**P0 완료, P1 18회 자동 성능·경로 기준 통과, P2 Fixed 기본값 후보 빌드·설치 완료. 환경 통제·수동 조작/화질·P3 10분 사용은 미확인**이라는 원문의 상태를 보존한다. 이 보관은 새 제품 검증이 아니다. 이후 공개 API·Sample2 Fast 기본값 및 iOS C 채택은 [work4 요약](../26-10-02/work4-summary.md)의 별도 결정이며, 기존 Fixed 결과의 미완료 항목을 대신 통과시키지 않는다.

## 1. 목표와 계획 전환 근거

사용자는 Fixed 1/4가 가장 부드럽고 육안의 큰 이질감이 없다고 평가했다. Adaptive/Kawase의 60 FPS 추가 최적화보다 **현재 Fixed 검증 → 같은 바이너리 비교 → 샘플 기본값 후보 → 장기 사용**을 우선했다. 약한 블러의 원본 세부가 필요한 사용처는 Adaptive를 선택하도록 했다.

Gaussian backing 축소 이전의 같은 바이너리·모드별 3회 결과:

| 모드 | 평균 표시 FPS 범위 | 표시 p95 |
| --- | ---: | ---: |
| Off | 59.41–59.58 | 16.72ms |
| Fixed 1/4 | 58.81–58.91 | 16.72ms |
| Adaptive | 31.51–32.14 | 33.44ms |
| Fast adaptive | 31.64–32.61 | 33.44ms |
| Dual Kawase | 32.24–32.98 | 33.44ms |
| Full quality | 29.67–29.71 | 50.16ms |

후속 backing 축소 후보의 Adaptive/Fast/Kawase 측정에는 Fixed가 없으므로 최종 같은 바이너리 순위로 합치지 않았다. Fixed는 위치별 sigma를 유지하면서 가로/세로 약 1/4, 작업 픽셀 약 1/16을 사용한다. 빈 child BackdropFilter는 이미 축소 결과를 최종 타깃에 직접 linear sampling한다. 낮은 sigma의 글자·가는 선·사진 softness와 sigma 0 복귀 차이는 별도 화질 범위다.

## 2. 채택 기준과 단계

우선 범위는 180 논리 단위 overlay·Clamp·Release Mono, sigma 20/32다. 공개 API의 당시 Gaussian/원본 해상도 기본값은 유지하고 **샘플만 Fixed 후보**로 변경했다.

| 기준 | 요구값 |
| --- | --- |
| 평균 표시 FPS | sigma 20/32 각각 3회 모두 ≥58, Fixed 평균 ≥Off 평균의 97% |
| 표시 interval | p95 ≤17.5ms, p99 ≤34.2ms, >25ms 비율 ≤2% |
| 실행·경로 | 오류 0, 유효한 30초 표시 이력, 실제 단일 1/4 Gaussian 두 패스 |
| 수동 품질·사용 | 낮은 sigma/끝 경계/연속 전환/손가락 스크롤과 10분 사용 확인 |

- **P0:** 새 Release/Mono identity, Fixed 실제 GPU crop/full·직접 합성/복원 경로, DPR 1/2/3·반전·fractional ROI·alpha·Clamp/Decal, 기존 CPU/Metal 회귀.
- **P1:** 같은 payload의 Off/Fixed/Adaptive × sigma 20/32 × 3회, 총 18회. 실행 40초 중 첫 표시 이후 5~35초 집계. 정지 캡처/녹화는 성능 수집과 분리.
- **P2:** Sample2의 초기/Off 복귀를 `fixed`, `resolutionScale=0.25`, `adaptiveResolution=false`, `kernel=gaussian`로 맞추고 모드 선택을 유지. 일반 앱 조작·재진입은 수동 잔여.
- **P3:** 최종 후보의 같은 조건 Fixed/Off 10분 사용·오류·자원 회수·품질 정리. 당시 미완료.

## 3. 실제 실행 결과

[Fixed 실행 기록](../26-10-03/works/results/2026-10-01-variable-blur-fixed-adoption.md)과 [집계 JSON](../26-10-03/works/results/variable-blur-2026-10-01-fixed-adoption-summary.json)에 원본 identity와 판정을 보존했다.

- Fixed 실제 GPU/직접 합성 **72조건, 최대 1/255**로 PASS. kernel, raster capture **48조건·최대 2/255**, 기존 Metal GPU·공통 CPU 회귀도 PASS.
- P1 동일 payload의 18회에서 Fixed 평균은 sigma 20 **59.09 FPS**, sigma 32 **58.86 FPS**. Off 대비 **99.57% / 99.18%**이며 Fixed 6회 모두 수치·경로 기준 PASS.
- 별도 iPhone Full/Fixed 정지 캡처 **14장**, macOS Metal 품질 장면 **14장**을 확보했다. 낮은 sigma의 softness/aliasing·sigma 0 원본 복귀를 기록했으나 움직임·연속 전환·사용자 품질 허용 확인으로 확대하지 않았다.
- 최종 Mono 후보는 경고/오류 **0/0**. Fixed σ20 1회 **58.97 FPS / p95 16.72ms / p99 33.44ms / 긴 간격 1.41% / 오류 0**. renderer/shader/host/API hash가 P1과 같았으며 benchmark 없는 Components 시작을 확인했다.

밝기·측정 기간의 충전/열·냉각 조건을 통제하지 못했다. 사용자는 이후 기기가 차갑고 배터리 100%라고 보고했으나 이를 앞선 측정 전 기간의 조건 확인으로 소급하지 않는다. Variable Blur 첫 진입·실제 조작·재진입과 10분 사용은 미완료다.

## 4. NativeAOT 후속 설치

.NET 11 RC1 SDK/workload/bgen을 확인한 뒤 같은 Fixed 후보를 Release/NativeAOT로 게시·서명 검증·iPhone 12 설치했다. `PublishAot=true`, `UseNativeAot=true`, `UseMonoRuntime=false`, 번들 관리 DLL 0개를 확인했다. 최초 iOS 보안 차단은 사용자 개발자 신뢰 처리 후 해소돼 Components 화면과 프로세스 유지를 확인했다.

[NativeAOT 설치 기록](../26-10-03/works/results/2026-10-01-sample2-nativeaot-rc1.md)에 RC1 ILC 진단·게시 옵션을 남겼다. **NativeAOT 성능·전체 수동 조작은 미검증**이며 Mono 반복 수치와 합치지 않는다.

## 5. 실패 시 후속과 검증 경계

Fixed 성능 회귀는 identity·프레임 설정·작업 크기/패스/Surface부터 조사한다. 화질 미달은 해당 사용처의 Adaptive 선택을 우선하며, 고해상도/원본 혼합이나 외부 Metal/MPS는 확인된 병목·품질 근거가 있을 때만 비교한다. 새 Surface 풀·정지 캐시·갱신 빈도 감소를 채택의 해결책으로 추가하지 않았다.

표시 interval은 GPU 단독 시간이 아니며 단일 종료 Metal 할당량은 blur peak/live VRAM·누수 판정이 아니다. 다른 플랫폼·120Hz·전체 affine/tile/child/opacity 행렬은 별도 범위다. 이후 iOS frame 정책과 Fast 기본값 변경은 새 payload의 결과로 구분한다.

주요 회귀: [GPU 검사](../../Doroti/tests/Doroti.Tests/VariableBlurGpuRegression.cs), [kernel/capture 실행 안내](../../Doroti/tests/README.md), [기기 수집기](../../Doroti/tests/variable_blur_device.py). 이전 재평가·부분 캡처 기록은 [재평가 결과](../26-10-03/works/results/2026-10-01-variable-blur-reassessment.md)와 [축별 부분 캡처](../26-10-03/works/results/2026-10-01-variable-blur-per-axis.md)에 있다. 과거 `--serial-frames` 명령은 당시 정책 이력이며 현재 C 단일화 이후의 실행 안내로 쓰지 않는다.

원문의 raw 경로는 `temp/testing/variable-blur/`·`Doroti/artifacts/`다. 삭제 가능한 산출물의 현재 존재를 보장하지 않는다. 보관 및 링크 검사는 제품 재실행을 뜻하지 않는다.

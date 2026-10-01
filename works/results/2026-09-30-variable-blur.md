# Variable Blur 작업 결과

계획: [work3.md](../../work3.md). 실행일 2026-10-01, 시작 HEAD `8c7127e3`.
이번 변경은 P0 진단·재현 경로를 구현하고 iPhone 12 예비 비교 15회를 실행했다. Gaussian/Adaptive 알고리즘·샘플 기본값·캡처 정책은 유지한다.
P1~P4 채택 결과와 15% GPU 개선 실적은 아직 없다.
아래 P0 성능 수치는 고정 7회 Fast 커널을 사용한 수정 전 값이다. 이후 사용자 보고에 따른
[Fast adaptive 겹침 수정](#fast-adaptive-겹침-수정)은 화질 수정이며 새 성능 수치를 의미하지 않는다.

## 구현

- `DOROTI_VARIABLE_BLUR_PROFILE=1`: CPU stage별 최근 4,096회 호출 p50/p95/p99와 누적 호출·시간·최대값.
  `ResetVariableBlurProfile()`로 별도 측정 구간을 시작할 수 있다. 기본 비활성이다.
- `CaptureVariableBlurDiagnostics()`: 마지막 기록 프레임의 최대 128개 작업과 누락 개수,
  입력/작업 해상도, draw bounds, 캡처 원점·fallback 사유, 사유별 누적 횟수·처리 면적.
- 기존 Surface lease가 pool hit/miss/temporary를 직접 전달한다. 글로벌 counter 차이로 추정하지 않는다.
  풀 슬롯 선택·회수·clear·snapshot 정책은 바꾸지 않았다.
- MAUI evidence에 위 진단을 연결하고 UIKit Graphite의 실제 drawable 표시 간격을 최대 4,096개 보관한다.
  Metal device·할당량과 terminal command buffer 카운터도 전달한다. 계측 활성화 시만 수집한다.
- SampleApp2에서 `DOROTI_VARIABLE_BLUR_BENCHMARK=off|full|adaptive|fast|fixed`를 설정하면
  해당 탭·모드에서 같은 10초 왕복 경로를 40초간 합성 스크롤한다. 초기 5초와 마지막 여유 5초를 제외한
  최초 표시 기준 5~35초 구간을 Python 수집기가 분석한다. 일반 실행에서는 기존 동작을 유지한다.
- `Doroti/tests/variable_blur_device.py`: 기존 서명 앱 설치(선택), 모드별 독립 기동,
  기본 3회 교차 순서 실행, 완료 표식과 표시 구간 검증, 원본 JSON·로그·요약 보관.
  실기기 입력 자동화가 아니라 동일 렌더링 경로 재현용이다.

## 해석 제한

CPU stage와 `filter-total`은 중첩된다. 합산하지 않는다. CPU stage 분포는 초기 준비를 포함하며
GPU stage/프레임 분포가 아니다. CPU raster 요약은 기존 `rasterEnd`에 직접 기록한 경과 시간을 사용하며,
8192개 trace 중 남아 있는 마지막 구간만 대표한다. 인과 순서용 timestamp를 빼서 build/layout/paint 시간을 만들지 않는다. 표시 간격의 1.5주기 초과 비율은 긴 표시 간격의 비율로,
컴포지터 deadline miss의 정확한 계수와 구분한다.

작업별 `EstimatedRgbaBytes`는 출력 크기 × 4 추정이며 실제 backing 할당·복사량·live/peak VRAM이 아니다.
`scene-surface-clear`는 다른 scene filter도 포함한다. UIKit의 command buffer 수는 terminal marker만
집계하며 Graphite 내부 제출 수는 모른다. `MetalAllocatedBytes`는 끝 시점의 device 전체 할당량이다.
정상 프레임에 readback이나 GPU 완료 대기를 추가하지 않았다.

## 확인한 환경과 검증

- 연결된 물리 기기: iPhone 12 (`iPhone13,2`), iOS 26.6.1, 1170×2532, DPR 3.
  실제 실행 backend와 표시 이력은 실행 JSON으로 별도 확인한다.
- 빌드 도구: .NET SDK 10.0.401, iOS workload 27.0.10722, Xcode 27.0 (27A266a).
- 공통 `Doroti.Tests` Release 실행 PASS: 기존 CPU 회귀와 캡처 fallback 사유 검증.
- Python 집계 테스트 5개 PASS: 초기 stall 제외, 긴 표시 간격, 부족/유실 이력 거부, CPU raster 구간 시간의 구분, 겹치는 GPU channel 구간의 중복 합산 방지.
- UIKit 호스트 Release 컴파일 PASS, 경고/오류 0.
- 최신 계측 포함 iPhone Release/Mono 서명 빌드 PASS, 경고/오류 0 (`build-final.log`, 5분 23초). 실기기 실행 결과는 아래에 기록한다.

원본 빌드/CPU 로그: `temp/testing/variable-blur/2026-10-01/`.
이 경로는 로컬 검증 산출물이며 저장소에 포함되지 않는다.

## 재현

현재 명령은 [SampleApp2 README](../../samples/DorotiSampleApp2/README.md)에 있다.
실기기 측정 시 `--conditions`에 전원·밝기·열 상태·외부 profiler 사용 여부를 기록한다.
기기 모델 최대 주사율 대신 실제 설정값을 `--hz`로 전달한다.

```sh
python3 Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Release
python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
```

과거 `Doroti/validation/cupertino-sample`은 현재 존재하지 않는다. README의 실행 불가능한 명령을
현재 CPU/실기기 진입점으로 교체했다. 새로운 GPU 픽셀 비교 하네스는 아직 확보하지 못했다.

## 단계별 상태

| 단계 | 구현 | 실행/측정 | 다음 조건 |
| --- | --- | --- | --- |
| P0 | 진단·표시 간격·반복 스크롤 구현 | CPU/집계/Release 기기 빌드 PASS, 실기기 예비 비교 15회 및 짧은 Metal trace | 블러 GPU stage 분리, 계측 부하, 열·전원 통제, 픽셀 기준 필요 |
| P1 | 미착수 | 미검증 | 전역 격자를 보존하는 좌표 설계와 전체 캡처 GPU 비교 |
| P2 | 미착수 | 미검증 | P1 이후 독립 A/B 및 alpha/clip 회귀 |
| P3 | 미착수 | 미검증 | 앞 단계 측정 이후 sigma 보정·움직임·전체 패스 비용 비교 |
| P4 | 진입하지 않음 | 미검증 | GPU/제출 병목 증거와 공통 경로 대비 이득 근거 |

120Hz, 다른 GPU backend, 장시간 열/수명, opacity/child/transform/Decal 조합, GPU 픽셀 오차 예산은
미검증이다. 전체 작업 완료로 표시하지 않는다.

## iPhone 12 예비 비교 — 5개 모드 × 3회

원본: `temp/testing/variable-blur/2026-10-01/comparison/`.
[반복별 수치](variable-blur-2026-10-01-summary.json)를 함께 보관한다.
Release/Mono, UIKit/MTKView/Graphite-Metal, Apple A14 GPU, 1170×2532, DPR 3에서 실행했다.
각 실행 40초 중 최초 drawable 표시 이후 5~35초 구간을 사용했다.
60Hz 명목 기준이며 실제 기본 간격은 약 16.72ms다. 두 번째 반복은 역순이다.
화면 녹화·GPU profiler 없이 진단을 켜고 측정했다.
**전원·밝기·열 상태는 통제하지 않았고 진단 부하 A/B가 없으므로 채택용 baseline으로 확정하지 않는다.**

| 모드 | 표시 p50 범위 (ms) | p95 범위 (ms) | p99 범위 (ms) | 25ms 초과 간격 비율 | 종료 Metal 할당 (MiB) |
| --- | --- | --- | --- | --- | --- |
| Off | 16.72–16.72 | 16.72–16.72 | 16.72–16.72 | 0.22–0.85% | 52.06–52.23 |
| Full | 50.14–50.15 | 50.15–50.16 | 50.16–50.16 | 95.92–96.50% | 216.56–216.58 |
| Adaptive | 33.44–33.44 | 50.15–50.16 | 50.16–50.16 | 87.13–87.41% | 298.31–298.31 |
| Fast | 33.43–33.43 | 50.15–50.15 | 50.16–50.16 | 85.76–86.50% | 298.30–298.30 |
| Fixed | 33.43–33.44 | 50.15–50.15 | 50.16–50.16 | 84.63–85.57% | 187.98–188.09 |

15회 모두 렌더러 failed frame 및 Metal terminal buffer error 0.
GPU 실행 시간이나 전체 프레임 CPU 비용을 위 표시 간격과 동일시하지 않는다.

캡처와 작업량 관찰:

- Full은 1170×904, 원점 (0,799)의 부분 캡처를 사용했다.
- Adaptive/Fast/Fixed는 모든 기록된 호출에서 `rounded-working-grid` 사유로 1170×2532 전체 캡처를 사용했다.
- 마지막 프레임의 Variable Blur Gaussian 패스는 Full/Fixed 2회, Adaptive/Fast 6회였다.
- 마지막 프레임의 scene surface는 Full 6개, Adaptive/Fast 13개, Fixed 7개이며 모두 pool hit였다. 다른 scene filter를 포함한 개수다.
- 블러 호출 CPU p95와 실제 표시 간격에는 큰 차이가 있다. 별도 GPU/제출 분석 없이 그 차이를 모두 GPU 커널 시간으로 돌릴 수 없다.

다음 P1 후보는 비배수인 축의 원래 도메인을 유지하면서 정렬 가능한 다른 축만 줄이는 제한적 crop이다.
이 기기의 가로 1170은 유지하고 4의 배수인 세로 2532에서 ROI+halo를 자르는 방식부터 조사할 수 있다.
전체 비배수 크기를 무조건 정수 1/4 scale로 바꾸면 현재 가로 작업 비율 `293/1170`을 잃으므로 제외한다.
아직 구현·GPU 픽셀 비교를 하지 않았으며, 화면 면적 감소만으로 속도 향상을 주장하지 않는다.

## 별도 Metal 추적

전체 프로세스를 대상으로 한 20초 trace는 종료 후 저장 단계에서 150초 timeout이 발생했다.
해당 `fast-metal.trace`는 export 시 `Document Missing Template Error`였으므로 분석 근거에서 제외했다.
앱 PID만 대상으로 한 5초 재시도는 저장 및 XML export에 성공했다.
원본은 `temp/testing/variable-blur/2026-10-01/fast-metal-short.trace`와 같은 디렉터리의
`metal-gpu-intervals.xml`, `metal-submissions.xml`, `metal-short-toc.xml`이다.
[집계 결과](variable-blur-2026-10-01-metal.json)를 보관한다.

Fast adaptive, 같은 Release 앱에서 `DOROTI_MAUI_WALL_TRACE=1`도 켠 별도 진단 실행이다.
trace 경계의 command buffer를 제외하고 앱 PID에 연결되는 내부 166개 buffer를 분석했다.
각 buffer의 Compute/Vertex/Fragment 등 **겹치는 GPU 활동 시간은 합집합**으로 계산했다.

| 관찰 단위 | p50 | p95 | p99 |
| --- | --- | --- | --- |
| nonempty command buffer의 앱 GPU 활동 구간 합집합 | 19.09ms | 24.74ms | 25.64ms |
| 같은 buffer의 최초 GPU 시작~최후 종료 범위 | 22.06ms | 27.24ms | 28.59ms |

Metal submissions에는 encoder 70개의 buffer 168개와 encoder 없는 buffer 168개가 있었다.
70은 Variable Blur의 Gaussian 패스 수가 아니라 다른 화면 작업을 포함한 Metal encoder 수다.
위 시간 역시 **앱 전체 작업이며 블러 단독 시간·프레임 GPU p95가 아니다**.
단 5초의 profiler 관찰이므로 30초 warm 구간/3회 반복 조건을 만족한 GPU baseline으로 쓰지 않는다.
profiler 부하 A/B와 개별 capture/downsample/blur/composite stage 연결이 남았다.

그럼에도 이 실행에서는 앱의 GPU 활동이 60Hz의 16.67ms 예산을 넘는 구간을 확인했다.
CPU 블러 기록만 줄이는 접근으로 설명하기 어렵고, 전체 캡처·Surface·다른 scene filter 작업을 포함한
GPU/대역폭 비용을 다음 조사 대상으로 삼을 근거다. 각 후보의 원인별 기여도는 아직 확정하지 않는다.

현재 도구에서 확인한 재현 형태 (`<PID>`는 해당 실행의 sample PID):

```sh
xcrun xctrace record --template 'Metal System Trace' --device <DEVICE_UDID> --attach <PID> --time-limit 5s --no-prompt --output temp/testing/variable-blur/new-short.trace
xcrun xctrace export --input temp/testing/variable-blur/new-short.trace --xpath '/trace-toc/run[@number="1"]/data/table[@schema="metal-gpu-intervals"]' --output temp/testing/variable-blur/gpu.xml
xcrun xctrace export --input temp/testing/variable-blur/new-short.trace --xpath '/trace-toc/run[@number="1"]/data/table[@schema="metal-application-command-buffer-submissions"]' --output temp/testing/variable-blur/submissions.xml
python3 Doroti/tests/variable_blur_metal.py --gpu temp/testing/variable-blur/gpu.xml --submissions temp/testing/variable-blur/submissions.xml --pid <PID> --output temp/testing/variable-blur/metal-summary.json
```

## Fast adaptive 겹침 수정

사용자가 강한 쪽의 블러가 여러 장 겹친 것처럼 보인다고 보고하여 고정 7회 커널을 조사했다.
기존 구현은 sigma 2용 가중치를 고정하고 오프셋만 늘렸다. 큰 sigma에서도 중앙 가중치가 약 0.2로
남고 넓어진 샘플 사이가 비어 텍스트와 테두리를 여러 위치에 반복했다.
iPhone 12의 기존 앱에서도 이 모양을 캡처했다(`temp/testing/variable-blur/ghosting/old-fast-20.png`).

수정은 `variable_blur.sksl`의 Fast 커널에 한정한다. 현재 working sigma로 가중치와 샘플 수를
계산하고, 예산이 허용하는 구간에서 한 working pixel 간격의 인접 가중치 쌍을 bilinear read로 묶는다.
짝이 남는 마지막 탭도 처리하고 전체 가중치를 정규화한다. 기존 3 sigma 반경 안에서 읽으며
pass 순서, Adaptive 레벨·마스크·캡처, sigma 0 및 2 이하의 기존 커널은 유지한다.

한 working pixel보다 멀리 떨어진 탭을 하나로 묶으면 다시 빈 구간이 생기므로 강한 구간의 예산을
늘렸다. 최대 읽기 수는 패스당 `min(4 * maxSamples + 1, 129)`이며 sigma 2~3 전환에는 두 커널이 실행된다.
기본 `maxSamples=32`에서 working sigma 15/24는 각각 47/73회다. 7회 고정 비용보다 증가하므로
이 변경을 성능 개선으로 집계하지 않는다. 극단적으로 큰 반경이나 작은 명시적 예산에서는 여전히 근사다.

실제 embedded SkSL을 Skia raster runtime-effect에서 실행하는 `VariableBlurKernelRegression`을 추가했다.
앱과 같은 `FrameworkShaderLoader`를 거치며 manifest SHA-256과 uniform/sampler ABI도 검증한다.
새 셰이더에 맞춰 `FrameworkShaderAssets.cs`의 manifest hash를 함께 갱신했다.
별도의 이산 Gaussian impulse 기준과 비교하므로 Adaptive 합성과 GPU 스케줄링에서 커널을 분리한다.

| 검사 | 수정 전 | 수정 후 |
| --- | --- | --- |
| working sigma 4 impulse 최대값 / 기준 | 0.24222 / 0.09991, FAIL | 0.09994 / 0.09991 |
| sigma 4 impulse L1 오차 | 1.03527 | 0.00028 |
| working sigma 15 L1 오차 (DPR 3, sample sigma 20, 1/4) | 미수집 (sigma 4에서 중단) | 0.00153 |
| working sigma 24 L1 오차 (DPR 3, sample sigma 32, 1/4) | 미수집 (sigma 4에서 중단) | 0.00508 |
| sigma 0/1/2 | 기존 Gaussian 경로 | 기존 Gaussian과 일치 |
| 전환 sigma 2.01/2.5/2.99/3 | 기존 경로 보존 기준 | 기준 응답 허용 오차 통과 |

강한 커널은 L1 ≤ 0.02, 전환 구간은 ≤ 0.08, 최대값은 독립 기준의 1.5배 이하,
impulse 에너지 오차는 0.03 이하로 검증한다. 전체 `Doroti.Tests` Release 회귀도 통과했다.
로그는 `temp/testing/variable-blur/ghosting/`의 `before-test.log`, `regressions-final.log`, `kernel-final.log`에 있다.
이는 raster 셰이더 검사이며 GPU 픽셀 행렬 전체를 검증한 것으로 확대하지 않는다.

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kernel
```

수정 후 실기기 검증도 완료했다. iPhone 12, iOS 26.6.1, Release/Mono, Graphite-Metal에서
Fast sigma 0/20/32와 Adaptive sigma 20/32의 정지 화면을 확인했다. Fast의 반복 윤곽이 사라지고
강한 구간이 연속적으로 흐려지는 것을 확인했으며, 다섯 실행 모두 렌더러·Metal 오류 0이었다.
`fast-20.png`, `fast-32.png`, `adaptive-20.png`, `adaptive-32.png`, `fast-0.png`와
`device-quality.json`은 `temp/testing/variable-blur/ghosting/`에 보관한다.
기존 앱의 `old-fast-20.png`는 스크롤 종료 직후라 위치가 완전히 같지는 않으며 시각적 증상 비교용이다.
새 Fast/Adaptive 정지 화면끼리는 같은 초기 스크롤 위치를 사용한다.

수정 후 Fast 40초 반복 스크롤도 완료했다(`ghosting/scroll/`). 렌더러·Metal 오류 0,
warm 30초 표시 간격 p95는 50.16ms였다. 1회 smoke이므로 정식 성능 채택 결과로 사용하지 않는다.
최종 iOS 서명 빌드는 경고·오류 0(`ios-build-final.log`)이었다.

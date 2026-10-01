# Variable Blur FPS 개선 — 전체 재평가와 실행

재평가일: **2026-10-01** · 시작 HEAD: `8c7127e3` · 대상: SampleApp2 / 공통 Skia 렌더러 / Graphite-Metal

**중간 마무리:** 렌더러 최적화와 기기 비교를 여기서 정리하고 추가 실험을 멈춘다.
iPhone 12의 모드별 3회 측정은 Adaptive **33.22–34.07 FPS**, Fast **33.69–33.81 FPS**,
Dual Kawase **36.15–36.79 FPS**였다. 60 FPS 목표는 미달이며 샘플 기본값은 Adaptive로 유지한다.
iOS 프레임 연결 실험은 추가 이득이 확인되지 않아 기본 비활성화한다.

이번 목표는 스크롤 중 실제 표시 FPS를 높이면서 선명한 끝부분을 유지하는 것이다.
기존 P0~P4를 일괄 구현하는 계획을 **같은 바이너리에서 후보를 비교하고, 효과가 있는 경로를 선택하는 작업**으로 바꾼다.
현재 구현·검증 결과와 미검증 범위는 [재평가 실행 기록](works/results/2026-10-01-variable-blur-reassessment.md)에 모은다.

## 1. 재평가 결론

캡처 면적 축소만으로 FPS가 개선된다는 가설은 기존 iPhone 기록에서 지지되지 않았다.
Fast의 반복 윤곽 수정 이후에는 강도에 따라 샘플 수가 늘어난다. 이전 고정 7회 커널의 성능 표를 현재 커널에 적용할 수 없다.
따라서 우선순위를 **부분 캡처 → 영역·합성 최적화 → 강한 블러의 연산량을 줄이는 공유 단계 → 조건부 Metal 비교**로 정한다.

| 현재 구성 | 재평가 | 이번 조치 |
| --- | --- | --- |
| Full Gaussian | 기준 품질이지만 강한 구간의 샘플 수가 큼 | 품질 비교 경로 유지 |
| Adaptive / Fast | 1·1/2·1/4 각각 두 Gaussian 패스. 높은 sigma에서 여전히 많은 읽기 필요 | working 이미지로 직접 샘플링하고 최종 합성에서 가중치 합산 |
| Fast 커널 | 겹침 수정 후 sigma 15/24에서 47/73회 읽기. 고정 비용 모드가 아님 | 기존 품질 수정 유지. 샘플 간격을 벌리는 7회 커널로 되돌리지 않음 |
| 부분 캡처 | 축별 crop으로 1170×2532 → 1170×920 감소했지만 이전 비교에서 표시 p95 개선 없음 | 유지하되 단독 FPS 성과로 집계하지 않음 |
| Adaptive 출력 | capture 크기의 중간 출력과 레벨별 복원/합성 비용 | 빈 backdrop child에서 shader 합성 후 타깃에 한 번 그리기 |
| Dual Kawase | 작업 트리에 셰이더·enum만 있고 렌더러 연결 없음 | 공유 down 체인, 강도별 up 재구성, sigma 보정과 보간을 연결 |
| Surface 풀 | 기존 풀과 픽셀 예산이 있음 | 슬롯 상한 48, 기존 32M pixel 예산 유지. 새 풀 추가 없이 단계 자원 재사용 |
| scene 전체 라우팅 | VariableBlur가 하나 있으면 무관한 형제 scope까지 owned Surface 경로를 사용함. 최초 smoke에서 추가 full-frame Surface 4개 확인 | 셰이더 없는 형제는 native Skia 경로, 캡처가 있는 조상만 owned 경로 |
| iOS 프레임 연결 | pending frame 1개 제한 + coordinator 설정만으로 transaction presentation을 사용 | 조건부 2-frame 실험 구현, 1회 smoke에서 추가 이득 미확인. 기본 비활성화 |
| iOS Metal | 이미 Skia Graphite-Metal 실행 | 별도 device/queue나 외부 제출 경로는 추가 이득 근거가 생길 때 검토 |

기존 결과: [P0·Fast 윤곽 수정](works/results/2026-09-30-variable-blur.md),
[축별 부분 캡처](works/results/2026-10-01-variable-blur-per-axis.md).
이전 기록은 당시 코드와 빌드의 결과이며 새 경로의 성능을 대신하지 않는다.

## 2. P0 — 실제 표시 기준으로 비교

- [x] CPU 호출 시간, GPU 실행 시간, 실제 표시 간격을 구분하는 기존 진단 유지.
- [x] 수집기에 평균 표시 FPS와 Kawase down/up 패스 개수 추가.
- [x] Off / Full / Adaptive / Fast / Fixed / Kawase를 같은 바이너리에서 선택 가능하게 연결.
- [x] `--intermediate` / `--full-capture` / `--owned-subtrees` / `--full-stages`로 합성·캡처·형제 scope·단계 영역을 각각 선택하는 A/B 진입점 추가.
- [x] iPhone Release/Mono에서 예열 5초 이후 30초 표시 구간을 3회 교차 비교. 중간 후보 6모드 18회, Gaussian backing 축소 후보 4모드 12회 완료.
- [ ] profiler 부하 A/B와 블러 단독 GPU p95 확보. 표시 간격을 GPU 실행 시간으로 사용하지 않음.

판정은 평균 FPS, 표시 p50/p95/p99, 긴 표시 간격 비율, 오류, 끝 시점 Metal 할당량을 함께 사용한다.
전원·열·밝기를 완전히 통제하지 못한 결과는 제한된 조건의 비교로 표시한다.
종료 시 device 할당량은 블러의 peak/live VRAM이나 누수 검사와 구분한다.

## 3. P1 — 캡처 좌표와 경계

Gaussian의 기존 축별 crop은 비배수 축을 유지하여 `ceil(size × scale) / size` 비율을 보존한다.
Repeat/Mirror 및 미지원 격자는 기존 전체 도메인 경로를 사용한다.

Kawase는 자체 dyadic 격자를 사용한다. crop 원점을 가장 깊은 단계 격자에 정렬하고,
모든 down/up/reconstruction의 지원 범위보다 큰 halo를 확보한다.
비배수 크기는 Clamp 가장자리로 padding하며 **원본을 padding 크기로 늘려 그리지 않는다**.
이렇게 해서 각 축의 다운샘플 비율을 정확히 2:1로 유지한다.

- [x] 기존 raster crop/full 비교 48조건, 최대 채널 차이 2/255 확인.
- [x] 실제 macOS Graphite-Metal에서 production renderer의 crop/full 비교: DPR 1/2/3, 방향 반전, 부분 픽셀 ROI, 비배수 가로, 반투명 입력.
- [ ] rotation/shear·oversized backing·화면 가장자리 전체 행렬과 iPhone GPU 픽셀 비교.

P1의 처리 면적 감소와 P0의 표시 성능 개선은 별도로 판정한다.

## 4. P2 — 같은 Gaussian 의미의 비용 감소

Adaptive 각 레벨의 이미지를 원래 작업 해상도로 보관하고 최종 shader에서 linear sampling한다.
가중치는 기존 gradient mask를 유지하고 투명 바탕에서 `Plus`로 합산한다.
최종 clip/blend/opacity는 기존 합성 위치에서 한 번 적용한다.
기존 backdrop에 레벨들을 차례로 SrcOver하는 방식은 사용하지 않는다.

- [x] 레벨별 full-size 복원 Surface 제거.
- [x] Gaussian 띠와 선명한 끝부분의 backing 자체를 줄이기. 전체 backing 기준과 GPU 픽셀 비교 통과.
- [x] Repeat/Mirror는 기존 전역 저장 경로 유지, Clamp/Decal에서만 적용.
- [x] 빈 backdrop child에서 capture 크기의 Adaptive 출력 Surface 제거.
- [x] 중간 출력이 필요한 일반 ImageFiltered/child 경로 유지.
- [x] macOS 실제 GPU에서 직접 합성/중간 합성 비교, 최대 채널 차이 1/255.
- [x] 셰이더 없는 형제 scope의 native 라우팅. opacity 안의 VariableBlur는 owned 경로 유지. mixed-scene GPU 비교 최대 차이 0/255.
- [x] Kawase 단계는 소비되는 띠 + 최종 linear footprint만 재구성하고, 다음 up 패스가 읽는 1.5 texel halo를 역산. GPU 전체 단계/부분 단계 비교 최대 차이 0/255.
- [ ] opacity·비어 있지 않은 child·Decal·전체 affine 조합 및 장기 수명 검증.

각 레벨의 입력 도메인과 실제 축소 비율은 유지하되 두 Gaussian 패스의 출력 저장 영역을 필요한 띠 + halo로 줄인다.
shader는 원래 working 좌표에서 계산하고 로컬 Surface 원점만 별도로 보정한다.
Kawase의 선명한 끝부분도 같은 경로를 사용한다. 읽지 않는 픽셀의 clear 제거는 이번에 적용하지 않는다.
현재 좌표·투명도 기준을 통과한 합성 변경부터 사용한다.
정지 화면 캐시와 갱신 빈도 축소는 스크롤 FPS 해법의 우선순위에서 제외한다.

## 5. P3 — 공유 Dual Kawase 근사 모드

공개 선택은 `VariableBlurKernel.dualKawase`, 샘플 이름은 **Dual Kawase**다.
Gaussian과 Fast의 의미는 유지한다. Kawase는 `resolutionScale`과 별도로 자체 피라미드를 사용한다.

최초 full-stage Kawase smoke는 평균 29.30 FPS / 표시 p95 50.16ms였다.
형제 scope·up 영역만 수정한 중간 후보는 약 33 FPS / p95 33.44ms였지만 여전히 full-size Gaussian backing이 남았다.
형제 scope·재구성 영역·Gaussian backing까지 줄인 후보는 3회 측정에서 36.15–36.79 FPS였다.
단계 공유만의 성과와 전체 최적화 결과를 구분한다.

1. 원본을 dyadic 격자에 Clamp padding한다.
2. 입력을 한 번의 공유 down 체인으로 축소한다. down 패스는 5회 읽는다.
3. 각 강도 단계는 up 패스로 재구성한다. up 패스는 8회 읽는다.
4. 첫 강도 결과는 1/2, 이후 결과는 1/4까지 재구성한다. raw mip를 완성된 블러 결과로 사용하지 않는다.
5. 최종 픽셀의 sigma에 따라 인접 결과의 **분산**을 보간한다. 반열린 구간으로 alpha 중복 합성을 막는다.
6. 두 device pixel 이하의 sigma는 위치별 Gaussian을 사용한다. 그 이후 첫 Kawase 단계로 연결한다.

| 깊이 | 최종 linear reconstruction까지 포함한 device sigma |
| --- | ---: |
| 2 | 3.5824 |
| 3 | 7.1995 |
| 4 | 15.2698 |
| 5 | 30.9597 |
| 6 | 62.1276 |
| 7 | 124.3597 |

이 값은 실제 embedded 셰이더의 독립 impulse 측정으로 검사한다.
단계 번호를 sigma로 사용하지 않으며, 보간 결과를 정확한 Gaussian으로 표시하지 않는다.
Clamp와 similarity transform(회전·반사·균일 scale)을 지원한다.
다른 tile, 비균일 scale/shear, 범위를 넘는 sigma는 Gaussian으로 대체하며 사유를 진단에 남긴다.

- [x] 셰이더 manifest SHA-256/ABI와 추가 child image 바인딩 연결.
- [x] impulse sigma·에너지·여러 픽셀 phase·반복 윤곽 검사.
- [x] 단계 구간의 premultiplied alpha 및 방향 반전 검사.
- [x] macOS GPU에서 선명한 끝부분과 Gaussian 기준 비교, 최대 채널 차이 0/255.
- [x] iPhone FPS 3회 비교. 기본 Adaptive 유지, Kawase는 명시적 근사 선택으로 남김.
- [ ] iPhone 정지/움직임 화질 검토.
- [ ] 사진·1px 선·작은 글자·sigma 연속 변경·긴 스크롤의 전체 품질 행렬.

## 6. P4 — iOS 제출·표시 연결과 외부 GPU 재판정

Gaussian backing을 줄인 뒤에도 Adaptive 약 33–34 FPS, Kawase 약 36 FPS에 머무는 3회 비교를 확보했다.
호스트의 `maximumPending=1` 및 불필요한 transaction presentation을 확인해 동일 커널의 프레임 연결 실험을 구현했다.

- [x] 새 셰이더 장면이며 native/shield가 없는 경우만 최대 2개 GPU frame 허용.
- [x] paint admission 시 같은 조건을 다시 확인하고, 재표시/새 native 장면이면 소비하지 않고 대기.
- [x] native composition·resize·rotation은 기존 직렬/transaction 경로 유지.
- [x] 역순 GPU 완료가 최신 replay source를 덮지 않는 회귀와 fresh/native/replay admission 회귀 통과.
- [x] Release/Mono 기기 빌드 및 Adaptive 1회 smoke: 33.69 FPS / 표시 p95 33.44ms / 오류 0. 직렬 후보 대비 추가 이득은 확인하지 못함.
- [x] 기본 비활성화. `DOROTI_VARIABLE_BLUR_PIPELINE=1` 또는 수집기의 `--pipeline-frames`에서만 활성화하며 `--serial-frames`가 우선함.
- [ ] 같은 바이너리의 반복 A/B와 수명 검증. 기본 비활성화 전환 이후 iOS 재빌드는 이번 중간 마무리에서 생략.


**조건부 보류.** 현재 GPU backend는 이미 Metal이고, 강한 Gaussian의 반복 연산량을 줄이는 공통 경로가 먼저다.
새 공통 경로에서도 표시 목표를 놓치거나 남은 제출/GPU 비용이 확인되면 동일 품질의 Metal fragment/MPS 비교로 진입한다.
MPS는 고정 sigma 단계 생성과 최종 Variable Blur 합성 비용을 포함해 비교한다.
Compute는 입력 재사용 이득과 halo/barrier 비용을 측정할 근거가 있을 때만 진행한다.

외부 GPU 경로가 필요해지면 기존 device/queue, Linear sampler, premultiplied alpha,
Skia→외부 패스→Skia 자원 가시성 및 GPU 완료 후 회수 계약을 보존한다.
이 단계의 보류는 구현 완료나 성능 검증으로 집계하지 않는다.

## 7. 검증·채택·남은 범위

동일 의미 변경은 같은 커널의 crop/full 및 직접/중간 합성으로 비교한다.
근사 모드는 impulse/edge, 선명한 구간, 움직임 안정성과 전체 표시 FPS를 함께 평가한다.
60Hz 기준 전체 표시 예산은 16.67ms다. GPU 단독 15% 개선 목표는 별도 계측이 없으면 달성으로 표시하지 않는다.

이번 결과와 다음 검증을 구분한다:

- **이번:** 공통 빌드, Gaussian/Kawase raster 검사, macOS Metal GPU 픽셀 비교, iPhone Release/Mono 실행·표시 비교.
- **후속:** NativeAOT, 120Hz, Android/Windows/Linux/Web GPU, 모든 tile/affine/child/opacity 조합, 10분 열·자원 수명 검사.

재현 명령은 [SampleApp2 README](samples/DorotiSampleApp2/README.md)와
[이번 실행 기록](works/results/2026-10-01-variable-blur-reassessment.md)을 따른다.
부분 구현, 테스트 통과, 실제 FPS 개선, 전체 플랫폼 완료를 서로 구분하여 기록한다.

조사 근거: [Android RenderEngine의 Skia 기반 Kawase 구현](https://android.googlesource.com/platform/frameworks/native/+/d647d6cde7f28d83dd03aeff48c3f1bdfe4622df/libs/renderengine/skia/filters/KawaseBlurDualFilter.cpp).
이 자료의 방향을 참고했으며 Doroti의 커널·단계 보정·Variable Blur 합성은 자체 구현이다.
다른 구현의 성능 수치를 Doroti의 예상 성과로 사용하지 않는다.

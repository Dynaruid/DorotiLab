# Variable Blur 개선·최적화 작업계획

작성일: **2026-09-30** · 검토 기준 HEAD: `a40cdcf6` · 상태: **P0 진단·실기기 예비 비교 15회 완료 / GPU 기준·P1~P4 미완료**

대상은 `samples/DorotiSampleApp2`의 Variable Blur와 이를 처리하는 Doroti 공통 Skia 렌더러, 필요한 경우 iOS Metal 효과 경로다. iOS에서 스크롤 중 블러의 프레임 비용을 줄이고 선명한 끝부분·강도 변화·움직임의 품질을 유지한다. 이 문서의 성능 기대는 코드 검토에 따른 가설이며, 측정 결과가 아니다.

진행 순서는 **실기기 기준 측정 → 부분 캡처 개선 → Adaptive 처리량 축소 → Skia Dual Kawase 실험 → 필요한 경우 Metal 비교 → 채택·회귀 검증**으로 한다. 모든 후보를 제품에 넣는 것이 목표는 아니다. 효과가 검증된 변경만 채택하고, 나머지는 결과와 보류 이유를 남긴다.

2026-10-01 진행 기록: [실행 결과와 미검증 범위](works/results/2026-09-30-variable-blur.md). 최초 검토 이후 HEAD는 `8c7127e3`이며 아래 기존 사실 표는 계획 당시 기준이다.

Fast adaptive 겹침 수정: 고정 7회 커널의 샘플 간격 확대로 발생한 이중 윤곽을 수정하고,
강도별 인접 Gaussian 샘플 쌍을 사용한다. 아래 7회 커널 설명과 기존 성능 표는 수정 전 기준이며
새 커널의 비용으로 사용하지 않는다. [수정·검증 기록](works/results/2026-09-30-variable-blur.md#fast-adaptive-겹침-수정)을 따른다.

## 1. 현재 구현에서 확인한 사실

| 영역 | 확인 내용 | 계획에 미치는 영향 |
| --- | --- | --- |
| 샘플 | 60개 리스트 항목, 높이 180 논리 단위의 상단 오버레이, sigma 기본 20·최대 32 | DPR을 반영한 실제 픽셀 크기로 비용을 비교한다. |
| 모드 | 기본은 Adaptive. Full quality / Fast adaptive / Fixed 1/4와 Off를 비교할 수 있음 | 기존 모드로 baseline을 먼저 확보한다. 기본값은 측정 전에 바꾸지 않는다. |
| Gaussian | 그라데이션 방향을 먼저 처리하고 수직 방향을 처리하는 두 패스 | 위치별 sigma 의미와 패스 순서를 보존한다. 임의의 수평→수직 변경은 제외한다. |
| Fast Gaussian | 강한 블러에서 패스당 7회 bilinear 샘플링. 작업 해상도 sigma 2~3에서 기존 커널과 혼합 | 7회 고정 비용을 모든 픽셀에 적용되는 수치로 사용하지 않는다. |
| Adaptive | 현재 샘플에서 1·1/2·1/4 해상도별 결과를 띠 영역과 그라데이션 마스크로 합성 | 전체 화면의 세 벌 Gaussian이라는 단순 추정 대신 실제 영역·패스별 작업량을 측정한다. |
| 부분 캡처 | 1/4 해상도에서 렌더 타깃 가로·세로가 모두 4의 배수가 아니면 전체 타깃 범위 사용. Repeat/Mirror 등도 전체 범위 사용 | 기기별 크기에 따라 최적화가 빠질 수 있다. 실제 capture bounds를 기록한다. |
| 다운샘플 | 축소 단계마다 원본 캡처 전체를 해당 작업 해상도로 그린 뒤, Gaussian 출력 영역은 별도로 제한 | 캡처와 다운샘플의 불필요한 면적을 우선 조사한다. |
| 중간 자원 | SceneSurface는 기존 풀을 사용. Adaptive 출력은 캡처 크기의 Surface이며 Snapshot·추가 합성이 존재 | 풀을 새로 만드는 대신 재사용률·실제 backing 크기·clear/copy 비용을 확인한다. |
| iOS | Skia의 Metal 호스트 경로가 이미 존재 | Metal 이식 자체를 GPU 가속 전환으로 설명하지 않는다. 실행 시 실제 backend를 기록한다. |
| 외부 Metal 효과 | `AppleGpuEffects`는 기존 device/queue 사용, 호출별 입출력 텍스처 생성, Skia segment 제출, 별도 command buffer, Nearest sampler 사용 | 단순 이식은 비용을 늘릴 수 있다. 블러 전용 패스 연결·자원 재사용·Linear sampler 검토가 필요하다. |
| 기존 계측 | `DOROTI_VARIABLE_BLUR_PROFILE=1`은 Stopwatch 기반 호출 구간 측정 | CPU 기록 시간과 GPU 실행·표시 지연을 분리한다. |

핵심 근거 파일:

- [샘플 화면](samples/DorotiSampleApp2/src/VariableBlurPage.cs), [샘플 안내](samples/DorotiSampleApp2/README.md)
- [공개 VariableBlur 계약](Doroti/src/Doroti.Ui/ImageFilter.VariableBlur.cs)
- [캡처 범위](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.VariableBlur.Capture.cs), [필터·합성 연결](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.Filters.cs)
- [Gaussian 실행](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.VariableBlur.cs), [Adaptive 실행](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.VariableBlur.Adaptive.cs), [SkSL 커널](Doroti/src/Doroti.Skia.Rendering/Shaders/variable_blur.sksl)
- [CPU 단계 계측](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.VariableBlur.Profile.cs), [Surface 풀](Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaImageFilterRenderer.cs)
- [UIKit Graphite/Metal 호스트](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs), [외부 Metal 효과](Doroti/src/Doroti.Host.Maui/AppleGpuEffects.cs)
- [기존 렌더링 회귀](Doroti/tests/Doroti.Tests/RenderingRegressions.cs), [공통 렌더링 잔여 작업](work2.md#04-gpu-품질성능-예산기기-수명--p1)

## 2. 아이디어 검토와 우선순위

| 후보 | 판단 | 주요 조건·위험 | 우선순위 |
| --- | --- | --- | --- |
| 부분 캡처 일반화 | 기존 품질을 유지하며 처리 면적을 줄일 가능성이 큼 | 전역 샘플 격자, 홀수 크기, halo, 타일 경계 의미 보존 | P1 필수 |
| Adaptive 영역·중간 합성 축소 | 알고리즘을 유지하는 다음 후보 | 띠 경계 중첩, 투명도, 중간 이미지 수명 | P2 필수 검토 |
| 해상도 피라미드 공유 | 원본 읽기·필터 생성 작업을 줄일 가능성 | 연쇄 축소는 현재 직접 축소와 다른 필터이며 항상 더 싸지는 않음 | P2 선택 실험 |
| Dual Kawase + 단계별 결과 보간 | 넓은 블러용 근사 모드의 우선 실험 | 단계 생성 비용, 보간 오차, 움직임 안정성, 약한 블러 품질 | P3 실험 |
| Metal fragment 직접 실행 | 동일 알고리즘에서 Skia 경유 비용을 분리하는 비교군 | 캡처·제출·재합성 비용까지 포함해야 함 | P4 조건부 |
| MPS Gaussian 단계 생성 | Apple 최적화 구현과 비교 가능 | 고정 sigma API이므로 Variable Blur용 여러 단계와 합성 필요 | P4 조건부 |
| Metal compute | 픽셀 재사용이나 패스 통합 여지가 있을 때 검토 | 작은 커널은 공유 메모리·barrier 비용으로 fragment보다 느릴 수 있음 | P4 후순위 |
| 무효화 기반 캐시 | 정지 화면·파라미터만 변경되는 장면에 유효 | 스크롤·동영상·외부 텍스처 변화는 캐시 무효화 필요 | P2 조건부 |
| 1/8 해상도·동적 품질 | 강한 블러에서 추가 비용 절감 후보 | 현재 캡처 최적화는 grid 8 미지원. 프레임별 전환은 선명도 떨림 가능 | 후속 |
| iOS Vulkan 도입 | 이번 최적화 범위에서 제외 | MoltenVK는 Metal 위의 계층이며 도입만으로 이득을 보장하지 않음 | 제외 |
| 블러 갱신 빈도만 낮추기 | 기본 해법에서 제외 | 스크롤 배경과 블러 결과의 시간차가 보일 수 있음 | 후속 UX 실험 |

Bilinear Gaussian, 분리 패스, Surface 풀, Gaussian 가중치 점화식은 이미 구현되어 있다. 이를 신규 최적화 성과로 집계하지 않는다. FFT·적분 영상 기반 필터는 현재 상단 오버레이 규모에서 복잡도와 준비 비용을 정당화할 근거가 없으므로 우선 구현하지 않는다.

## 3. P0 — 기준 측정과 병목 분리

### 작업

- [ ] 사용 기기 모델·iOS·GPU·실제 렌더러·빌드 모드·DPR·viewport/backing 픽셀 크기·표시 주사율을 기록한다. 시뮬레이터 결과를 iPhone 성능으로 사용하지 않는다.
- [ ] Release 빌드의 동일 리스트·스크롤 경로에서 Off / Full / Adaptive / Fast adaptive / Fixed 1/4를 비교한다. Debug와 Release 결과는 분리한다.
- [ ] 최초 셰이더 준비 비용과 warm 상태를 분리하고, 각 조건을 예열 후 30초 이상·3회 이상 측정한다. 모드 순서를 교차하고 온도·전원 조건을 기록한다.
- [ ] 실제 capture bounds, 전체 범위 fallback 사유, 레벨별 입력/출력 크기·draw bounds, Surface pool hit/miss, 추정 자원 바이트, pass/submission 수를 진단 출력에 추가한다.
- [ ] CPU build/layout/paint·필터 호출 시간, GPU 캡처/축소/블러/합성 시간, 실제 표시 간격을 분리한다. stage가 중첩되므로 `filter-total`과 내부 stage를 합산하지 않는다.
- [ ] 기존 Stopwatch 계측과 Xcode GPU profiler/Metal System Trace를 함께 사용한다. GPU 구간을 분리하기 어려우면 전체 GPU 시간과 그 한계를 기록한다.
- [ ] GPU capture·화면 녹화·계측의 부하를 별도 확인한다. 주 성능 측정에 매 프레임 readback 또는 GPU 완료 대기를 넣지 않는다.
- [ ] README의 `Doroti/validation/cupertino-sample` 등 과거 명령은 실행 전에 존재 여부를 확인한다. 현재 검토에서는 해당 프로젝트가 확인되지 않았다. 살아 있는 테스트 구조를 조사하여 GPU 픽셀 비교·반복 스크롤 측정 진입점을 확보하고 재현 명령을 기록한다.

### 측정 항목과 완료 기준

평균 FPS 외에 CPU/GPU 프레임 시간 p50·p95·p99, 표시 간격, 목표 표시 기한을 놓친 프레임 비율, peak/live GPU 자원, 최초 사용 지연을 남긴다. 60Hz의 16.67ms, 120Hz의 8.33ms는 전체 프레임 예산이며 블러 단독 예산이 아니다. 실제 기기에서 허용·설정된 주사율을 기준으로 한다.

**완료:** 재현 가능한 baseline 표와 capture 크기 로그, profiler 근거를 확보하고 병목을 CPU 기록·GPU 필터·대역폭/복사·표시 스케줄링 중 어느 영역으로 볼지 판정한다. Off에서도 목표를 못 맞추면 리스트 또는 호스트 병목을 별도 작업으로 식별하며, 모든 지연을 블러 탓으로 돌리지 않는다.

## 4. P1 — 부분 캡처와 샘플링 좌표 보존

### 설계·구현

- [ ] `VariableBlurCaptureBounds`가 홀수 크기·4의 배수가 아닌 크기에서 전체 범위를 반환하는 실제 빈도와 추가 처리 면적을 확인한다.
- [ ] 원래 캡처 도메인, 전역 픽셀 원점, 실제 축소 비율, 지역 텍스처 원점을 구분하는 내부 좌표 정보를 설계한다. 영역을 줄였다는 이유로 `ceil(croppedWidth * scale) / croppedWidth`를 새 전역 축소 비율로 사용하지 않는다.
- [ ] 전역 작업 격자에서 필요한 texel 범위를 먼저 계산하고, 두 패스의 샘플 반경과 축소·복원 footprint를 포함해 입력 영역을 산정한다. 필요하면 정렬된 padding 영역을 사용한다.
- [ ] 최초 적용은 샘플의 affine transform·Clamp·1/4 해상도 경로로 제한하고, 기존 fallback을 유지하면서 지원 범위를 확장한다.
- [ ] Clamp/Decal의 원본 도메인 바깥 처리와 잘라낸 텍스처의 가장자리 처리를 구분한다. Repeat/Mirror는 전체 도메인이 필요한 기존 경로를 유지한다.
- [ ] Adaptive 모든 레벨에 같은 전역 원점과 좌표 규칙을 전달한다. 현재 주석의 retained Graphite snapshot 좌표 문제를 실제 회귀 사례로 재현한다.
- [ ] 큰 sigma 또는 거의 전체 화면을 덮는 ROI에서는 전체 캡처가 합리적일 수 있으므로, 항상 작은 영역을 강제하지 않는다.

### 검증·완료 기준

- [ ] 변경 전 전체 도메인 렌더링을 비교 기준으로, 동일 입력에서 crop 적용 여부만 바꾸어 GPU 결과를 비교한다.
- [ ] 4의 배수/비배수·홀수 크기, 부분 픽셀 이동, DPR 1/2/3, oversized backing, 화면 가장자리, 그라데이션 방향 반전, 회전·비균일 scale·shear를 검증한다.
- [ ] 기존 capture 정책 회귀를 새로운 좌표 불변성 검사로 보강한다. 단순 반환 사각형 검사와 GPU 픽셀 검증을 구분한다.

**완료:** 지원 조건에서 캡처/다운샘플 면적이 줄고, 같은 커널의 기준 결과와 좌표·경계 품질을 유지하며 P0의 전체 프레임 비용이 악화되지 않는다. `Snapshot()` 호출을 없앤 수만으로 실제 GPU 복사 감소를 주장하지 않고 profiler로 확인한다.

## 5. P2 — Adaptive 처리량과 중간 합성 축소

### 기존 필터 의미를 유지하는 변경

- [ ] 레벨별 필요한 띠 영역과 다음 패스가 읽는 halo를 역산하여 draw·다운샘플 영역을 줄인다. 중간 이미지가 읽지 않는 영역을 계산하거나 초기화하는 비용을 조사한다.
- [ ] 캡처 크기의 Adaptive 출력 이미지를 거치지 않고 최종 타깃에 합성할 수 있는지 검토한다. 먼저 빈 child를 가진 직접 backdrop 경로를 대상으로 한다.
- [ ] `Plus` 기반 레벨 합성의 가중치 합, premultiplied alpha, clip·blend·opacity 적용 횟수를 유지한다. 각 레벨을 타깃에 단순 SrcOver로 그리는 방식은 동등하지 않으므로 별도 증명이 필요하다.
- [ ] 기존 Surface 풀의 miss·크기 변동·임시 할당·snapshot 유지로 인한 복사를 측정한다. GPU가 참조하는 슬롯을 조기에 재사용하지 않는다.
- [ ] Clear 제거는 이후 읽히는 모든 픽셀이 덮어써짐을 증명한 경로에만 적용한다. 투명한 가장자리·halo·재사용 Surface의 이전 내용 노출을 검증한다.

### 별도 A/B 실험

- [ ] 원본→1/2→1/4 피라미드와 원본→각 해상도 직접 축소를 비교한다. 반복되는 동일 해상도 입력이 있을 때만 해당 입력의 중복 생성을 제거했다고 집계한다.
- [ ] 피라미드 필터의 추가 흐림·aliasing 변화와 메모리 쓰기 비용을 포함한다. 직접 축소와 동등하지 않으면 품질 변경 실험으로 분리한다.
- [ ] sigma 2~3 전환 구간의 이중 커널 비용이 실제 병목일 때만 전용 커널·전환 폭·가중치 LUT를 실험한다. 분기 제거가 무조건 유리하다고 가정하지 않는다.
- [ ] 정지 배경에서는 입력/출력 revision과 bounds·transform·DPR·색 형식·context generation·외부 texture revision을 키로 재사용 가능성을 확인한다. 파라미터만 바뀌면 입력 재사용과 출력 무효화를 분리한다.

**완료:** 각 변경을 독립 비교하여 효과를 설명할 수 있고, 채택한 변경이 P1 대비 전체 프레임·메모리·화질 기준을 만족한다. 캐시 효과는 정지 화면과 스크롤 결과를 나눠 기록한다.

## 6. P3 — Skia Dual Kawase 기반 Variable Blur 실험

### 설계

Dual Kawase 자체는 위치별 sigma 필터를 바로 대체하지 않는다. 서로 다른 강도의 결과를 준비하고 출력 위치의 목표 sigma에 따라 인접 단계의 결과를 보간하는 **근사 모드**로 구성한다. 현재 Adaptive가 해상도별로 위치별 Gaussian을 다시 계산하는 것과 구별한다.

- [ ] 먼저 SkSL과 기존 Surface 풀을 이용해 고정 강도 Dual Kawase downsample/upsample 패스를 구현한다. 기존 Gaussian 경로를 비교 기준으로 유지한다.
- [ ] 약한 블러부터 강한 블러까지 출력 단계의 유효 sigma를 impulse/edge 응답으로 보정한다. 단계 번호나 mip LOD를 sigma와 동일시하지 않는다.
- [ ] 공유 가능한 downsample 체인과 각 강도 결과를 재구성하는 비용을 설계한다. 중간 mip 한 장을 완성된 블러 단계로 가정하지 않고, 단계 생성 전체 패스·메모리를 집계한다.
- [ ] 최종 셰이더에서 목표 sigma에 맞는 인접 결과를 보간한다. sigma 0은 원본을 사용하고, 약한 블러에서 원본과 강한 결과가 겹쳐 보이는 현상은 저강도 전용 Gaussian 또는 추가 단계로 검토한다.
- [ ] 두 Gaussian 결과의 선형 혼합이 중간 sigma Gaussian과 같지 않음을 API·문서에 명시한다. 보간 파라미터는 측정한 응답으로 정하고 분산 기반 보간도 후보로 비교한다.
- [ ] 단계 수·최저 해상도·필터 offset별 품질/성능을 조사한다. 기본 mipmap 생성만으로 대체하는 안은 별도 낮은 품질 비교군으로만 취급한다.
- [ ] 최종 합성의 분기와 texture binding 수를 확인한다. 소스 코드상 두 결과를 선택한다는 이유만으로 모든 backend에서 실제 두 번만 읽는다고 가정하지 않는다.
- [ ] 샘플에 실험 모드를 추가하되 기존 공개 enum·기본값 변경은 채택 판단 이후에 한다. 실험 결과가 지원하는 transform·tile 범위를 명시한다.

### 검증·완료 기준

- [ ] 작은 글자, 1px 선, 체크무늬, 고대비 점, 반투명 색상, 사진을 사용해 강도별 정지 화면을 비교한다.
- [ ] subpixel 스크롤, 방향 반전, sigma 연속 변경에서 단계 경계·격자·깜빡임·이중 윤곽·선명한 끝부분의 과도한 연화를 확인한다.
- [ ] 원본 캡처부터 단계 생성·최종 합성까지 포함해 P2의 Fast adaptive와 비교한다. 최종 합성 패스 단독 수치를 전체 효과로 보고하지 않는다.

**완료:** 품질·성능 곡선과 지원 범위를 기록하고, 신규 근사 모드 채택 또는 보류를 결정한다. 기존 Gaussian의 정확한 대체라고 표시하지 않는다. 장점이 없다면 제품 모드를 추가하지 않고 측정 결과를 남긴다.

## 7. P4 — iOS Metal 직접 실행·MPS·Compute 비교

**진입 조건:** P0~P3 결과에서 남은 GPU/제출 비용이 분명하거나, MPS 비교로 유효한 대안을 평가할 수 있을 때 진행한다. 공통 최적화로 목표를 달성하면 후속 과제로 남길 수 있다.

- [ ] P2 또는 P3에서 선택한 동일 커널·해상도·입출력 형식·품질 설정을 Metal fragment에 옮겨 backend 차이를 먼저 비교한다. 알고리즘과 backend를 동시에 바꾼 결과는 별도로 표시한다.
- [ ] 기존 호스트 device/queue를 재사용하고 Skia 캡처→외부 패스→Skia 합성 순서, resource visibility, GPU 완료 후 회수를 연결한다. 정상 프레임에 CPU 픽셀 왕복이나 device/queue idle wait를 추가하지 않는다.
- [ ] `AppleGpuEffects`의 호출별 texture/sampler/buffer 생성 비용을 줄이고, 한 효과의 내부 패스를 묶는 실행 경로를 검토한다. 여러 효과 위젯을 중첩하여 패스마다 캡처·제출하는 구성을 성능 기준으로 삼지 않는다.
- [ ] Linear sampler를 블러 경로에 명시하고 기존 Nearest 기반 효과의 동작을 바꾸지 않는다. 텍스처 좌표·색 공간·premultiplied alpha를 Skia와 맞춘다.
- [ ] `MPSImageGaussianBlur`로 고정 sigma 단계들을 만들고 같은 위치별 합성으로 비교한다. 각 단계 생성 비용·임시 메모리·재사용을 포함하며 MPS를 Variable Blur 단일 호출 API로 취급하지 않는다.
- [ ] fragment/MPS 이후에도 입력 재사용 이득이 예상되면 Compute의 tile+halo·threadgroup 크기·공유 메모리·패스 통합을 실험한다. 큰 halo의 중복 읽기·barrier·occupancy 저하도 측정한다.
- [ ] FP16 누산 등 정밀도 완화는 별도 품질 실험으로 분리한다. 빌드 시 셰이더 준비 또는 warmup은 최초 지연 개선과 steady-state FPS 개선을 구별한다.
- [ ] background/foreground, resize, 취소, pending GPU 작업이 있는 dispose·context 재생성에서 자원 수명을 검증한다.

**완료:** 최선의 공통 Skia 경로 대비 전체 프레임의 추가 이득을 확인하고 iOS 전용 경로 유지 비용과 함께 채택 여부를 판단한다. 측정 이득이 작거나 반복 오차 범위이면 공통 경로를 유지한다. Vulkan/MoltenVK 전환은 이 단계에도 포함하지 않는다.

## 8. 공통 품질·성능 채택 기준

### 비교 행렬

| 축 | 필수 조건 |
| --- | --- |
| 강도 | sigma 0·1·2·4·8·20·32, 작업 해상도 sigma 2~3 전환 부근 |
| 방향 | 수직 기본, 반전, 수평, 대각선, startSigma=endSigma |
| geometry | 180 논리 높이의 기본 장면, 작은/큰 ROI, 가장자리, DPR 1/2/3, 비정수 이동, 홀수 backing, oversized backing |
| 내용 | 실제 리스트·한글/영문 텍스트·가는 선·고대비 점·사진·반투명 픽셀 |
| 움직임 | 정지, 일정 속도 스크롤, 빠른 스크롤·방향 반전, sigma 연속 변경 |
| 합성 | clip, opacity, 빈/비어 있지 않은 backdrop child, ImageFiltered, 지원 tile·affine transform |
| 수명 | 반복 진입/종료, resize·DPR 변경, 앱 복귀, GPU 사용 중 해제 |

### 판정 규칙

- **동일 의미 최적화(P1/P2):** 기존 전체 도메인 결과와 수치·경계·시간 안정성을 비교한다. 작은 독립 CPU 참조 또는 분석 가능한 impulse/edge 응답을 함께 사용하여 기존 오류의 복제를 방지한다. GPU 부동소수·색 형식에 따른 허용 오차를 baseline에서 정해 변경 전에 고정한다.
- **근사 알고리즘(P3/MPS):** sigma 응답 오차·저강도 선명도·공간 오차·움직임 안정성을 따로 보고한다. 전체 이미지 PSNR/SSIM만으로 승인하지 않고 원본·차이 이미지·동일 움직임 비교를 남긴다.
- **성능:** 3회 이상 반복 측정에서 편차를 넘는 개선이 재현되어야 한다. 초기 목표는 선택한 기준 모드 대비 블러 GPU p95 비용 15% 이상 감소이며, 이는 달성 수치가 아닌 제안 목표다. CPU/표시 지연·전체 프레임 p95·peak 메모리의 악화가 있으면 이득과 원인을 별도 판단한다.
- **사용자 체감:** 60Hz/120Hz 지원 기기의 실제 프레임 예산 충족과 missed-frame 비율을 확인한다. vsync에 묶인 평균 FPS가 같아도 GPU 여유·전력 개선이 있으면 별도 가치로 기록한다. 120Hz 기기가 없으면 해당 결과는 미검증으로 남긴다.
- **장시간:** 최종 후보는 10분 이상 스크롤하여 열 상태에 따른 성능 변화와 live 자원 증가를 확인한다. 전체 앱 메모리가 즉시 원상복귀하지 않는 것과 GPU 자원 누수를 구분한다.
- **플랫폼 회귀:** 공통 Skia 변경은 iOS 실기기와 사용 가능한 다른 GPU backend에서 핵심 픽셀·수명 회귀를 검증한다. Android/Windows/Linux/Web 전체 실행 여부를 각각 기록하며 CPU 테스트 통과를 GPU 검증으로 대체하지 않는다.

정확한 픽셀 허용 오차·메모리 예산·기기별 프레임 목표는 P0 산출물에 수치로 확정한다. 후보 구현을 본 뒤 통과시키기 위해 기준을 완화하지 않으며, 변경이 필요하면 근거와 이전 기준의 결과를 함께 남긴다.

## 9. 산출물·작업 단위·완료 조건

| 작업 단위 | 산출물 | 의존성 |
| --- | --- | --- |
| A. baseline·진단 | 실기기 설정, 재현 명령, 단계/영역 로그, 성능 표 | P0 |
| B. 캡처 좌표 개선 | 공통 렌더러 변경, GPU 비교 이미지, geometry 회귀 | A |
| C. Adaptive 비용 감소 | 독립 변경별 A/B 결과, 최종 합성·풀 수명 증거 | B |
| D. Dual Kawase 실험 | 샘플 비교 모드, sigma 보정, 품질/성능 곡선, 채택 판단 | C |
| E. Metal 비교 | 동일 커널 비교, MPS/Compute 실험 결과, iOS 수명 검증 | C/D 및 P4 진입 조건 |
| F. 통합·문서 | 채택 경로, 기본값 유지/변경 근거, 지원 범위, 재현 가능한 회귀 | 채택한 작업 |

- [x] 결과는 `works/results/2026-09-30-variable-blur.md`에 기록한다. 측정별 기기·커밋·빌드·명령과 원본 산출물 위치를 연결한다.
- [ ] 각 단계에 구현 완료·실행 검증·성능 측정·미검증 범위를 구분한다. 외부 Metal 경로의 과거 코드 존재를 현재 실기기 PASS로 확대하지 않는다.
- [ ] 샘플 README의 모드 설명·실행 가능한 검증 명령·성능 기록 링크를 실제 채택 결과에 맞춘다. 계획된 실험을 지원 기능으로 먼저 문서화하지 않는다.
- [ ] 근사 모드가 채택되면 공개 API·플랫폼 capability·명시적 대체 동작을 정한다. 요청한 Gaussian 품질을 알리지 않고 더 낮은 근사 품질로 변경하지 않는다.
- [ ] `work2.md`의 렌더링 잔여 작업에는 확보한 결과만 연결하고 다른 입력·플랫폼·출시 검증까지 완료로 표시하지 않는다.

전체 완료는 채택된 경로의 iOS 실기기 개선, 품질·자원 수명 검증, 공통 변경의 회귀 결과, 재현 문서가 확보된 상태다. 보류 후보는 사유를 명시하고 완료된 구현으로 집계하지 않는다.

## 10. 조사 근거

아래 자료는 알고리즘과 구현 방향의 근거다. 다른 GPU·다른 시기의 성능 수치를 Doroti나 iPhone의 예상 배수로 옮기지 않는다.

1. [ARM — Bandwidth-efficient graphics, SIGGRAPH 2015](https://developer.arm.com/cfs-file/__key/communityserver-blogs-components-weblogfiles/00-00-00-20-66/siggraph2015_2D00_mmg_2D00_marius_2D00_notes.pdf): Dual filtering의 축소·확대 커널, 모바일 대역폭·움직임 안정성 비교. P3 알고리즘 근거.
2. [Android RenderEngine — KawaseBlurDualFilter.cpp](https://android.googlesource.com/platform/frameworks/native/+/d647d6cde7f28d83dd03aeff48c3f1bdfe4622df/libs/renderengine/skia/filters/KawaseBlurDualFilter.cpp): Skia RuntimeEffect와 Surface로 구성한 Dual Kawase 구현. Skia 유지 실험의 참고 코드이며 임의의 위치별 sigma를 직접 지원한다는 근거는 아님.
3. [RasterGrid — Efficient Gaussian blur with linear sampling](https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/): bilinear 샘플링으로 인접 가중치를 묶는 기법. 현재 Fast Gaussian에 이미 반영된 원리.
4. [Apple — MPSImageGaussianBlur](https://developer.apple.com/documentation/metalperformanceshaders/mpsimagegaussianblur): 고정 sigma의 빠른 근사 Gaussian API. P4 비교 후보.
5. [Apple — Optimizing GPU performance](https://developer.apple.com/documentation/xcode/optimizing-gpu-performance): GPU timeline·셰이더 비용 분석. CPU Stopwatch와 GPU 측정 분리의 근거.
6. [Khronos — MoltenVK](https://github.com/KhronosGroup/MoltenVK): Vulkan을 Apple Metal 위에 구현하는 구조. iOS 전용 최적화에서 Vulkan 전환을 우선하지 않는 판단 근거.

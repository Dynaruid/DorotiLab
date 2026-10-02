# Variable Blur 전체 재평가 결과

실행일: **2026-10-01** · 시작 HEAD `8c7127e3` + 작업 트리 변경.
계획: [work3 요약](../../../26-10-01/work3-summary.md). 대상: SampleApp2와 공통 Skia 렌더러.

**실측 종료 당시의 중간 마무리 상태:** 사용자 요청으로 추가 실험을 멈추고 아래 완료 범위와 미검증 항목을 남겼다.
렌더러 최적화는 유지하고, iOS 프레임 연결은 기본 비활성화한 실험으로 남긴다.
Adaptive/Fast/Kawase는 60 FPS 목표에 미달했다. Fixed의 약 59 FPS 결과는 별도로 평가한다.

캡처 면적만 줄이는 접근에서 **scene 라우팅·Gaussian backing·강한 블러의 단계 공유**를 함께 검증하는 접근으로 바꿨다.
아래 구현 단계별 후보를 구분한다. 처음 만든 Dual Kawase의 부진한 결과도 그대로 기록한다.

## 사용자 체감 반영 재평가 — 2026-10-01

사용자는 현재 앱에서 **Fixed 1/4가 블러 옵션 중 가장 부드럽고, 체감상 60 FPS에 가까우며,
육안으로 큰 이질감은 없다**고 보고했다. 새 계측 결과가 아니라 수동 사용 중의 중간 품질·성능 평가다.

기존 `intermediate`의 같은 바이너리 3회 비교에서 Fixed는 **58.81–58.91 FPS / 표시 p95 약 16.72ms**,
Off는 59.41–59.58 FPS였다. Fixed의 표시 p99는 약 33.44ms이고,
25ms를 넘는 표시 간격은 1.19–1.36%였다. 사용자 체감은 이 기존 계측과 일치한다.
최종 `croppedStorageSerialHost` 비교에서는 Fixed를 재측정하지 않았으므로 해당 빌드의 확정 수치로 옮기지 않는다.

현재 샘플에서는 **Fixed 1/4를 성능·체감 화질의 우선 채택 후보**로 올린다.
Adaptive는 약한 블러의 원본 해상도 선명도가 필요한 품질 선택으로 평가한다.
Kawase가 강한 블러의 비용을 줄였다는 결과는 유지하지만, 현 샘플의 최우선 성능 선택으로 평가하지 않는다.
이 관찰로 항상 60 FPS를 유지하거나 모든 콘텐츠에서 품질이 동일하다고 확정하지 않는다.

현재 코드의 샘플 기본값은 Adaptive다. 기본값 전환에 앞서 최종 같은 바이너리의 Fixed / Off / Adaptive 비교와
약한 블러·작은 글자·가는 선·사진·움직임의 품질 확인을 후속 우선순위로 둔다.
추가 실험 중단 상태는 유지하며, 이번 갱신은 문서 재평가다. 후속 작업 순서와 채택 기준은 [work3 작업계획 요약](../../../26-10-01/work3-summary.md)에 반영했다.

## 구현과 채택 범위

- Shader capture가 없는 형제 scope는 기존 native Skia saveLayer/filter 경로로 처리한다.
  VariableBlur를 포함한 조상은 owned Surface 경로를 유지한다. 무관한 전체 화면 Surface를 만들지 않는다.
- Adaptive 레벨 이미지를 working 해상도로 보관하고 shader에서 가중치 합을 구해 타깃에 한 번 합성한다.
  일반 ImageFiltered/child 경로의 중간 출력은 유지한다.
- Gaussian 두 패스의 **출력 저장 영역**을 띠와 halo로 제한한다.
  입력 도메인·원본 축소 비율·world/working 좌표는 유지하고 텍스처 원점만 명시적으로 보정한다.
  첫 패스 입력을 band 크기로 늘려 그리거나 crop extent로 축소 비율을 재계산하지 않는다.
  Clamp/Decal에서 적용하며 Repeat/Mirror는 기존 전역 저장 경로를 유지한다.
- Dual Kawase 선택을 완성했다. 공유 down 체인, 강도별 up 재구성, 독립 impulse sigma 보정,
  분산 보간과 반열린 interval 합성으로 구성한다. sigma ≤ 2 device pixel은 Gaussian으로 처리한다.
  자체 피라미드를 사용하며 Clamp·similarity transform·약 124 device sigma까지 지원한다.
  미지원 tile/transform/radius/좌표 정밀도는 Gaussian으로 대체하고 사유를 남긴다.
- 기존 Surface 풀을 재사용한다. 슬롯 상한 48과 32M pixel 예산을 유지한다.
  최종 clip/blend/opacity는 한 번 적용한다. 정상 프레임에 readback이나 GPU 완료 대기를 추가하지 않았다.
- `variable_blur_device.py`에 평균 표시 FPS, Kawase 패스, σ 선택, A/B 플래그, source/app hash와 evidence copy 재시도를 추가했다.

공개 API의 Gaussian 기본값과 샘플의 Adaptive 기본값은 유지한다.
Dual Kawase는 명시적인 근사 선택이며 Gaussian의 정확한 대체라고 표시하지 않는다.

## 실제 GPU 픽셀과 회귀

macOS Graphite-Metal의 production renderer에서 입력·출력 좌표와 합성을 검사했다.
`--variable-blur-gpu`는 실제 GPU에서 동작하며 동기 readback은 이 검증 명령 안에서만 수행한다.

| 검사 | 실행 범위 | 최대 채널 차이 |
| --- | --- | ---: |
| crop / full domain | Gaussian·Fast·Kawase, DPR 1/2/3, 반전, 고정 sigma, Clamp/Decal 지원 조합 60조건 | 2/255 |
| cropped / full working storage | 같은 60조건 | 0/255 |
| 직접 / 중간 합성 | 같은 GPU 조건 | 1/255 |
| Kawase 부분 / 전체 up 재구성 | 지원 조합 | 0/255 |
| Kawase의 선명한 끝 / Gaussian | 선형 gradient의 약한 구간 | 0/255 |
| native / owned 형제 scope | 일반 opacity/blur + opacity 안의 VariableBlur | 0/255 |

기존 raster 캡처 48조건, Gaussian impulse, Kawase impulse·에너지·phase·interval alpha·fallback 검사를 통과했다.
Kawase의 sigma는 depth 2~7에서 3.5824 / 7.1995 / 15.2698 / 30.9597 / 62.1276 / 약 124.359 device pixel이다.
공통 CPU 회귀와 Python 집계 검사 5개를 실행했다. 첫 CPU 전체 실행의 long-list fixture가 실패했으나
테스트를 완화하지 않고 재실행 및 최종 실행에서 통과했다. 최초 실패 원인은 확정하지 않는다.

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kawase
python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
```

## 중간 후보 — Gaussian backing 축소 이전

iPhone 12 / iOS 26.6.1 / Graphite-Metal / DPR 3 / 1170×2532 / 60Hz / Release Mono AOT.
각 모드 40초 중 첫 표시 이후 5~35초 구간. 모드별 3회이며 두 번째 반복은 역순이다.
USB 연결·진단 활성, profiler/화면 캡처/녹화 없음. 열·전원·밝기는 통제하지 않았다.

| 모드 | 평균 표시 FPS 범위 | 표시 p95 ms |
| --- | ---: | ---: |
| Off | 59.41–59.58 | 16.72 |
| Full | 29.67–29.71 | 50.16 |
| Adaptive | 31.51–32.14 | 33.44 |
| Fast | 31.64–32.61 | 33.44 |
| Fixed | 58.81–58.91 | 16.72 |
| Kawase | 32.24–32.98 | 33.44 |

18회 모두 renderer/terminal Metal 오류 0. 이것은 최종 Gaussian backing 축소가 들어가기 전의 결과다.
최초 full-stage/owned-sibling Kawase smoke는 29.30 FPS / p95 50.16ms였다.
형제 Surface 4개 제거와 up draw 면적 964,880 → 119,690 pixel 감소만으로는 약 32~33 FPS에 머물렀다.
Fixed/Off의 약 59 FPS는 기기 전체가 30 FPS로 제한된 상태라는 가설과 맞지 않는다.
이후 약한 구간에 남은 전체 크기 Gaussian backing을 줄여 다음 비교를 수행했다.

중간 표의 source hash와 반복별 수치는 [집계 JSON](variable-blur-2026-10-01-reassessment-summary.json)에 보관한다.
`reassessment-comparison`의 첫 Fast evidence copy가 일시 실패했다. 같은 원본을 재복사한 기록은 예비 자료로만 남겼고,
위 표는 재시작한 `reassessment-main`의 완전한 18회를 사용한다.

## Gaussian backing 축소 — 직렬 호스트 비교

같은 기기·Release/Mono·40초/30초 warm·3회 역순 교차 조건에서 실행했다.
Adaptive 33.22–34.07 FPS, Fast 33.69–33.81 FPS, Kawase 36.15–36.79 FPS,
Off 59.34–59.48 FPS였다. 표시 p95는 세 블러 모드 모두 약 33.44ms, Off는 16.72ms였다.
12회 모두 renderer/Metal 오류 0. Gaussian working storage가 줄어든 실제 backing 크기를 진단에서 확인했다.

이 결과를 60 FPS 달성으로 표시하지 않는다. 렌더러 작업량 감소 이후에도 여기서 측정한 세 블러 모드는 표시 목표에 미달한다.
남은 GPU 실행·제출·표시 비용의 기여는 아직 계측하지 않았다.

## iOS 프레임 기록·표시 연결

기존 호스트는 pending frame을 1개로 제한했다. PlatformView coordinator가 설정되었다는 이유로
native composition이 없는 프레임도 transaction presentation을 사용했다.
새 경로는 **새 셰이더 장면 + native/shield 없는 장면**에서만 GPU 작업과 다음 CPU 기록을 겹친다.
paint admission에서 조건을 다시 확인하여 새 native 장면이나 replay로 바뀌면 소비하지 않고 대기한다.
크기 변경·rotation·native composition은 기존 경로를 사용한다.
GPU 완료가 역순으로 도착해도 최신 replay source를 유지하는 실제 renderer 회귀를 통과했다.

## 프레임 연결 실험과 중간 마무리

실험 경로를 포함한 iOS Release/Mono 빌드는 경고·오류 0으로 통과했다.
Adaptive 1회 40초 smoke의 5~35초 구간은 **33.69 FPS / 표시 p95 33.44ms / renderer·Metal 오류 0**이었다.
직렬 후보의 33.22–34.07 FPS 범위와 겹치며, 동일 바이너리 반복 A/B는 완료하지 않았다.
추가 이득을 확인하지 못했으므로 기본 실행에서는 비활성화했다.

현재 소스는 `DOROTI_VARIABLE_BLUR_PIPELINE=1` 또는 수집기 `--pipeline-frames`에서만 실험을 활성화한다.
`--serial-frames`는 이를 다시 끈다. 마지막 기본 비활성화 전환과 수집기 옵션 추가는 기기 빌드 이후의 변경이며,
그 전환을 포함한 iOS 재빌드는 수행하지 않았다. 당시 기기 바이너리는 실험이 기본 활성화된 버전이다.
추가 trace는 수집하지 않았고, 남아 있던 테스트 앱은 종료했다.

공통 CPU 회귀·macOS Metal GPU 비교는 통과했다. 마지막 옵션 변경 후 Python 검사 5개와 `git diff --check`도 통과했다.
렌더러 최적화는 현재 작업 트리에 유지하고 샘플 기본 모드는 Adaptive로 남긴다.

## 재현과 원본

```sh
dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:ArtifactsPath=/Users/ceramic/Labo/DorotiLab/Doroti/artifacts/variable-blur-reassessment-final-mono
python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app <SIGNED_APP_PATH> --output temp/testing/variable-blur/fresh-final --hz 60 --repeats 3 --modes adaptive fast kawase off --conditions '<전원·열·밝기·계측 조건>'
```

A/B 플래그: `--full-bands` / `--full-detail` / `--owned-subtrees` / `--full-stages` / `--intermediate` / `--full-capture` / `--serial-frames`.
각 플래그는 입력·커널을 그대로 두고 대응 최적화를 끈다. `--pipeline-frames`는 기본 비활성화된 iOS 실험을 켠다.
`--sigma 32`로 최대 강도를 선택한다.

로컬 원본: `temp/testing/variable-blur/reassessment/`, `reassessment-smoke/`,
`reassessment-comparison/`, `reassessment-main/`, `reassessment-final/`, `reassessment-pipeline-smoke/`.
iOS 빌드·CPU/GPU 회귀 로그와 기기 evidence를 분리했다.
앱 payload/source hash는 dirty HEAD만으로 미추적 셰이더 구현을 식별하지 못하는 문제를 보완한다.

## 검증 한계와 후속

NativeAOT, 120Hz, 다른 GPU backend, oversized backing과 전체 회전/shear/child/opacity/resize 행렬,
10분 열·자원 수명, profiler 부하 A/B 및 블러 단독 GPU p95는 미완료다.
iPhone의 정지/움직임 화질 검토와 프레임 연결의 동일 바이너리 반복 A/B도 남아 있다.
표시 p95나 CPU Stopwatch를 블러 GPU 실행 시간으로 바꾸어 보고하지 않는다.
끝 시점 Metal 할당량은 device 전체 값이며 peak/live 블러 VRAM이 아니다.
macOS 검증의 Skia snapshot/asImage 경고가 있었으나 지정한 픽셀 비교는 통과했다. 전체 snapshot 정책을 변경한 검증은 아니다.

외부 Metal/MPS/Compute는 별도 진입 조건이 생길 때 비교한다.
현재 공통 경로의 결과를 먼저 판단하며, P4 보류를 완료한 기능으로 집계하지 않는다.

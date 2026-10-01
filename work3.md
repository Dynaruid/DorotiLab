# Variable Blur 작업계획 — Fixed 1/4 우선 채택

수정일: **2026-10-01** · 대상: SampleApp2 / 공통 Skia 렌더러 / iPhone 12 Graphite-Metal, 60Hz
상태: **P0 완료, P1 18회 자동 성능·경로 기준 통과. P2 Fixed 기본값 후보 빌드·설치·자동 smoke 완료. 환경 통제·수동 조작/화질·P3 10분 사용은 미확인이라 전체 채택 완료는 아님.**

**목표:** 현재 사용자에게 허용 가능한 화질을 유지하면서, Fixed 1/4의 약 59 FPS를 현재 소스에서도 확인하고 샘플 기본값으로 채택한다.
원본 해상도의 선명한 끝부분이 필요한 경우에는 Adaptive를 선택한다.
기존의 “Adaptive/Kawase를 계속 최적화해 60 FPS에 도달”하는 순서에서
**Fixed 검증 → 같은 바이너리 성능·화질 비교 → 샘플 기본값 전환 → 장시간 사용 확인**으로 작업 순서를 바꾼다.

## 1. 재분석 근거와 남은 불확실성

이 절의 이전 비교·검증 공백은 계획 수립 시점의 근거다. 이번 실행으로 확인한 결과와 남은 게이트는 9절 및 실행 기록에 구분한다.

사용자는 현재 앱에서 **Fixed 1/4가 블러 옵션 중 가장 부드럽고, 체감상 60 FPS에 가까우며, 육안으로 큰 이질감은 없다**고 평가했다.
이는 현재 샘플의 수동 사용에 대한 중간 평가이며, 기존 표시 간격 계측과도 일치한다.

아래는 Gaussian backing 축소 이전의 **같은 바이너리·같은 합성 스크롤·모드별 3회** 비교다.

| 모드 | 평균 표시 FPS 범위 | 표시 p95 ms | 이번 계획에서의 역할 |
| --- | ---: | ---: | --- |
| Off | 59.41–59.58 | 16.72 | 블러 없는 표시 성능 기준 |
| Fixed 1/4 | 58.81–58.91 | 16.72 | 우선 채택 후보 |
| Adaptive | 31.51–32.14 | 33.44 | 약한 블러의 선명도를 보존하는 품질 선택 |
| Fast adaptive | 31.64–32.61 | 33.44 | 기존 선택 유지, 추가 최적화는 조건부 |
| Dual Kawase | 32.24–32.98 | 33.44 | 공유 단계의 근사 선택, 추가 최적화는 조건부 |
| Full quality | 29.67–29.71 | 50.16 | 원본 해상도 품질 기준 |

이후 backing을 줄인 후보에서 Adaptive 33.22–34.07, Fast 33.69–33.81, Kawase 36.15–36.79 FPS를 확인했지만,
**그 마지막 비교에는 Fixed가 없다.** 단계가 다른 수치를 최종 동일 바이너리의 순위로 사용하지 않는다.
Fixed의 이전 표시 p99는 약 33.44ms, 25ms 초과 표시 간격 비율은 1.19–1.36%였다.
엄밀한 60 FPS 고정 달성이 아니라 **60Hz 화면에서 대부분 한 표시 주기로 갱신되는 실용적인 결과**로 평가한다.

코드 재검토로 확인한 내용:

- Fixed도 sigma가 위치에 따라 바뀌는 Variable Blur다. 가로·세로를 각각 약 1/4로 줄여 작업 픽셀이 약 1/16이 된다.
- 현재 빈 child의 BackdropFilter 경로는 `keepWorkingResolution: true`로 축소 결과를 최종 타깃에 직접 linear sampling한다.
  Fixed에 원본 크기 복원 Surface를 없애는 최적화를 다시 계획할 필요가 없다.
- Adaptive는 1·1/2·1/4 레벨과 가중 합성을 사용한다. Kawase도 여러 강도 결과의 재구성과 약한 블러의 Gaussian 처리가 필요하다.
  이 작업량 차이는 성능 차이의 유력한 설명이지만, GPU 단독 계측으로 주 병목을 확인한 것은 아니다.
- 기존 raster 캡처 검사는 Fixed 설정과 축소 레벨을 검사했다. 그러나 production GPU 비교의 설정은
  `adaptiveResolution: true` 중심이고, Fixed의 실제 직접 합성 경로를 독립적으로 검증하지 않았다.
- 마지막 iOS 기기 빌드 이후 프레임 연결의 기본 비활성화가 반영되었다. 현재 소스를 새로 빌드해야 그 설정까지 검증할 수 있다.
- 낮은 sigma에서도 Fixed의 필터 출력은 축소 입력을 사용한다. 작은 글자·가는 선·사진의 세부가 부드러워지거나
  sigma를 0으로 바꿔 필터를 끌 때 선명도가 달라질 수 있다. 현재 사용자 관찰을 이 모든 조건의 검증으로 확대하지 않는다.

계획 수립 시 샘플 기본값은 Adaptive였다. 이번 구현에서는 Fixed 후보로 변경했으며, 구현·기기 측정·수동 품질 평가의 결과를 각각 기록한다.

## 2. 이번 작업의 범위와 채택 기준

우선 대상은 **현재 SampleApp2의 180 논리 단위 오버레이, Clamp, iPhone 12 / DPR 3 / 60Hz / Release Mono**다.
블러 강도는 기본 sigma 20과 슬라이더 상한 32를 성능 비교하고, 낮은 강도와 전환은 수동으로 검토한다.
공개 API의 Gaussian·원본 해상도 기본값과 각 커널의 의미는 유지한다.
샘플 기본값 변경만으로 모든 플랫폼이나 모든 tile/transform/child 조합이 검증되었다고 표시하지 않는다.

다음은 **이번 계획에서 정한 채택 기준**이다. 이번 실행의 수치·경로 판정은 9절에 기록하고, 환경 통제·수동 화질·지속 사용의 미확인 조건은 통과로 확대하지 않는다.
기존 Fixed의 약 58.9 FPS·p95 16.72ms·긴 간격 약 1.3%를 재현할 수 있도록 작은 여유를 둔다.

| 항목 | 60Hz 대상의 기준 |
| --- | --- |
| 평균 표시 FPS | sigma 20/32 각각 Fixed 3회 모두 58 이상, 각 강도의 Fixed 평균이 Off 평균의 97% 이상 |
| 표시 p95 | 각 실행 17.5ms 이하 |
| 표시 p99 | 각 실행 34.2ms 이하 |
| 긴 표시 간격 | 수집기 정의인 25ms 초과 간격이 각 실행 2% 이하 |
| 오류·실행 상태 | renderer/terminal Metal 오류 0, 완전한 30초 표시 이력, 요청한 모드의 작업 경로 확인 |
| 체감 화질 | 현재 장면에서 큰 이질감 없음. 약한 구간·끝 경계·강도 변경·스크롤에서 눈에 띄는 깜빡임이나 반복 윤곽 없음 |
| 지속 사용 | 10분 연속 사용에서 반복되는 멈춤·화질 이상·렌더러 오류 없이 사용 가능 |

Off도 평소의 약 59 FPS를 크게 벗어나면 열·전원·계측 조건을 확인하고 해당 비교 묶음을 다시 수집한다.
실패한 실행과 재실행 사유도 남기며, 좋은 실행만 골라 채택하지 않는다.
표시 간격은 GPU 실행 시간이 아니다. 종료 시 Metal 할당량은 device 전체 값이며 블러의 peak/live VRAM이나 누수 판정으로 사용하지 않는다.

## 3. P0 — 현재 소스와 Fixed 검증 기반 확정

**목적:** 이전 바이너리의 결과와 현재 Fixed 경로 사이의 검증 공백을 닫는다.

- [x] 현재 소스를 Release/Mono로 새로 빌드하고 설치한다. pipeline 기본 비활성화를 포함한 빌드임을 확인한다.
- [x] source hash·app payload hash·기기/OS·backend·DPR·표시 Hz를 기록한다.
- [x] 기존 `VariableBlurGpuRegression`에 `adaptiveResolution: false`, `resolutionScale: 0.25`, Gaussian인 Fixed 비교를 추가한다.
  실제 `ApplyVariableBlur`의 축소 출력과 최종 linear sampling을 사용하며 Adaptive shader 경로로 대체하지 않는다.
- [x] Fixed의 crop/full domain 및 축소 결과 직접 합성/기존 복원 결과를 GPU에서 비교한다.
  DPR 1/2/3, 반전, fractional ROI, 비배수 크기, 반투명 입력, 지원하는 Clamp/Decal을 포함한다.
  같은 의미의 비교에는 기존 GPU 검사의 채널 오차 상한 3/255를 적용하고 실제 최대 차이를 남긴다.
- [x] Full Gaussian과 Fixed의 화질 차이는 별도로 검토한다. 원본 해상도 출력과 픽셀 동등성을 요구하지 않는다.
- [x] 기존 Gaussian kernel·capture 회귀와 macOS Metal GPU 검사를 실행한다.
  GPU 비교가 실패하면 좌표·배율·halo·투명도 문제를 먼저 해결하고 P1로 진행한다.

대상 파일:
[GPU 회귀](Doroti/tests/Doroti.Tests/VariableBlurGpuRegression.cs),
[필터 라우팅](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.Filters.cs),
[Fixed/Gaussian 처리](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.VariableBlur.cs).

**완료 조건:** 현재 소스의 빌드 identity를 확보하고, Fixed의 실제 처리·합성 경로에서 동일 의미 GPU 비교를 통과한다.
일반 ImageFiltered나 비어 있지 않은 child의 복원 Surface 제거는 이번 기본값 채택 작업에 포함하지 않는다.

## 4. P1 — 같은 바이너리에서 표시 성능과 체감 화질 확인

**목적:** Fixed를 선택할 실용적인 근거를 최종 후보에서 확보한다.

- [x] 같은 바이너리에서 **Off / Fixed / Adaptive**, sigma **20 / 32**, 모드별 **3회**를 비교한다. 총 18회다.
  기존 수집기의 두 번째 반복 역순을 사용한다. Fast/Kawase/Full 전 모드 반복은 이번 채택의 필수 작업에서 제외한다.
- [x] 각 실행 40초 중 첫 표시 이후 5~35초를 집계한다. `--serial-frames`로 기존 직렬 기준을 명시하고 pipeline 실험은 끈다.
- [ ] 전원 연결·밝기·기기 온도/냉각 상태·진단 활성 조건을 기록하고 비교 중 유지한다.
  성능 측정 중에는 profiler·스크린샷·화면 녹화를 사용하지 않는다.
- [x] 평균 FPS, 표시 p50/p95/p99, 긴 간격 비율, 오류, 작업 크기·패스 수를 실행별로 보고하고 2절의 기준으로 판정한다.
  Fixed가 단일 축소 해상도의 Gaussian 두 패스를 실행하는지 확인한다.
- [ ] 자동 스크롤과 별도로 실제 손가락 스크롤의 느린 이동·빠른 이동·방향 전환을 확인한다.
- [ ] sigma **0 / 1 / 2 / 4 / 8 / 20 / 32**와 연속 슬라이더 변경, 블러 켜기/끄기, 모드 변경을 검토한다.
  특히 0↔작은 sigma에서 필터 비활성화에 따른 선명도 변화와 끝 경계의 이음새를 확인한다.
- [ ] 현재 리스트의 글자·숫자·색상 경계를 검토하고, 별도 검증 장면에서 작은 글자·1px 선·고주파 무늬·사진을 비교한다.
  정지/움직임 결과와 콘텐츠 조건을 기록한다. 기존 샘플 화면을 계측 정보로 확장하지 않는다.
- [x] 품질 비교용 캡처/녹화는 성능 수집과 분리한다. 낮은 sigma에서 끊김이 관찰되면 해당 강도만 추가 계측한다.

**판정:** 기준을 충족하면 P2로 진행한다. 성능 또는 필요한 화질이 미달하면 실패 조건을 기록하고 7절의 해당 분기로 들어간다.
사용자의 이번 체감 평가는 채택의 근거에 포함하고, 추가 품질 확인은 낮은 sigma·전환·새 콘텐츠 조건에 집중한다.

## 5. P2 — Fixed 1/4를 샘플 기본값으로 전환

**목적:** 확인된 성능 선택을 앱을 열었을 때 바로 사용할 수 있게 한다.

- [x] `VariableBlurPageState._mode` 기본값을 `"fixed"`로 바꾼다.
  benchmark 미설정 경로와 Off를 선택한 뒤 다시 블러를 켜는 경로의 초기 모드도 일치시킨다.
- [x] Fixed 설정은 `resolutionScale: 0.25`, `adaptiveResolution: false`, `kernel: gaussian`으로 유지한다.
  새로운 커널·공개 preset·자동 모드 선택을 추가하지 않는다.
- [x] 기존 모드 선택은 유지하고 설명 문구에서 Fixed의 부드러운 스크롤과 Adaptive의 약한 블러 선명도를 구분한다.
- [x] SampleApp2 README의 기본 모드·사용 예제·benchmark 설명을 현재 동작에 맞춘다.
  공개 `ImageFilter.variableBlur`와 `ImageFilterConfig.CreateVariableBlur` 기본값은 변경하지 않는다.
- [ ] 후보를 새로 빌드해 benchmark 환경변수 없이 첫 진입·슬라이더·켜기/끄기·모드 전환·페이지 재진입을 smoke 확인한다.
  렌더러·셰이더·호스트가 P1과 같으면 최종 빌드의 Fixed/sigma 20을 1회 수집해 작업 경로와 표시 상태를 확인하고,
  채택의 반복 성능 근거는 P1 기록을 사용한다. 해당 구현이 바뀌었거나 smoke에서 이상이 발견되면 P1 비교를 다시 수행한다.

대상 파일: [VariableBlurPage.cs](samples/DorotiSampleApp2/src/VariableBlurPage.cs), [README](samples/DorotiSampleApp2/README.md).

**완료 조건:** 일반 앱 진입에서 Fixed가 기본 선택되고, 기존 조작이 정상 동작하며, 최종 빌드의 smoke와 P1의 채택 근거를 연결해 기록한다.
단순 초기값 변경을 검사하는 테스트를 새로 만들지 않고, 실제 기기 진입과 기존 검증을 사용한다.

## 6. P3 — 장시간 사용 확인과 결과 정리

- [ ] 최종 후보로 10분 연속 스크롤을 확인한다. 동일 전원·밝기 조건의 Off도 비교해 열·공통 렌더링 영향을 구분한다.
  기존 자동 수집기는 실행당 40초이므로 10분 검증은 별도 수동 연속 사용으로 진행한다.
- [ ] 시작/종료의 자원 진단과 오류, 반복되는 끊김·화질 이상을 기록한다.
  단일 종료 할당량으로 누수를 확정하지 않는다. 자원 증가가 의심되면 GPU 완료 후 회수·풀 상한을 별도로 조사한다.
- [x] 품질 허용 조건, 실제 표시 성능, 빌드 identity, 기본값 변경, 미검증 범위를 실행 기록에 반영한다.
- [x] `git diff --check`와 변경에 해당하는 기존 검사로 마무리한다.
  렌더러를 수정했다면 공통 CPU 회귀도 실행하고, 수집기를 수정했다면 Python 집계 검사도 실행한다.

**이번 계획의 완료:** P0~P3를 통과하고 iPhone 12/60Hz의 현재 샘플에서 Fixed를 기본으로 사용한다.
다른 플랫폼·120Hz·NativeAOT·전체 affine/tile/child/opacity 행렬은 별도 검증 범위로 남긴다.

## 7. 실패 조건에 따른 후속 작업

| 확인된 문제 | 다음 작업 | 채택 조건 |
| --- | --- | --- |
| 현재 Fixed의 성능이 이전보다 나빠짐 | source/app identity와 프레임 설정, 캡처·작업 크기·패스 수·scene Surface부터 비교. 필요할 때 profiler 부하 A/B와 GPU 구간 측정 | 같은 입력의 표시 성능 회복. CPU 호출 시간 감소만으로 채택하지 않음 |
| Fixed의 약한 구간/끝 경계가 실제 요구 화질에 미달 | 먼저 해당 사용처에서 Adaptive 선택. 성능과 선명도가 모두 필요하면 별도 후보로 원본 입력과 Fixed 출력의 약한 구간 연속 합성 또는 제한된 고해상도 처리를 비교 | 반복 윤곽·alpha·경계·강도 연속성 확인과 Fixed 대비 표시 비용 검증. 단순 원본 혼합을 정확한 Gaussian으로 표시하지 않음 |
| sigma 32나 특정 콘텐츠에서만 실패 | 조건을 분리해 재현하고 그 조건의 샘플 비용·움직임을 분석 | 실패 범위를 숨기거나 슬라이더를 임의로 줄이지 않고 수정 또는 명확한 적용 범위 기록 |
| 특정 고품질 사용처에서 Adaptive/Kawase가 필요하지만 표시 목표에 미달 | 해당 사용처를 고정해 비교하고 GPU/제출 비용을 계측. 근거가 생기면 같은 품질의 Metal fragment/MPS를 비교 | 생성·합성·자원 연결을 포함한 전체 표시 성능 개선 |
| 제출/표시 연결이 병목으로 확인됨 | 기존 pipeline opt-in의 같은 바이너리 반복 A/B와 수명·native/resize 검증 | 반복 이득과 올바른 완료 순서·자원 회수 확인 전에는 기본 활성화하지 않음 |

Full/Fast/Kawase 선택과 기존 품질 수정을 유지한다.
정지 화면 캐시·갱신 빈도 감소·과거 고정 7회 Fast 커널 복원·새 Surface 풀·외부 device/queue는 현재 채택 작업의 해결책으로 추가하지 않는다.
외부 GPU가 필요하면 기존 device/queue, premultiplied alpha, Skia 자원 가시성 및 GPU 완료 후 회수 계약을 보존한다.

## 8. 실행 진입점과 기존 완료 범위

다음 명령은 **재현 진입점**이다. 이번 실제 실행의 결과·identity는 아래 별도 기록에 남겼으며, 명령 자체를 통과 근거로 사용하지 않는다.
저장소 루트에서 서명된 Release/Mono 앱을 만든 뒤, 매 측정은 새 출력 폴더를 사용한다.
sigma 32 비교에도 같은 바이너리의 `--app`을 지정해 payload hash를 남긴다.

```sh
dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:ArtifactsPath=/Users/ceramic/Labo/DorotiLab/Doroti/artifacts/variable-blur-fixed-adoption-mono
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kernel
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-capture
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu
python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app <SIGNED_APP_PATH> --output temp/testing/variable-blur/fixed-adoption-sigma20 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 20 --serial-frames --conditions '<전원·열·밝기·진단 조건>'
python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app <SIGNED_APP_PATH> --output temp/testing/variable-blur/fixed-adoption-sigma32 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 32 --serial-frames --conditions '<전원·열·밝기·진단 조건>'
```

정지 화면은 `DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC=1`과 benchmark 모드/sigma로 별도 실행한다.
자동 스크롤 수집기와 정지 옵션을 함께 사용하지 않는다.
`--full-capture`는 필요할 때 Fixed의 같은 바이너리 캡처 A/B에 사용한다.

이전 P0~P4는 새 계획과 구분한다.

| 이전 작업 | 기존 완료·결과 | 현재 처리 |
| --- | --- | --- |
| 표시 진단·6모드 비교 | 평균 FPS, 표시 간격, 해시, A/B 진입점과 반복 기기 기록 확보 | 재사용. 새 Fixed 비교를 기존 완료로 대신하지 않음 |
| 축별 부분 캡처 | raster 48조건, production GPU 비교. 처리 면적 감소만으로 표시 p95 이득은 미확인 | 유지. Fixed의 실제 GPU 경로 보완 |
| scene 라우팅·Adaptive backing/합성 | native 형제 scope, 축소 결과 직접 합성, Gaussian 저장 영역 제한 구현 | 유지. 불필요한 재구현 제외 |
| Dual Kawase | 공유 down/up, sigma 보정·분산 보간·fallback 연결과 회귀 | 명시적 근사 선택 유지 |
| iOS 프레임 연결 | 조건부 2-frame 실험과 회귀. Adaptive 1회 smoke의 추가 이득 미확인 | 기본 비활성화, 현재 소스 빌드 확인 |
| 외부 Metal/MPS/Compute | 별도 경로 구현·비교 미수행 | 7절의 계측 근거가 생길 때 진행 |

기존 구현·측정 원본은 [재평가 실행 기록](works/results/2026-10-01-variable-blur-reassessment.md),
[집계 JSON](works/results/variable-blur-2026-10-01-reassessment-summary.json),
[축별 부분 캡처](works/results/2026-10-01-variable-blur-per-axis.md),
[Fast 윤곽 수정](works/results/2026-09-30-variable-blur.md)에 보관한다.
빌드·테스트 통과, 표시 FPS 개선, 사용자 체감 화질, 전체 플랫폼 검증을 구분해 기록한다.


## 9. 2026-10-01 실행 결과와 남은 게이트

[실행 기록](works/results/2026-10-01-variable-blur-fixed-adoption.md),
[집계 JSON](works/results/variable-blur-2026-10-01-fixed-adoption-summary.json)에 새 결과를 보관했다.
P0 Fixed 실제 GPU 처리·직접 합성 72조건은 최대 1/255로 통과했다.
Gaussian kernel, raster capture 48조건(최대 2/255), 기존 Metal GPU와 공통 CPU 회귀도 통과했다.

P1 같은 app/source hash의 18회에서 Fixed 평균은 sigma 20 **59.09 FPS**, sigma 32 **58.86 FPS**였다.
강도별 Off 평균 대비 **99.57% / 99.18%**이며 Fixed 6회가 FPS/p95/p99/긴 간격·오류·실제 단일 1/4 Gaussian 두 패스 기준을 모두 통과했다.
이 판정은 수치·경로 기준이다. 밝기·실제 충전·열/냉각 상태를 통제하지 못했으며 수동 화질·입력 게이트가 남았다.

P1 성능 수집 후 iPhone Full/Fixed 정지 캡처 14장과 별도 macOS Metal 품질 장면 14장을 확보했다.
낮은 sigma의 축소 입력에 따른 글자·1px 무늬·둥근 경계의 softness/aliasing과 sigma 0의 원본 복귀를 기록했다.
정지 상태 비교를 움직임·연속 슬라이더 전환·사용자 허용 확인으로 확대하지 않았다.

P2는 Fixed 기본값 **후보 구현**이며 최종 채택 완료는 아니다.
현재 요청 범위에서 후보를 빌드·설치해 확인 가능하게 준비하지만, 일반 진입 후 실제 조작·페이지 재진입과
P3의 동일 조건 Fixed/Off 10분 수동 사용은 직접 확인이 필요하다.
완료한 자동 검사만 체크했으며 이 수동 항목들은 미완료로 유지한다.


최종 후보 Release/Mono 빌드(경고·오류 0)와 Fixed/sigma 20 1회는 **58.97 FPS / p95 16.72ms / p99 33.44ms / 긴 간격 1.41% / 오류 0**으로 통과했다.
renderer/shader/host/API source hash가 P1과 동일함을 확인했다.
benchmark 미설정 일반 Components 시작을 확인하고 앱을 실행 상태로 남겼다.
Variable Blur 첫 진입·실제 조작 smoke와 P3 수동 게이트는 여전히 미완료다.

사용자 추가 관찰: 최종 자동 검증 뒤 현재 iPhone이 차갑고 배터리 100%라고 보고했다. 밝기·앞선 측정 기간의 조건 유지와 수동 조작/10분 결과는 미확인이다.

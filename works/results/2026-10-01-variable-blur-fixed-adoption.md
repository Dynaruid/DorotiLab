# Fixed 1/4 검증·기본값 변경 후보 — 2026-10-01

현재 소스의 P0 GPU 검증과 같은 iPhone 바이너리의 P1 18회 자동 표시 성능 수집을 완료했다. Fixed의 수치·실제 작업 경로 기준은 모두 통과했다. SampleApp2 초기 모드는 Fixed로 변경한 후보이며, 환경 조건 통제·물리 입력·연속 전환·10분 수동 사용이 미확인이라 P0~P3 전체 채택 완료로 판정하지 않는다.

## 빌드 identity와 조건

기준 HEAD: `99687edfa5eb7d4123de39ac3c934be9a23e3867`. 측정 당시 작업 트리는 검증 코드와 문서 변경으로 dirty였다. iOS Release/Mono 빌드의 경고·오류는 0이다. .NET SDK 10.0.401 / iOS workload 27.0.10722 / Xcode 27.0 (27A266a)를 사용했다.

P1 설치 app payload SHA-256: `e480a29ff27f2ef5ca9d329bf23324816c9b83e6261d09c9bb2d97279b347153`. sigma 20/32의 app hash와 renderer/shader/host/API/page source hash가 모두 일치한다. 상세 source hash와 실행별 수치는 [집계 JSON](variable-blur-2026-10-01-fixed-adoption-summary.json)에 보관한다. 기준 서명 앱은 `temp/testing/variable-blur/fixed-adoption-validation/baseline.app`에도 보존했다.

대상은 iPhone 12 (iPhone13,2), iOS 26.6.1 (23G83), UIKit/MTKView/Graphite-Metal, DPR 3, viewport 1170×2532, 60Hz다. USB wired 연결을 확인했다. 진단/evidence는 켰고 `--serial-frames`로 GPU 완료 후 기록하는 경로를 강제했다. pipeline은 끄고 profiler·스크린샷·녹화를 성능 수집 중 사용하지 않았다.

밝기·실제 충전 상태·기기 온도·냉각 상태의 수치와 일정한 유지 여부는 확인하지 못했다. Off 평균은 두 강도 모두 약 59.34 FPS로 기존 기준과 비슷하지만, 이것만으로 해당 환경 조건이 통제되었다고 판정하지 않는다.

## P0 GPU·CPU 결과

Fixed Gaussian 설정(`adaptiveResolution: false`, `resolutionScale: 0.25`)으로 실제 `ApplyVariableBlur`를 호출했다. 축소 출력 크기도 검사하고, 최종 linear sampling과 기존 복원 Surface 합성을 GPU에서 비교했다. Adaptive shader로 대체하지 않았다.

72개 조건은 DPR 1/2/3, 강도 ramp 반전, 수직/수평 gradient, 내부/edge fractional ROI, 반투명 입력, Clamp/Decal, 385×1536·1536×385·385×1537을 포함한다. 양축 비배수 조건에서는 현재 전체 도메인 fallback을 확인한다. 최종 fractional ROI clip을 한 번 적용하고 부분 커버리지 경계 픽셀의 RGBA도 비교한다. 직접/복원 각각 crop/full과 두 도메인의 직접/복원 비교가 모두 통과했고 최대 채널 오차는 **1/255**, 상한은 **3/255**다.

기존 Adaptive/Fast/Kawase crop/full·저장 영역·직접 합성·약한 끝·native/owned scope GPU 비교도 통과했다. Gaussian kernel 회귀, raster capture 48조건(최대 2/255), 공통 CPU 회귀도 통과했다. 제품 렌더러·셰이더·호스트 구현은 수정하지 않았다.

## P1 같은 바이너리의 18회 표시 성능

각 실행은 40초 합성 스크롤이며 첫 drawable 표시 이후 5~35초의 완전한 표시 간격을 집계했다. 두 번째 반복은 Adaptive→Fixed→Off 역순이다. 모든 실행의 완료 표식과 30초 측정 범위 이력이 유효하고, 렌더러/terminal Metal 오류는 0/0이었다. 실패한 성능 실행이나 좋은 결과만 고른 재실행은 없다.

| sigma | 모드 | 반복 | 평균 FPS | p50 ms | p95 ms | p99 ms | >25ms % | Gaussian 패스 | 작업 해상도 |
| ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| 20 | off | 1 | 59.36 | 16.72 | 16.73 | 16.73 | 0.73 | 0 | — |
| 20 | fixed | 1 | 59.12 | 16.73 | 16.73 | 33.45 | 1.13 | 2 | 293×230 |
| 20 | adaptive | 1 | 33.29 | 33.45 | 33.46 | 33.46 | 78.66 | 6 | 293×129 / 293×131 / 585×30 / 585×32 / 1170×39 / 1170×41 |
| 20 | adaptive | 2 | 33.46 | 33.45 | 33.46 | 50.18 | 77.37 | 6 | 293×129 / 293×131 / 585×30 / 585×32 / 1170×39 / 1170×41 |
| 20 | fixed | 2 | 59.16 | 16.72 | 16.73 | 33.44 | 1.07 | 2 | 293×230 |
| 20 | off | 2 | 59.42 | 16.72 | 16.73 | 16.73 | 0.62 | 0 | — |
| 20 | off | 3 | 59.26 | 16.73 | 16.73 | 16.73 | 0.62 | 0 | — |
| 20 | fixed | 3 | 58.99 | 16.72 | 16.73 | 33.45 | 1.36 | 2 | 293×230 |
| 20 | adaptive | 3 | 33.48 | 33.45 | 33.45 | 50.17 | 76.89 | 6 | 293×129 / 293×131 / 585×30 / 585×32 / 1170×39 / 1170×41 |
| 32 | off | 1 | 59.53 | 16.72 | 16.73 | 16.73 | 0.45 | 0 | — |
| 32 | fixed | 1 | 59.03 | 16.72 | 16.73 | 33.45 | 1.30 | 2 | 293×284 |
| 32 | adaptive | 1 | 33.40 | 33.45 | 33.45 | 50.17 | 77.52 | 6 | 293×132 / 293×134 / 585×20 / 585×22 / 1170×25 / 1170×27 |
| 32 | adaptive | 2 | 33.51 | 33.45 | 33.45 | 50.17 | 76.82 | 6 | 293×132 / 293×134 / 585×20 / 585×22 / 1170×25 / 1170×27 |
| 32 | fixed | 2 | 59.09 | 16.73 | 16.73 | 33.45 | 1.19 | 2 | 293×284 |
| 32 | off | 2 | 59.29 | 16.73 | 16.73 | 16.73 | 0.84 | 0 | — |
| 32 | off | 3 | 59.22 | 16.73 | 16.73 | 16.73 | 0.62 | 0 | — |
| 32 | fixed | 3 | 58.45 | 16.73 | 16.73 | 33.45 | 1.43 | 2 | 293×284 |
| 32 | adaptive | 3 | 33.96 | 33.45 | 33.46 | 50.18 | 74.66 | 6 | 293×132 / 293×134 / 585×20 / 585×22 / 1170×25 / 1170×27 |

| sigma | Off 평균 FPS | Fixed 평균 FPS | Adaptive 평균 FPS | Fixed/Off | 수치·경로 판정 |
| ---: | ---: | ---: | ---: | ---: | --- |
| 20 | 59.35 | 59.09 | 33.41 | 99.57% | 통과 |
| 32 | 59.34 | 58.86 | 33.63 | 99.18% | 통과 |

Fixed 6회 모두 평균 FPS ≥58, p95 ≤17.5ms, p99 ≤34.2ms, >25ms 간격 ≤2%를 만족한다. 강도별 Fixed 평균도 Off 평균의 97% 이상이다. 매 실행에서 1/4 downsample 하나와 같은 작업 해상도의 Gaussian 두 패스, `direct-backdrop` 최종 합성을 확인했다. restore와 Adaptive/Kawase 작업은 없다.

표시 FPS/간격은 GPU 실행 시간이 아니다. 종료 시 Metal 할당량은 device 전체 값으로 JSON에 남기며, 블러 peak/live VRAM이나 누수 판정으로 사용하지 않는다.

## 성능과 분리한 정지 화질

P1 수집이 모두 끝난 뒤 같은 기준 앱을 `DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC=1`로 실행해 sigma 0/1/2/4/8/20/32의 Full/Fixed iPhone 캡처 14장을 확보했다. 경로는 `temp/testing/variable-blur/fixed-adoption-device-quality/`다. 이 캡처의 시작 진단도 렌더러/terminal 오류 0을 확인했다.

별도 macOS Metal 장면은 9 논리 픽셀 글자, 1 device-pixel 선, 고주파 무늬, 저장소의 참조 사진을 사용했다. DPR 3/180 논리 단위 오버레이에 같은 7개 sigma의 Full/Fixed PNG 14장을 `temp/testing/variable-blur/fixed-adoption-quality-final/`에 저장했다. sigma 0은 샘플처럼 필터를 우회하고 두 PNG의 hash도 같다. Full Gaussian과 Fixed의 픽셀 동등성을 요구하지 않았다.

정지 캡처 검토에서 Fixed의 낮은 sigma는 원본 Gaussian보다 글자·사진 세부가 축소 입력의 영향을 받고 1px 무늬는 손실/aliasing이 크다. 실제 리스트의 부제와 둥근 색상 경계에서도 낮은 sigma의 샘플링 차이가 보인다. 끝 경계 밖의 원본과 대비되는 선명도 변화도 있다. sigma 0→1의 정지 상태 차이를 확인했지만 연속 슬라이더·스크롤 중 깜빡임/반복 윤곽과 사용자 허용 여부는 확인하지 못했다.

이는 위치/alpha 오차 GPU 회귀 실패와 구분한다. 선명한 약한 블러·끝부분이 필요한 사용처는 Adaptive를 선택하도록 설명했다. 자동 모드 전환·원본 혼합·새 커널은 추가하지 않았다. 기존 사용자의 Fixed 스크롤/화질 평가는 현재 장면의 중간 관찰로 유지하고, 새 콘텐츠·전환·10분 사용의 통과 근거로 확대하지 않는다.

## P2 후보와 P3 남은 확인

SampleApp2 `_mode`와 benchmark Off의 재활성화 초기 모드를 `fixed`로 맞췄다. Fixed 설정은 Gaussian/0.25/비 Adaptive 그대로다. 기존 모드 선택을 보존하고 Fixed의 부드러운 스크롤과 Adaptive의 선명한 끝부분을 UI·README에서 구분했다. 공개 API의 원본 해상도/Gaussian 기본값은 변경하지 않았다.

최종 후보의 Release/Mono 재빌드, 설치, 1회 Fixed/sigma 20 smoke와 benchmark 미설정 시작 결과는 아래 최종 확인 기록에 연결한다. renderer/shader/host source가 P1과 동일한 경우 18회 반복 수치는 위 P1을 채택 근거로 재사용한다.

남은 필수 확인은 전원·밝기·열 조건 기록/유지, 실제 느린/빠른/방향 전환 손가락 스크롤, 연속 sigma 변경·켜기/끄기·모드 변경·페이지 재진입, 최종 후보 10분 연속 사용과 같은 조건 Off 대조다. 사용 가능한 도구에는 physical iPhone 터치 조작이 없으며, 합성 40초 실행이나 정지 캡처를 해당 수동 검사로 대체하지 않았다.

10분 시작/종료 자원·오류 기록과 사용자 체감 허용 조건이 확보되기 전에는 P3를 완료 표시하거나 누수를 판정하지 않는다. 다른 플랫폼·120Hz·NativeAOT·전체 affine/tile/child/opacity 행렬도 미검증이다.

## 원본·재현·실패 기록

성능 원본: `temp/testing/variable-blur/fixed-adoption-sigma20/`, `fixed-adoption-sigma32/`. 빌드·회귀 로그: `temp/testing/variable-blur/fixed-adoption-validation/`. 최종 품질 명령은 아래와 같으며 성능 수집과 분리한다.

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-quality temp/testing/variable-blur/new-quality-review
```

품질 유틸리티의 첫 빌드는 obsolete SkiaSharp overload와 Path 이름 충돌로 실패했고 수정했다. 초기 사진 입력은 검증 recorder의 raster→Graphite 업로드 부재로 draw가 빠져 해당 캡처를 채택하지 않았다. 명시적 `ToTextureImage`로 수정한 최종 유틸리티에서는 사진이 포함된 14장과 오류 없는 로그를 확보했다. 이전 실패 로그·불완전 캡처는 구분해 보존했다.

기기 모든 모드의 시작 로그에는 기존 observer Dispose/background-fetch Info.plist 메시지가 남아 있다. macOS GPU 비교에는 기존 snapshot/asImage 혼용 경고가 있었으며 지정한 픽셀 비교는 통과했다. 이 메시지를 renderer/terminal 오류 0과 혼동하거나 자원 수명 문제 없음으로 해석하지 않는다.

마무리 `git diff --check` 결과와 최종 빌드 identity는 최종 확인 기록에 추가한다. 수집기는 수정하지 않았다.


## 최종 확인 기록

Fixed 기본값 후보의 iOS Release/Mono 재빌드는 **경고·오류 0**으로 완료했다(4분 40.31초).
최종 설치 payload SHA-256은 `0f52b83b1fe94698b022ddce7521e11b897b55d95c867f9492b4fa3672990313`다.
P1 대비 관련 source 변경은 `samples/DorotiSampleApp2/src/VariableBlurPage.cs` 한 파일이며 renderer/shader/host/API hash는 모두 같다.

최종 Fixed/sigma 20 1회 smoke는 **58.97 FPS / p50 16.72ms / p95 16.72ms / p99 33.44ms / >25ms 1.41%**였다.
완전한 warm 측정 이력, Gaussian 293×230 두 패스, 축소 출력의 직접 합성, 오류 0/0을 확인했고 restore Surface는 없다.
이 1회를 3회 반복 근거로 확대하지 않고, source가 같은 위 P1의 반복 결과를 유지한다.
원본은 `temp/testing/variable-blur/fixed-adoption-final-smoke/`다.

benchmark 환경변수 없이 진단/evidence만 설정해 최종 앱을 일반 실행했다.
Components 첫 화면의 실제 표시와 Graphite-Metal/renderer·terminal 오류 0을 확인했다.
이는 Variable Blur 탭 최초 진입·물리 슬라이더/스위치/라디오·탭 재진입 smoke의 완료를 뜻하지 않는다.
기기는 해당 앱을 실행한 상태로 남겼고 수동 진단은 `Documents/fixed-adoption-final-manual.json`에 기록하도록 준비했다.
초기 증거와 화면은 `temp/testing/variable-blur/fixed-adoption-validation/final-normal-initial-evidence.json` 및 `final-normal-start.png`다.

최종 CLI 버전으로 macOS GPU 회귀도 통과했다(72 Fixed 조건 최대 1/255, 기존 비교 통과).
일반 시작 evidence의 첫 copy는 CoreDevice socket 종료로 실패했으나 순차 재시도로 성공했다.
성능 수집 실패/선별 재실행과 구분한다.

**남은 게이트:** 환경 조건 통제, 실제 손가락 이동/연속 조작/화질 허용 판정, 동일 조건 Fixed/Off 10분 수동 사용과 시작/종료 자원 비교.
기본값 후보의 구현·빌드·자동 검증은 완료했고, 이 수동 게이트가 미완료인 상태를 전체 채택 완료로 보고하지 않는다.

`git diff --check`는 통과했다. 최종 fractional-clip GPU 회귀도 72조건 최대 1/255로 통과했고 상세 로그는 `gpu-final-fractional-clip.log`에 보존했다. 수집기 변경이 없어 Python 집계 검사 변경은 없으며 공통 CPU 회귀는 이미 통과했다.

최종 자동 검증 뒤 사용자가 **현재 iPhone은 차갑고 배터리는 100%**라고 보고했다. USB wired 연결과 함께 현재 상태의 관찰로 기록한다. 밝기와 앞선 측정 기간의 유지 여부, 실제 수동 스크롤·전환·10분 사용 결과는 아직 확인되지 않았다.

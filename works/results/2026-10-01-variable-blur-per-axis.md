# Variable Blur 후속 — 축별 부분 캡처

2026-10-01. 사용자가 iPhone AOT 실행에서 체감 개선이 없다고 보고하여 P1의 처리 면적 감소를 진행했다.
기존 Fast 겹침 수정은 품질 수정이며 성능 개선이 아니었다. 기존 전체 캡처 fallback을 먼저 줄인다.

## 구현

`VariableBlurCaptureBounds`는 폭·높이의 dyadic 격자 정렬 여부를 각각 판단한다.
비배수 축은 원래 도메인과 `ceil(size * scale) / size` 비율을 유지한다.
정렬 가능한 축만 기존 Gaussian 두 패스 반경과 bilinear footprint를 포함하여 원점·크기를 정렬한다.
이 규칙은 Adaptive의 1·1/2·1/4 레벨에 공통으로 적용된다.
Repeat/Mirror, 두 축 모두 비배수, 미지원 grid, perspective의 fallback은 유지한다.
진단에는 `cropped-x`/`cropped-y`를 남긴다. 공개 품질·커널·기본값은 바꾸지 않았다.

iPhone 12의 기존 장면(1170×2532, DPR 3, sigma 20, visible y=981..1520.7832)은
회귀 검사에서 1170×920, 원점 (0,792)로 캡처된다. 전체 대비 처리 면적 63.67% 감소다.
면적 감소를 GPU 실행 시간 감소와 동일시하지 않는다.

같은 바이너리의 전체 도메인 비교를 위해 `DOROTI_VARIABLE_BLUR_DISABLE_CROP=1` 진단 스위치를 추가했다.
`variable_blur_device.py --full-capture`로 실행하며, 기본값은 crop 사용이다.

## 검사

- 축별 bounds, iPhone DPR3 geometry, 모든 Adaptive 레벨의 실제 축소 비율·texel 원점 불변성을 검사한다.
- embedded production SkSL을 래스터에서 실행하여 전체 도메인과 부분 도메인을 비교했다.
  비배수 가로/세로, scale 1·1/2·1/4, Gaussian/Fast, Clamp/Decal, 방향 반전,
  DPR 3과 fractional translation, 반투명 고주파 색상 입력 48개 조건이 통과했다.
  채널별 최대 차이 2/255이며 허용 한계도 2/255다.
- 이 픽셀 검사는 개별 레벨의 downsample·두 Gaussian 패스·restore 좌표 검사다.
  실제 production Adaptive 마스크/풀 경로나 Graphite snapshot GPU 픽셀 검사를 대신하지 않는다.
- 전체 `Doroti.Tests` Release 회귀와 Python 계측 집계 검사(5개)를 통과했다.

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-capture
dotnet run --project Doroti/tests/Doroti.Tests -c Release
python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
```

## AOT 게시 시도

.NET 11 RC1 Native AOT 게시를 시도했으나 ILC `IL2037`로 실패했다.
MAUI/iOS 인터페이스 멤버 보존 해석과 `IUIKitAnimatedViewport.AnimatedViewport` 등이 오류 대상이다.
기존 `temp/testing/sample2-ios-nativeaot/publish-net10.log`에도 동일 계열 IL2037 오류가 기록되어 있다.
이번 작업에서는 게시 설정이나 MAUI 의존성을 변경하지 않았다. AOT 빌드·실행 검증은 미완료다.

```sh
# samples/DorotiTestbedApp/ios에서 .NET 11 SDK 선택
dotnet publish ../../DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net11.0-ios -p:DorotiCompilationMode=NativeAot -p:ValidateXcodeVersion=false -p:ArtifactsPath=/Users/ceramic/Labo/DorotiLab/Doroti/artifacts/sample2-ios-nativeaot
```

## 실기기 결과

iPhone 12/iOS 26.6.1, .NET 10 `net10.0-ios27.0`, Release/Mono AOT, Graphite-Metal,
DPR 3, 1170×2532, 60Hz. 실기기 빌드는 경고·오류 0이었다.
같은 바이너리에서 full은 `--full-capture`, cropped는 기본 실행으로 비교했다.
각 모드 40초 스크롤 중 warm 30초 표시 간격을 수집했다. 모드 순서는 full Fast→Fixed,
cropped Fixed→Fast다. USB 연결·진단 활성 상태이며 열·전원·밝기를 별도 통제하지 않았다.
**각 조건 1회 예비 비교이며 정식 성능 채택 기준을 충족하지 않는다.**

| 캡처 | 모드 | 캡처 px | 표시 p50 ms | 표시 p95 ms | 종료 시 Metal MiB | 렌더러/Metal 오류 |
| --- | --- | --- | --- | --- | --- | --- |
| full | fast | 1170×2532 | 33.44 | 50.15 | 298.30 | 0/0 |
| full | fixed | 1170×2532 | 33.44 | 50.16 | 187.98 | 0/0 |
| cropped | fixed | 1170×920 | 33.44 | 50.16 | 200.48 | 0/0 |
| cropped | fast | 1170×920 | 33.44 | 50.16 | 234.34 | 0/0 |

캡처 처리 면적은 실제로 줄었지만 표시 p95 개선은 관찰하지 못했다.
Fast 종료 시 Metal 할당량은 약 64MiB 감소했지만 Fixed는 약 12.5MiB 증가했다.
이 값은 앱 전체 종료 시점 값이며 peak/live 블러 자원이나 안정적인 메모리 감소 증거로 확대하지 않는다.
렌더러·Metal 오류는 네 실행 모두 0이었다.
P1 제한 구현은 처리 면적 감소를 확인했지만, 사용자 체감 성능 개선 달성으로 표시하지 않는다.
다음 분석은 아직 남아 있는 Gaussian 샘플 비용과 Adaptive 레벨 생성/합성에 집중해야 한다.
이번 결과만으로 어느 단계가 주 병목인지 확정하지 않는다.

원본:

- `temp/testing/variable-blur/per-axis-validation/`: capture/regressions/aot/ios-build 로그
- `temp/testing/variable-blur/per-axis-full/`: 전체 캡처, 2회 실행 evidence·summary·소스 diff
- `temp/testing/variable-blur/per-axis-cropped/`: 부분 캡처, 2회 실행 evidence·summary·소스 diff
- `temp/testing/variable-blur/per-axis-before/`: 변경 전 설치 앱의 예비 기록(빌드 identity 독립 검증 없음; 주 A/B 표에서 제외)

실행한 빌드와 비교 명령:

```sh
dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:ArtifactsPath=/Users/ceramic/Labo/DorotiLab/Doroti/artifacts/variable-blur-per-axis-mono
python3 Doroti/tests/variable_blur_device.py --device 00008101-001144CA3642001E --app Doroti/artifacts/variable-blur-per-axis-mono/bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app --output temp/testing/variable-blur/per-axis-full --hz 60 --repeats 1 --modes fast fixed --full-capture --conditions 'Same Release/Mono binary; USB connected; diagnostics enabled; thermal state not controlled; full-domain baseline'
python3 Doroti/tests/variable_blur_device.py --device 00008101-001144CA3642001E --output temp/testing/variable-blur/per-axis-cropped --hz 60 --repeats 1 --modes fixed fast --conditions 'Same Release/Mono binary as per-axis-full; reverse mode order; USB connected; diagnostics enabled; thermal state not controlled; per-axis crop enabled'
```

실기기에 현재 설치한 검증 앱은 Release/Mono AOT다. Native AOT 성공으로 표시하지 않는다.


## 남은 범위

AOT 실행, 전체 GPU crop/full 픽셀 비교, shear/rotation·DPR 1/2·oversized backing,
30초 warm 구간 3회 이상 교차 비교, profiler 부하 A/B와 블러 단독 GPU 시간,
10분 thermal/resource 검증과 다른 GPU backend 회귀는 아직 완료하지 않았다.
P1 전체 완료나 15% GPU p95 목표 달성으로 표시하지 않는다.

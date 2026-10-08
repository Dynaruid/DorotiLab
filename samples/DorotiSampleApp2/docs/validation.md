# 회귀 검사와 Variable Blur 성능 측정

DorotiSampleApp2의 동작 검사와 블러 프로파일링 방법입니다. 명령은 저장소 루트 `DorotiLab`에서 실행합니다.
검사 명령은 20분 제한으로 실행합니다. 결과와 산출물은 실행 환경별로 생성합니다.

## 회귀 검사

현재 CPU 회귀 진입점은 다음과 같습니다. 포인터 탭·입력·다이얼로그·viewport/DPR·리스트 수명과
캡처 정책을 확인하며, GPU Variable Blur 픽셀 비교는 포함하지 않습니다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kernel
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-capture
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-kawase
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu # macOS Metal only
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-quality temp/testing/variable-blur/quality-review # separate still-image review
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
```

## iPhone 반복 스크롤 측정

Variable Blur의 iPhone 반복 스크롤 측정은 서명된 **Release/Mono** 앱을 먼저 빌드한 뒤 실행합니다.
`--app`은 빌드 산출물 경로, `--device`는 연결된 실기기 식별자입니다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Release -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:EnableCodeSigning=true '-p:CodesignKey=Apple Development' -p:CodesignProvision=<PROFILE_UUID>
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app samples/DorotiSampleApp2/ios/bin/ios-arm64/Release/net10.0-ios27.0/ios-arm64/DorotiSampleApp2.iOS.app --output temp/testing/variable-blur/new-run-sigma20 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 20 --conditions '전원·밝기·온도 조건 기록'
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app samples/DorotiSampleApp2/ios/bin/ios-arm64/Release/net10.0-ios27.0/ios-arm64/DorotiSampleApp2.iOS.app --output temp/testing/variable-blur/new-run-sigma32 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 32 --conditions '동일 전원·밝기·온도 조건 기록'
```

위 명령은 같은 바이너리에서 Off/Fixed/Adaptive, sigma 20/32를 각각 3회씩 총 18회 비교하고
두 번째 반복은 역순으로 진행합니다. 수집기에서 `--modes`를 생략하면 기존 6개 모드(Off/Full/Adaptive/Fast/Fixed/Kawase)를 각각 3회 실행합니다.
`--hz`에는 실제 설정된 표시 주사율을 입력합니다. 각 실행은 40초이며 최초 표시 이후
5~35초 구간의 실제 drawable 표시 간격을 집계합니다. 합성 스크롤이므로 물리 입력 검증은 아닙니다.
완료 표식·30초 표시 이력·렌더러 오류 여부를 검사하며 불완전한 실행을 PASS로 집계하지 않습니다.
앱 설치는 기존 `dev.doroti.sample2`를 업데이트하며 실행마다 해당 앱을 재시작합니다.

## 수동 프로파일링과 진단

수동 프로파일링에서는 `DOROTI_VARIABLE_BLUR_BENCHMARK=off|full|adaptive|fast|fixed|kawase`로
탭·모드·반복 경로를 선택합니다. 미설정 시 일반 UI/Fast adaptive 기본값을 사용합니다.
Off로 benchmark를 시작한 뒤 블러를 다시 켜도 초기 선택은 Fast adaptive입니다.
정지 화면 비교에는 `DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC=1`을 함께 지정하고,
강도는 `DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA=0..32`로 지정할 수 있습니다.
정지 모드는 자동 스크롤 수집기와 함께 사용하지 않습니다.
`--variable-blur-quality`는 macOS Metal에서 DPR 3, sigma 0/1/2/4/8/20/32의 Full/Fixed PNG 14장을
별도 저장합니다. 작은 글자·1px 선·고주파 무늬·참조 사진의 축소 손실을 검토하는 장면이며,
Full과 Fixed의 픽셀 동등성이나 iPhone의 움직임 화질 검증을 뜻하지 않습니다.
`DOROTI_VARIABLE_BLUR_PROFILE=1`과 `DOROTI_MAUI_EVIDENCE=blur.json`을 함께 설정하면
iOS 앱의 Documents에 진단 JSON을 기록합니다.

Dual Kawase는 공유 down 체인에서 강도별 결과를 up 재구성하고, device sigma에 맞춰 분산을 보간하는
명시적 근사 모드입니다. 두 device pixel 이하에서는 Gaussian을 사용해 선명한 끝부분을 유지합니다.
Clamp·회전/반사/균일 scale·최대 약 124 device sigma를 지원하며 다른 설정은 Gaussian으로 대체합니다.
`VariableBlurKernel.dualKawase`는 자체 해상도 피라미드를 사용하므로 `resolutionScale`과 독립적입니다.
공개 `ImageFilter.variableBlur`와 `ImageFilterConfig.CreateVariableBlur` 및 샘플의 초기 선택은
**Fast adaptive**입니다. 기본 조합은 `resolutionScale: 0.25`, `adaptiveResolution: true`,
`kernel: VariableBlurKernel.fastGaussian`입니다. 약한 구간의 원본 해상도와 선명한 끝부분을 보존합니다.
원본 해상도 Gaussian은 `resolutionScale: 1, kernel: VariableBlurKernel.gaussian`으로 명시할 수 있습니다.
Fixed와 Dual Kawase도 개별 옵션으로 선택할 수 있습니다.

`--intermediate`는 Adaptive의 중간 출력 합성을, `--full-capture`는 전체 캡처를 강제하는 같은 바이너리 A/B 옵션입니다.
`--owned-subtrees`는 셰이더 없는 형제 scope도 별도 Surface로 처리하며,
`--full-stages`는 Kawase 강도 결과를 전체 캡처에 재구성합니다. 기본 실행은 native 형제 scope와 필요한 띠만 사용합니다.
`--full-bands` / `--full-detail`은 Gaussian 띠·선명한 구간의 기존 전체 크기 backing을 유지하는 A/B 옵션입니다.
iOS는 새 shader-only 장면에 최대 2개 GPU frame과 비동기 표시를 사용하는 C 경로가 기본입니다.
native·회전·replay는 직렬 admission과 필요한 transaction 표시를 유지합니다.
iOS 수집기는 프레임 정책 선택 옵션 없이 C를 측정합니다.
`--sigma 32`로 최대 강도를 측정할 수 있습니다.
수집기의 평균 FPS는 warm 구간의 실제 표시 간격으로 계산하며 CPU stage나 GPU 실행 시간에서 추정하지 않습니다.

진단에는 캡처 fallback 사유, 작업 해상도·draw bounds, Surface pool hit/miss/temporary,
CPU stage p50/p95/p99, 실제 표시 간격과 Metal 할당량을 포함합니다.
CPU stage는 최대 최근 4,096회 호출이며 초기 준비를 포함하고, `filter-total`은 내부 stage와 중첩됩니다.
Surface 정보는 마지막 프레임의 scene filter 전체를 포함합니다. RGBA 추정 바이트를 합산해 live/peak VRAM으로
해석하지 않습니다. UIKit command buffer 카운터는 terminal marker만 세며 Skia 내부 제출 횟수는 아닙니다.
GPU 구간 시간과 peak 메모리는 별도 Metal System Trace가 필요합니다.

부분 캡처는 축소 격자가 정렬 가능한 축에만 적용합니다.
`--variable-blur-capture`는 실제 embedded SkSL의 1·1/2·1/4 레벨을 래스터에서 실행하여
전체 도메인과 부분 캡처의 픽셀을 비교합니다. Graphite GPU 픽셀 비교나 Adaptive 마스크 검증을 대신하지 않습니다.
실기기 측정 스크립트의 `--full-capture` 옵션은 `DOROTI_VARIABLE_BLUR_DISABLE_CROP=1`로
같은 바이너리의 전체 도메인 기준을 실행합니다. 기본 실행과 각각 새 출력 폴더로 비교하세요.

## 텍스트 입력 커서 검사

개행·빈 줄·자동 줄바꿈의 커서 좌표, 후속 IME 입력과 플랫폼별 선택 정책을 확인합니다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.Cupertino/tests/Doroti.Cupertino.Tests -c Release -- --text-input-tap-cursor
```

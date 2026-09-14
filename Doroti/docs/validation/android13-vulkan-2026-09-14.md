# Android 13 Vulkan 실행 수정 — 2026-09-14

Android 13 x64 AVD `19_12T` (`emulator-5554`)에서 DorotiTestbedApp의 검은 화면을 수정했다.
공식 SkiaSharp Graphite/Vulkan으로 갤러리와 Material 샘플을 표시하고 회전·복귀·탭·스크롤을 확인했다.
이 결과의 범위는 해당 에뮬레이터의 자동 검증이다.

## 원인과 변경

- 장치는 `NVIDIA GeForce RTX 4060 Laptop GPU`, Vulkan `1.3.0`을 보고하지만
  `vkGetDeviceProcAddr`에서 RenderPass2 core/KHR 함수 8개를 모두 반환하지 않았다.
  `VK_KHR_create_renderpass2`도 확장 목록에 없었다.
- 인스턴스 조회는 주소를 반환했으나, 별도 네이티브 프로세스에서 실제
  `vkCreateRenderPass2`를 호출하면 null dispatch로 SIGSEGV가 발생했다.
  따라서 인스턴스 함수 포인터로 대체하지 않는다.
- `GraphiteVulkanWindow`는 Android에서 실제 observer dispatch로
  `vkCreateRenderPass2` 가용성을 확인한다. 활성화된 KHR 확장 alias도 사용할 수 없으면
  Skia의 공개 `MaxApiVersion`을 1.1로 제한하여 기존 Vulkan render pass 경로를 사용한다.
  물리 장치의 보고 값이나 Vulkan 함수를 위조하지 않는다.
- API 상한은 인스턴스의 요청 버전 1.2 이하이며, 이 변경은 장치 선택 최소 조건을
  완화하지 않는다. RenderPass2를 사용할 수 있는 Android와 다른 플랫폼의 1.2 경로는 유지된다.
  1.1 경로의 필수 함수는 Skia 컨텍스트 생성 과정에서 검증된다.
- Material 입력 검증은 탭 개수에 의존하지 않도록 하고 화면 크기를 기준으로 스와이프한다.
  태블릿의 오른쪽 navigation 예제가 아닌 가운데의 주 스크롤 영역을 사용한다.

Skia는 API 상한과 보고된 버전 중 작은 값으로 인터페이스를 구성하고 Vulkan 1.1을
최소로 요구한다. [Skia MakeInterface 소스](https://github.com/google/skia/blob/main/src/gpu/vk/VulkanUtilsPriv.cpp).
RenderPass2는 선택된 Vulkan 1.2 이상 또는 활성화된 확장에 따라 로드한다.
[Skia VulkanInterface 소스](https://github.com/google/skia/blob/main/src/gpu/vk/VulkanInterface.cpp).
실제 호환성은 설치된 공식 `4.154.0-preview.1.26454.9` APK로 확인했다.

## 검증 결과

생성 결과: `Doroti/artifacts/validation/android-emulator-launch-20260914/`.

| 항목 | 결과 | 증거 |
| --- | --- | --- |
| Release android-x64 빌드 | PASS, 경고 0 / 오류 0 | `build-fixed.log` |
| 공식 x86_64 네이티브 자산 및 GPU 초기화 | PASS | `fixed-process.log`, `fixed.png` |
| 회전 및 복귀 2회 | PASS-scoped-device-lifecycle | `lifecycle/result.json` |
| GPU 정리 | 제출/완료 19/19 및 1/1, 미완료 0 | `lifecycle/process.log` |
| Color / Components 탭 전환, 스크롤 | PASS-automated-material-input | `material-input-retry/result.json`, 해당 PNG/XML |
| 갤러리 버튼 | 카운트 1 재표시 확인 | `input/button.xml`, `input/button.png` |
| 기존 영문 입력 assertion | FAIL: 한글 키보드가 `work0`를 `재가0`로 입력 | `input/text-keyboard.png`, `input/text-keyboard.xml` |

최초 Material 스와이프는 오른쪽 navigation 예제에서 실행되어 내용 변화 assertion이 실패했다.
주 스크롤 영역으로 조정 후 통과했다. 이전 실패 캡처는 `material-input/`에 보존했다.
수정된 앱 프로세스에서는 렌더러 오류·앱 크래시를 관찰하지 않았다.
앞선 네이티브 dispatch 진단 프로세스의 SIGSEGV는 앱 크래시와 별개다.
물리 입력, 전체 한글 IME, 성능, Vulkan synchronization validation, 장치 손실,
다른 Android 13 GPU 및 arm64 실기기는 이 실행으로 검증하지 않았다.

## 재실행

저장소 루트 PowerShell에서 빌드하고 생성 APK를 직접 설치한다.
`dotnet run`의 자동 배포에서 관찰한 `DOTNET_HOST_PATH` 누락을 피하도록 경로를 명시한다.

```powershell
$env:DOTNET_HOST_PATH = (Get-Command dotnet).Source
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64
python Doroti/validation/run-with-timeout.py adb -s emulator-5554 install -r --user 0 DorotiTestbedApp/android/bin/android-x64/Release/net10.0-android/android-x64/dev.doroti.testbed-Signed.apk
adb -s emulator-5554 shell monkey -p dev.doroti.testbed -c android.intent.category.LAUNCHER 1
```

`validation/stock-graphite-vulkan/android-dispatch-probe.c`는 NDK로 빌드하는 독립 진단 소스다.
기본 실행은 확장 및 함수 주소만 조회한다. `--invoke-instance-renderpass2`는 해당 로더의
잘못된 trampoline 호출을 재현하는 명시적 진단 옵션이며 이 AVD에서는 SIGSEGV를 발생시킨다.
실행 파일과 로그는 `artifacts/validation` 아래에 둔다.

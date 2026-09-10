# Android 스크롤 후속 개선 — 2026-09-10

이전 수정 후에도 FPS가 낮다는 사용자 보고에 따라 GPU 제출/완료 대기, shader 이미지 캐시, 접근성 이후 Android measure/layout을 분리해서 조사했다. **주요 병목은 UI 스레드의 GPU fence 대기였다.** 이를 최대 2프레임 파이프라인으로 바꾸고 움직이는 shader 캐시의 반복 생성을 줄였다. Galaxy에 실제 설치한 최종 APK hash는 [installed-apk.json](../../../history/26-09-10/android-scroll-followup/installed-apk.json)에 기록했다.

## 원인과 변경

1. **GPU 완료까지 UI 스레드가 정지:** 계측한 평균 copy-fence 구간이 7.749ms였다. Android WSI에 독립 backing/Graphite target/command/fence/acquire semaphore를 가진 슬롯 두 개를 두었다. acquire는 timeout 0, GPU copy는 acquire semaphore를 기다리며, 제출 후 UI 스레드는 즉시 반환한다. 슬롯 재사용은 fence를 비차단 polling하여 실제 GPU 완료를 확인한 뒤 허용한다. 이미지 layout 전환까지 acquire 대기 범위에 포함한다. 진행 중 프레임은 두 개로 제한한다.
2. **캐시를 계속 만들고 버림:** 소수점 스크롤 위치가 달라져 무효화된 shader output을 매 프레임 다시 생성했다. 한 번 무효화된 후보는 동일 transform/크기/content generation이 3프레임 유지될 때 재승격한다. 그동안 full child texture 크기·원점과 shader 좌표는 유지하면서 output image를 거치지 않고 그린다. 후보 수는 64개로 제한하며 context 해제 시 함께 반환한다. 첫 사용 캐시와 안정된 이미지의 재사용은 유지한다.
3. **접근성은 주된 지속 병목이라는 근거가 없음:** native atrace에서 시작 후 10초를 제외한 스크롤 구간의 measure/layout은 각각 6회, 총 18.552ms였다. UI animation 구간 중앙값은 16.795ms였다. 접근성 초기 생성은 약 61ms spike가 있었으나 매 프레임 발생하지 않았다. 접근성 컨트롤·서비스를 끄거나 semantics 발행/입력 동작을 축소하지 않았다.

`DorotiAndroidVulkanView`는 idle에서도 마지막 GPU 프레임을 반환하는 별도 polling callback을 사용한다. 이 callback은 화면을 다시 그리지 않는다. SurfaceDestroyed/resize/종료에서는 drain 후 target·semaphore·native window를 해제한다. 취소/실패하여 copy를 제출하지 않은 acquire signal도 빈 GPU submit으로 소비한 뒤 반환한다. 실제 `VK_ERROR_DEVICE_LOST`는 Graphite에 전달한다. 비차단 프레임에도 기존 5초 완료 timeout을 유지한다.

파이프라인은 Android에서 선택한다. idle completion callback을 아직 갖추지 않은 Qt는 동기 완료 계약을 유지하고, Windows D3D12 import 경로도 기존 방식으로 동작한다. [Khronos의 swapchain semaphore 재사용 지침](https://docs.vulkan.org/guide/latest/swapchain_semaphore_reuse.html)과 [acquire timeout 계약](https://docs.vulkan.org/refpages/latest/refpages/source/vkAcquireNextImageKHR.html)을 확인했다. swapchain 이미지별 present semaphore와 GPU 완료 후 texture state 갱신을 유지한다.

## 같은 휴대폰에서 비교

Galaxy S25 SM-S931N, API 36, Adreno 830, 1080×2340, SurfaceFlinger 주기 약 8.333ms. 동일 Gallery에서 바깥 스크롤을 1,500ms씩 왕복했다. 기기의 접근성 서비스 설정은 유지했다. 기준 APK는 **직전 개선 코드에 동일한 phase 계측만 추가한 빌드**이며 현재 소스와 관계없는 오래된 설치본이 아니다. APK와 초기 계측 실행을 별도 보존했다.

새 `measure-continuous.py`는 swipe 진행 중 약 200ms마다 SurfaceFlinger를 조회한다. 이전처럼 swipe 종료 후 한 번만 읽으면 120Hz에서 128프레임 ring buffer가 잘려 마지막 구간만 남기 때문이다. 아래는 양쪽 모두 초기 두 swipe를 제외한 네 swipe의 새 표시 timestamp 간격이다.

| 지표 | 기준 `continuous-before` | 최종 `verified-after` |
| --- | ---: | ---: |
| 표시 간격 표본 | 384 | 899 |
| 중앙값 | 16.664ms | **8.331ms** |
| p95 | 33.327ms | **8.333ms** |
| 최대 | 124.963ms | 91.625ms |
| 12.5ms 초과 간격 | 350 / 384 | 21 / 899 |
| 전체 실행 frame receipt | 641 | 1,441 |
| shader용 GPU 표면 생성 | 323 | **23** |
| 접근성 active control / 적용 / 속성 쓰기 | 26 / 7 / 365 | **26 / 7 / 365** |

첫 60프레임 계측 블록을 제외한 CPU phase 평균은 그리기 callback(프레임워크 처리 포함) **9.067→4.749ms**, 제출 **1.568→1.032ms**, copy fence 구간 **7.749→0.001ms**, 전체 Render **18.668→6.055ms**였다. 새 fence 구간은 소유권 기록 비용이며 GPU 실행 시간이 0이라는 뜻이 아니다. 실제 완료는 후속 poll에서 확인한다. 진행 중 프레임 최대 2, 이 측정의 busy/acquire-unavailable은 0이다.

파이프라인만 적용한 중간 APK에서도 주로 8.3ms 주기를 확인했다. 최종 shader 변경의 추가 근거는 GPU 표면 생성 감소와 픽셀 비교이며, 두 변경 각각의 FPS 기여도를 독립적인 무작위 실험으로 추정한 것은 아니다.

## 검증과 한계

- 최종 Android arm64 Release/AOT canonical build: **PASS**, 경고·오류 0. 설치한 `base.apk` SHA-256이 빌드한 서명 APK와 일치한다.
- FCR-7 전체, FCR-5, runtime shader, MAUI semantics projection/action 계약: **PASS**.
- GPU compositing **moving/phase/blend/owners/eviction/exception/huge** 7종: **PASS**. 새 moving 검사는 fractional translation, viewport clipping, 반투명 색상, 색 변환 shader, 안정 후 캐시 재사용을 20프레임 검사하며 기존 cached-output 경로와 모든 픽셀이 일치한다. 움직이는 동안 매 프레임 output surface를 생성하지 않는지도 검사한다.
- Windows NVIDIA/AMD × 3개 크기 D3D12 import: **72개 GPU 픽셀 프레임과 취소 검사 PASS**. Android Vulkan validation-layer 증거로 간주하지 않는다.
- 최종 비교 실행의 failed/dropped는 모두 0이다. 이 값은 물리 scan-out 누락이나 vsync miss 수가 아닌 앱 frame transaction 진단이다.
- 실제 중첩 목록의 항목 이동·부모 영역 유지와 shader/blur, 키보드 열기·닫기를 스크린샷으로 확인했다. 실제 SurfaceDestroyed를 기다린 최종 smoke에서 **화면 파괴·재생성 6회**(스크롤 중 홈 이동 4회 포함) PASS. 각 generation의 제출/반환은 **340/340, 1/1, 111/111, 87/87, 84/84, 83/83**, 잔류 frame 모두 0이다.
- 최종 smoke의 frame receipt 961, replay 36, failed/dropped/superseded/software fallback 0, 마지막 입력/표시 sequence 535/535 일치. `smoke-verified/result.json`에 기록했다. 검증 후 최종 APK를 진단 intent 없이 다시 열어 두었다.

장시간 발열·전력/배터리, 다른 Android 기기, 모든 화면/물리 터치의 일정한 120fps, 실제 device loss, Android Vulkan validation layer 및 다른 OS 실행은 이번 측정으로 입증하지 않았다. 초기 GPU shader 준비와 간헐적 긴 간격은 여전히 존재한다. 접근성 초기 생성 spike도 남아 있다. 이전 [1차 개선 보고서](android-scroll-2026-09-10.md)와 과거 Graphite gate를 완료로 바꾸지 않는다.

## 재현과 실패 보존

[실행 자료](../../../history/26-09-10/android-scroll-followup/)에 phase log, 원시 atrace 압축본/요약, continuous latency 압축본, framework evidence, 스크린샷, APK hash와 `checks.json`을 보관한다. 테스트 subprocess에는 각각 **1,200초 timeout**을 적용했다.

```powershell
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64
adb -s R3CY30KZA4B install -r --user 0 DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk
python history/26-09-10/android-scroll-followup/measure-continuous.py new-measurement --evidence
python history/26-09-10/android-scroll-followup/run-checks.py
```

출력 label은 새 이름을 사용한다. 비교용 APK는 버전 관리 외 `Doroti/artifacts/android-scroll-followup/`에 보관했다. 최초 pipeline build의 nullable `CS8602`와 수정 후 성공을 `pipeline-build.log`/`pipeline-build-retry.log`로 구분했다. 처음 두 smoke는 6회 파괴 로그를 기대했으나 2회만 관측해 실패했다. 처음에는 진단 intent 누락을 의심했지만, intent를 유지한 재실행과 Android native log를 통해 **빠른 홈→복귀 4회에서 SurfaceDestroyed 자체가 발생하지 않았음**을 확인했다. 최종 fixture는 실제 파괴/반환 이벤트를 기다린 후 복귀한다. 첫 가정과 두 실패를 `smoke-first-result.json`, `smoke-second-result.json`, 원본 스크립트와 `smoke/`, `smoke-retry/`에 보존한다. 기존 단발 latency 결과도 삭제하지 않았고 최종 수치에는 continuous 수집만 사용한다.

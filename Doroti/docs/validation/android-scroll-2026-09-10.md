# Android 스크롤 지연 개선 — 2026-09-10

사용자가 휴대폰 스크롤의 버벅임을 보고하여 현재 Graphite/Vulkan 제품 경로를 검토하고 수정했다. `work.md`의 전 플랫폼 전환 계획을 새로 실행하라는 요청으로 해석하지 않았다. 작업 전부터 존재하던 Graphite 변경과 과거 검증 기록을 유지했다.

## 변경

- `MauiHostAdapter.HandleAndroidFrame`은 이미 Choreographer의 현재 프레임 콜백 안에서 실행된다. 그런데 Graphite 뷰가 `PostOnAnimation`으로 다음 프레임을 다시 기다렸다. `IMauiSkiaSurface.InvalidateSurfaceFromVsync`와 Android `DrawFromVsync`를 연결해 현재 콜백에서 그리도록 했다. 일반 invalidation은 기존 예약·병합을 유지하고, 현재 콜백에서 소비한 예약은 취소한다. GL 비교 경로와 Apple 경로의 갱신 방식은 유지된다.
- `GraphiteVulkanWindow.Render`의 매 프레임 `vkQueueWaitIdle`을 제거했다. presentation semaphore를 swapchain 이미지마다 보유하고, 같은 이미지의 acquire fence가 완료된 뒤 재사용한다. 이는 [Khronos의 swapchain semaphore 재사용 지침](https://docs.vulkan.org/guide/latest/swapchain_semaphore_reuse.html)에 따른다. GPU copy fence, Graphite frame 반환, texture state 갱신, resize/종료 drain은 유지한다.
- 렌더링 API 기본값, 스크롤 물리·위젯, shader/blur, 입력·semantics 정책은 변경하지 않았다.

## 휴대폰 비교

연결된 Galaxy S25 **SM-S931N / Adreno 830 / API 36**, 1080×2340, SurfaceFlinger 보고 주기 8.333ms에서 실행했다. 기존 설치본과 이번 Release/AOT APK의 Material Gallery에서 오른쪽 바깥 스크롤 영역을 1,500ms씩 왕복했다. 초기 두 swipe를 제외한 동일한 네 swipe의 SurfaceFlinger 실제 표시 timestamp 간격을 집계했다.

| 실행 | 간격 표본 수 | 중앙값 | p95 | 최대 |
| --- | ---: | ---: | ---: | ---: |
| 변경 전 휴대폰 설치본 | 242 | 24.995ms | 49.974ms | 149.932ms |
| 수정본, 진단 기록 끔 | 344 | 16.664ms | 33.327ms | 224.930ms |
| 수정본, 진단 기록 켬 | 355 | 16.663ms | 33.330ms | 224.931ms |

중앙값과 p95는 이 비교에서 약 33% 감소했다. **최대 간격은 개선되지 않았으며, 120fps 유지나 모든 버벅임 해결을 의미하지 않는다.** 측정에는 gesture 종료/잔여 애니메이션/idle 경계가 포함되어 있으므로 최대 간격을 순수 raster 시간이나 입력 지연으로 해석할 수 없다. 같은 소스의 두 옵션을 무작위 교차한 실험이 아니라 기존 설치본과 현재 수정 APK 비교다. 다른 선행 미배포 변경까지 완전히 통제한 인과 추정으로 취급하지 않는다.

초기 시도의 SurfaceFlinger 수집 시작 지연/이전 buffer 포함과 `/proc/uptime`의 suspend 포함 clock 차이를 발견했다. `installed-before` 및 `installed-before-repeat` 원시 결과를 보존하되 최종 집계에서는 제외했다. 최종 스크립트는 수집 전 마지막 유효 present timestamp보다 새로운 timestamp만 사용하며 초기 두 swipe는 집계에서 제외한다. 진단 옵션이 꺼진 실행에서 읽힌 예전 기기 cache JSON도 현재 증거에서 제외하고 파일 hash와 이유만 `stale-evidence-excluded.json`에 남겼다.

## 검증

모든 build/test subprocess에 **1,200초 timeout**을 적용했다.

- canonical Android arm64 Release/AOT build: **PASS**, 경고 0·오류 0. 서명 APK 설치 성공. APK SHA-256은 `apk.json`에 기록했다.
- FCR-5 스크롤 계약 Release: **PASS**.
- FCR-7 Material/widget 전체 Release: **PASS**.
- 공통 Vulkan의 Windows D3D12 interop 회귀: NVIDIA/AMD × 3개 크기, **72개 프레임 픽셀·취소 검사 PASS**. 이것은 Android WSI validation-layer 검사가 아니다.
- 실제 Android 중첩 스크롤: 첫 가시 항목 **Lazy item 1 → 5**, 부모 영역 유지 확인. shader/blur 출력 유지.
- 홈 화면 → 앱 복귀 **2회**, 키보드 열기/닫기: **PASS**, 재생성 후 화면 출력과 프로세스 생존 확인. 한글 조합·TalkBack 전체 검증은 수행하지 않았다.
- 진단 실행부터 후속 smoke까지 앱 frame receipt **892**, replay **27**, failed/dropped/superseded **0**, software fallback **0**. 마지막 입력 sequence **605**와 표시 sequence **605** 일치. 이 카운터는 실제 scan-out 또는 누락 vsync 수가 아니다.
- 최종 앱은 진단 intent 없이 다시 실행해 휴대폰에 열어 두었다.

남은 한계: Gallery의 shader/blur 포함 raster 및 GPU copy fence 대기는 여전히 UI 스레드에서 실행된다. 일부 긴 간격이 남아 있으므로 사용자 손가락의 체감, 장시간 발열/전력, 다른 화면·기기, Android Vulkan validation layer, Linux WSI 실행은 `notVerified`다. 이번 개선을 전 플랫폼 Graphite 수용 gate 완료로 바꾸지 않는다.

## 증거와 재현

[실행 자료 폴더](../../../history/26-09-10/android-scroll/)에 `build.log`, `fcr5.log`, `fcr7.log`, `d3d12.json`, `comparison.json`, `apk.json`, 각 실행의 raw latency·logcat·스크린샷을 보관한다. 최신 framework trace는 `fixed-diagnostics/evidence.json.gz`와 `smoke/evidence.json.gz`에 손실 없이 압축했다.

```powershell
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64
adb -s R3CY30KZA4B install -r --user 0 DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk
python history/26-09-10/android-scroll/measure.py new-run
python history/26-09-10/android-scroll/measure.py new-diagnostics --evidence
# smoke는 위 측정의 마지막 Gallery 위치에서 실행한다.
python history/26-09-10/android-scroll/smoke.py

# 진단 없이 다시 열기
adb -s R3CY30KZA4B shell am force-stop dev.doroti.testbed
adb -s R3CY30KZA4B shell am start -n dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity
```

측정 스크립트는 앱을 재시작한다. 기존 evidence를 덮지 않도록 실행 label을 새 이름으로 지정한다. 일반 build 명령에도 이 세션과 동일하게 실행 관리자의 20분 timeout을 적용한다.

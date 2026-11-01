# Android 백그라운드 복귀 시 GPU 캐시 재생성 수정

작성일: 2026-11-01 (Asia/Seoul)  
기준 커밋: `38e1603e3419c20085666de6fb550ea9669de938`  
상태: **수정·에뮬레이터 생명주기 검증 PASS / 실기기 체감·전체 성능 notVerified**

집계·소스/설치 APK hash: [검증 결과 JSON](android-resume-scroll-results.json).
기존 [갤럭시 스크롤 작업](../../../../work.md)의 전체 성능 `PARTIAL` 판정은 유지한다.

## 확인한 문제

기존 `SurfaceDestroyed`는 `GraphiteVulkanWindow` 전체를 종료했다. Home에서 돌아오면
새 Vulkan device·Graphite context·recorder를 만들므로 이전 스크롤에서 준비한 GPU 자원과
파이프라인도 다시 준비했다. 같은 프로세스에서 복귀 두 번만으로 context 생성이 세 번 발생했다.
캐시가 누적됐다는 추측과 별도로, 이 불필요한 재생성 경로를 소스와 실행 기록에서 확인했다.

오래 기다리는 대신 foreground full GC, Home 후 `BACKGROUND` trim 및 full GC를 명시적으로
유도했다. 관리 힙은 회수됐고 GC만으로 지속적인 스크롤 저하가 재현되지는 않았다.
따라서 모든 메모리 누수나 갤럭시 체감 원인을 확정한 결과로 확대하지 않는다.

여러 Components 구간 방문 후 복귀하는 진단에서는 첫 draw owner가 925–957ms,
그중 paint가 665–674ms인 경우도 있었다. Android task snapshot이 남아 DOWN을 막아
그 회차는 스크롤 비교에서 제외했다. 하네스는 고정 1초 대기 대신 snapshot 제거를 확인하고,
실제 DOWN/UP 및 제출 frame이 모두 관측된 회차만 통과시킨다.

## 변경

- [Android Vulkan 수명](../../../../Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.Android.cs)을
  Surface 교체와 context 최종 종료로 분리했다. Surface 파괴 시 draw/poll admission을 닫고
  GPU 작업과 미소비 acquire semaphore를 worker에서 drain한다. 정확한 task가 성공한 뒤
  원래 UI owner에서 recording을 회수하고 swapchain·출력 target·VkSurface·ANativeWindow를 해제한다.
- 살아 있는 view는 device·recorder와 기존 제한을 가진 캐시를 유지한다. 복귀 시 새 Surface를
  같은 device에 연결하고 present 지원을 확인한다. 크기가 같은 경우 host GPU 캐시를 유지하며,
  크기가 바뀌면 기존 무효화 경로를 따른다. Surface가 없는 동안 렌더링을 허용하지 않는다.
- 최종 `RetireAsync`/Disconnect가 진행 중인 Surface drain과 겹치면 두 종료를 순서대로 join한다.
  renderer 자원 해제는 정확한 기존 owner에 연결했다. timeout만으로 device loss로 판정하거나
  새 세대를 허용하지 않는다. 실제 device-loss 결과는 owner에서 수락한 뒤 완전 종료로 전환한다.
- Android 메모리 압박은 `FreeGpuResources()`로 사용하지 않는 context 자원을 정리한다.
  drain 중 요청은 완료 후 처리한다. 정상 실행에는 강제 GC를 추가하지 않았다.
- [재현 하네스](../../../../Doroti/eng/measure-android-resume-scroll.py)와 opt-in GC·Activity
  재생성/종료 진단을 추가했다. `DOROTI_SCROLL_PROFILE=1`로 시작한 프로세스에서만 강제 GC와
  lifecycle 진단이 동작한다. shell용 `ScrollGesture`는 Android 13의 InputManager도 지원한다.

[Android의 trim 안내](https://developer.android.com/reference/android/content/ComponentCallbacks2)와
[Graphite 자원 정리 구현](https://github.com/google/skia/blob/main/src/gpu/graphite/Context.cpp)을
참고했다. 실제 검증은 아래 로컬 최종 APK로 수행했으며 문서 설명만으로 수락하지 않았다.

## 검증 결과

Pixel 5 x86_64 에뮬레이터, Android 13/API 33, 1080×2340,
NVIDIA RTX 4060 Laptop GPU, 공식 SkiaSharp 4.154.0을 사용했다.
Release Mono/profiled AOT, interpreter off, Optimize on이다. arm64도 같은 빌드 프로필이다.

- 최종 APK 3회 실행에서 foreground GC 3회 및 Home/trim/GC 후 복귀 총 6회 통과.
  각 실행의 context 생성 1회, Surface detach/reattach 각 2회, unused GPU purge 최소 2회.
  각 detach의 outstanding GPU frame은 0개였다. swapchain은 정상적으로 새 세대를 만들었다.
- 명시 `Recreate()`로 같은 PID의 Activity 교체를 확인했다. 교체 전후 42개 앱 semantics 노드의
  텍스트·종류·좌표가 일치했다. 이전 context 종료와 마지막 `Finish()`의 GPU 완전 종료도
  outstanding=0으로 확인했다. fatal/render/retirement 오류는 없었다.
- 진단 옵션 없이 새 프로세스로 일반 실행한 뒤 Home/trim/복귀와 실제 shell touch 스크롤을
  추가 확인했다. context는 한 개였고 화면이 이동했다. profiler 없이 보낸 GC intent는 무시됐으며
  `debug.mono.log`는 원래 빈 값으로 복구됐다. 에뮬레이터에는 일반 실행 상태를 남겼다.
- 기존 `Doroti.Scroll.Tests`의 ticker·관성·시각·semantics·caret·공유 컨트롤러·약한 알림·native
  입력 수명 회귀가 모두 PASS였다. 모든 빌드와 검증 하네스에 20분 timeout을 적용했다.
- 최종 x64 Release: 0 warning / 0 error. arm64 Release: 1 warning / 0 error.
  arm64의 경고는 기존 Gradle 도구와 SDK XML 버전 차이이며 빌드는 성공했다.

첫 최종 2회는 arm64 빌드와 겹친 기능 진단이다. 아래 표는 빌드가 모두 끝난 뒤 별도로 실행한
`verified-quiet` 한 회차이며, DOWN부터 UP까지 **실제 제출된 draw의 CPU owner 시간**만 집계했다.

| 구간 | 제출 draw 수 | p95 ms | max ms | 50ms 이상 |
| --- | ---: | ---: | ---: | ---: |
| 첫 세션 | 157 | 11.310 | 16.351 | 0 |
| foreground full GC 후 | 156 | 11.544 | 19.368 | 0 |
| Home/trim/full GC 후 첫 복귀 | 158 | 10.046 | 14.144 | 0 |
| 같은 정리 후 두 번째 복귀 | 157 | 13.229 | 20.916 | 0 |

별도 회차의 background full GC 후 관리 힙은 38,538,464 → 38,593,680 bytes였다.
이는 확인한 두 복귀에서의 회수·안정성 근거이며 장기 누수 부재 증명은 아니다.
빠른 setup 플링의 정확한 scroll offset과 방문 범위는 회차 사이에 고정되지 않았다.
그러므로 이 기록을 엄밀한 수정 전후 성능 배율 비교로 사용하지 않는다.

복귀 후 첫 swapchain 준비에는 별도 회차에서도 102–111ms draw가 남았다.
snapshot 제거 관측까지는 약 556–569ms이며 OS 전환 및 polling 비용을 포함한다.
이 값을 표시 FPS, 손가락부터 표시까지의 지연, 전체 deadline 합격으로 해석하지 않는다.
실기기 Galaxy/Adreno·손가락 체감·표시 deadline·장시간 메모리·의도적인 device-loss/정지 fault
주입은 미검증이다.

## 재현과 전달

저장소 루트 PowerShell에서 실행한다. `gesture.jar`는 shell 전용이며 앱에 패키징하지 않는다.
실기기에서는 해당 serial과 `android-arm64`를 사용하고 에뮬레이터 APK를 설치하지 않는다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64 -m:1 -nr:false -v:minimal
& "$env:LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe" -s emulator-5554 install -r samples/DorotiTestbedApp/android/bin/android-x64/Release/net10.0-android/android-x64/dev.doroti.testbed-Signed.apk

New-Item -ItemType Directory -Force temp/android-resume-scroll/gesture | Out-Null
javac -source 8 -target 8 -classpath "$env:LOCALAPPDATA/Android/Sdk/platforms/android-34/android.jar" -d temp/android-resume-scroll/gesture Doroti/eng/android/ScrollGesture.java
& "$env:LOCALAPPDATA/Android/Sdk/build-tools/36.1.0/d8.bat" --min-api 24 --output temp/android-resume-scroll/gesture.jar temp/android-resume-scroll/gesture/ScrollGesture.class

python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-resume-scroll.py --serial emulator-5554 --label resume-check --trials 2 --warm-swipes 8 --expect-retained-context --verify-lifecycle --gesture-jar temp/android-resume-scroll/gesture.jar

python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

측정 전 빌드한 x64 APK를 해당 에뮬레이터에 설치한다. `--gc-log`는 별도 원인 진단용이며,
일반 측정과 구분한다. `--skip-forced-gc`는 Home/trim만 사용하는 대조 조건이다.

최종 APK SHA-256:

- x64 설치본: `d748b199d2f6a69ec8294d442123e125b8b459b7d67e5c31c6730658f4ee1a6c`
- arm64 빌드본: `39c229f9558098f018641c30dc8e7cd93981e25bd35cbebd6f816bda0e3b4bb7`

arm64 APK는 로컬 `temp/android-resume-scroll/doroti-testbed-resume-arm64.apk`에 준비했다.
연결된 실기기가 없어 설치/실행하지 않았다. x64 설치본은 다시 pull한 APK hash와 일치했다.
원시 APK·ring·XML·로그·스크린샷은 삭제 가능한 `temp/android-resume-scroll`에 있으며,
보존할 집계·소스 hash·APK/native asset hash·판정은 이 문서와 결과 JSON에 남겼다.

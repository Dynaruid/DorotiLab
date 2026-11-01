# Galaxy 복귀 후 관성 스크롤 후속 수정

작성일: 2026-11-01 (Asia/Seoul)  
기준 HEAD: `38e1603e3419c20085666de6fb550ea9669de938` + 앞선 Surface 수명 수정  
상태: 수정·회귀·Release arm64 빌드·갤럭시 설치 및 비교 완료. 전체 성능은 **PARTIAL**.
기계 판독 결과: [android-resume-inertia-results.json](android-resume-inertia-results.json).

사용자의 퇴근 요청에 따라 추가 실험을 종료했다. 최종 APK는 갤럭시에 설치되어 있으며,
해제 전 진단 intent 없이 일반 실행으로 복구했다. 커밋은 만들지 않았다.

## 재현 범위 수정

사용자는 첫 수정 후에도 Galaxy에서 복귀 뒤 잔끊김이 계속되고, 손을 뗀 관성에서 특히
보이며 긴 스크롤일수록 자주 나타난다고 확인했다. 새 항목의 lazy load 지연은 수용 가능하며,
이미 로딩이 끝난 영역에서 복귀 후 생기는 끊김이 이번 범위다.

앞선 [에뮬레이터 검증](android-resume-scroll-fix.md)은 250ms 정지 후 UP하는 접촉 드래그였다.
관성 구간 합격의 근거로 사용하지 않는다. 실제 Galaxy 로그에서 context generation 1의
재연결은 확인했으므로 Surface 수명 수정은 동작하지만 사용자 증상을 해소하지 못했다.

Galaxy SM-S931N/Adreno 830, Android 17/API 37, 1080×2340/density 480의 기본 가변 주사율
정책을 사용했다. 비교 중 접근성 서비스는 비활성이었다.
기존 사용자 APK는 교체 전에 pull하여 보존했다. SHA-256은
`e99651721eae6c736e07af74face432e6d78b283c519b429655bc46863a00ca1`이다.
앞서 전달한 APK와 hash가 달라 이를 같은 소스의 matched baseline이라고 가정하지 않았다.

## 확인과 변경

긴 플링의 별도 GC/할당 진단에서 관리→Java bridge 탐색 객체 최대는 0개였다.
관성 약 1.5초 동안 minor GC가 약 20회 발생했고, STW는 평균 약 4ms인 구간이 있었다.
이 계측은 상세 로그와 할당 프로파일을 켠 원인 조사이며 일반 비교 수치에 섞지 않는다.
캐시 누수나 복귀만의 단일 원인을 확정한 결과가 아니다.

짧은 플링에서 잡힌 91ms outlier는 새 이미지 데모의 첫 build/layout을 포함했다.
사용자가 명확히 구분한 lazy load 비용이므로 후속 잔끊김 비교에서는 정확한 플링 범위를
미리 방문한다. 이 outlier의 제거를 이번 증상의 해결로 보고하지 않는다.

- `PictureRecorder`가 녹화를 마칠 때 숫자 인수를 읽기 전용으로 닫는다. frozen scene은
  내부에서 닫힌 것으로 확인한 명령과 자원 없는 paint를 다시 복사하지 않는다. 외부에서
  전달한 `Picture`/`PathCommand`의 변경 가능한 인수에는 기존 방어 복사를 유지한다.
- Canvas가 이미 닫은 path·paragraph의 재스냅샷을 재사용한다. 원본의 수정/relayout/dispose는
  기존 녹화에 영향을 주지 않으며 내부의 닫힌 스냅샷은 수정할 수 없다.
- 텍스처가 없는 frozen scene은 texture admission에서 저장소를 retain한다. 텍스처가 있는
  분기만 복사하며 live/frozen 구분·중복 capture 제거·GPU 소비 완료까지의 lease를 보존한다.
- Android의 renderer가 surface generation 한 값만 읽을 때 전체 진단 snapshot을 만들지 않는다.
- 상세 할당 진단에 frozen capture/texture capture scope를 추가했다. 정상 실행은 강제 GC를
  사용하지 않으며 런타임·주사율·접근성 설정을 변경하지 않았다.

이미 만들어진 `RenderSectionList` 자식을 keep-alive 캐시로 옮기거나 되돌릴 때도
`dropChild`/`adoptChild`를 호출하여 전체 하위 트리를 detach/attach하던 경로를 확인했다.
새 위젯을 만들지 않는 이동에서도 semantics를 반복 무효화했다. 최종 수정은 같은 부모와
owner에 속한 캐시 이동에서 연결과 parentData를 유지하고 형제 목록만 옮긴다.
이 동작은 sealed `RenderSectionList`만 명시적으로 사용한다. 실제 제거·캐시 축출·owner
변경은 기존 detach/teardown을 수행하고 다른 sliver의 기본 동작은 유지한다.

scroll offset·검증된 컨테이너의 geometry 변경과 실제 semantics 내용 변경도 구분했다.
명시적으로 동의한 Android 호스트에서 활성 접근성 소비자가 없을 때만 geometry 갱신을
묶고 기존 유한 타이머와 정지 시점에 최신 소수점 offset을 반영한다. 내용·action·extent
변경은 우선 처리한다. 기본 호스트와 활성 서비스·가상 focus·최근 provider 조회는 즉시
갱신한다. dispose 시 ScrollPosition listener도 해제한다.

## 비교 방법

기준 APK는 같은 작업 트리의 앞선 Surface 수명 수정을 유지한 상태에서 만들었다.
당시 추가되었던 녹화 관련 4개 파일만 HEAD로 되돌려 빌드한 뒤 후보 소스를 byte 단위로
복구했다. 이후 추가한 semantics·캐시 연결 수정은 기준에 포함되지 않는다. 최종 후보와
기준은 Release Mono/profiled AOT/최적화 켬, interpreter 끔의 같은 빌드 설정이다.

All Components에서 setup 스크롤 12회와 정확한 up/down 플링 범위를 미리 방문했다.
x=950에서 1300 physical px/90ms의 DOWN→MOVE→UP를 주입하고 3초 tail을 수집했다.
프로세스별 첫 세션과 Home/복귀 2회를 수행했다. 기준 1프로세스, 최종 후보 2프로세스의
자연 복귀를 완료했다. 각 프로세스에서 context 생성 1회, Surface detach/재연결 각 2회,
미사용 GPU purge 0회였고 fatal/render/retirement 오류는 없었다.

긴 background 대기 대신 실제 Surface detach 완료를 확인해 복귀한다. 별도 진단은 full GC와
BACKGROUND trim을 명시적으로 유도한다. Samsung의 보이지 않는 RequestedLayerState 이름을
입력 차단으로 오인하지 않고 실제 input window stack을 확인한다. 실제 DOWN/UP와 제출 frame이
없는 회차는 비교 실패다.

하네스의 `ownerMs`와 제출 시각은 CPU/제출 증거다. 120Hz 표시 합격이나 물리 손가락 체감을
대체하지 않는다. SurfaceFlinger tail은 일부 회차에서 관성 전체를 포함하지 않아 표시 FPS로
환산하지 않는다.

모든 빌드/회귀/기기 하네스는 `run-with-timeout.py --timeout 1200`을 사용한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-resume-scroll.py --serial R3CY30KZA4B --label loaded-check --trials 1 --warm-swipes 12 --warm-fling-pairs 1 --warm-x 950 --gesture-x 950 --motion fling --fling-duration-ms 90 --fling-distance 1300 --fling-tail-seconds 3 --skip-forced-gc --resume-cleanup natural --expect-retained-context --gesture-jar temp/android-resume-scroll/gesture.jar
```


## 최종 비교 결과

아래는 trace가 UP 이전부터 남아 있고 scrollEnd가 확인되는 **실제 이동 구간**이다.
UP부터 scrollEnd를 마친 첫 제출까지 집계했다. ring buffer에서 앞부분이 지워진 구간이나
3초 tail의 idle flush를 이 표에 섞지 않는다. 같은 마지막 down 구간의 후보 2회를 비교했다.

| 항목 | 기준 1회 | 최종 1회 | 최종 2회 |
| --- | ---: | ---: | ---: |
| 구간 | resume-again-down | resume-again-down | resume-again-down |
| 제출 수 | 177 | 173 | 176 |
| 이동 구간 길이(ms) | 1473.762 | 1443.082 | 1481.760 |
| owner CPU p95(ms) | 11.384 | 6.514 | 6.362 |
| owner CPU 최대(ms) | 15.121 | 14.538 | 16.199 |
| 프레임 할당 중앙값(bytes) | 424760 | 139584 | 147248 |
| Gen0 GC 수 | 19 | 6 | 7 |
| semantics build 수 | 177 | 1 | 1 |
| native vsync 시각 간격 >12ms 수 | 0 | 0 | 1 |

p95는 약 43–44%, 프레임 할당 중앙값은 약 65–67% 감소했다. 역방향의 완전한 후보 trace도
JSON에 보존했다. 회차별 관성 길이·최종 extent가 조금 달라 제출 수의 단순 비율은 사용하지 않는다.
이 수치는 CPU/제출 진단이며 표시 FPS나 실제 손가락 체감의 합격을 뜻하지 않는다.
`presentObserved=false`, `physicalInput=notVerified`를 유지한다.

## 검증과 전달 상태

- 최종 Scroll 회귀는 PASS 출력 10개로 종료했다. immutable 녹화의 원본 변경 격리,
  texture capture/lease 수명, 실제 semantics 트리의 내용 우선순위·정확한 최종 offset·기본
  호스트 보수 동작·listener 해제, 실제 sliver의 부모/owner/형제/parentData 보존과 정확한
  축출 teardown을 포함한다. 로그: `temp/galaxy-resume-followup/final-regression-v4.log`.
- 최종 Release arm64 빌드: 경고 0, 오류 0, 1분 13.07초.
  로그: `temp/galaxy-resume-followup/final-build-v4.log`.
- 최종 APK: `temp/galaxy-resume-followup/doroti-testbed-galaxy-inertia-arm64.apk`.
  SHA-256: `27679b3f63b5df98bfbe60de6d11d45477c2ceffeef163c32eb19c0246b7f753`.
  설치 APK를 pull해 같은 hash임을 확인한 비교다. SkiaSharp native 버전은 `4.154.0`이다.
- 기준 APK: `temp/galaxy-resume-followup/matched-baseline.apk`.
  SHA-256: `d0ae2b82b4f0959b3d989c2be3abadadd8b32b471bdf011a75f6648b45d63e82`.
- 기기 해제 전 `debug.mono.log`의 빈 원래 값을 복원하고 profiled 프로세스를 종료했다.
  진단 intent 없이 다시 연 일반 앱 PID는 `14885`, top activity는 Testbed MainActivity였다.

## 남은 현상과 다음 시작점

**전체 성능 판정은 PARTIAL이다.** 최종 1회차의 첫 복귀 up 구간(gesture 17)에서
UP 후 약 1295ms에 owner 92.492ms/callback 90.228ms/Vulkan 2.123ms,
할당 2399168 bytes의 단발 지연이 남았다. 상세 trace는 ring buffer에서 지워져 원인을
귀속하지 못했다. 미리 로딩한 비교에서도 관측된 이 지연을 lazy load나 idle로 임의 제외하지
않는다. 앞선 짧은 플링에서 새 이미지 데모의 첫 build/layout으로 확인한 별도 91ms 지연과
혼동하지 않는다. 최대 지연과 일부 제출 간격도 남아 있어 p95 개선만으로 완료 처리하지 않는다.

사용자가 요청한 강제 GC/정리 유도는 별도 최종 진단에서 foreground full GC와 첫
BACKGROUND trim/GC·GPU purge 후 복귀까지 수행했다. 두 번째
`am send-trim-memory dev.doroti.testbed BACKGROUND`가 OS 명령 exit 255로 끝나
전체 진단은 **PARTIAL**이다. trim-40 GC와 purge 1회, Surface attach 2회가 부분 로그에
남았다. 하네스 manifest의 trials는 끝까지 완료되지 않아 비어 있으며, 이를 전체 PASS로
해석하지 않는다. 최종 후보의 Activity recreate/finish 검증은 도달하지 못했다.
앞선 에뮬레이터 수명 검증과 구분한다. 부분 JSON/log도 hash와 함께 결과 파일에 기록했다.

다음 작업은 같은 최종 APK에서 완전히 로딩한 긴 관성 스크롤의 첫 세션/복귀 후 실제
손가락 체감을 확인하고, 드문 첫 복귀 92ms 지연의 trace를 구간별 즉시 export하여 귀속하는
것부터 시작한다. 활성 TalkBack의 갱신·focus/action 실기기 검증도 `notVerified`다.

이동 구간 집계 재실행:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/summarize-android-resume-scroll.py temp/galaxy-resume-followup/loaded-baseline-manifest.json temp/galaxy-resume-followup/retained-final-manifest.json --output temp/galaxy-resume-followup/moving-summaries.json
```

원시 APK·trace·log는 ignored `temp/galaxy-resume-followup/`에 있다. `loaded-baseline-*`와
`retained-final-*`가 최종 자연 복귀 비교이며 `loaded-candidate-*`, `loaded-final-*`,
`loaded-delivered-*`, `snapshot-only.apk`, `position-only.apk`, `geometry-only.apk`는 중간
후보 증거다. 계측 overhead가 있는 `work-*`도 주 비교에 섞지 않는다. 원시 파일은 향후
정리될 수 있으므로 장기 보존 결과·source/APK hash·잔여 조건은 연결된 JSON에 별도로 담았다.

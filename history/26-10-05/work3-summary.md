# 플랫폼별 후속 실행과 잔여 수락 요약 — work3.md

원문 작성/보관일: **2026-10-05 KST**. [work2 구조 계약](work2-summary.md)의 미완료 80개 항목을 공통·AppKit·Catalyst·iOS·Android·Windows/Qt/Web·도구·릴리스의 **44개 후속 항목**으로 연결했다. 최종 기준이 남아 44개 체크는 모두 미완료이며 **전체 PARTIAL**, `wholePlanComplete=false`다. `tools/Doroti.DartToCSharp/`는 계속 제외했다.

보존 근거는 [execution](../../Doroti/docs/migrations/design-platform/execution.json), [checkpoint](../../Doroti/docs/migrations/design-platform/latest-checkpoint.json), [재개 receipt](../../Doroti/docs/migrations/design-platform/resume-verification-2026-10-05.json)와 아래 실행별 JSON이다. 원문의 초기 현황·미구현 진단보다 이후 실행 결과를 우선하되, 이전 Windows/Qt/Web/Apple PASS를 뒤에 변경한 공통 소스의 새 native 수락으로 재사용하지 않는다.

## 공통 계약과 44개 후속 항목

한 application의 논리 tree/build owner와 view별 WindowId/generation·focus/IME/Semantics/Navigator/restoration/frame 격리를 유지한다. 정적 plan→PrepareProcess→Configure→primary 생성/adopt→등록/Seal→attach 순서, Owned/Borrowed·bounded frozen 제출·GPU consumer 완료 뒤 해제, 내부 .NET typed 호출과 실제 외부 wire만 직렬화하는 계약을 따른다. Auto는 Unsupported에서만 Overlay로 전환한다.

| 묶음·ID | 목표와 남은 최종 기준 |
| --- | --- |
| C1~C6 | 전 provider semantic/public API/reflection 소유권·의존·버전/ABI 감사; 모든 staged startup/Stop/Restart 경쟁; OS WindowRequest 매핑·shared lease/frame·build profile; sample friend/public 계약·provider native/bootstrap 단일 소유 |
| U1~U4 | Cupertino native preview/route의 backdrop/restoration/focus/Semantics; Tooltip hover/long press/delay/no-activate/late install; Raw 입력/selection·controller dispose; 실제 Windows 확장 G1의 slow consumer/modal/reentrant close/late IME |
| A1~A5 | AppKit NSApplication/survivor·WindowKind/owner/좌표/focus·typed NSMenu·Graphite/Ganesh/input·모든 디자인 독립 소비·개발/서명/공증/clean OS |
| K1~K4 | Catalyst shared scenes·adopt/close/recreation; public UIKit에서 실제 지원/거절; scene별 입력·메뉴 제한; 독립 디자인/개발·배포 |
| I1~I5 | iOS surface/background/rotation·profile별 matching toolchain; typed device/dev transport와 owned Stop/Restart; UIKit native/디자인/입력; simulator/device·Mono/CoreCLR/NativeAOT·package/배포 별도 수락 |
| D1~D5 | Android Application/Activity/surface 수명; Vulkan GPU/ANativeWindow drain; ADB session nonce/PID/port·owned dev; runtime/toolchain profile; 두 샘플·세 디자인 device/NuGet/서명 인수 |
| T1~T4 | builtin config/device/doctor/template/native-maintenance; .NET ALC/process lifecycle/crash/reconnect와 app ownership; 실제 CLI/IDE; 외부 provider 독립 설치·typed codec 0/wire/ABI |
| R1~R3 | Windows native 확장·mixed DPI/AT, Qt 전체 창/typed menu/staged loop, Web 모든 main/worker profile·Offline/mobile Overlay/browser Stop |
| P1~P4 | SDK/세 디자인/모든 profile, core/design/provider 독립 버전·최소/범위 밖/ABI 소비, profile별 release receipt/provenance, 문서·구 구조 제거 |
| V1~V4 | 공통 aggregate·host/profile/device gate, trim/AOT·물리/배포, 원 계획 잔여 감사·재개 가능한 인계 |

의존 순서는 **C → U/Windows → AppKit → Catalyst → iOS → Android → 도구/Qt/Web → 후보/최종 감사**다. 가용 Windows 공통/Android 작업은 Apple 호스트 확보와 독립적으로 진행할 수 있다. 부분 PASS로 원 항목 전체를 체크하지 않는다.

## 14. 2026-10-05 실행 결과

[공통·Apple receipt](../../Doroti/docs/migrations/design-platform/work3-verification-2026-10-05.json)의 새 실행 범위를 보관한다. macOS **26.6.2 (25G83)** / Apple M1 arm64, .NET **10.0.401**, Xcode **27.0 (27A266a)**에서 AppKit `net10.0-macos27.0/osx-arm64`, Catalyst `net10.0-maccatalyst27.0/maccatalyst-arm64`, iOS `net10.0-ios27.0/iossimulator-arm64`를 구분했다. simulator는 iPhone 17/iOS 27.0이다.

| 범위 | 구현·명시 범위의 PASS와 제한 |
| --- | --- |
| C1/C5/C6 | core 6개·두 디자인·tool·Apple Host 4 profile 등 13 ownership closure의 실제 MSBuild 입력 평가. provider profile import 순서·iOS arm64/x64 simulator/device 평가와 단일 profile 소유, template 복사본 제거. 전 provider semantic audit·.NET 11 matching build·sample friend/native 책임 감사는 남음 |
| C2/C3 | MAUI OS startup의 PrepareProcess→Configure, 명시 app dispatcher/shared session. AppKit Manual/Dock 비설정, Catalyst PlatformDefault 매핑 및 owner/modality/anchor/activation 사전 거절. 모든 coordinator/race 수락은 남음 |
| C4/U4 | branch unmount·typed callback quiesce 뒤 per-view/GPU drain, shared Stop/Dispose join·실패 재시도·survivor. 공유 SKTypeface wrapper lease로 다른 View 한글 편집 crash 수정. CPU 입력/late callback 격리 PASS, native 확장 G1은 남음 |
| U1~U3 | caller theme/locale/directionality·child Navigator 결과, Cupertino long-press/Overlay, ModalScope animation/duration·route tween 수정. Raw/Material menu/radio/Tooltip/ExpansionTile·Cupertino selection/dialog 회귀. filtered native backdrop는 **Native Unsupported / Auto Overlay**; 완전한 restoration/focus/AT와 native Tooltip 확대 수락은 남음 |
| AppKit | Regular·owned Dialog/Popup/Tooltip/Satellite·sheet, no-activate/anchor/visibleFrame·Escape/로컬 outside click·owner cascade/focus 복구. typed NSMenu popup/menubar·enabled/checked/submenu/문자 shortcut·active owner/stale action 보호. Graphite/Ganesh resize/retirement/reconnect/drain·native editor/WebView/서비스·두 lifetime survivor·60초/정상 종료 PASS |
| AppKit 제한 | global/다중 display·앱 밖 클릭/경쟁·popup tracking/shortcut 확대·물리 IME/VoiceOver는 남음. platform role/logical-key shortcut Unsupported |
| Catalyst | activating/unowned Regular 추가 scene, shared session·primary close/survivor·UIKit 서비스/input/WebView/Semantics/navigation/restoration·typed/GPU drain PASS. aux/owner/modal/no-activate/hidden-first-frame과 OS menu는 Unsupported; 모든 scene race/물리/독립 디자인·개발/배포는 남음 |
| iOS | 공유 app session과 branch/view identity, background/handler 상실 시 tree 보존, async detach/실제 Metal completion 및 Stop join·view Dispose·pending 0. typed simctl/CoreDevice 검색·RID/명시 ID `_DeviceName` 연결. simulator native/synthetic subset PASS, multi-scene 미지원 manifest는 SKIPPED |

**독립 AppKit 소비자:** core `0.4.0-alpha.1` / provider `0.4.0-alpha.2`, widgets, 26개 prebuilt nupkg·격리 cache, `net10.0-macos/osx-arm64` 선언/실제 platform SDK 27.0이다. 실제 두 창·resize/close와 `.pkg`에서 추출한 앱의 local copy/같은 후보 교체/제거·한글 userdata 보존을 확인했다. `copy`/`LinkMode=None`·adhoc signing 범위이며 실제 trimming/NativeAOT·OS Installer/Finder·공증/clean OS·다른 버전 upgrade를 의미하지 않는다.

초기 connected filter의 physical device 0과 .NET 11 rc1/rc2 불일치는 당시 결과다. 이후 iPhone 검색·NativeAOT 성공은 아래 별도 후보에 기록하며 당시 SKIPPED를 소급 PASS로 바꾸지 않는다. typed iOS dev agent/prepared acknowledgment/network transport·owned Stop/Restart는 **Unsupported**로 남는다.

## 15. 2026-10-05 iOS 실기기 후속 검사

[iPhone Debug Mono receipt](../../Doroti/docs/migrations/design-platform/work3-ios-device-verification-2026-10-05.json): **iPhone 12 / iOS 26.6.1 (23G83) / USB / Developer Mode**, `net10.0-ios27.0/ios-arm64` 개발 서명. 엔진/의존성 Mono AOT, 앱/진입 assembly interpreter이며 최종 build 경고·오류 0이다. 기존 앱 데이터는 유지했다.

CoreDevice idle tunnel의 connectable paired 장치를 bounded 상세조회로 확인하고 같은 UDID만 반환하도록 수정했다. Documents probe 경로·pulse 이후 cached geometry 계측·정적 scene 복귀의 GPU 완료 확인을 보강했다. 강제 FPS 제한을 제거해 UIKit 기본 cadence를 유지했다.

128MiB offset 파일 읽기/취소·editor/WebView 재생성 4회·WKWebView HTML/JS/asset/message/stale close·Semantics action·양방향 회전·Stop/GPU pending 0 PASS다. background/foreground 3회는 같은 PID, GPU 완료 **4→5→6→7**, failure 0이었다. **1170×2532** capture와 방향별 중간 raster 폭 11개를 확인했다.

평균/최대 phase 오차는 가로 **23.10/48.54pt**, 세로 **26.57/60.06pt**로 엄격한 평균 5%/최대 10% 예산 PASS가 아니다. 사용자의 “특별한 이상 없음” 피드백은 해당 사용 경험이며 정식 물리 한글 IME/VoiceOver 수락을 대신하지 않는다.

## 16. 회전 버벅임 재현 및 Release Mono AOT 설치

[Release Mono AOT receipt](../../Doroti/docs/migrations/design-platform/work3-ios-release-aot-verification-2026-10-05.json): 실제 **Material Components**에서 Debug Mono의 심한 회전 지연을 재현했다. 앞선 단순 reload scene의 기능 PASS는 Components 성능을 대신하지 못했다. 진단 writer 없는 평균 갱신 간격은 가로/세로 **107.50/93.97ms**, 최대 **142.88/147.39ms**였다.

NativeAOT 최초 시도는 reflection JSON을 source-generated로 고친 뒤에도 .NET 10의 explicit-interface IL2037 및 .NET 11 workload/Xcode 불일치로 실패했다. 이를 숨기거나 SDK 검사를 우회하지 않았다. 그 시점 가능한 **Release Mono AOT/LLVM**, `UseInterpreter=false`, `MtouchInterpreter=-all`, `Optimize=true`로 publish/개발 서명/데이터 보존 설치했다. NativeAOT와 구분한다.

같은 화면·기기·cold process 조건의 평균 간격은 **39.96/37.89ms**, 최대 **50.55/52.72ms**, raster 폭 **10/11개**였다. native 서비스/input/WebView/Semantics·회전·Stop drain·background 3회 기능 PASS지만 엄격한 phase 오차와 표시/scanout FPS 인수는 남았다. 사용자는 “약간 개선됨”이라고 답했다. instrumented callback/raster 평균 **14.914/15.313ms**는 겹치는 비용이며 정확한 rotation-only 표시 계측으로 합산하지 않는다.

## 17. 2026-10-05 iOS 회전 FPS 개선

[회전 FPS receipt](../../Doroti/docs/migrations/design-platform/work3-ios-rotation-fps-verification-2026-10-05.json): 레이아웃·접근성 배치·GPU 대기로 다음 pulse를 건너뛰는 비용을 개선했다. drawable/generation 고정·native composition 없음 조건에서 다음 shader scene CPU 준비를 앞당기되 최대 **2 consumer**와 lease를 유지한다. native scene 전환/최종 resize는 drain, native 회전은 최대 **1 consumer**다. UIKit cadence와 Core Animation transaction presentation을 유지한다. VoiceOver/Switch Control이 꺼진 회전만 accessibility 갱신을 병합하고 종료 frame에 반영한다.

전후 각각 cold process **3회**, 상세 profiling/evidence writer 없음 조건이다. 갱신 간격과 Metal `PresentedTime`으로 계산한 표시 FPS를 구분한다.

| 방향 | 전 평균 갱신 간격 | 후 평균 갱신 간격 | 간격 감소 | 후 평균 표시 FPS·3회 범위 |
| --- | ---: | ---: | ---: | --- |
| 가로 | 38.04ms | 23.42ms | 38.45% | 42.61 / 39.88–45.74 |
| 세로 | 38.20ms | 22.48ms | 41.15% | 44.15 / 42.73–44.86 |

이전 probe에는 viewport별 표시 timestamp가 없어 수정 전 표시 FPS/표시 FPS 향상률을 만들지 않는다. 정착 replay·회전 사이 대기·중복 시각은 제외했고 장기 thermal 성능 수락은 수행하지 않았다.

실기기 Release Mono AOT 기능/rotation/Stop pending 0·background 3회 PASS, native UI 회전 중간 크기 **20/17개**·consumer 1·failure 0, CPU retained shader/native 거절·texture budget와 FPS 계산 3개 회귀 PASS다. Catalyst Host build와 Release Mono AOT simulator(`LLVM=false`) 중간 크기 **15/15개**·기능/종료 PASS를 기록한다. **Debug interpreter simulator는 2/3/3개로 4개 기준 미달 FAIL**이며 미해결이다.

**Components 60 FPS 유지와 엄격한 peak phase 예산은 미달**이다. 최종 가로 peak **56.2–61.7pt**는 10% 기준 **45.4pt**를 넘었다. 물리 VoiceOver/Switch Control 성능은 별도다.

## 18. 2026-10-05 iPhone NativeAOT 설치

[NativeAOT device receipt](../../Doroti/docs/migrations/design-platform/work3-ios-nativeaot-device-verification-2026-10-05.json): **DorotiTestbedApp Release NativeAOT**를 같은 iPhone 12에 데이터 보존 설치했다. .NET **10.0.401**, iOS SDK **27.0.10722**, MAUI **10.0.110**, ILCompiler **10.0.12**, Xcode **27.0**, publish 경고·오류 0·서명 검사 PASS다. `UseNativeAot=true`, `UseMonoRuntime=false`, NativeAOT object/runtime libraries와 `_RhpNewFast` 링크 맵으로 Mono AOT와 구분했다.

입력/WebView/native design probe의 reflection JSON을 source-generated로 전환했다. SDK explicit-interface marking 문제를 피하는 `--skip-marking-nsobjects-in-user-assemblies=true`, `_UseDynamicDependenciesForMarkNSObjects=false`와 `NativeAotRoots.xml`의 실제 native 진입/뷰/delegate **23개 타입** 보존을 사용했다. roots 없는 중간 실행의 AppDelegate/MAUI 서비스/Metal selector 누락을 기록했다. 최종은 원본 SDK이며 SDK 사본을 고친 중간 실험이나 진단 suppress를 사용하지 않았다.

native 서비스/WKWebView/JS/Semantics·회전·joined Stop/GPU drain·같은 PID background 3회 통과다. 단, 최초 입력 뷰 교체는 **`Metal terminal failed: NotEnqueued` 1회**였다. 같은 산출물의 후속 독립 **3회 PASS**에도 원인이 미확정이며 최초 full smoke를 전체 PASS로 바꾸지 않는다. GPU 완료 판정을 완화하지 않았다.

중간 raster 폭 **19/20개**, 1회 Metal 표시 FPS **47.85/49.84**는 evidence writer를 사용한 결과로 앞선 Mono AOT cold 3회와 직접 비교하지 않는다. 예전 publish blocker는 이 설치로 정리했으나 NotEnqueued·60 FPS/phase·물리/배포 수락과 전체 계획은 미완료다. 최종 일반 foreground 실행은 당시 probe/evidence/profiling 없는 Material 화면이었다.

## 19. 2026-10-05 Android와 공통 잔여 실행

[Android/common receipt](../../Doroti/docs/migrations/design-platform/work3-android-verification-2026-10-05.json)는 최신 D1~D5와 Windows 가용 공통·tool·package 실행을 보존한다. **Galaxy S25 SM-S931N / Android 16 / android-arm64**, **Pixel 5 emulator / Android 13 / android-x64**를 구분한다. emulator GPU는 NVIDIA RTX 4060 Laptop GPU였다. Windows .NET **10.0.400**, Android workload **36.1.69**, MAUI **10.0.90**, JDK **21.0.8**, API/build-tools **36/36.0.0**, NDK **28.2.13676358**를 실제 SDK 평가에서 기록했다.

| 범위 | 구현·명시 범위의 결과 |
| --- | --- |
| D1/C2/C4 | 비종료 Activity 재생성의 논리 surface·widget State·IME/navigation 보존, 이전 callback 취소 후 새 view frame 재요청. 두 ABI에서 같은 PID 실제 `Activity.Recreate`와 새 PID process restart. zero-view 구간 CPU State/scene 보존 PASS |
| D2/C4/U4 | DisposeAsync가 Vulkan retirement/ANativeWindow release까지 await. replay completion race에도 raster/GPU의 frozen-scene lease 유지. 실제 Windows 오류와 CPU 회귀를 수정 후 PASS |
| D3/T2/T3 | typed plan의 패키지 포함 Python ADB adapter·선택 adb/device/RID·session nonce·device/package/directory lease. stale Stop nonce 무시·startup port identity 고정·graceful Stop. 실제 CLI delta/컴파일 오류/rude edit 복구 및 설치 VSIX Run/Reload/상태·PID·text·scroll 유지/Restart 새 PID/Stop PASS |
| D4/T1 | 실제 SDK-selected JDK/SDK/NDK/API/build-tools doctor. arm64/x64 개발 profile·Mono startup hook 및 Release/optimized/trim/AOT/CoreCLR dev 혼합 거절. 일반 Debug standalone assembly APK 설치 |
| D5/P1~P3 | 두 샘플 build/install/native 실행, widgets/material/cupertino core `0.4.0-alpha.1` / provider `0.4.0-alpha.2`의 독립 cache/prebuilt nupkg publish·실기기 first frame PASS. 초기 MAUI downgrade·출력/ID/workspace runner·mobile receipt 수정 |
| R1/V1/V2 | 최신 Build/Source/G0·frame/typed/tool와 전체 WindowsSmoke PASS, describe stderr/JSON 수정·VS Code Node **9개** 회귀 PASS. AndroidSmoke 추가, device case 미선택/SKIPPED는 **PARTIAL/exit 2**. Windows는 실제 검증한 Release apphost/WinRT manifest 사용 |

S25의 Sample2 회전·background/foreground 3회·같은 PID Activity/새 PID process 재생성과 Testbed synthetic composing/commit/paste·native endpoint focus·**60초** frame 진행·Stop pending GPU 0을 확인했다. emulator의 두 샘플/lifecycle/rotation/recreate/Stop도 PASS다. 초기 x64 복귀 blank/0 frame은 수정 전 실패로 남기고 surface 재사용/frame 재요청 후 별도 재검사를 기록했다. 설정 변경만으로 Activity 재생성이 발생하지 않은 이전 시도는 PASS로 바꾸지 않았다.

Sample2 **Release trim + profiled Mono AOT**, `AndroidAotMode=Normal`, interpreter off APK의 실기기 lifecycle/recreate/restart도 PASS다. Android NativeAOT/full-method AOT가 아니며 SDK 기본 개발/debug key로 서명했다. 세 디자인 NuGet APK는 별도 Release **untrimmed/non-AOT** 소비자다. production signing/clean OS/다른 버전 upgrade·물리 IME/TalkBack 수락은 아니다. 실제 USB unplug/reconnect/crash/동시 경쟁 전체도 남는다.

이 공통 변경 뒤 새 Apple/Qt/Web native 실행은 없었다. Windows scene lease 수정으로 iPhone NotEnqueued가 해결됐다고 주장하지 않는다. 공개 배포는 수행하지 않았다.

## 원 계획 대응과 전체 잔여

| work2 미완료 원 항목 | work3 대응 |
| --- | --- |
| DS0-1~4 | C1, C5, P2, V4 |
| DS1-1~5 | C5, C6, P1~P3 |
| DS5-1~5 | U1~U3, A4, K3, I4, D2, V3 |
| DS6-1 | C2, C6, P1 |
| DS7-4 | P2, P3 |
| DS8-2~4 | A5, K4, I5, D5, R1~R3, V2~V3 |
| DS9-1~5 | P3, P4, V4 |
| PW0-1~6 | C1~C4, P2, V4 |
| PW1-1~7 | C1, C4~C6, T4, P1 |
| PW2-1~6 | C2, C4, C6, A1, K1, I1, D1, R2, R3 |
| PW3-1~8 | C5, C6, T1~T4, P1~P3 |
| PW4-1~7 | C4, U4, A1, K1, I1, D1, R2 |
| PW5-1~5 | C3, U4, R1, V3 |
| PW6-1~4 | U1~U3, A2~A3, K2~K3, R1~R2 |
| PW7-1~6 | T1~T4, I3, D3~D4, P3 |
| PW8-1~7 | A1~A5, K1~K4, I1~I5, D1~D5, R1~R3, T2, T4, P2~P4, V1~V4 |

최종 체크는 전 provider semantic/API/reflection·staged coordinator/native bridge 소유권, 완전한 native route/Tooltip·확장 G1, builtin template/native-maintenance·iOS typed dev·실제 Apple IDE, Qt 창/OS menu·모든 Web profile, 전 core/design/provider 최소/범위 밖/version/ABI·Apple 세 디자인 독립 소비, matching .NET 11 rc2/CoreCLR/x64 simulator, 물리 IME/키보드/터치/AT·mixed display·scanout·production signing/공증/clean OS·다른 후보 upgrade와 iOS 성능 기준을 각각 대조해야 한다.

원문 초기 `currentFailing=[]`는 그 시점에 수행한 검사 상태다. 보관 시점 checkpoint에는 **초기 iPhone NativeAOT NotEnqueued 원인 미확정**이 남아 있으며 Debug interpreter simulator 회전 기준 FAIL도 보존한다. 모든 미완료가 해소되기 전 `wholePlanComplete`를 true로 바꾸지 않는다.

## 재실행과 증거 보존

테스트 외부 timeout **1,200초**, 반복 통상 **30회 이내**다. Build/Packages/WindowsSmoke/Developer/Release와 matching host의 MacOSSmoke/CatalystSmoke/IOSSmoke, 명시 device/RID의 AndroidSmoke를 사용한다. Targets/Release 이름으로 모든 mobile/Apple 장치 검사를 수행했다고 해석하지 않는다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py WindowsSmoke
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py AndroidSmoke --device <serial> --rid android-arm64
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py AndroidSmoke --cases profiles,tools
```

`temp/testing/work3/`, `temp/testing/ios-rotation-fps/`와 `Doroti/artifacts/`는 disposable이다. durable receipt의 source/APK/bundle/nupkg/native hash·명령·범위·실패/재시도를 보존 근거로 삼는다. 이 요약은 당시 장치 foreground 상태나 raw 존재를 현재 상태로 보장하지 않으며, 보관 과정에서 제품/장치 검증을 새로 실행하지 않았다.

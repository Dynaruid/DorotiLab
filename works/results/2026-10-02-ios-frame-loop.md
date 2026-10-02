# iOS 프레임 연결 수정 — 2026-10-02

**현재 iOS 기본 경로는 C이며 공개 VariableBlur API와 Sample2의 기본 블러는 Fast adaptive다.** lifecycle 수정 후 사용자가 아이폰의 정상 동작을 확인하고 기본 경로 적용을 명시했다. 별도 환경변수 없이도 새 shader-only 장면에 최대 2개 GPU frame과 비동기 표시를 사용하며 native·회전·replay의 직렬 경로와 A/B 비교 옵션을 유지한다.

아래 자료는 후보별 실행 이력이다. Adaptive C의 σ20/32 반복 평균 59.714/59.634 FPS와 Fast σ20/32의 단일 smoke 59.829/59.827 FPS를 서로 다른 payload 자료로 보존한다. 복귀 직후 잠깐의 frame 증가만으로 통과시켰던 초기 preflight는 철회했고 lifecycle 후속 검증으로 대체했다. 정량 입력 지연·10분 사용·전체 모드의 반복 검증은 아직 수행하지 않았다. Safari 비교는 사용자 지시로 제외하며 기기 상태는 양호하게 가정한다.

## lifecycle 후속 수정 전 후보의 같은 바이너리 비교

이 비교 당시 후보는 MTKView display link로 shader frame pulse를 구동했다. 요청·새 장면·GPU pending이 없어지면 멈추며 native/rotation/비활성화/종료에서도 루프를 정지한다. 최신 pending scene 하나와 GPU 두 프레임 상한을 유지한다. **presentation credit 실험과 drawable pool 2 실험은 제거했다.** 비교 당시 기본 실행은 기존 serial A이고 C는 opt-in이었다. 이후 기본 채택은 문서 마지막에 기록한다.

SurfaceGeneration 조회 중 전체 진단 history를 복사하던 비용을 scalar 조회로 수정했다. 아래 최종 비교에는 이 수정 이전의 v1~v7/첫 final 실행을 합치지 않는다.

| σ20 Adaptive | 1회 FPS | 2회 FPS | 3회 FPS | 평균 FPS | 실제 overlap 합계 |
| --- | ---: | ---: | ---: | ---: | --- |
| A: serial/transaction | 38.059 | 38.094 | 36.456 | 37.536 | 0 |
| B: serial/async | 35.702 | 35.704 | 36.168 | 35.858 | 0 |
| C: prepared pipeline/async | 59.756 | 59.762 | 59.623 | 59.714 | 559/5372 |

C는 세 반복 모두 대응 A를 개선했고 평균은 **59.1%** 높다. C의 p95/p99는 모두 약 **16.72ms**, >25ms 비율은 **0.11 / 0.11 / 0.34%**다. GPU pending 최대 **2**, renderer/terminal 오류 **0/0**이다. B의 shader transaction/scheduling 대기는 제거됐지만 FPS 개선은 없었다. capture **1170×920**, Gaussian **6패스**와 블러 설정은 A/B/C에서 같다.

σ20 Off/Fixed A/C 각 3회도 표시 성능 기준을 통과했다. C Off **59.821 / 59.824 / 59.723 FPS**, C Fixed **59.588 / 58.685 / 59.793 FPS**. Fixed/Off 평균 비율은 σ20 A/C에서 **99.19 / 99.27%**, σ32 A/C에서 **98.35 / 99.57%**다.

σ32 Adaptive C는 **59.688 / 59.588 / 59.624 FPS**로 대응 A를 모두 개선했고 평균 개선율은 **60.4%**다. 실제 overlap 합계는 **570/5365**다. σ32의 callback→표시 p50은 A 약 **32.7ms**, C 약 **49.9ms**, C의 p95는 **61.3–61.7ms**로 지연 증가가 반복됐다.

| σ | 모드 | 정책 | 평균 FPS | 최악 p95 ms | 최악 p99 ms | 최악 >25ms | 3회 모두 60Hz 기준 |
| --- | --- | --- | ---: | ---: | ---: | ---: | --- |
| 20 | Off | A | 59.299 | 16.719 | 16.721 | 0.678% | 통과 |
| 20 | Off | C | 59.789 | 16.718 | 16.719 | 0.168% | 통과 |
| 20 | Fixed | A | 58.819 | 16.719 | 33.432 | 1.306% | 통과 |
| 20 | Fixed | C | 59.355 | 16.719 | 16.720 | 0.512% | 통과 |
| 20 | Adaptive | A | 37.536 | 33.434 | 50.144 | 63.187% | 미달 |
| 20 | Adaptive | B | 35.858 | 33.434 | 50.140 | 66.916% | 미달 |
| 20 | Adaptive | C | 59.714 | 16.718 | 16.719 | 0.336% | 통과 |
| 32 | Off | A | 59.258 | 16.718 | 16.719 | 0.737% | 통과 |
| 32 | Off | C | 59.673 | 16.717 | 16.718 | 0.392% | 통과 |
| 32 | Fixed | A | 58.278 | 16.718 | 33.432 | 1.646% | 미달 |
| 32 | Fixed | C | 59.415 | 16.718 | 16.718 | 0.452% | 통과 |
| 32 | Adaptive | A | 37.172 | 33.435 | 33.437 | 60.595% | 미달 |
| 32 | Adaptive | C | 59.634 | 16.718 | 16.719 | 0.392% | 통과 |

60Hz 판정은 각 실행 FPS≥58, p95≤17.5ms, p99≤34.2ms, >25ms≤2%를 모두 요구한다. **C 핵심 18회는 모두 통과했다.** A Fixed σ32 첫 실행의 **56.785 FPS**와 83.6/635.2/434.6ms 긴 간격은 그대로 보존했다. 평균이나 좋은 실행만으로 이 실행을 통과 처리하지 않았다.

블러 capture는 σ20 **1170×920**, σ32 **1170×1136**이며 정책 간 같았다. Off 패스 0, Fixed Gaussian 2, Adaptive Gaussian 6패스 의미도 같다. 39개 유효 실행 모두 renderer/terminal 오류 0/0이며 source/payload/runtime 일치를 확인했다. 별도 시작 타임아웃은 유효 실행으로 세지 않았다.

실행별 FPS·percentile·overlap·pending·CPU 단계·callback 대기·지연·오류·작업량과 34개 source hash는 [정책별 집계 JSON](2026-10-02-ios-frame-loop-summary.json)에 보존한다.

| σ20 Adaptive | callback→실제 표시 p50, 반복별 ms | p95, 반복별 ms |
| --- | --- | --- |
| A | 32.79 / 32.81 / 32.61 | 33.03 / 49.27 / 33.02 |
| B | 32.61 / 32.60 / 32.62 | 49.17 / 33.02 / 33.02 |
| C | 49.87 / 49.87 / 49.91 | 49.94 / 61.28 / 62.94 |

이는 framework callback부터 실제 표시까지의 지연이며 **물리 손가락 입력 지연이 아니다**. 입력→scene→표시는 아직 측정하지 않았다. FPS 개선만으로 입력 지연 무회귀 게이트를 통과했다고 판정하지 않는다. Full/Fast/Kawase 추가 36회는 핵심 모드의 채택 게이트가 통과하면 확대하는 조건이므로 현재 모든 옵션의 60Hz를 주장하지 않는다.

### 빌드 identity와 측정 범위

| 항목 | 기록 |
| --- | --- |
| commit | `019965e7144173b8e9d298abe615ec6478c81787` + 작업 트리 변경 |
| 성능 앱 payload SHA256 | `8647e2a0ecf819ae2e800f1b166c4f5a62333f3c3bd7ff05dd94c8f8d2a7f5e6` |
| runtime / TFM | NativeAOT · `.NET 11.0.0-rc.1.26425.128` · `net11.0-ios` |
| SDK / workload | `11.0.100-rc.1.26425.128` / `11.0.100-rc.1.26458.5` |
| iOS SDK / MAUI | `26.5.12193-net11-rc.1` / `11.0.0-rc.1.26451.6` |
| Xcode | `27.0 / 27A266a` |
| 기기 | iPhone 12 · A14 · iOS `26.6.1 / 23G83` |
| viewport / DPR / Hz | `1170×2532` / `3` / `60` |
| 측정 | 40초 합성 스크롤, 첫 실제 drawable 표시 이후 5–35초 |

같은 payload/runtime과 34개 source input hash를 비교한다. 모드와 정책 순서를 교차하며 두 번째 반복은 역순이다. USB 연결 상태에서 FPS 측정 중 profiler·캡처·녹화를 실행하지 않았다. 기존 Mono의 33.759 FPS는 다른 runtime/payload의 참고 기록이며 개선율 계산에 사용하지 않는다.

앱은 `Doroti/artifacts/ios-frame-loop-nativeaot-final/bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app`, 빌드 로그는 `temp/testing/variable-blur/ios-frame-loop-build-nativeaot-final-generation.log`다. SDK profile에 한해 `ValidateXcodeVersion=false`, `PrepareAssemblies=false`, `_UseDynamicDependenciesForMarkNSObjects=false`, `InlineClassGetHandle=disabled`를 명령 단위로 적용했다. 전역 SDK 설정은 바꾸지 않았다. 기존 `NSLayoutAnchor<T>` proxy ILC 진단은 보존했으며 native 검증은 별도로 수행한다.

최종 데이터는 `ios-frame-loop-generation-adaptive20-abc`, `ios-frame-loop-generation-off-fixed20-ac`, `ios-frame-loop-generation-core32-ac`, `ios-frame-loop-generation-core32-fixedC-retry`, `ios-frame-loop-generation-core32-adaptive-rest`다. σ32 Fixed C3의 첫 시작은 100초 timeout이며 초기화 로그와 새 evidence가 없었다. 원인은 미확정이다. 실패를 보존하고 같은 payload의 별도 재실행으로 59.660 FPS를 확보했으며 남은 C3/A3 순서를 이어갔다. collector는 이후 원격 evidence 파일명과 실행 환경도 실행 전에 저장한다. 이하 초기 수치와 실험은 최종 반복 비교와 구분한다.

## 구현

- iOS frame pulse에서 immutable pixel/DPR/context generation을 게시한 뒤 framework callback을 실행하고, 준비된 장면과 native 상태를 검사한다.
- callback을 한 번 꺼내 실행하고 callback 내부 재요청은 다음 pulse에 남긴다. GPU 슬롯이 가득 차도 framework 준비와 최신 pending scene 교체를 허용한다.
- 새로운 shader-only 장면만 최대 두 프레임을 허용한다. native/shield, replay, resize/rotation은 직렬 admission을 유지한다.
- A(serial/transaction), B(serial/async), C(prepared pipeline/async)를 같은 앱에서 선택한다. collector의 `--policies A B C`가 정책과 모드 순서를 교차한다.
- 실제 scene/native 상태로 shader-only 비동기 presentation을 선택한다. 같은 Metal queue의 rendering 다음 terminal buffer에서 present하고 native/rotation transaction 계약은 유지한다.
- 실제 drawable 표시, GPU 완료 도착, owner 처리, framework 준비, recorder 및 Snap/Insert/Submit/flush/WaitUntilScheduled를 독립적으로 계측한다. bounded 65,536-event 이력이 잘리면 집계를 거부한다.
- 성공한 terminal은 한 번만 commit한다. submission 실패/종료는 GPU 완료 또는 확인된 context loss 전 자원 회수를 허용하지 않는다.

Metal terminal의 실제 `GpuEndTime`도 기록해 콜백 전달 지연과 GPU 완료를 구분한다.

기본 정책 채택은 실기기 반복 비교 후 판단한다. 샘플 Fixed 기본값·공개 ImageFilter·블러 kernel/capture/해상도·다른 플랫폼 기본 정책은 바꾸지 않는다.

## 현재 확보한 검증

- 기존 Release/Mono Adaptive/sigma20 baseline: **33.759 FPS**, 표시 p95 **33.442ms**, renderer/terminal 오류 **0/0**. 이전 서명 앱을 사용한 단일 계측 기준이며 새 정책 A/B/C의 같은 바이너리 비교와 구분한다.
- framework callback이 처음 pending scene을 만드는 호스트 ordering 회귀 통과.
- callback 중복/재요청, 최대 두 프레임, native 전환, replay/viewport fallback, 역순 renderer 완료, 실패/context loss/중복 retirement/종료 callback 회귀 통과.
- 공통 CPU 회귀 통과.
- 기존 macOS Graphite-Metal VariableBlur 픽셀 회귀 통과. 기존 makeImageSnapshot/asImage 경고는 로그에 보존한다.
- 같은 recorder/cache를 사용하는 두 async recording의 픽셀 보존 검사: scale 1/.25, 각 두 프레임 모두 **0/255** 오차.
- Python collector 검사 8개 통과.

원본: `temp/testing/variable-blur/ios-frame-loop-*`. 빌드/기기 반복 수치와 최종 앱 identity는 검증 완료 후 아래에 기록한다.

## 표시·자원 계약 근거

[Apple presentsWithTransaction 문서](https://developer.apple.com/documentation/quartzcore/cametallayer/presentswithtransaction)의 transaction 경로는 commit → waitUntilScheduled → drawable.present를 요구한다. shader-only 비동기 경로는 같은 queue의 terminal command buffer에 presentDrawable을 배치한다. scheduling 완료와 실제 GPU 완료/표시는 각각 구분한다.

[Skia Graphite Recorder 계약](https://skia.googlesource.com/skia/+/refs/heads/main/include/gpu/graphite/Recorder.h)을 바탕으로 기존 recorder의 순차 snap과 recording/native resource 소유권을 유지한다. 호스트 frame/recording/drawable은 terminal GPU 완료 전 해제하지 않는다.

## 초기 단계의 미검증 항목 — 이후 실험 기록과 구분

동일 앱의 실제 overlap과 A/B/C 반복 성능, Off/Fixed 회귀, 각 옵션의 60Hz 판정, native 입력·회전·background/foreground·실제 손가락 조작·지속 사용. 자동 합성 스크롤은 실제 입력이나 수동 지속 사용의 통과 증거로 취급하지 않는다.

## 첫 NativeAOT A/B/C 진단과 추가 수정

같은 NativeAOT 앱의 첫 Adaptive/sigma20 실행:

| 정책 | 표시 FPS | p95 ms | p99 ms | >25ms | 실제 overlap | GPU pending 최대 | callback→표시 p50/p95/p99 ms |
| --- | ---: | ---: | ---: | ---: | --- | ---: | --- |
| A | 33.627 | 33.440 | 50.156 | 75.60% | 0/1010 | 1 | 32.68 / 33.05 / 49.17 |
| B | 33.714 | 33.440 | 50.154 | 74.95% | 0/1011 | 1 | 32.60 / 33.03 / 49.20 |
| C v1 | 59.280 | 16.720 | 16.722 | 0.62% | 289/1783 | 2 | 49.88 / 61.82 / 66.55 |

A의 admission은 새 장면이 없다고 판단한 뒤 BeginPaint callback에서 장면을 생성했다. C는 callback을 준비한 후 eligible 장면을 기록했으며, 앞 same-queue terminal의 실제 GPU 종료 전에 새 raster를 시작한 기록을 확보했다. B에서는 warm 구간의 transaction/waitUntilScheduled가 0이지만 FPS 개선은 거의 없다. 세 정책의 capture **1170×920**, Gaussian **6패스**와 shader/설정은 같다.

C v1은 표시 60Hz 기준을 통과했지만 callback→표시 지연이 약 한 프레임 증가했다. **기본 채택 근거로 사용하지 않는다.** GPU 완료 시 자원을 회수한 뒤 drawable이 실제로 표시되기 전에도 다음 출력을 제출하여 presentation 대기열이 늘어나는 원인을 추가로 확인했다. 이는 물리 pointer 입력 지연의 직접 측정과 구분한다.

v2에서는 GPU 자원 회수와 presentation admission credit을 분리한다. GPU 완료 후에도 실제 표시 전인 프레임을 admission에 포함하여 shader-only outstanding 출력을 최대 두 개로 제한하고, 실제 drawable 표시 콜백이 credit을 돌려준다. framework 준비는 계속 진행해 최신 scene 하나만 유지한다. GPU/표시 콜백의 owner 처리는 admission 직전에 다시 drain한다. 자원 자체는 이전처럼 GPU 완료에 회수하며 scheduling/표시 시점을 GPU 완료로 대신하지 않는다.

v1 원본: `ios-frame-loop-nativeaot-adaptive20-abc-first/`, `ios-frame-loop-nativeaot-adaptive20-abc-rest/`. v1/2는 서로 다른 payload다. 당시 v2를 재검증했지만 아래 화면 갱신 중단 때문에 제거했으며, 최종 P3는 맨 위의 generation 후보만 사용한다.

## v2 → v3 표시 슬롯 반환 연결

v2 첫 Adaptive C는 **40.00 FPS**, 실제 overlap **489/1200**, 표시 대기 최대 **2**, warm dropped drawable **0**였다. callback→표시 p50/p95/p99는 **33.14 / 44.06 / 49.87ms**로 v1의 backlog를 줄였지만, 표시 슬롯 반환 후 다음 UIKit Draw까지 기다리는 비용으로 60Hz에 미달했다. 대응 A는 **33.95 FPS**, callback→표시 **32.67 / 33.06 / 49.19ms**였다.

v3은 실제 표시/GPU 완료 후 이미 준비된 새 shader scene의 raster admission만 즉시 깨운다. 해당 wake는 framework callback을 dispatch하지 않으며 callback 내부 재요청은 다음 display pulse에 남는다. viewport generation 일치, native 상태, fresh scene, GPU/표시 두 프레임 상한을 다시 검사한다. 장면 생성의 scene/frame/generation ID를 따로 기록하여 raster-only wake에서도 실제 생성 callback부터 표시까지 지연을 이어서 집계한다.

표시 콜백의 `PresentedTime == 0`은 [Apple 계약](https://developer.apple.com/documentation/metal/mtldrawable/presentedtime)에 따라 실제 표시로 집계하지 않는다. 기기에서 버려진 drawable의 presentation credit은 반환하되 GPU 자원은 terminal 완료 전 반환하지 않는다. 초기 empty frame과 warm dropped frame을 구분해 기록한다.

v2의 9회 비교는 원본에 보존한다. v3의 실제 경로·지연·반복 성능을 새 동일 payload로 검증한 뒤 확장/기본 후보를 결정한다. Python 검사는 scene ID로 준비 pulse와 raster wake를 잇는 회귀를 포함해 **8개** 통과했다.

## 표시 credit 실험의 실패와 render loop 후보

v2 repeat3 C는 실제 표시 이력이 **1.705초**에서 끝나 collector가 거부했다. v4 C는 **1.989초**, v5 C는 **1.204초**, v6 C는 **2.859초** 이력으로 거부됐다. 성공 실행만 선별하지 않았으며 모두 원본/failedRun 기록에 남긴다.

v5/v6의 preparation-only evidence 갱신으로 종료까지의 host 상태를 확보했다. framework는 약 40초 동안 다음 scene을 준비했지만 native 상태는 active/비 fault/미종료, GPU pending **0**, 모든 terminal 완료 상태에서 presentation credit **2**에 막혔다. v6의 두 drawable은 서로 다른 ID **114/115**, GPU retirement 완료, `PresentedTime=0`이었다. 같은 drawable 재사용 거절은 **0회**였다. renderer/Metal terminal 오류는 없지만 실제 화면 갱신이 중단된 **실패 후보**다.

표시 callback을 admission semaphore로 사용하는 실험은 제거했다. 기기에서 표시되지 않는 drawable의 통보를 무기한 기다리지 않는다. 자원은 실제 terminal GPU 완료로 회수하며 기존 GPU 두 프레임 상한을 유지한다. v7은 MTKView의 display link로 일반 shader frame pulse를 구동하고, framework 요청/새 scene/GPU pending이 없으면 render loop를 멈춘다. serial A/B와 native/rotation transaction 경로를 보존한다. v7의 표시 지연·성능을 다시 확인하며, 이전 실험을 기본 경로로 적용하지 않는다.

v3는 컴파일 후보이고 기기 성능 채택 자료가 없다. v4~v6은 진단 실패 자료다. 이전 성공 C v1의 약 59 FPS를 최종 후보의 반복 검증으로 대신하지 않는다.

## 추가 제거 후보 — drawable pool 2

기존 CAMetalLayer drawable pool을 3개에서 2개로 줄인 별도 NativeAOT 후보를 시험했다. Adaptive σ20 C는 **29.63 FPS**, overlap **0/889**, callback→표시 p50 **49.93ms**, renderer/terminal 오류 **0/0**이었다. CurrentDrawable p50/p95가 **16.82 / 28.17ms**로 늘었고 지연 개선도 없었다. 이 변경은 제거했다. 같은 payload의 최종 36회에 포함하지 않는다.

원본: `ios-frame-loop-drawable2-adaptive20/`, `ios-frame-loop-build-drawable2.log`. drawable pool의 기본값과 허용 범위는 [Apple 문서](https://developer.apple.com/documentation/quartzcore/cametallayer/maximumdrawablecount)에서 확인했다.

## native 검증 앱 준비

기존 Testbed 회전 probe는 anonymous object의 reflection JSON 직렬화 때문에 NativeAOT의 IL2026/IL3050 검사에서 빌드가 중단됐다. 회전 sample/update/result를 typed record와 source-generated JSON context로 바꿔 같은 SDK/runtime의 게시를 통과했다. 이 수정은 Testbed의 검증 코드이며 최종 SampleApp2 성능 payload와 호스트 소스를 바꾸지 않았다.

로그: `ios-frame-loop-build-native-testbed.log`(실패 보존), `ios-frame-loop-build-native-testbed-aot-json.log`(게시 성공).

FPS 수집과 분리한 실제 iPhone 자동 probe 결과:

- A/C에서 editor와 WebView를 각각 생성·재생성하여 native 생성 **4회씩** 통과. renderer/terminal 오류 **0/0**, GPU pending 최대 **1**. C에서도 native 직렬 admission **19회**를 확인했다.
- native editor가 있는 C에서 UIKit `RequestGeometryUpdate`로 landscape→portrait 실제 회전 통과. 중간 viewport 너비 **19/20개**, phase 오차 평균 **0.15/0.36pt**, 최대 **0.35/2.68pt**. 최종 pixel/safe-area 일치, 회전 display link 정지, GPU pending 최대 **1**, 오류 **0/0**.

원본과 별도 Testbed payload/probe hash: `temp/testing/variable-blur/ios-frame-loop-native-probes/identity.json`. 물리 IME·native button 조작과 손으로 회전하는 UX는 이 probe로 대신하지 않는다.

## benchmark 없는 초기 후보의 첫 진입·복귀 — 판정 철회

최종 성능 앱과 같은 NativeAOT payload를 다시 설치하고 **benchmark 환경변수 없이 C opt-in**으로 정상 Sample2를 실행했다. 첫 Components 화면의 text/control/tab 배치와 실제 screenshot을 확인했다. 성능 수집은 이 캡처 전에 종료했다.

Settings 앱을 활성화하여 background로 보낸 뒤 Sample2를 foreground로 복귀시켰다. **PID 6600 유지**, terminal 완료 수 **749→813**, renderer/terminal 오류 **0/0**을 확인했지만 이후 evidence가 813에서 갱신되지 않았다. 사용자도 Sample2 멈춤을 보고했다. **몇 frame의 초기 진행만으로 정상 복귀로 판정했던 결과는 철회한다.** 이 실행은 durable lifecycle 회귀의 통과 자료로 사용하지 않는다.

원본: `temp/testing/variable-blur/ios-frame-loop-manual-preflight/summary.json`, `before-background.json`, `after-foreground.json`, `first-entry.png`. Testbed도 benchmark/probe 없이 C 환경으로 background 실행해 두어 Platform views/WebView를 확인할 수 있게 했다. 두 앱을 강제 종료하거나 새 process로 재실행하면 기본 A가 적용된다.

### 남은 완료 조건

- 물리 input→scene→표시와 실제 손가락 스크롤·연속 slider·모드/강도 변경·페이지 재진입.
- native 버튼, 실제 IME/한글 및 WebView 문자 입력, 손으로 회전하는 UX.
- 최종 후보의 **10분 실제 사용과 Off 대조**, 시작/종료 자원·오류와 반복 멈춤/입력 지연 기록.
- 시작 타임아웃 원인 확인. 수정 전 clean NativeAOT와의 계측 비용 분리 기준은 확보하지 못했으며, 과거 Mono baseline으로 대신 판정하지 않는다.

기본 채택과 Full/Fast/Kawase의 조건부 확대는 보류했다. `work4.md` 전체 완료로 체크하지 않았고 `work3.md`의 기존 Fixed 게이트도 변경하지 않았다. 요청한 iOS 구현·자동 검증과 실제 수동 완료 조건을 구분한다.

## 사용자 보고 후 lifecycle 수정

사용자의 최초 “잘된다” 회신은 일반 동작의 정성 확인으로 남기고, 뒤의 “sample2는 멈춘건가”를 후속 문제 보고로 반영했다. 상세 손가락/IME 프로토콜이나 10분 통과로 확대하지 않았다.

- pipeline pause 시 `EnableSetNeedsDisplay=true`를 복구한다. paused MTKView에 `SetNeedsDisplay`만 보내던 resume를 `RequestFramePulse`로 바꿔 적절한 display link/직렬 redraw를 재시작한다.
- 실제 owner scene이 active일 때 연속 pulse를 시작한다. suspend 상태를 animator 중지 전에 게시하여 animator callback이 loop를 다시 켜지 못하게 한다.
- window detach에서도 pipeline을 중지하고 재부착 때 frame pulse를 요청한다. native/rotation 직렬 fallback은 SetNeedsDisplay 모드를 복구한다.
- lifecycle suspend/resume phase와 paused/needsDisplay/pulseRunning/ownerActive/frameRequested 상태를 진단에 남긴다.
- 실기기 lifecycle 검사는 Components animation의 terminal 완료 수가 복귀 직후와 **5초 뒤에도 계속 증가**하는지를 같은 PID로 3회 확인한다. 몇 초기 frame만으로 통과시키지 않는다.

첫 수정 NativeAOT 앱의 실제 복귀 후 완료 수는 **596→5282**로 증가하여 계속 렌더링됨을 확인했다. 다만 첫 두 lifecycle runner는 live evidence 파일 복사 중 socket close/timeout으로 중단됐다. rendering 통과로 바꾸지 않고 원본에 보존했다.

진단 파일을 쓰면서 읽는 경합을 피하기 위해 iOS opt-in evidence를 같은 directory의 임시 파일에 완전히 쓴 뒤 교체하도록 바꿨다. 수집기도 복사/JSON/timeout 실패를 보존하고 재시도한다. 변경된 앱의 성능·native·3회 lifecycle 검증은 이전 payload와 구분해 수행한다.

원본: `ios-frame-loop-build-resume.log`, `ios-frame-loop-build-resume-atomic.log`, `ios-frame-loop-resume-lifecycle/`, `ios-frame-loop-resume-lifecycle-retry/`. 재사용 가능한 기기 검사는 `Doroti/tests/ios_frame_loop_lifecycle_device.py`다.

atomic publication을 포함한 후속 앱의 반복 lifecycle 검사는 **3회 모두 통과**했다. 같은 PID로 복귀한 뒤 early→5초 후 late의 terminal 완료 수는 **596→917 / 1074→1395 / 1556→1882**였다. 각 구간 **321/321/326프레임**이 계속 진행됐다. owner active, pulseRunning=true, paused=false, pending 최대 2, renderer/terminal 오류 0/0이다. 이전 초기 64프레임 관찰을 이 지속 진행 검사로 대체했다.

후속 Sample2 payload SHA256: `93757deec6bda478f1ed02ea8f9648bfbaba3fcb98ceec87d8e664efe82bcca0`. 앱은 `Doroti/artifacts/ios-frame-loop-resume/bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app`다. 빌드/runtime은 위와 동일하며 payload/source는 이전 39회 자료와 다르다. 현재 binary의 성능 smoke와 native 재검증을 별도 기록한다. lifecycle 원본: `ios-frame-loop-resume-lifecycle-atomic/summary.json`.

후속 앱의 Adaptive σ20 A/B/C **각 1회 smoke**는 **39.91 / 37.21 / 59.73 FPS**, C p95 **16.72ms**, overlap **178/1791**, pending 최대 **2**, renderer/terminal 오류 **0/0**이다. 원본은 `ios-frame-loop-resume-adaptive20-abc-smoke/`다. 단일 smoke를 최종 binary의 3회 통계나 σ32 통과로 확대하지 않는다. Testbed 후속 binary도 게시했지만 native probe 재실행은 아직 하지 않았다.

사용자가 Sample2 정지를 다시 문의하여 자동 스크롤 검사 실행을 끝내고 **benchmark 환경변수 없는 일반 Sample2 수정본**으로 실행했다. 현재 explicit C 설정이며 새 process의 기본 A는 유지한다. launch/evidence는 `ios-frame-loop-resume-lifecycle-atomic/normal-final-*`에 보존한다. 자동 검사 실행은 40초 이후 ticker가 정지하고 collector가 수집 후 앱을 종료하므로 일반 앱의 수동 사용과 구분한다.

## 후속 요청 — Fast adaptive 기본값

사용자는 Fixed의 블러 품질과 Kawase의 형태를 이유로 Fast adaptive 기본값을 요청했고, 공개 API까지 함께 적용하는 범위를 명시했다. `ImageFilter.variableBlur`와 `ImageFilterConfig.CreateVariableBlur`의 기본값을 **fastGaussian + adaptiveResolution=true + resolutionScale=0.25**로 변경했다. Sample2의 초기 mode와 Off benchmark에서 다시 켰을 때 mode도 `fast`다.

약한 구간의 원본 해상도와 선명한 끝부분을 보존하는 기존 Fast adaptive 경로를 사용한다. 원본 해상도 Gaussian은 `resolutionScale: 1, kernel: VariableBlurKernel.gaussian`으로 명시할 수 있다. 기존 셰이더·Fixed/Kawase 옵션은 유지한다. 공통 CPU 회귀와 두 공개 entry point의 snapshot/정규화/bounds 및 명시적 Gaussian 검증을 통과했다.

원본 로그: `temp/testing/variable-blur/fast-adaptive-default-cpu.log`, `fast-adaptive-default-gpu.log`, `fast-adaptive-default-ios-build.log`. 이는 사용자가 지정한 블러 기본값 채택이며, iOS frame policy 기본 전환과 구분한다. 과거 Gaussian/Fixed 자료를 Fast adaptive의 새 성능 통과 증거로 사용하지 않는다.

기본값 적용본 검사 결과: 공통 CPU, public API/config snapshot 계약, macOS Graphite/Metal 픽셀, collector **8개** 통과. native/owned 및 async recording 보존 비교도 **0/255** 오차다. 새 NativeAOT 게시·설치를 완료했다.

| Fast adaptive, C | FPS | p95 ms | p99 ms | >25ms | overlap | pending 최대 | 오류 |
| --- | ---: | ---: | ---: | ---: | --- | ---: | --- |
| σ20, 1회 | 59.829 | 16.716 | 16.717 | 0% | 100/1794 | 2 | 0/0 |
| σ32, 1회 | 59.827 | 16.717 | 16.717 | 0% | 497/1794 | 2 | 0/0 |

40초 합성 스크롤의 첫 실제 표시 +5–35초를 집계한 단일 smoke다. 반복/물리 입력/10분 검증으로 확대하지 않는다. 새 payload는 `fc4e5c89d5dd5acacb34e08fce65aa8e260e58676362743459aae780003ec4a8`, 앱은 `Doroti/artifacts/variable-blur-fast-default/bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app`다. 같은 payload/runtime/source로 σ20/32를 비교했으며 공개 config source도 hash 대상에 추가했다.

수집 후 benchmark 없는 일반 Sample2를 C opt-in으로 다시 열었다. 기본 블러 선택은 Fast이며 iOS frame policy의 새 process 기본 A는 유지한다. launch는 `temp/testing/variable-blur/fast-adaptive-default-normal/launch.json`, [기본값 적용 집계](2026-10-02-fast-adaptive-default-summary.json)에 상세 identity와 수치를 기록했다.

## 사용자 아이폰 확인 후 C 기본 경로 적용

사용자가 “한번 아이폰확인했는데 잘 되네 기본경로로하자”라고 요청하여 iOS의 미설정 경로를 **C**로 바꿨다. `IosFrameLoopOptions.Resolve`가 기본 pipeline+async와 명시 A/B 복귀 옵션을 결정한다. Mac Catalyst의 기존 기본 선택은 유지한다. native·회전·replay admission과 GPU 자원 수명은 기존 검증 구현을 그대로 사용한다. Fast adaptive의 API·Sample2 기본값도 유지한다.

명시적 `DOROTI_VARIABLE_BLUR_SERIAL_FRAMES=1` 또는 `DOROTI_VARIABLE_BLUR_PIPELINE=0`은 A이며 `DOROTI_IOS_SHADER_PRESENTATION=transaction`도 A다. serial 설정에 presentation=async를 더하면 B, 미설정/명시 pipeline은 C다. CPU 검사에서 8가지 기본/우선순위 조합을 확인했다. 수집기 역시 정책 옵션 미설정 시 C를 측정한다.

새 NativeAOT 앱 payload는 **`3b2d6b7afe6fee2eedccfb010d4ac294f27a3541457515291d2d603b7a189b75`**다. 빌드·설치 후 `ios_frame_loop_lifecycle_device.py --default-path --cycles 3`로 확인했다. 실행 환경은 profile/evidence 두 항목뿐이며 Graphite/pipeline/serial/presentation은 전혀 지정하지 않았다. 실제 진단 정책 C, GPU pending 최대 2, frame/terminal 오류 0/0이다. 같은 PID **6912**의 복귀 후 5초 지속 완료 증가가 **320 / 323 / 324**였다.

그 뒤 **환경변수 설정을 전혀 전달하지 않은** 일반 Sample2로 새로 실행했다. PID **6920**가 4초 후에도 실행 중임을 확인했고 앱을 그대로 두었다. benchmark와 per-frame 진단 모두 꺼져 있다. 사용자 손가락 확인은 이전 후보의 일반 동작 확인이며 이 새 payload의 10분/정량 입력 프로토콜로 확대하지 않는다.

원본: `temp/testing/variable-blur/ios-default-C-build.log`, `ios-default-C-policy.log`, `ios-default-C-collector.log`, `ios-default-C-lifecycle/`, `ios-default-C-normal/`. [C 기본 적용 집계](2026-10-02-ios-default-C-summary.json)에 identity/source/payload와 실행별 설정을 보존했다. 최종 전체 반복·정량 입력 지연·10분 Off 비교는 별도 미검증 항목으로 남긴다.

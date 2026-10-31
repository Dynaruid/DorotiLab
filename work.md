# 갤럭시 스크롤 지연과 관성 감속 끊김 개선 계획

작성일: 2026-10-31 (Asia/Seoul)  
검토 기준: `de6664ed913e733673b1adef9c9771c4be210d75`  
상태: **공통·Android 수정 및 SkiaSharp 4.154.0 정식 전환 검증 / 성능 PARTIAL**

실행 기록: [공통 엔진·Android 구현과 갤럭시 검증](history/26/10/31/galaxy-scroll-implementation.md).
기준/첫 후보 3회 비교와 연속 단일 접촉 드래그를 실행했다. 공통 경로의 할당을 줄였지만,
Perfetto에서 UI 스레드의 Vulkan graphics pipeline 생성 지연이 확인되어 성능 합격은 아직 아니다.
이후 사용자의 최신 SkiaSharp 도입 요청에 따라 공식 최신 안정판 `4.154.0`으로 전환했다.
Release arm64 빌드·Scroll 회귀·Windows 실행을 통과하고 갤럭시 3회 비교를 수행했다.
정식판에서도 주 비교 owner 최대 56.8ms, 별도 연속 드래그 최대 65.1ms가 남아
스크롤 성능 합격 조건은 미충족이다.
실제 손가락 체감·전체 layer deadline·활성 TalkBack action은 `notVerified`로 유지한다.

추가 요청인 [텍스트 커서 핸들 드래그](history/26/10/31/galaxy-caret-drag.md)도 구현·검증했다.
Android OS 확대경으로 반복 렌더링 예외와 첫 드래그의 큰 지연을 줄이고,
변하지 않은 글자 위치 정보의 반복 생성을 제거했다. 수정 APK를 갤럭시에 설치했다.
일반 필드 최대 owner는 133.9–167.0ms에서 38.7–40.1ms로 줄었지만 p95는 증가하여,
지속 드래그 성능은 여전히 `PARTIAL`이다. 원시 제출 지표를 표시 FPS로 간주하지 않는다.

## 1. 목표와 범위

사용자의 갤럭시에서 스크롤 중 간헐적으로 멈칫하는 지연을 제거하고,
손을 뗀 뒤 관성이 감속하여 정지하는 동안의 미세한 끊김을 개선한다.
전체 화면과 목록이 움직이는 실제 느낌을 최종 기준으로 삼는다.

공통 엔진의 스케줄러·스크롤·레이아웃·semantics 경로와 Android 호스트의
입력·Choreographer·Graphite/Vulkan·SurfaceView 표시 경로를 **양쪽 모두 필수 검토·수정 범위**로 둔다.
각 영역의 개선 대상과 수정안을 도출하고, 확인된 문제는 해당 영역에서 수정·검증한다.
공통 엔진·Android 플랫폼·두 영역의 연결 경계마다 검토 결과와 변경 효과를 남긴다.
이 계획 작성 당시에는 소스 검토만 수행했다. 후속 전체 작업 요청으로 구현·APK 교체·
실기기 측정을 진행하고 있으며, 결과와 미충족 조건은 실행 기록에서 구분한다.

### 1.1 영역별 책임과 구현 원칙

| 영역 | 필수 검토·수정 대상 | 결과로 남길 근거 |
| --- | --- | --- |
| 공통 엔진 | scheduler/ticker의 시각 진행·frame 요청, drag→ballistic→idle 전환, simulation 재시작·소수점 offset, build/layout/paint/semantics dirty 전파와 할당 | 같은 입력·시각을 재생한 연속성 회귀, 단계별 CPU/할당 비용, 엔진 변경 전후 결과 |
| Android 플랫폼 | MotionEvent 시각·history, Choreographer와 가변 주사율, UP 후 callback 유지, invalidate/retry, UI thread 점유, SurfaceView와 GPU 제출·표시 | 갤럭시 callback/queue/GPU/layer 기록, 플랫폼 변경 전후 결과와 native 기능 회귀 |
| 연결 경계·공유 렌더러 | input→engine tick→scene→raster→submit의 시각·순서·소유권, admission과 GPU completion, 최신 scene 전달 | 동일 gesture/frame/scene ID 연결, 누락·중복·과거 scene 표시 여부, 공유 Vulkan 소비자의 회귀 |

공통 상태 전이·불필요한 작업·할당 문제는 공통 엔진에서 고치고,
Android의 native callback·시각 변환·표시 정책 문제는 플랫폼에서 고친다.
두 영역에 걸친 결함은 양쪽 계약과 구현을 함께 수정한다. `GraphiteVulkanWindow`처럼
여러 플랫폼이 공유하는 코드를 변경하면 다른 소비자의 수명·동기화 계약도 확인한다.
문제가 없다고 판단한 세부 항목은 측정·재생 근거를 남기며, 변경 자체를 만들기 위한 수정은 하지 않는다.

## 2. 현재 확인한 사실

### 2.1 연결된 갤럭시 — 이번 검토에서 ADB로 확인

| 항목 | 확인값 |
| --- | --- |
| 연결 | USB ADB `device`, arm64-v8a |
| 모델 | `SM-S931N` |
| OS | Android 17 / API 37, One UI 속성 `90000` |
| 화면 | 1080 × 2340, density 480 |
| 디스플레이 | modeId 1, 보고된 renderFrameRate 약 120Hz |
| 주사율 정책 | SEAMLESS, physical 허용 범위 10–120Hz |
| GPU 속성 | `ro.hardware.vulkan=adreno`; 실제 Vulkan device/driver는 추후 확인 |
| 설치 앱 | `dev.doroti.testbed`, 0.4.1 / versionCode 5, targetSdk 36 |
| 설치 APK SHA-256 | `3342af069f581e1b8dcdeb91640c2caac831627426b462a988f368c06d27a20d` |
| 앱 상태 | 프로세스 실행 중; 패키지 플래그에 DEBUGGABLE 없음 |
| 접근성·전원 | 접근성 서비스 비활성, enabled services `null`, low_power `0` |

주사율 값은 장치가 보고한 모드이며 앱의 표시 FPS 측정값이 아니다.
현재 APK의 Mono/CoreCLR/AOT 설정과 소스 기준 커밋 일치는 아직 확인하지 않았다.
`DEBUGGABLE`이 없다는 사실만으로 AOT 또는 최적화 설정을 확정하지 않는다.
초기에 연결 기기가 없었으나 사용자가 USB 디버깅을 켠 뒤 위 정보를 확인했다.

### 2.2 이전 수정·측정의 범위

근거: [2026-10-31 검증 기록](history/26/10/31/scroll-inertia-review.md).

- 입력의 하드웨어 시각을 유지하고, release velocity가 처리 지연 때문에 0이 되는 문제를 수정했다.
- 접근성 소비자가 없을 때 Android 가상 노드의 매 프레임 인덱싱·비교·알림을 지연했다.
  provider 조회 시 최신 트리를 반영하며, 활성 접근성 서비스에는 즉시 갱신한다.
- x86_64 에뮬레이터에서 접촉 구간 owner 처리 시간의 회차별 p95 중앙값은
  20.701 → 15.074ms로 개선됐다. 여러 수정을 합친 결과여서 접근성 단독 효과는 미분리다.
- 수정 후에도 owner 최대 54.6–60.1ms, 최초 접촉 dispatch 최대 94.2–103.2ms가 남았다.
- 이 수치는 갤럭시 실기기나 관성 감속 구간의 표시 간격 합격을 증명하지 않는다.
  120Hz의 간격은 약 8.33ms이므로 60Hz의 16.67ms만으로 목표를 잡지 않는다.

## 3. 소스 검토 결과와 확인할 가설

아래는 조사 후보이며, 갤럭시 지연의 원인으로 확정한 항목은 없다.

| 우선순위 | 확인한 구조와 조사할 위험 | 주요 소스 |
| --- | --- | --- |
| 1 | Choreographer 시각을 이미 framework clock으로 매핑한다. 접촉 유지용 재등록과 UP 후 관성의 callback 재등록·pending 해제 경로가 다르므로, 손을 뗀 뒤 pulse 누락·추가 대기·오래된 시각 소비를 추적한다. | [MauiHostAdapter](packages/platforms/maui/Doroti.Host.Maui/MauiHostAdapter.cs), [callback queue](packages/platforms/maui/Doroti.Host.Maui/MauiFrameCallbackQueue.cs) |
| 1 | `DrawFromVsync`에는 이중 PostOnAnimation을 피하는 처리가 이미 있다. GPU admission 거절 후 재시도와 completion polling도 PostOnAnimation을 사용하므로 중복 wake, 누락된 새 scene, 한 refresh 지연을 구분한다. | [Android Vulkan host](packages/platforms/maui/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs) |
| 2 | Vulkan에는 최대 2개 in-flight 슬롯, 비차단 fence polling과 pipelined acquire timeout 0이 이미 있다. QueuePresent 비용, busy/unavailable 반복, 공유 자원·texture 등의 동기 fence 분기와 UI thread 점유를 측정한다. | [Vulkan window](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.cs), [frames](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.Frames.cs), [admission policy](Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs) |
| 2 | Android bridge의 지연 갱신은 공통 semantics 생성·레이아웃 비용 전체를 없애지 않는다. 긴 프레임과 managed 할당/GC, Java bridge 작업, build/layout/paint/semantics 생성이 겹치는지 확인한다. | [rendering binding](Doroti/src/Doroti.Framework.Rendering/binding.cs), [Android semantics](packages/platforms/maui/Doroti.Host.Maui/MauiAndroidSemanticsBridge.cs), [work profile](Doroti/src/Doroti.Ui/FrameworkWorkProfile.cs) |
| 3 | 관성은 AnimationController의 시각과 시뮬레이션 값으로 진행한다. dimensions 변경 때 `goBallistic`을 다시 호출하므로 불필요한 재시작, 시각 보정, 마지막 이동·정지 전환을 확인한다. | [scroll activity](Doroti/src/Doroti.Framework.Widgets/scroll_activity.cs), [scroll position](Doroti/src/Doroti.Framework.Widgets/scroll_position_with_single_context.cs), [simulation](Doroti/src/Doroti.Framework.Widgets/scroll_simulation.cs), [ticker](Doroti/src/Doroti.Framework.Scheduler/ticker.cs) |
| 3 | 입력은 EventTime을 보존하지만 현재 touch dispatch는 MotionEvent의 현재 위치를 사용한다. history 누락이 샘플 간격·release 추정에 영향을 주는지 조사한다. UP 이후의 표시 지연과는 별도 가설이다. | [pointer mapping](packages/platforms/maui/Doroti.Host.Maui/AndroidPointerMapping.cs), [subscription](packages/platforms/maui/Doroti.Host.Maui/AndroidPointerSubscription.cs), Android Vulkan host |

저속 구간은 logical offset → scene transform → raster 좌표 → 표시된 위치를 비교한다.
반올림이 원인이라는 증거는 아직 없다. 한 프레임 이동이 1 physical pixel보다 작은
정상적인 반복 화면과, 계산된 이동을 놓친 뒤 크게 뛰는 현상을 구분해야 한다.

## 4. 측정 설계

### 4.1 같은 갤럭시·같은 앱 조건 고정

1. 현재 설치 APK와 사용자가 증상을 본 실행 모드·화면·동작을 먼저 보존하고 재현한다.
   실제 설치 APK의 런타임과 빌드 설정, 네이티브 라이브러리·드라이버를 확인한다.
2. 주 성능 비교는 최적화된 Release arm64로 기준/후보를 맞춘다. Mono/CoreCLR,
   AOT/interpreter, trimming을 동시에 바꾸지 않는다. Debug에서만 재현되면 별도 결과로 남기고,
   Release 합격만으로 사용자가 쓰는 Debug 경로까지 해결했다고 처리하지 않는다.
3. 기본 120Hz/가변 주사율 모드를 주 검증 조건으로 사용한다. 실제 refresh 전환과
   expected present 간격을 기록하고 60Hz 비교는 원인 분리가 필요할 때 보조 조건으로 쓴다.
4. 배터리·충전·온도·절전·접근성 상태를 회차별 기록한다. 측정 중 빌드, 화면 녹화,
   UIAutomator 순회, layout/allocation 상세 프로파일을 동시에 실행하지 않는다.
5. 첫 프레임·자원 초기화 구간, 첫 접촉, warm 스크롤을 따로 집계한다.
   셰이더·글리프 준비 등의 최초 비용도 버리지 않고 사용자 증상과 연결해 보고한다.

### 4.2 구간과 동작

기본 화면은 사용자가 증상을 본 Testbed Components 목록으로 잡고 정확한 목록·위치는 재현 시 확정한다.
스크롤 끝에 닿지 않는 중간에서 시작하며 초기 위치 복원은 측정 구간 밖에서 한다.

| 구간 | 관찰할 내용 |
| --- | --- |
| DOWN 및 최초 MOVE | 하드웨어 이벤트 age, UI queue, focus 처리, 첫 framework dispatch 지연 |
| 손가락 접촉 이동 | 입력 샘플 간격, 움직임과 frame 요청의 대응, build/layout/paint 비용 |
| UP 직후 | release velocity, 마지막 MOVE→UP 간격, 첫 ballistic tick과 첫 새 scene/표시 사이의 틈 |
| 관성 중·후반 | 일정 cadence, GPU 재시도, frame/scene 누락·반복, simulation 재시작 |
| 저속 감속·정지 | fractional offset, 예상 이동 대비 표시 이동, tolerance와 마지막 정지 전환 |
| 재접촉·역방향 | 즉시 관성 중단, 남은 과거 frame의 뒤늦은 표시·위치 튐 여부 |

한 조건에서 기준 3회·후보 3회, 회차당 중속/빠른 fling/느린 감속 확인을 각 1번 수행한다.
주 비교는 총 18동작으로 시작한다. 원인 분리용 추가 동작은 핵심 검증 전체 30회 이내를
우선 목표로 하고, 실패·변경·자료 부족이 있는 경우에만 이유를 남기고 추가한다.
전체 모드와 모든 화면의 조합을 반복하지 않는다. 손가락 재현과 ADB 입력은 별도 표기한다.

### 4.3 수집할 자료와 연결 키

- gesture ID, pointer ID, input event 시각/도착/dispatch 시각, MOVE history 수.
- native vsync 시각, callback 실제 실행 시각, framework tick, request/invalidate/paint 시각.
  Android uptime·nanoTime 계열과 framework clock의 변환·단조성·resume 재설정을 확인한다.
- frame/scene ID, scroll offset·velocity·activity·extent, activity 교체 사유,
  framework에서 계산한 scene과 실제 raster/submit한 scene의 대응.
- build/layout/paint/semantics 생성 및 bridge 적용 비용, 할당량과 GC pause,
  UI thread runnable 대기, Vulkan acquire/submit/fence/present 호출 비용.
- admission 거절 사유, in-flight 수, busy/unavailable 수, completion 처리와 retry 시각.
- 가능하면 해당 SurfaceView layer의 latch/present 시각·buffer/frame ID와 기대 표시 시각.

기존 `DOROTI_INPUT_TIMING`, `DOROTI_PLATFORM_FRAME_PROFILE`은 초기 확인에 활용한다.
본 비교는 사전 할당한 bounded memory ring/집계와 선택적 trace marker를 우선 사용하고,
측정 후 내보낸다. 매 MOVE/프레임 문자열 생성·logcat·파일 쓰기의 계측 영향을 따로 비교한다.
`FrameworkWorkProfile`의 layout/allocation 계측은 원인 조사 회차에만 켜며,
Android 실행 경로에서 flag를 실제 전달하는 방법도 확인한다.
managed GC·ART GC·native allocation을 구분하고 현재 런타임이 지원하는 수집기를 선택한다.

### 4.4 처리 완료와 화면 표시 증거 구분

`ownerMs`, QueuePresent 성공, GPU fence 완료는 화면 표시 FPS의 대체값이 아니다.
Perfetto로 UI thread·CPU scheduling·GPU·SurfaceFlinger를 함께 보되,
상위 MAUI window의 FrameTimeline을 Doroti SurfaceView의 표시 기록으로 오인하지 않는다.
공식 [FrameTimeline 문서](https://perfetto.dev/docs/data-sources/frametimeline)는
SurfaceViews 지원 제한을 명시한다. 실제 기기의 layer/buffer 연결 가능 여부부터 확인한다.

해당 layer의 표시 시각이 FrameTimeline에 없으면 장치에서 제공되는 SurfaceFlinger
layer timing 또는 지원되는 Vulkan display timing 경로를 조사한다. 지원되지 않는 확장이나
권한을 전제하지 않는다. 관측 불가 시 `presentObserved=false`로 남기고 처리 시간·실기기
육안 결과를 각각 보고한다. 화면 녹화 FPS만으로 합격을 선언하지 않는다.

## 5. 작업 순서와 완료 조건

2026-10-31 실행 상태는 다음과 같다. 아래 체크는 개별 작업의 수행 여부이며,
전체 성능 합격을 뜻하지 않는다. 복합 조건에서 미검증 부분이 남으면 체크하지 않았다.

| 단계 | 수행한 작업 | 남은 조건 |
| --- | --- | --- |
| P0 | 원본 APK 백업·hash, 재현 화면 직접 확인, 소스 기준 Release 및 최종 APK 고정 | 원본 설치 APK의 정확한 source revision 복구 불가 |
| P1 | input/pulse/frame/scene/submit ring, 기준 3회·GC/할당 집계, Perfetto 병목 분해 | 전체 표시 deadline 연결, logger 오버헤드 단독 분리 |
| P2 | callback/queue 검토, 최초 지연 callback의 clock 매핑 수정, 최신 SkiaSharp 검증 | UI 스레드 layout·pipeline 생성의 긴 점유 해소 |
| P3 | 실제 scheduler/scroll 회귀, semantics·행렬 할당 축소, provider 조회 | 활성 TalkBack action/focus 실기기 검증 |
| P4 | 60/120Hz·누락 pulse 재생, scene 순서 집계, 연속 단일 접촉 드래그 | 표시와 offset의 전 구간 연결 및 체감 합격 |
| P5 | 기준/첫 후보/정식판 각 3회, Windows 휠·native view·IME, Android 전환·IME·resume | 50ms/CPU 예산, 실제 손가락·트랙패드 확인 |

### P0. 실기기 기준 고정

- [x] USB 갤럭시 연결, 모델·OS·주사율 정책·설치 APK 식별값을 읽기 전용으로 확인.
- [ ] 설치 APK 런타임/빌드 설정·source 일치 및 사용자의 실제 재현 화면 확인.
- [x] 현재 APK를 기준으로 재현하고 Release 비교 기준을 확보.
- 산출물: device/build manifest, APK hash, 재현 시나리오와 프로파일별 상태.

### P1. 공통 엔진과 플랫폼을 연결하는 계측과 기준 기록

- [ ] 4장의 구간·연결 키를 구현하고 기준 3회 기록. 간헐적 긴 프레임의 전후 흐름 보존.
- [ ] 표시 관측 가능 범위, logger 오버헤드, runtime GC 수집 방법 확인.
- [ ] 원인을 callback 대기 / CPU 작업·GC / GPU·present / offset·physics 중 하나로
  단정하기 전에 각 stall의 소요 시간을 분해하고 설명되지 않는 시간도 별도 집계.
- [ ] 공통 엔진 비용·시각 진행과 Android callback/queue/GPU 비용을 각각 집계하고,
  두 영역을 연결하는 frame/scene 기록을 확보. P2와 P3의 검토·수정 대상을 각각 도출.
- 완료 조건: 최초 접촉과 관성 감속의 문제 frame을 input→tick→scene→submit으로 추적 가능.
  병목과 긴 프레임의 시각적 증상이 연결되지 않으면 수정 대상을 확정하지 않는다.

### P2. Android 플랫폼 필수 검토와 수정 — frame cadence·UI thread·GPU

- [ ] UP 후 pending callback 소비·재등록, GPU full/retry와 invalidate coalescing을 검증.
  정상적인 한 pulse당 한 번의 framework 진행과 다음 pulse 유지, idle 시 종료를 보장.
- [ ] 실제 가변 refresh와 clock 매핑, 지연 frame 뒤의 animation time 처리를 검토.
  API/바인딩이 지원하면 VsyncCallback의 deadline/expected-present 정보 활용을 비교한다.
  신규 API 존재를 가정하거나 시간 증가량을 임의 고정하여 끊김을 숨기지 않는다.
- [ ] QueuePresent·동기 fence 등 UI thread의 확인된 긴 점유를 줄인다.
  이미 있는 비동기 경로를 재구현하거나 in-flight 수만 늘려 지연을 뒤로 미루지 않는다.
  native view/texture의 GPU 자원 수명·순서와 surface 종료 계약을 유지한다.
- 완료 조건: 기준에서 확인한 pulse 손실/대기 또는 긴 UI 점유가 후보 기록에서 사라지고,
  input-to-new-scene 지연과 GPU 대기열이 악화되지 않음.

### P3. 공통 엔진 필수 검토와 수정 — 스크롤 진행·작업량·할당

- [ ] scheduler/ticker와 drag→ballistic→idle의 시각·요청·상태 전이를 재생 검증하고,
  pulse 누락 뒤의 위치 점프, 불필요한 activity 재시작·frame 요청 문제를 공통 경로에서 수정.
- [ ] 스크롤 프레임의 build/layout/paint/semantics dirty 전파와 할당 상위 항목 확인.
- [x] 실제로 변하지 않은 작업, tree 복사·열거·hash, 단명 객체를 비용 근거에 따라 축소.
  최신 semantics snapshot과 실제 접근성 action dispatch의 분리는 유지.
- [ ] 접근성 비활성/조회/활성 서비스를 별도 확인. 활성 서비스나 focus가 있는 경우의
  최신 위치·스크롤 action·노드 제거·포커스 갱신을 지연하거나 접근성을 일괄 끄지 않음.
- 완료 조건: 긴 frame/GC와 연관된 작업량 감소를 확인하고 기능·노드 최신성 회귀 없음.
  접근성 단독 효과와 다른 변경의 합산 효과를 구분하여 기록.

### P4. 공통 엔진·플랫폼 연결과 관성 감속 말미의 연속성 개선

- [ ] 엔진에서 계산한 offset/scene과 Android가 raster/submit한 scene을 같은 ID로 대조.
  엔진 진행이 정상이어도 플랫폼이 이전 scene을 반복하는 경우와, 엔진 값 자체가
  불연속인 경우를 구분하여 해당 구현과 연결 계약을 수정.
- [x] 동일 시뮬레이션을 60/120Hz 및 누락 pulse(50ms) 시각으로 재생하여 offset/velocity 비교.
- [ ] 불필요한 `applyNewDimensions`/activity 교체, timestep 점프, 중복·누락 scene을 조사.
- [ ] fractional offset 보존과 최종 raster 변환을 확인. 조기 반올림이 확인될 때 해당 경계만 수정.
  자연스러운 subpixel 정지와 긴 정지 뒤의 위치 점프를 별도로 판정.
- [ ] history 샘플이 문제이면 시각·순서·다중 pointer·UP/CANCEL을 보존하여 입력을 보완.
  history 전체에 불필요한 build/paint를 실행하거나 처리량을 늘리는 방식은 피함.
- 완료 조건: simulation 값이 연속적이며 설명되지 않는 plateau→jump·역방향·조기 정지 없음.
  마찰/감속 상수나 정지 tolerance를 먼저 바꿔 관성을 짧게 만드는 방법으로 해결하지 않음.

### P5. 후보 검증과 사용자 체감 확인

- [ ] 공통 엔진과 Android 플랫폼의 검토·수정 결과를 각각 정리하고 두 영역을 함께 적용한
  최종 후보를 검증. 변경 효과는 가능한 한 개별 후보의 짧은 진단·동일 입력 재생으로 확인하고,
  4.2의 추가 동작 예산 안에서 수행. 주 비교 18동작의 후보는 두 영역을 통합한 최종 APK로 고정.
- [x] 기준과 같은 갤럭시·빌드 프로파일·화면·동작으로 후보 3회 비교.
- [ ] 기본 가변 주사율을 먼저 검증하고 필요 시 60Hz, 접근성 활성, 화면 전환·resume를 보조 확인.
- [x] 입력 시각/관성 연속성 및 실제 수정 경계에 의미 있는 회귀 테스트 추가.
  기존 [Scroll 회귀 테스트](Doroti/tests/Doroti.Scroll.Tests/README.md)도 실행.
- [ ] 공통 엔진 수정 시 Windows 트랙패드와 일반 휠, Android native editor/IME·접근성
  action에 필요한 범위의 smoke 확인. 변경과 무관한 전 플랫폼 재검증은 확대하지 않음.
- [ ] 설치된 최종 APK hash를 확인하고 사용자가 손가락으로 감속 말미까지 확인.
- 산출물: 공통 엔진·Android 플랫폼·연결 경계별 검토/수정/회귀 결과,
  기준/후보 회차별 지표, 개별 변경과 통합 변경의 효과, 잔여 지연, 실기기 확인 결과.

P1 이후 P2와 P3는 양쪽 모두 수행하고, P4에서 연결 경계를 확인한 뒤 P5로 진행한다.
한 영역의 지표 개선으로 다른 영역의 검토·필요한 수정·검증을 생략하지 않는다.

## 6. 목표 지표와 합격 판정

아래 수치는 **앞으로의 목표**이며 이미 달성한 결과가 아니다.
기본 120Hz 모드의 기대 간격 약 8.33ms를 출발점으로 하되 실제 주사율 전환 시에는
해당 구간의 기대 간격 `T`와 deadline을 사용한다. idle을 120fps로 강제하지 않는다.

| 항목 | 목표/판정 |
| --- | --- |
| 간헐적 지연 | 정해진 비교 구간에서 프로젝트에 귀속되는 50ms 이상 stall 0회. 최초 접촉과 warm/관성 결과를 모두 제시 |
| 관성 cadence | 정상적인 subpixel 반복 화면·정지 tolerance 구간을 제외하고, 이동이 계속 예상될 때 설명되지 않는 2T 초과 새 frame 공백 0회. 표시 기록이 없으면 submit cadence 합격과 표시 합격을 구분 |
| deadline | 표시 연결이 가능한 안정 주사율 구간에서 앱 귀속 miss 비율 1% 미만을 목표. p50/p95/p99/max와 최대 연속 miss 수를 함께 보고 |
| CPU 여유 | 주요 owner 작업 p95 ≤ 0.8T를 예산 목표로 사용. 비동기 파이프라인의 전체 표시 합격을 이 값 하나로 대체하지 않음 |
| 저속 이동 | 예상 누적 이동이 physical pixel 단위로 충분한데도 멈췄다가 뛰는 현상, 불필요한 ballistic 재시작·역방향 이동 0회 |
| 기능 | 재접촉 취소, 경계/overscroll, native focus·IME, 최신 접근성 조회/action·focus 정상 |
| 완료 범위 | 공통 엔진·Android 플랫폼·연결 경계의 검토와 확인된 결함 수정, 영역별 회귀 및 통합 실기기 검증 근거 확보 |
| 최종 체감 | 같은 최종 APK에서 사용자가 간헐적 멈칫과 감속 구간 끊김 개선을 실기기로 확인 |

평균/p95만 좋아지고 긴 꼬리나 감속 말미 끊김이 남으면 완료 처리하지 않는다.
실기기 관측·사용자 확인이 없거나 표시 경로가 불명확하면 해당 항목은 `PARTIAL`/
`notVerified`로 남긴다. 기록에 없는 외부 요인으로 outlier를 임의 제외하지 않는다.

## 7. 실행 명령과 결과 보존

저장소 루트 PowerShell 기준. 아래 명령을 실제 실행했으며 빌드/회귀/실기기 결과는 실행 기록에 보존한다.
Android 테스트·빌드에는 저장소 지침의 20분 timeout을 적용한다.

```powershell
$adb = Join-Path $env:LOCALAPPDATA 'Android/Sdk/platform-tools/adb.exe'
& $adb devices -l

python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet msbuild ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -p:Configuration=Release -p:RuntimeIdentifier=android-arm64 '-getProperty:UseMonoRuntime,RunAOTCompilation,AndroidEnableProfiledAot,AndroidUseInterpreter,Optimize,DorotiCompilationMode'

python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal

python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

실기기 작업의 모든 ADB 명령은 확인한 serial을 `-s`로 지정한다. `android-x64`
에뮬레이터 APK를 갤럭시 비교에 사용하지 않는다. 최종 설치 전후 APK hash를 대조한다.
원시 trace/APK/log는 ignored 임시 폴더에 두고, 재현 조건·커밋·hash·집계·판정은
작업 종료 시 `history/YY/MM/DD/`의 검증 문서에 보존한다. 계측·임시 우회는 정리한다.

## 8. 참고 자료

- [Android rendering 진단](https://developer.android.com/topic/performance/issues/render): Release에 가까운 조건에서 실제 동작과 trace를 함께 확인.
- [Choreographer.FrameCallback](https://developer.android.com/reference/android/view/Choreographer.FrameCallback): callback 시각과 animation 시각의 의미를 확인하고 실제 API/바인딩 지원에 따라 적용.
- [Choreographer.FrameData](https://developer.android.com/reference/android/view/Choreographer.FrameData): preferred timeline/deadline/expected-present 활용 가능성 확인.
- [Perfetto FrameTimeline](https://perfetto.dev/docs/data-sources/frametimeline): SurfaceView 관측 범위와 실제 표시 지연의 원인 구분.

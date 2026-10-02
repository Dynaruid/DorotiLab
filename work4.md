# iOS 렌더링 작업계획 — 프레임 생성·GPU 제출·표시 연결 수정

작성일: **2026-10-02** · 대상: SampleApp2 / iOS Graphite-Metal 호스트 / iPhone 12, 60Hz
상태: **iOS 구현·CPU/Metal 회귀·수정 전 후보 39회 유효 비교 및 native 생성/회전 완료. Sample2 lifecycle 수정·지속 복귀와 Fast σ20/32 smoke 검증 완료. 사용자의 아이폰 확인과 명시적 지시에 따라 iOS C 경로 및 공개 API/Sample2 Fast adaptive 기본값을 채택했다. 전체 재반복·정량 입력 지연·10분 사용은 별도 미검증 범위다.**

아래 단계별 계획과 실행 이력의 당시 기본 A/Fixed 유지·채택 보류 조건은 후속 사용자 지시로 대체됐다. 이 결정은 미수행 측정 항목을 통과한 것으로 처리하지 않는다.

**목표:** 일반 스크롤에서 다음 framework 장면을 준비하는 일이 앞 프레임의 GPU 완료 대기에 묶이지 않도록 iOS 호스트를 수정한다.
native overlay가 없는 장면의 불필요한 transaction presentation을 제거하고,
실제 CPU 기록/GPU 실행의 overlap과 표시 성능을 같은 바이너리에서 검증한다.
Fixed의 현재 성능·화질을 유지하면서 Adaptive/Fast/Kawase의 표시 성능을 개선한다.
Full quality를 포함한 모든 옵션의 60 FPS 달성 여부는 별도로 판정한다.

작업 순서: **실행 경로 진단 → framework 프레임 준비와 raster admission 분리 → 표시 경로 분리 → 정확성 검증 → 같은 바이너리 성능 A/B → 기본 적용 및 지속 사용 확인**.

## 1. 근거와 확인해야 할 가설

사용자는 **같은 iPhone 12의 Safari**에서 웹 빌드의 VariableBlur 옵션들이 모두 60 FPS에 가까워 보이며,
네이티브에서는 Fixed 1/4만 부드럽다고 보고했다.
웹의 수치는 체감 관찰이다. 해당 웹 빌드의 revision·backend·DPR·표시 FPS를 아직 계측하지 않았다.

현재 소스의 Safari 기본 경로는 **Ganesh/WebGL2**, 네이티브 계측 경로는 **Graphite/Metal**이다.
공통 블러 구현에 웹만 블러를 생략하거나 샘플 수를 낮추는 분기는 발견하지 못했다.
웹은 기본적으로 실제 `devicePixelRatio`를 사용하지만, Safari viewport·실제 작업 크기가 네이티브와 같다는 뜻은 아니다.

기존 Release/Mono의 sigma 20 같은 바이너리 3회 결과:

| 모드 | 평균 표시 FPS | 보존 trace의 CPU 기록 시간 중앙값, 약 | 기록 시작→완료 통보 간격 중앙값, 약 |
| --- | ---: | ---: | ---: |
| Off | 59.35 | 3.5ms | 9.7ms |
| Fixed 1/4 | 59.09 | 3.4ms | 11.9ms |
| Adaptive | 33.41 | 5.3ms | 19.0ms |

trace 수치는 실행 후반부의 제한된 보존 이력에 대한 재분석이며, 30초 전체의 구간별 성능 측정은 아니다.
완료 통보 간격에는 flush·Snap/Insert/Submit·GPU 실행·콜백 처리가 함께 들어간다.
공통 trace는 timestamp를 앞으로 보정하므로 이 간격을 GPU 단독 시간이나 정밀한 단계 시간으로 사용하지 않는다.
Adaptive의 직렬 경로가 60Hz의 약 16.7ms 예산을 넘는다는 설명과 맞지만, 각 비용의 기여도는 새 진단으로 분리해야 한다.

현재 코드에서 확인한 순서:

1. `DorotiUIKitGraphiteView.Draw()`가 `NewShaderSceneAvailable`을 확인하고 pending GPU 프레임 수로 admission을 결정한다.
2. 이 query는 공통 renderer의 `_pendingFrame`에 새 셰이더 장면이 **이미 있어야** true다.
3. 일반 스크롤의 framework 콜백은 뒤의 `PaintGraphite → BeginPaint → DispatchPendingFrame`에서 실행되어 새 장면을 만든다.
4. 앞 GPU 프레임이 남아 있으면 1번에서 반환해 3번을 실행하지 못할 수 있다.

**유력한 가설:** 새 장면 생성 전의 query 때문에 pipeline 경로를 선택하지 못하고 직렬 경로를 반복한다.
이전 pipeline smoke의 보존 trace에서는 새 프레임 350개 중 앞 프레임 완료 통보 전에 raster를 시작한 경우가 0개였다.
따라서 기존 약 33.69 FPS 결과는 실제 overlap의 효과를 검증한 결과로 취급하지 않는다.
분기별 진단으로 query 실패·native composition·viewport 상태 중 실제 거절 이유를 확정한다.

또한 기본 경로는 `PlatformViews.IsConfigured`만으로 transaction presentation을 선택한다.
native overlay가 없는 장면에도 `WaitUntilScheduled()`를 실행할 수 있다.
이는 GPU 완료 대기가 아니라 command buffer의 scheduling 대기이며, 실제 UI 스레드 점유 시간을 따로 측정한다.

근거:
[기존 Fixed 검증](works/results/2026-10-01-variable-blur-fixed-adoption.md),
[기존 전체 재평가](works/results/2026-10-01-variable-blur-reassessment.md),
[iOS Draw/admission](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs),
[framework 콜백 연결](Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs),
[renderer의 새 장면 query](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs).

## 2. 범위와 유지할 계약

우선 수정 대상은 **iOS 호스트의 프레임 준비·raster admission·표시 연결**이다.
공통 renderer는 이 연결에 필요한 query·admission·완료 계약만 조정한다.
Gaussian/Adaptive/Fast/Kawase 셰이더와 블러 강도·해상도·캡처 최적화는 첫 비교에서 그대로 유지한다.

- normal shader-only 장면에서만 최대 **2개 GPU 프레임**을 허용한다. framework 요청은 최신 요청을 합쳐 처리하고 무제한 큐를 만들지 않는다.
- 장면·Surface·drawable·snapshot·recording·native lease의 수명은 실제 GPU 완료까지 보장한다.
  CPU 기록 완료, 제출 성공, `WaitUntilScheduled` 반환을 자원 회수 시점으로 바꾸지 않는다.
- GPU 완료 콜백이 역순으로 처리되어도 오래된 장면이 최신 replay source를 덮지 않는다.
- frame terminal은 한 번만 처리하고, 제출 실패·context loss에서 완료가 증명되지 않은 자원을 성급하게 회수하지 않는다.
- viewport/DPR/context generation이 바뀌면 장면과 타깃의 일치를 다시 검사한다.
  resize·rotation·native composition·replay는 기존 직렬 처리와 필요한 transaction 동기화를 유지한다.
- 새 콜백에서 native/shield 장면이 만들어지면 해당 장면을 소비하지 않고 직렬 경로로 보류한다.
  이미 제출한 shader 프레임을 drain한 뒤 native 장면을 표시한다.
- Off와 일반 UI도 회귀를 확인한다. 샘플 기본 Fixed, 공개 ImageFilter 기본값, 다른 플랫폼의 정책은 이번 작업에서 바꾸지 않는다.

기존 Surface 풀과 device/queue를 사용한다. 새 Surface 풀, 갱신 빈도 축소, 오래된 블러 결과 재사용,
외부 Metal/MPS 경로 도입은 첫 수정의 해결책에 포함하지 않는다.

## 3. P0 — 실제 경로와 기준 성능 진단

**목적:** pipeline이 동작하지 않는 이유와 제출 이후의 시간을 구분한다.

- [ ] 설치 앱의 runtime·payload hash·source hash·SDK/TFM·OS·backend·DPR·viewport·Hz를 확인한다.
  현재 설치 기록은 NativeAOT이며 기존 FPS 수치는 Mono다. 두 결과를 같은 바이너리의 비교로 사용하지 않는다.
  현재 비교 대상 runtime으로 수정 전 기준 앱과 계측 앱을 준비한다.
- [x] opt-in 진단으로 framework callback 준비/실행, scene 생성, admission 검사,
  recorder 시작/끝, Snap/Insert/Submit, terminal commit, GPU 완료 도착/owner 처리, drawable 실제 표시를 연결한다.
  scene/frame ID·generation을 함께 기록하고 renderer `present`와 drawable 실제 표시를 구분한다.
- [x] pipeline 활성 여부, eligible/거절 사유, GPU pending 수·최대값,
  앞 GPU 완료 전에 시작한 새 raster 수, native/resize/replay fallback 수를 집계한다.
  bounded 기록과 집계 카운터를 사용하고 프레임마다 console 출력·JSON 직렬화를 하지 않는다.
- [x] `CurrentDrawable` 획득, `WaitUntilScheduled`, flush, Snap/Insert/Submit의 CPU 시간을
  독립적인 monotonic timer로 측정한다. 보정된 phase timestamp 차이로 단계 시간을 만들어내지 않는다.
- [x] 기존 8192-entry trace의 후반부만으로 30초 전체 overlap 비율을 보고하지 않는다.
  측정 구간에 맞춘 카운터 또는 충분한 별도 bounded 이력을 사용한다.
- [x] 수집기의 source hash 대상에 현재 빠져 있는 `MauiHostAdapter.cs`와 새 연결·진단 파일을 포함한다.
  실제 pipeline/표시 정책과 runtime identity도 결과에 남긴다.
- [x] 같은 계측 바이너리에서 Adaptive/sigma 20의 serial/pipeline을 각각 1회 실행해
  admission 거절 이유와 콜백 생성 순서를 확인한다. 계측 자체의 성능 영향을 기존 기준과 비교한다.

**완료 조건:** 실제 실행에서 다음 장면이 어디서 준비되고 왜 pipeline에 들어가지 못하는지 설명할 수 있다.
추정과 다른 원인이 나오면 P1 설계를 그 증거에 맞춰 조정한다.

## 4. P1 — framework 프레임 준비와 GPU admission 분리

**목적:** GPU가 실행 중이어도 다음 framework 콜백과 새 장면 준비를 진행할 수 있게 한다.

- [x] framework 콜백 실행을 drawable 획득·GPU raster admission에 종속시키는 연결을 분리한다.
  iOS owner thread의 frame pulse에서 준비하고, 준비된 장면을 확인한 뒤 GPU admission을 결정한다.
  단순히 query 위치만 이동하거나 GPU pending 검사를 제거하는 방식으로 끝내지 않는다.
- [x] framework 준비 시 사용한 immutable viewport/resize epoch와 frame timestamp를 전달한다.
  첫 진입·layout/DPR 갱신·회전에서 이전 drawable의 metrics로 새 장면을 만들지 않는다.
- [x] 기존 `BeginPaint`의 callback dispatch와 중복되지 않게 콜백을 한 번만 실행한다.
  callback 안의 재요청은 다음 pulse에 남기고, reentrant paint·busy loop·유실된 invalidation이 없게 한다.
- [x] 준비된 **새 shader-only 장면**과 현재 native composition·pending native frame 상태를 함께 검사한다.
  recorder를 시작하거나 장면을 소비하기 직전에 조건을 다시 확인한다.
- [x] pipeline에서는 최대 2개 GPU 프레임을 유지하고, 슬롯이 차면 최신 장면을 보류한다.
  GPU 완료 시 누락 없이 다음 admission을 깨우되, shader-only 요청으로 과거 장면을 replay하지 않는다.
- [x] 같은 recorder의 순차 recording, scene Surface 재사용, snapshot 참조와 cache 소유권을 검토한다.
  완료 전 frame 간 자원 재사용이 queue 순서와 Skia 수명 계약을 지키는지 확인한다.
- [x] 기존 serial opt-in을 유지해 같은 바이너리에서 이전 동작을 재현할 수 있게 한다.

주요 파일:
[MauiHostAdapter.cs](Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs),
[DorotiUIKitGraphiteViewHandler.cs](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs),
[DorotiMauiSurface.cs](Doroti/src/Doroti.Host.Maui/DorotiMauiSurface.cs),
[MauiFrameworkHost.cs](Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs),
[MauiSkiaSurface.cs](Doroti/src/Doroti.Host.Maui/MauiSkiaSurface.cs),
[DorotiGraphiteView.cs](Doroti/src/Doroti.Host.Maui/DorotiGraphiteView.cs).

공통 연결은 필요할 때
[MauiSkiaCapabilities.cs](Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs),
[SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs),
[SkiaGraphiteSession.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.cs)를 수정한다.

**완료 조건:** callback에서 생성되는 새 장면도 pipeline에 진입하고,
앞 GPU 완료 전에 다음 새 장면의 raster를 시작한 실제 기록을 확보한다.
pending 상한을 2로 바꾼 것만으로 완료 판정하지 않는다.

## 5. P2 — 장면에 맞는 표시 방식과 정확성 검증

- [x] transaction 필요 여부를 coordinator의 존재가 아니라 **실제 표시할 장면과 native 상태**로 결정한다.
  shader-only 장면은 비동기 drawable presentation을 사용하고 scheduling 대기를 제거한다.
- [x] 비동기 표시에서는 같은 queue의 rendering 뒤에 presentation을 배치한다.
  native composition에서는 기존 transaction 안의 scheduling 대기·drawable/native 표시 계약을 유지한다.
  한 drawable을 두 번 present하거나 아직 렌더링되지 않은 drawable을 표시하지 않는다.
- [x] 완료 콜백의 owner-thread 처리는 유지하되 다음 framework 준비를 GPU 완료 통보에 묶지 않는다.
  제출→GPU 완료→실제 표시의 순서와 지연을 각각 기록한다.
- [x] 기존 `ShaderFramePipelineRegression`에 **처음에는 pending scene이 없고,
  framework callback이 새 장면을 생성하는 경우**의 호스트 연결 회귀를 추가한다.
  기존 검사는 scene을 미리 Submit하므로 이번 순서 문제를 독립적으로 검사하지 못한다.
- [x] 호스트 정책의 의미 있는 회귀를 추가한다: callback 중복/재요청, 두 프레임 상한,
  shader→native 전환, replay 보류, resize/generation 변경, 역순 완료, 실패/종료 후 retirement.
  정책 테스트와 실제 Metal 실행 검증을 구분한다.
- [x] 기존 공통 CPU 회귀, shader pipeline 회귀, macOS Metal GPU 픽셀 검사를 실행한다.
  공통 블러 처리까지 수정했다면 kernel/capture/Kawase 회귀도 실행한다.
- [ ] iPhone에서 일반 UI/Off, VariableBlur 각 옵션, native button/editor/WebView,
  회전·background/foreground·페이지 재진입을 확인한다.
  수동 조작이 필요한 항목은 자동 합성 스크롤 통과로 대체하지 않는다.

**완료 조건:** 장면별 표시 방식과 프레임 수명 계약을 검증하고,
pixel 오류·과거 장면 재표시·native 위치 불일치·frame 유실·Metal 오류가 없다.

## 6. P3 — 같은 바이너리 성능 A/B와 Safari 비교

프레임 준비와 표시 방식의 효과를 분리하기 위해 계측용 정책을 다음처럼 비교한다.
기존 `--serial-frames`와 `--pipeline-frames`는 유지하고,
B를 위한 별도 표시 선택은 P0에서 명세한 뒤 수집기에 연결한다. 아직 존재하는 옵션처럼 문서화하지 않는다.

| 비교 | GPU 프레임 상한 | shader-only 표시 | 목적 |
| --- | ---: | --- | --- |
| A: 기존 serial 기준 | 1 | 기존 transaction | 같은 수정 바이너리의 기준 |
| B: 표시 방식만 변경 | 1 | 비동기 | transaction 대기 제거 효과 분리 |
| C: 새 pipeline | 2 | 비동기 | 실제 overlap의 추가 효과 확인 |

- [x] Adaptive/sigma 20에서 A/B/C를 각각 3회 교차 실행한다.
  A의 callback·admission·표시가 기존 직렬 의미를 재현하는지도 확인한다.
  C에 실제 overlap이 없으면 성능 비교를 확대하지 않고 거절 이유부터 수정한다.
- [x] A/C의 Off·Fixed·Adaptive를 sigma 20/32, 모드별 3회 비교한다.
  총 36회이며 앞서 확보한 동일 조건 실행은 재사용할 수 있다.
- [ ] 핵심 모드가 회귀 없이 개선되면 Full·Fast·Kawase도 sigma 20/32, A/C 각각 3회 비교한다.
  모든 모드의 60 FPS 여부를 보고하려면 이 추가 36회를 포함한다.
- [x] 같은 app payload·runtime·source·장면·DPR·sigma를 사용한다.
  모드 순서뿐 아니라 A/C 정책 순서도 교차하고, 두 번째 반복은 역순으로 실행한다.
  기존 수집기는 모드 순서만 뒤집으므로 정책 교차는 수집기 또는 별도 실행 orchestration에 연결한다.
- [x] 실행당 40초 중 첫 실제 drawable 표시 이후 5~35초를 집계한다.
  전원·밝기·열/냉각·진단 조건을 기록하고 비교 중 유지한다.
  FPS 수집과 profiler·화면 캡처·녹화·GPU capture를 분리한다.
- [ ] 평균 표시 FPS, 표시 p50/p95/p99, >25ms 비율, 실제 overlap 수/비율,
  pending 최대값, CPU 단계, callback 대기, 오류, capture 크기·패스 수를 실행별로 보고한다.
  입력/장면이 큐에 밀려 FPS만 높아지는 일이 없는지 input→scene→표시 지연도 확인한다.
- [ ] **사용자 지시로 제외:** 같은 iPhone Safari의 실제 웹 빌드 revision·backend·DPR·viewport·작업 크기와
  sigma·옵션·움직이는 콘텐츠를 확인한다. browser의 rAF/제출 간격을 물리 표시 FPS로 보고하지 않는다.
  필요하면 성능 수집과 별도의 영상으로 콘텐츠 갱신 간격을 확인하고 측정 방법을 명시한다.
- [ ] **사용자 지시로 제외:** 웹과 네이티브의 진단량·runtime·작업 영역 차이를 결과에 기록한다.
  웹은 현재 benchmark 모드를 건너뛰므로 동일 합성 경로가 필요하면 검증용 진입점만 추가한다.
  제품 화면에 profiler 설정이나 진단 UI를 넣지 않는다.

### 판정 기준

| 항목 | 기준 |
| --- | --- |
| pipeline 실제 동작 | Adaptive 연속 스크롤에서 측정 구간의 반복적인 overlap 확인. pending 최대 2, 무제한 backlog 없음 |
| Off/Fixed 성능 보존 | sigma 20/32의 각 실행 평균 FPS ≥58, p95 ≤17.5ms, p99 ≤34.2ms, >25ms ≤2%. Fixed 평균은 같은 정책 Off의 97% 이상 |
| 반복 개선 | Adaptive의 C 3회 모두 대응 A보다 평균 FPS 개선, C 평균은 A 평균 대비 10% 이상 개선. p95/p99·입력 지연 악화 없음 |
| 60Hz 목표 달성 | 해당 모드·강도별 3회 모두 FPS ≥58, p95 ≤17.5ms, p99 ≤34.2ms, >25ms ≤2%를 만족할 때만 60Hz 목표 통과 |
| 작업량 일치 | 정책 A/B/C에서 같은 모드의 블러 설정·capture·pass 의미 유지. 품질/작업 축소로 얻은 수치를 호스트 개선으로 보고하지 않음 |
| 정확성 | renderer/terminal Metal 오류 0, 유효한 전체 표시 이력, stale replay·미완료 자원 회수·native 불일치 없음 |

10%는 이번 작업의 **호스트 성능 개선 채택 기준**이며 60 FPS 달성 기준과 별개다.
일부 모드만 목표를 통과하면 모드별로 보고한다.
Full의 GPU 비용이 화면 주기를 넘는 경우에는 프레임 연결 수정만으로 60 FPS를 약속하지 않는다.
Off가 평소 약 59 FPS를 벗어나면 환경·빌드·계측 조건을 확인하고 해당 비교 묶음을 다시 수집한다.

## 7. P4 — 기본 적용과 지속 사용

- [x] 검증한 iOS shader-only 범위에서 새 경로를 기본 적용한다. 사용자가 아이폰에서 정상 동작을 확인하고 기본 경로 적용을 명시했다.
  serial 비교/복귀 진입점은 유지하고 다른 플랫폼에는 자동 확대하지 않는다.
  이 채택 결정과 전체 정량·지속 사용 게이트의 통과 여부는 구분한다.
- [x] 실제 사용 runtime으로 최종 후보를 새로 빌드·설치한다.
  P3와 runtime이 다르면 해당 runtime에서 핵심 Off/Fixed/Adaptive A/C를 별도로 검증한다.
  Mono/NativeAOT 간 수치를 pipeline A/B로 합치지 않는다.
- [ ] benchmark 설정 없이 첫 진입, 실제 손가락 스크롤의 느린/빠른 이동·방향 전환,
  sigma 0/1/2/4/8/20/32, 연속 슬라이더, 켜기/끄기, 모드 변경, 페이지 재진입을 확인한다.
- [ ] 최종 후보로 10분 연속 사용하며 같은 조건 Off를 대조한다.
  시작/종료 자원·오류·열 상태와 반복되는 멈춤·깜빡임·입력 지연을 기록한다.
  GPU 완료 후 회수 여부와 pending 자원이 정상 범위로 돌아오는지 확인한다.
  단일 종료 Metal 할당량을 블러 peak VRAM이나 누수 판정으로 사용하지 않는다.
- [x] 결과 문서에 빌드 identity, 정책별 실제 경로, 수치, 실패 실행, 수동 결과와 미검증 범위를 남긴다.
  `work3.md`의 기존 Fixed 결과와 미완료 게이트는 이 계획의 통과 결과로 대신 체크하지 않는다.
- [x] `git diff --check`와 변경에 해당하는 기존 검사를 통과한다.

**완료 조건:** 새 iOS 경로가 실제로 동작하고, 반복 성능 개선·정확성·실제 입력·지속 사용을 검증한 최종 앱을 제공한다.
60Hz 목표에 미달한 옵션은 구체적인 수치와 다음 병목을 남긴다.

## 8. 실패 원인별 후속

| 확인된 결과 | 후속 작업 |
| --- | --- |
| C에서도 overlap 0 | callback 준비·query 시점·pending gate·native/resize 거절 이유를 재검토. 옵션 활성만으로 pipeline 검증을 통과시키지 않음 |
| B는 개선되고 C 추가 이득 없음 | 표시 대기 제거를 별도 채택 후보로 평가. overlap 비용·GPU 포화·입력 지연을 조사하고 불필요한 2-frame 정책은 기본 적용하지 않음 |
| overlap은 있으나 FPS 목표 미달 | Snap/Insert/Submit, 실제 GPU 실행, 표시 deadline을 별도 계측. CPU trace 간격을 GPU 시간으로 해석하지 않음 |
| Fixed/Off·native·회전 회귀 | 해당 경로를 직렬 정책으로 복귀하고 admission·generation·표시·retirement 계약부터 해결 |
| GPU 실행이 주 병목으로 확인됨 | 같은 장면·해상도·runtime에서 Ganesh/Metal과 Graphite/Metal의 제한된 비교를 검토. 전체 호스트 교체를 선행하지 않음 |
| 특정 셰이더/패스가 GPU 예산 초과 | 그 근거에 따라 공통 블러 셰이더나 Metal/MPS 후보를 비교. 생성·합성·자원 수명까지 포함한 전체 표시 성능으로 판단 |
| Safari 실제 경로/작업량이 다름 | 차이를 먼저 정렬해 비교. 웹 체감 60 FPS를 같은 커널·같은 입력의 GPU 성능 증거로 확대하지 않음 |

## 9. 검증 진입점과 산출물

저장소 루트에서 기존 검사를 실행했다. 아래 명령은 이번 검증에 사용한 **실행 진입점**이다.

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --shader-frame-pipeline
dotnet run --project Doroti/tests/Doroti.Tests -c Release
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu
python3 -m unittest discover -s Doroti/tests -p test_variable_blur_device.py
git diff --check
```

기기 비교 명령은 같은 서명 앱을 지정한다. 정책 교차 순서와 B의 표시 선택은 P3에서 수집기에 연결한 뒤 사용한다.
새 출력 폴더를 사용하고 sigma 32에도 동일 payload의 `--app`을 지정한다.

```sh
python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app <SIGNED_APP_PATH> --output temp/testing/variable-blur/ios-frame-loop-serial-sigma20 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 20 --serial-frames --conditions '<전원·열·밝기·진단 조건>'
python3 Doroti/tests/variable_blur_device.py --device <DEVICE_UDID> --app <SIGNED_APP_PATH> --output temp/testing/variable-blur/ios-frame-loop-pipeline-sigma20 --hz 60 --repeats 3 --modes off fixed adaptive --sigma 20 --pipeline-frames --conditions '<동일 조건>'
```

빌드 환경은 P0에서 실제 SDK/workload/TFM을 확인한 뒤 고정한다.
현재 NativeAOT 게시 조건과 알려진 진단은
[NativeAOT 설치 기록](works/results/2026-10-01-sample2-nativeaot-rc1.md)을 따른다.
기존 환경의 우회 옵션을 새 환경에 무조건 복사하거나 전역 toolchain 설정을 바꾸지 않는다.

주요 검증 파일:
[ShaderFramePipelineRegression.cs](Doroti/tests/Doroti.Tests/ShaderFramePipelineRegression.cs),
[VariableBlurGpuRegression.cs](Doroti/tests/Doroti.Tests/VariableBlurGpuRegression.cs),
[variable_blur_device.py](Doroti/tests/variable_blur_device.py),
[test_variable_blur_device.py](Doroti/tests/test_variable_blur_device.py).

실행 원본은 `temp/testing/variable-blur/ios-frame-loop-*`에 보존하고,
결과는 `works/results/2026-10-02-ios-frame-loop.md`와 정책별 집계 JSON에 정리한다.
실제 실행일이 달라지면 파일명과 기록 날짜도 실행일로 맞춘다.
웹 비교 원본·GPU profiler 원본·품질 캡처는 표시 성능 수집과 별도 폴더에 둔다.

**초기 자동 검사 시점의 판정:** 구현과 CPU/Metal 검사를 진행했고, 당시에는 실제 overlap·반복 표시 성능·수동 입력·지속 사용 확인 전이라 전체 완료와 기본 적용을 보류했다. 이후 검증과 사용자 기본 채택 결정은 아래 실행 기록에 이어서 남긴다.

## 10. 실행 기록 — 2026-10-02

사용자의 추가 지시에 따라 **Safari 비교는 제외하고 iOS 수정에 우선 집중**한다. 기기 상태는 사용자가 계속 양호한 것으로 가정하도록 요청했으며, 정량적인 밝기·열 통제의 확인으로 확대하지 않는다.

- framework 준비를 drawable/GPU admission 앞에 분리했다. immutable pixel/DPR/context generation을 게시하고 callback dispatch 중의 재요청은 다음 pulse에 남긴다.
- 실제 scene/native 상태로 비동기 표시를 결정한다. A/B/C 선택은 collector의 `--policies A B C`로 같은 앱에서 교차 실행한다. B의 별도 선택은 `--serial-frames --shader-presentation async`다.
- bounded host 이력과 CPU 타이머를 추가했다. overlap은 실제 Metal terminal GPU 종료 시점을 우선 사용하며, GPU 완료 통보와 owner 처리·실제 drawable 표시는 각각 구분한다.
- source hash 대상에 `MauiHostAdapter.cs`, 새 연결/수명/진단 파일과 Graphite session 파일을 포함했다.
- callback에서 처음 shader scene을 생성하는 호스트 회귀, native callback 전환·두 프레임 상한·재요청·viewport fallback·역순 완료·실패/종료 수명 회귀를 통과했다.
- 공통 CPU 및 기존 macOS Metal 픽셀 검사를 통과했다. 같은 recorder/cache의 두 async recording을 유지한 추가 비교도 두 해상도·각 두 프레임 모두 **0/255** 오차로 통과했다.
- Python collector 검사 **8개** 통과. 첫 iOS Release/Mono 빌드는 경고·오류 **0/0**이다.
- 이전 서명 Mono 앱의 Adaptive/sigma20 기준은 **33.759 FPS**, 표시 p95 **33.442ms**, 오류 **0/0**이다. 이전 payload hash를 원래 기록과 대조했으며 새 바이너리 A/B/C 결과로 사용하지 않는다.

상세 구현·정책별 수치·실패 및 남은 게이트는 [실행 결과](works/results/2026-10-02-ios-frame-loop.md)에 기록한다.

초기 NativeAOT C에서 표시 FPS는 개선됐지만 callback→표시 지연이 증가했다. 이후 표시 callback을 admission credit으로 사용하는 후보는 실제 화면 갱신 중단 때문에 제거했다. 현재 후보는 MTKView display link, 최신 pending scene 하나, GPU 두 프레임 상한, idle pause를 사용한다. SurfaceGeneration 조회 중 전체 진단 이력을 복사하던 계측 비용도 scalar 조회로 수정했다.

수정된 동일 NativeAOT 앱의 Adaptive/sigma20 C는 **59.756 / 59.762 / 59.623 FPS**, 실제 overlap **153 / 159 / 247회**, GPU pending 최대 **2**다. sigma32 C는 **59.688 / 59.588 / 59.624 FPS**, overlap **182 / 162 / 226회**다. Adaptive C 평균 개선율은 A 대비 sigma20/32 **59.1 / 60.4%**다. C 핵심 Off/Fixed/Adaptive 18회는 모두 표시 성능 기준을 통과했다. A Fixed sigma32 첫 실행 **56.785 FPS**는 미달로 보존했다.

핵심 A/C 36회와 B 3회 유효 비교를 완료했다. sigma32 Fixed C3의 첫 시작 timeout은 초기화 로그/evidence가 없고 원인 미확정이며 별도 실패로 남겼다. 같은 payload 재실행 **59.660 FPS**와 남은 원래 정책 순서를 별도 폴더에 보존했다. 모든 구버전/실패 실행은 최종 후보와 구분한다.

drawable 두 개 후보는 **29.63 FPS**, overlap 0, 지연 개선 없음으로 제거했다. callback→표시는 최종 C도 A 약 32.7ms 대비 약 49.9ms라 기본 정책은 아직 serial이다. Full/Fast/Kawase 추가 36회는 핵심 채택 게이트 미통과로 확대하지 않았으며 모든 모드 60Hz로 보고하지 않는다.

실제 iPhone의 A/C editor/WebView 생성·재생성 4회씩, C native editor가 있는 UIKit landscape→portrait 회전, benchmark 없는 Sample2 첫 진입과 동일 PID background/foreground 복귀를 통과했다. native GPU pending 최대 1, renderer/terminal 오류 0/0이다. native probe 종료에서는 pending 0과 terminal 전량 완료를 확인했다. 물리 IME/native 버튼·손가락 스크롤·수동 회전 UX·10분 실제 사용은 이 자동 검사로 대신 체크하지 않았다.

P0의 빌드/runtime/payload/source identity는 확인했다. 수정 전 clean NativeAOT로 계측 비용을 분리한 기준은 확보하지 못해 첫 복합 체크항목은 미완료로 남긴다. 기존 Mono baseline과 새 NativeAOT의 수치를 동일 바이너리 개선율로 합치지 않았다.

최종 Sample2와 native 확인용 Testbed를 benchmark/probe 없이 C opt-in으로 준비했다. 강제 종료 후 새 process로 실행하면 기본 A다. 수동 결과를 요청했으며, 기본 적용·전체 완료 판정은 보류한다. 상세 39개 실행/실패/identity/CPU 단계와 native·최종 앱 결과는 [정책별 JSON](works/results/2026-10-02-ios-frame-loop-summary.json) 및 [실행 결과](works/results/2026-10-02-ios-frame-loop.md)에 기록했다. `work3.md`는 변경하지 않았다.

### Sample2 멈춤 보고 후 수정

초기 일반 앱의 복귀 64프레임만으로 정상 판정했던 preflight는 사용자의 멈춤 보고 후 철회했다. pipeline을 pause할 때 SetNeedsDisplay 모드를 복구하고, resume/re-attachment에서 실제 frame pulse를 재시작하도록 수정했다. 실제 active scene에서만 continuous pulse를 시작하며 suspend 상태를 animator 종료보다 먼저 게시한다.

같은 PID의 background/foreground **3회**에서 복귀 직후→5초 뒤 완료 수 **596→917 / 1074→1395 / 1556→1882**, 오류 0/0으로 지속 갱신을 확인했다. live 진단 파일 복사 실패도 보존하고 iOS evidence를 임시 파일 완성 후 교체하도록 수정했다.

수정본 payload는 `93757deec6bda478f1ed02ea8f9648bfbaba3fcb98ceec87d8e664efe82bcca0`다. Adaptive σ20 새 앱 A/B/C 1회 smoke는 **39.91 / 37.21 / 59.73 FPS**, C p95 약 16.72ms, overlap **178/1791**, pending 최대 2, 오류 0/0이다. 기존 39회와 다른 payload이므로 합치지 않았다. 새 앱의 전체 36회/σ32 및 native probe 재실행을 통과한 것으로 확대하지 않는다.

사용자가 Sample2 정지를 다시 문의하여 자동 스크롤 검사를 종료하고 **benchmark 없는 일반 Sample2 수정본**을 C로 다시 실행했다. 자동 검사 화면은 실행당 40초 이후 멈추고 수집 후 종료되므로 일반 앱과 구분한다. 기본 A, 입력 지연/10분 사용 및 전체 완료 보류는 유지한다.

### 사용자 지시로 Fast adaptive 기본값 채택

Fixed의 세부 품질과 Kawase의 블러 형태에 대한 사용자 판단에 따라 **Fast adaptive를 Sample2와 공개 VariableBlur API의 기본값으로 적용**한다. 공개 API까지 적용한다는 범위를 사용자에게 확인했다. 이 지시는 처음 비교에서 기본값을 유지하던 범위를 대체하며, 39회 과거 성능 자료의 당시 설정은 변경하지 않는다.

- `ImageFilter.variableBlur`와 `ImageFilterConfig.CreateVariableBlur`: `resolutionScale: 0.25`, `adaptiveResolution: true`, `kernel: fastGaussian`.
- Sample2 초기 선택과 Off benchmark에서 다시 켰을 때 선택: `fast`.
- Full Gaussian은 `resolutionScale: 1, kernel: gaussian`으로 명시한다. Fixed/Kawase 등 선택지는 유지한다.
- API/config의 정규화 좌표·bounds와 명시적인 Fast 설정의 snapshot 일치를 검증하고, 명시적인 원본 해상도 Gaussian도 확인한다. 공통 CPU 회귀 통과.

이는 블러 기본값 선택이며 iOS 프레임 정책의 기본 A/C 채택이나 모든 모드의 60Hz 판정을 대신하지 않는다. 새 앱·픽셀 검사와 확인 결과는 실행 결과 문서에 별도로 기록한다.

Fast 기본값 적용본의 공통 CPU·공개 config/필터 계약·macOS Metal 픽셀 검사와 collector 8개 검사를 통과했다. 실제 iPhone C의 Fast σ20/32 각 1회 smoke는 **59.829 / 59.827 FPS**, p95/p99 약 **16.72ms**, >25ms **0%**, pending 최대 **2**, 오류 **0/0**이다. 이 두 실행은 기존 Adaptive Gaussian 반복 자료와 구분한다. 새 NativeAOT 앱을 설치하고 benchmark 없는 일반 Sample2로 다시 실행했다. [기본값 적용 집계](works/results/2026-10-02-fast-adaptive-default-summary.json)에 새 payload/source identity와 검사 범위를 보존한다.

### 사용자 아이폰 확인 후 iOS C 기본 경로 채택

사용자가 “한번 아이폰확인했는데 잘 되네 기본경로로하자”라고 명시하여 iOS Graphite/Metal 호스트의 미설정 기본 경로를 **C**로 전환했다. Fast adaptive의 공개 API·Sample2 기본값을 유지한다. native·회전·replay의 직렬 admission과 필요한 transaction 표시도 유지한다. `SERIAL_FRAMES=1`, `PIPELINE=0`, 명시적 transaction으로 A에 복귀할 수 있고 serial+async는 B다. 수집기의 미설정 정책도 C로 맞췄다.

새 NativeAOT Sample2를 설치하고 **pipeline/serial/presentation/Graphite 설정 없이** C가 선택되는 것을 확인했다. 진단만 켠 같은 PID **6912**의 background/foreground 3회에서 완료 수 **584→904 / 1060→1383 / 1543→1867**, pending 최대 **2**, 오류 **0/0**으로 지속 갱신했다. 이후 진단·benchmark를 포함한 모든 환경 설정을 빼고 새 PID **6920**의 일반 Sample2로 실행했다. 강제 종료 후 일반 재실행도 C다.

기본/명시 A/B/C 선택 8가지와 CPU 프레임 계약, Python collector 8개, NativeAOT 빌드·실기기 복귀, `git diff --check`를 통과했다. [C 기본 경로 적용 집계](works/results/2026-10-02-ios-default-C-summary.json)에 새 payload와 실행 설정을 기록한다. 이번 사용자 채택은 정량 입력 지연·10분 Off 대조·최종 payload 전체 반복 측정의 통과 판정과 구분한다.

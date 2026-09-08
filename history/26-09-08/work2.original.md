# Web framework 병목 조사 및 개선 작업계획

- 작성일: 2026-09-07
- 상태: **실행 결과 PARTIAL — P0/P1/P3 개선과 자동 검증 수행, 성능 수용 미완료**
- 실행 보고: [2026-09-07 framework 실행 결과](history/26-09-07/web-framework-work2-results.md). 계획 원문은 [보관본](history/26-09-07/web-framework-work2-plan.md)에 보존했다.
- 아래 조사 범위·과거 시제는 계획 작성 당시 기록이다. 현재 사용자의 전체 실행 요청을 제한하지 않는다.
- 요청 범위: 현재 코드, Flutter 구현, 공식 Web/.NET 문서를 검토하고 개선 계획을 작성한다.
- 검토 기준: HEAD `c9b9eb21d45eceaf9beb04c6d0338cf8cc54bd25` + 이전 direct 작업의 미커밋 변경. HEAD 단독으로 현재 실행 상태를 재현할 수 없다.
- Flutter 대조 기준: 로컬 `C:/Users/parti/flutter` HEAD와 reference 앱 `.metadata` 모두 `6b182d2c7585eba26d4edce0f97630effd256c33`.
- 이번 작업은 소스·기존 JSON 재분석 및 문서 변경만 수행했다. 새 성능 실행, 런타임 수정, 빌드, 물리 검증은 수행하지 않았다.
- 기존 계획 원문은 [실행 당시 work2 보관본](history/26-09-07/web-direct-work2-executed-plan.md)에 그대로 보존했다. 보관본의 상대 링크는 당시 저장소 루트 기준이다.
- 이전 결과는 [direct 전환 실행 보고](history/26-09-07/web-direct-default-execution.md), [샘플 retained rendering 기록](history/26-09-07/web-sample-retained-rendering.md)을 따른다. 기존 `work.md`는 별도 범위다.

## 1. 조사 결론과 우선순위

**현재 큰 지연에는 framework build 작업이 직접 관여한다. 다만 “공용 프레임워크의 단일 결함”으로 원인을 확정할 단계는 아니다.**
최종 원본에서도 첫 조작/재시작 프레임의 build 구간에 544~652ms가 쓰인다. 이 구간은 샘플의 build 함수,
Element 갱신, 위젯 생성, 그 과정의 런타임/할당 비용을 모두 포함한다. 각 함수의 self time이나 GC 정지 시간은 아직 없다.

우선순위는 다음과 같다.

1. **실제 작업량 차이 확인:** Flutter 샘플과 다른 상태 소유 범위 때문에 화면 전체 목록이 갱신되는 문제를 대조한다.
2. **공용 build 비용 귀속:** dirty/rebuild/delegate/implicit animation별 호출 수와 비용을 측정해 불필요한 작업과 동일 작업의 실행 비용을 분리한다.
3. **semantics 비용 귀속:** 공용 트리 갱신, Web host의 전체 snapshot 구성·직렬화, main DOM 적용을 나눠 개선한다.
4. **새 구간 진입·resize:** layout/텍스트/래스터 비용과 Worker 입력 대기를 별도 시나리오로 좁힌다.
5. **런타임·구조 실험:** 위 비용을 줄인 후에도 남는 CPU 병목에 한해 AOT 또는 Worker 분리의 필요성을 판단한다.

사용자 관찰 **“일부 개선됐지만 지연이 남음”**을 현재 수용 상태로 유지한다. direct 기본값 전환과 CSS·캐시 수정의
자동 PASS는 남은 onset/resize 성능 FAIL을 해소하지 않았다.

## 2. 현재 기준선과 증거의 한계

### 2.1 이미 구현되어 있는 부분

- 기본 렌더러는 이제 `worker-direct-webgl`이다. loader의 생략/auto/미인식 값과 target manifest를 함께 수정했다.
  CanvasKit/document/offscreen explicit 선택은 남아 있다. 다시 기본값을 전환하는 작업은 하지 않는다.
- main은 DOM/input/IME/semantics/viewport를, direct Worker는 .NET/framework/Skia/WebGL2/Worker rAF를 소유한다.
- resize capacity의 CSS 덮어쓰기 충돌은 수정했다. immutable epoch, exact 검증, 최신 입력 병합과 bounded admission은 유지된다.
- raster promotion은 frame당 최대 2개/합계 4M pixels, 누적 2ms 이후 추가 승격 중단이다.
  **단일 승격을 2ms에 중단하지는 못한다.** warm-up metadata는 128개 상한과 120-frame 미사용 만료를 가진다.
- CPU frame/paragraph/cache diagnostics는 opt-in이다. direct trace가 켜져도 현재 `DOROTI_STAGE_TRACE`는 켜지지 않아
  기존 `FrameworkWorkCounters`의 build/layout 작업량을 함께 보지 못한다.

### 2.2 최종 JSON의 첫 조작 frame 재분석

원본 위치: `Doroti/validation/web-playwright/artifacts/sample-perf/`.
아래는 새 측정이 아니라 `direct-verified-*` 원본의 `RecordedAtMicroseconds`를 다시 계산한 값이다.

| 원본 label | 실제 input→new-scene commit ms | build 구간 ms | layout+compositing 구간 ms | semantics flush 구간 ms |
| --- | ---: | ---: | ---: | ---: |
| direct-verified-onset-1 | 714.9 | 616.4 | 3.0 | 118.7 |
| direct-verified-onset-2 | 755.8 | 651.8 | 14.0 | 112.5 |
| direct-verified-onset-3 | 694.8 | 592.6 | 11.1 | 114.6 |
| direct-verified-restart-1 | 570.9 | 544.2 | 3.6 | 78.5 |
| direct-verified-restart-2 | 598.2 | 572.0 | 2.6 | 86.2 |
| direct-verified-restart-3 | 632.5 | 600.7 | 4.4 | 86.0 |

재현 절차:

1. `causalOnset.requestId`와 일치하는 `CausalFrameId`의 Phase 13/raster 항목을 찾는다.
2. 그 항목 앞의 마지막 Phase 7/build부터 Phase 8/layout까지의 실제 기록 시각 차를 계산한다.
3. Phase 8→9/paint는 layout+compositing이다. Phase 27/semanticsBuild→28/semanticsBuildEnd는 flush 전체다.
4. `directDiagnostics[0].managed.frame.Skia.Trace`를 사용하며 frame/request/input 식별자가 끊긴 표본은 제외 사유를 기록한다.

해석 제한:

- build 구간은 `BuildOwner.buildScope`의 포괄 시간이다. 위젯 하나의 비용이나 공용 코드만의 self time이 아니다.
- 위 여섯 frame에서 semantics는 raster 뒤에 있다. 이를 input→첫 commit에 단순 가산하거나
  “semantics를 없애면 첫 commit이 그만큼 빨라진다”고 해석하지 않는다. callback 종료와 다음 입력 처리를 늦출 수 있다.
- callback에는 동기 raster가 포함된다. callback 시간과 surface 시간을 더하면 이중 계산이다.
- 첫 앱 mount와 첫 progress 조작은 다르다. restart에도 지연이 있어 최초 로딩/JIT/폰트만의 문제로 설명할 수 없다.
- 과거 `direct-p2-phases`의 build 511.5ms/semantics 99.8ms는 별도 중간 빌드 증거다. 위 최종 표본과 혼합하지 않는다.
- 3회 표본의 min/median/max는 기술 통계다. 충분한 모집단의 p95를 입증한 것으로 표현하지 않는다.

### 2.3 남은 성능 FAIL

| 항목 | 마지막 증거 | 판정 |
| --- | --- | --- |
| 첫 progress | input→commit 694.8~755.8ms; callback max 755.4~819.6ms | FAIL |
| stop→5초 idle→restart | input→commit 570.9~632.5ms | FAIL |
| 새/방문 section 혼합 sweep | callback p95 96.0ms/max 497.5ms; surface max 107.9ms | FAIL, 신규/방문 독립 통계 미완료 |
| CDP resize DPR 1/2 | CSS scale 오류 0/10, active front 각 1개; 최종 exact 1531.9/1459.7ms | CSS 수정 확인, 연속성·지연 FAIL |
| 진단 OFF | 6842.8ms 동안 front request advance 342, 오류 0 | onset/percentile notMeasured |
| 실제 창 테두리/첫 입력 체감 | 사용자: 일부 개선됐지만 지연이 남음 | 개선 일부 확인, 수용 미완료 |

sweep의 전체 commit 최대 간격에는 경계에서 화면이 바뀌지 않는 시간도 포함된다. 순수 render stall로 사용하지 않는다.
CDP target 중 superseded된 크기는 allocation 완료 증거가 아니다. commit notification은 픽셀 표시/scan-out이 아니다.
원본 일부가 사라지면 보고서 수치만 남았다고 명시하고 P0에서 새 기준선을 만든다.

## 3. 코드와 Flutter 비교

### 3.1 상태 소유 범위: 가장 먼저 검증할 구체적인 차이

- Doroti `DorotiTestbedApp/src/MaterialSample/Components.cs:24`: `ComponentsState`가 progress뿐 아니라 다수 control의 상태를 소유한다.
- 같은 파일 `build()`는 group factory와 전체 section 배열, 각 column의 `MeasuredSlivers` delegate를 다시 만든다.
- progress 클릭은 이 상위 State에서 `setState`한다. 방문해 유지 중인 child까지 delegate 갱신에 참여할 수 있다.
- Flutter `reference/flutter_sample_app/lib/src/component_screen.dart:1006`는 별도 `ProgressIndicators` StatefulWidget 안에서
  progress bool과 `setState`를 소유한다. 목록의 다른 group은 `const` widget을 사용한다.
- 두 샘플은 lazy item 구분 단위도 다르다. Flutter의 group 단위와 Doroti의 세부 section 단위를 기록하고,
  한꺼번에 구조를 바꾸지 않은 대조 fixture를 둔다. 단순 실행 시간 비율을 언어/프레임워크 우열로 해석하지 않는다.

Flutter 공식 [build 비용 지침](https://docs.flutter.dev/perf/best-practices#control-build-cost)은 상태 갱신 범위를 좁히고,
변하지 않는 child 인스턴스를 재사용하도록 설명한다. **샘플 이식에서 확인된 차이**와 **공용 framework 성능 결함**을 분리한다.
상태 범위를 맞추는 수정은 정당한 sample parity 개선이지만, 그 성공만으로 일반 앱/resize 병목 해결을 선언하지 않는다.

### 3.2 Element·Sliver: 이미 존재하는 최적화와 확인할 차이

- `Doroti/src/Doroti.Framework.Widgets/framework.cs`의 `Element.updateChild`에는 같은 widget이면 child 갱신을 생략하는 경로가 있다.
  `_scheduleBuildFor`도 `_inDirtyList`로 중복 삽입을 막는다. 이 기능을 없는 것처럼 새로 만들지 않는다.
- `sliver.cs:270` 부근의 delegate update/`performRebuild`는 Flutter와 마찬가지로 delegate 변경과 `shouldRebuild`를 확인하고
  기존 child를 key/index로 재조정한다. Flutter도 유지된 child map을 순회한다.
- 대조 소스: pinned Flutter `packages/flutter/lib/src/widgets/framework.dart`, `sliver.dart`.
  공개 구현: [Element.rebuild](https://api.flutter.dev/flutter/widgets/Element/rebuild.html),
  [Sliver.performRebuild](https://api.flutter.dev/flutter/widgets/SliverMultiBoxAdaptorElement/performRebuild.html).
- 계획에서 측정할 것: sibling 재빌드 수, forced rebuild 원인, build 중 새 dirty 등록/재정렬, delegate 갱신 및 kept-alive 방문 수,
  `Widget.canUpdate`/key 처리, dependency 알림, 컬렉션/closure 할당.
- prior trace의 여러 `AnimationController` 시작은 소유 위젯을 식별하지 못한다. progress 두 개 외에 implicit animation이
  왜 시작되는지 소유 State/type과 tween 변경 이유를 붙여 검증한다. 의도된 전환을 중지시키지 않는다.
- 공용 sliver의 dynamic→typed 실험은 이미 개선이 없어 되돌렸다. 이를 새로 발견한 해결책으로 반복하지 않는다.
  다른 hot path의 dynamic/conversion/boxing은 CPU 근거가 있을 때만 좁혀 다룬다.
- widget 전체 deep Equals 추가, keep-alive child 무조건 skip, dirty flag 강제 제거는 하지 않는다.
  identity 재사용은 immutable configuration과 theme/locale/dependency/key 수명을 보존해야 한다.

### 3.3 semantics: 공용 트리와 Web bridge를 분리해야 한다

- `Widgets/binding.cs:953`는 build→layout/compositing→paint→scene 제출→조건부 semantics→finalizeTree 순서다.
  active scroll/metrics의 semantics coalescing과 최종 flush 경로는 이미 있다.
- `Rendering/object.cs:870`의 `flushSemantics`는 dirty node/geometry 처리를 하고 `SemanticsOwner.sendSemanticsUpdate`를 호출한다.
- `Semantics/semantics.cs`와 [Flutter flushSemantics](https://api.flutter.dev/flutter/rendering/PipelineOwner/flushSemantics.html),
  [sendSemanticsUpdate](https://api.flutter.dev/flutter/semantics/SemanticsOwner/sendSemanticsUpdate.html)의 처리 순서를 비교한다.
- Web `BrowserSkiaCapabilities.cs:208`의 `UpdateSemantics`는 delta를 `_semantics`에 병합한 뒤 reachable tree를 정리하고,
  **전체 보유 node를 정렬·projection·JSON 직렬화**한다. `contentUnchanged`는 payload 내용을 줄이지만 전체 node 처리를 없애지 않는다.
- `Web/doroti.web.ts:2440`도 수신 snapshot으로 map/liveIds를 만들고 geometry를 적용한다.
  따라서 C#에서 changed node만 보내면 되는 단순 수정이 아니다. 현재 수신자는 전체 snapshot 의미에 의존한다.
- 위 78.5~118.7ms에는 공용 tree와 동기 host 호출이 포함될 수 있다. serialize/transport/DOM 각각의 비용을 아직 분리하지 못했다.
  main DOM 적용 시간을 Worker flush 시간과 동일시하지 않는다.

### 3.4 Worker event loop와 runtime

- direct는 .NET/framework와 raster를 한 Worker에서 동기 실행한다. 긴 callback 동안 그 Worker의 다음 input/resize message와 rAF가 기다린다.
  [HTML event loop 규약](https://html.spec.whatwg.org/multipage/webappapis.html#event-loop-processing-model),
  [OffscreenCanvas 문서](https://web.dev/articles/offscreen-canvas)가 설명하는 실행 구조에 근거한 판단이다.
- Worker로 이동했다는 사실은 main 부하 분리의 근거다. Worker 자체의 600ms build를 없앤다는 근거는 아니다.
- [`scheduler.yield` 제안 규약](https://wicg.github.io/scheduling-apis/#dom-scheduler-yield)은 협력적으로 다음 task에 양보하는 API다.
  이미 실행 중인 동기 C# build/layout을 밖에서 선점하지 못한다. 불완전한 트리를 노출하는 await를 frame 중간에 넣지 않는다.
- Flutter의 native UI/raster thread 설명을 Web에 그대로 적용하지 않는다. [Flutter Web Wasm](https://docs.flutter.dev/platform-integration/web/wasm)의
  컴파일 모드/렌더러/worker 조건을 기록하고 pinned engine `engine/src/flutter/lib/web_ui/lib/src/engine/skwasm`도 함께 대조한다.
- Web csproj는 `net10.0`, `WasmBuildNative=true`다. 이 속성은 managed full AOT 활성화 증거가 아니다.
  [Microsoft AOT 문서](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0)는
  non-AOT IL interpreter/Jiterpreter와 AOT의 CPU·다운로드 크기 tradeoff를 구분한다. 실제 publish 속성과 runtime 산출물을 확인해야 한다.
- 이전 full AOT는 compiler stack overflow, isolated partial AOT는 browser `Maximum call stack size exceeded`로 실패했다.
  [원래 실패 기록](history/26-09-05/web-canvaskit-redesign-v2-results.md)을 유지한다. AOT를 즉시 켜면 해결된다고 제안하지 않는다.

## 4. 실행 계획

순서: **P0 → P1 → P2/P3/P4 중 측정상 큰 비용 순서 → P5 조건 판단 → P6**.
계획 당시 체크박스이며, 이번 실행에서 충족한 항목만 갱신했다. 성능 FAIL과 미검증 항목은 남긴다. 실행 시 source fingerprint/dirty diff부터 갱신한다.
`Doroti.Framework.*`는 유지보수하는 제품 소스다. 일반 수정에 전체 Dart 재생성이나 Flutter 최신판 일괄 이식은 필요하지 않다.

### P0. 측정 경계 보완과 원인 귀속

- [x] 기존 direct trace 옵션에 opt-in work-counter 수집을 연결한다. runtime 생성 전에 env를 설정하고 초기화 시점/ThreadStatic 소유를 검증한다.
- [ ] input sequence → Worker 수신 → callback 시작 → build/layout/paint/scene/raster → commit → callback 종료를 같은 frame에 연결한다.
  semantics tree/projection/serialize/post/DOM apply, finalizeTree도 별도 span으로 남긴다.
- [x] dirty/rebuild/delegate/keep-alive/animation owner별 bounded 집계를 추가한다. numeric type ID+후처리 이름표를 사용하고,
  진단 OFF 경로에 문자열·stack·트리 참조 보관을 넣지 않는다. retained node 참조로 GC를 방해하지 않는다.
- [x] 주요 type의 inclusive/self time을 구분한다. 상세 CPU sampling/할당 프로파일은 짧은 별도 원인 조사 run으로 수집한다.
  Worker/Wasm symbol이 불완전하면 귀속 불가로 기록하고 선택한 함수에 제한된 span을 추가한다. JS stack만으로 managed 함수 이름을 추정하지 않는다.
- [x] managed GC/할당, Wasm heap, GPU resource 수는 지원되는 관측 범위 안에서 기록한다. native allocation 측정값을 Web 값으로 대체하지 않는다.
- [x] 브라우저 main long-task/LoAF 정보와 Worker 자체 callback을 별도 열로 저장한다. main long task 0으로 Worker 정상 판정을 하지 않는다.
- [x] 상세 진단 ON 원인 조사와 최소 계측/OFF 사용자 동작을 분리한다. OFF에서도 최소 input/commit marker로 onset을 잴 수 있게 하고,
  observer 비용을 짧은 ON/OFF 대조로 추정한다. 진단 자체가 지연을 바꾸면 수치와 한계를 함께 기록한다.

산출물: trace schema/분석기, frame별 비용표, 상위 5개 type/호출 경로와 할당 후보, 비용 귀속이 안 된 잔여 구간.
완료 조건: 적어도 cold start/restart/새 section/resize의 지연 frame을 인과관계로 찾고 누락·이중합산을 탐지할 수 있다.

### P1. Flutter와 같은 상태 범위의 대조 및 샘플 이식 수정

- [x] 원래 넓은 State fixture(A)를 보존하고 progress만 별도 State로 옮긴 fixture(B)를 만든다. 동작·레이아웃·입력은 같게 유지한다.
- [ ] 실제 Flutter reference의 progress 동작(C)을 같은 viewport/DPR/폰트/진입 순서/접근성 조건에서 비교한다.
  Flutter Web는 Chrome Performance timeline을 사용한다. native DevTools profile 절차를 Web에 그대로 적용하지 않는다.
- [x] A/B의 work counter와 first frame 비용을 비교해 “작업량 감소”를 먼저 입증한다. Flutter와 비교할 때는 lazy 경계/방문 child 수 차이를 함께 표시한다.
- [ ] B에서 다른 section의 재빌드가 멈추고 개선이 확인되면 sample에 Flutter와 같은 state ownership을 반영한다.
  icon/selection/text 등 나머지 control도 같은 문제가 있는지 검토하고 필요한 부분만 상태 경계를 나눈다.
- [ ] 기존 lazy section, scroll controller, height estimate, key, focus, selection, theme 갱신, 한/두 column 전환에서 상태 보존을 확인한다.
- [x] A의 넓은 갱신 fixture는 공용 framework 스트레스 회귀로 유지한다. B의 성공을 A 또는 resize 성공으로 대체하지 않는다.

판단: B만 빨라지면 sample의 과도한 invalidation이 큰 원인이다. B도 느리거나 같은 child 수에서 Doroti 비용이 크면 P2의 공용 경로를 우선한다.
산출물: A/B/C 비교표, 상태 소유 변경 근거, 공용 수정이 여전히 필요한 범위.

### P2. 공용 rebuild·dependency·implicit animation 비용 감소

- [x] P0 hot path 순서로 `Widgets/framework.cs`, `sliver.cs`, `implicit_animations.cs`, 관련 Material widget을 검토한다.
  pinned Flutter의 동일 API와 최소 fixture를 나란히 비교한다.
- [ ] 중복 dependency 알림/불필요한 forced rebuild/반복 정렬이 확인되면 소유 경계에서 제거한다.
  기존 `_inDirtyList`, dirty depth 순서, build 중 dirty 추가, GlobalKey 이동, deactivate/dispose 계약을 보존한다.
- [ ] 불변 child 재사용이 유효한 공용 widget은 configuration 수명을 명시해 reuse한다. C#에 Dart const 효과가 자동 존재한다고 가정하지 않는다.
- [ ] implicit animation의 target equality/curve 교체/tween 갱신을 대조한다. 같은 값의 재빌드가 controller를 다시 시작하는 사례가 입증되면 수정한다.
  Trace에 보이는 controller 수만으로 불필요한 animation으로 분류하지 않는다.
- [ ] 측정상 할당/boxing/변환/컬렉션 순회가 큰 곳은 한 경로씩 typed fast path/재사용 buffer 등으로 개선한다.
  무효화·clear 규칙과 크기 상한을 둔다. 의미가 같은데 코드 모양만 바꾸는 대규모 치환은 하지 않는다.

필수 회귀: 동일 instance/변경 configuration, same-key update/key reorder, kept-alive 복귀 시 최신 데이터,
InheritedWidget/MediaQuery/theme/locale 갱신, callback 중 setState, 정당한 animation 재시작/정지/dispose.
완료 조건: 원래 문제를 재현하는 공용 fixture에서 불필요한 작업 수와 end-to-end 지연이 함께 줄고 기능 계약이 유지된다.

### P3. semantics를 변경 범위에 비례하도록 개선

- [ ] 공용 dirty propagation/geometry/projected tree와 Web full snapshot 처리 중 큰 구간부터 최적화한다.
- [ ] 모든 노드를 매번 정렬/직렬화하는 비용이 지배적이면 Web protocol에 명시적인 snapshot/delta 구분을 설계한다.
  `generation`, baseline generation, upsert/remove, child order/parent 관계, geometry/content 변경을 각각 표현한다.
- [ ] worker sender와 main receiver를 함께 수정한다. delta에서 빠진 node는 삭제가 아니며 삭제는 explicit remove로 전달한다.
  parent 변경·subtree 제거·root 교체·unknown baseline·restart/context recovery는 전체 snapshot으로 정확하게 복구한다.
- [x] 기존 `contentUnchanged`와 node/listener 재사용을 보존한다. 전체 snapshot protocol을 계속 쓸 경우에도 stable 순서와 변경 집계로 중복 allocation을 줄인다.
- [ ] coalescing 중 최신 content/action/focus/IME 갱신이 밀리지 않도록 우선순위와 최종 flush 계약을 검증한다.
  semantics 전체 비활성화는 비용 분리용 실험만 허용하며 제품 개선으로 채택하지 않는다.

필수 회귀: label/value/action 갱신, live region, 접근 가능한 순서, 스크롤 geometry, 화면 밖/안 이동,
node 제거 후 listener 정리, focused text/caret/한글 IME, selection, worker restart의 baseline 복구.
완료 조건: subtree 변경에서 처리 node/bytes/CPU 감소를 입증하고 일반·보조기술 입력 의미를 유지한다.
물리 screen reader/IME 관찰이 없으면 해당 항목은 notVerified다.

### P4. 새 section과 resize의 layout·raster·queue 개선

- [ ] 새 구간과 방문 구간을 각각 측정한다. 첫 section 구성, intrinsic/layout 반복, paragraph layout, paint recording,
  picture replay/promotion, GPU flush/resize allocation을 구분한다. 첫 progress의 낮은 layout 시간을 resize 전체로 일반화하지 않는다.
- [ ] 동일 constraints/텍스트/style/font generation에서 중복 paragraph/layout이 입증되면 공용 text 경계에서 재사용한다.
  width/text scale/locale/font/context 변화의 invalidate와 메모리 상한을 함께 정한다.
- [ ] single raster promotion의 최악값이 여전히 frame budget을 넘으면 pixel threshold/재사용 이력/예상 비용으로 admission을 조정한다.
  defer 시 원래 picture를 그려 픽셀을 보존하고 deferred 작업의 기아를 추적한다. 무조건 cache 확대나 강제 warm-up은 하지 않는다.
- [ ] resize를 observer→ingress→admission→framework begin→new exact scene→front로 분해한다.
  동기 frame 전에 최신 target을 한 번 수렴시키고 중복 metrics 알림/불필요한 layout이 확인되면 공용 경계에서 제거한다.
- [ ] 입력이 긴 frame을 기다리는 시간은 queue 최적화만으로 없어지지 않는다. frame 실행 시간을 줄이는 P2/P3 결과와 함께 판단한다.
  기존 Worker rAF와 bounded early resize wake를 유지하고 timer 강제 FPS를 해결책으로 삼지 않는다.
- [ ] capacity 안 resize와 실제 grow 완료를 분리한다. grow target에서 allocation/commit을 확인한 후 복귀하며 DPR/zoom/context loss를 별도 검증한다.

불변 계약: epoch/size/DPR 일치, 오래된 scene 재라벨 금지, CSS stretch 금지, current+latest backlog 상한,
terminal accounting, latest exact settle, context/surface resource 정리, 스크롤 anchor와 hit-test 일치.
완료 조건: CSS가 맞는 것에 더해 active resize 동안 새 layout front가 계속 나오고 지연 gate를 만족한다.

### P5. runtime 또는 Worker 분리의 제한된 후속 실험

이 단계는 무조건 도입하는 작업이 아니라 **조건을 판정하고 채택/보류 근거를 남기는 작업**이다.

- [ ] P1~P4 후에도 Web CPU 비용이 지배적이면 동일 work count의 Doroti native/Web fixture로 실행 비용 차이를 조사한다.
  renderer/GPU 차이가 섞인 전체 앱 숫자를 순수 runtime 비율로 쓰지 않는다.
- [ ] [WebAssembly runtime 문서](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-10.0)와
  설치 SDK/runtime props를 대조해 Release/trim/relink/Jiterpreter/SIMD 실제 설정을 기록한다. 이미 활성화된 설정을 개선안으로 중복 적용하지 않는다.
- [ ] AOT가 유력하면 이전 실패를 재현하는 최소 사례와 원인을 먼저 좁힌다. full/partial/non-AOT bin/obj/publish를
  `--artifacts-path`로 격리한다. 설치 SDK 수정·무제한 compiler retry·기본값 전환은 하지 않는다.
- [ ] AOT 후보는 publish 성공→실제 browser cold boot→sample correctness→동일 onset/sweep/resize 측정 순서로 진행한다.
  compile/runtime gate 실패 시 비교 불성립으로 종료하고 로그를 남긴다. 성공하면 download bytes/첫 content/RAM도 평가한다.
- [ ] UI/raster Worker 분리는 raster와 UI의 동시 진행이 유효하다는 trace가 있을 때만 별도 설계 후보로 남긴다.
  raster 분리로 600ms UI build 자체가 사라지지는 않는다. immutable scene IR, resource ID/수명,
  current+latest backpressure와 context 재생성을 먼저 설계하며 managed object/Skia handle을 그대로 넘기지 않는다.

산출물: 적용 조건 충족 여부, 최대 한 개 우선 후보의 bounded 결과 또는 보류 이유. 미입증 구조를 제품 기본으로 승격하지 않는다.

### P6. 통합 검증과 결과 보존

- [x] 변경된 공용 API의 계약/native Material/sample 회귀를 먼저 실행한다. Web TypeScript 및 Release build/publish를 검증한다.
- [ ] 기본/auto/오타와 explicit renderer, progress pixel 변화·정지, selection/text/scroll, resize admission,
  DPR/zoom, restart/context-loss 복구를 이전 suite로 확인한다. strict resize FAIL assertion을 낮추지 않는다.
- [ ] source runner와 별도 package 소비 앱에서 실제 public framework/host 변경이 반영되는지 확인한다.
  package/template/API 영향 시 한국어·영어 실행 문서와 ADR을 현재 기본값/상태에 맞춘다.
- [x] 아래 matrix를 실행하고 구현 완료, 자동 기능, 자동 성능, 사용자 관찰, notVerified를 별도로 보고한다.
- [x] 처음 실패·중간 실험·최종 결과를 dated history에 남기고 원본 JSON/trace와 source fingerprint를 연결한다.
  gate가 실패하면 PARTIAL을 유지한다. 계획 체크 완료나 build PASS를 성능 수용 PASS로 바꾸지 않는다.

## 5. 측정·수용 기준

### 5.1 공통 실행 조건과 반복 상한

- `.github/copilot-instructions.md`에 따라 **모든 테스트 프로세스에 20분 timeout**을 적용한다.
- 성능 실행은 서로 겹치지 않게 한다. 동시 build/다른 benchmark를 피하고 브라우저 visibility, GPU/backend,
  전원 상태, viewport/DPR/실제 Hz, font/asset cache, SDK/runtime/Flutter revision, publish fingerprint를 저장한다.
- 기능 검증은 조건별 1회부터, 성능 대조는 기본 3회다. 같은 조건의 반복은 원칙적으로 총 10회 이하,
  조건 변경·변동성 근거를 문서화한 경우에도 최대 20회다. 100회 반복이나 PASS까지 retry는 하지 않는다.
- 한 run의 조작 cycle도 기본 10 이하/근거 있을 때 최대 20이다. frame/event/다양한 data key 표본 수는 cycle과 구분한다.
- 상세 profile은 원인 조사 전용으로 제한하고 최종 성능 corpus에 섞지 않는다. 가벼운 gate가 실패하면 원인을 고친 뒤 필요한 경우에만 반복한다.
- 각 run과 pooled 분포를 별도로 제공한다. 작은 onset 표본에서 p95는 참고값이며 3/3 단기 통과와 장기 신뢰도를 구분한다.

### 5.2 반드시 분리할 시나리오

| 시나리오 | 측정 범위 | 중요한 통제 |
| --- | --- | --- |
| 첫 progress | pointerup→첫 새 scene, 첫 100/500/1000ms, 이후 5초 | 기존 3초 warm-up 제거, cold app boot와 별도 |
| idle restart | start→stop→5초 idle→restart | 최초 시작과 혼동 금지 |
| 첫 scroll/새 section | 실제 wheel→새 scene와 새 child 구성 비용 | visited 상태 초기화와 진입 구간 식별 |
| 방문 section 재진입 | 같은 입력/거리의 revisit | 신규 구간 원본과 통계 분리 |
| live resize | slow/fast/reversal, idle 및 animation 중 | active front, target age, exact settle, CSS/pixel marker |
| capacity/DPR/context | grow 완료→복귀, DPR1/2, 실제 page zoom, context loss | superseded target을 allocation PASS로 세지 않음 |
| semantics/입력 | 선택·focus·한글 IME·접근성 action과 scroll/resize 동시 | ON/OFF 기능 축과 성능 fixture 조건 명시 |

### 5.3 성능 hard gate와 판정

60Hz 기준의 이전 목표를 유지한다. 실제 120Hz 수용은 별도 주기 예산으로 검증한다.

| 지표 | 목표 | 관측의 의미 |
| --- | --- | --- |
| input→첫 새 scene commit | p95 ≤50ms, max <100ms | commit notification; 실제 표시 지연은 별도 |
| 시작 후 첫 1초 | 50ms 초과 framework callback 0 | 동기 raster 포함, main task와 별개 |
| 변화가 계속 있는 steady 구간 commit 간격 | p95 ≤20ms, max ≤33.4ms | 제출/commit cadence; 표시 FPS 아님 |
| active resize 새 front 간격 | p95 ≤33.4ms, max <100ms | 시작/끝 경계를 포함해 starvation 탐지 |
| resize target 추종 | p95 ≤50ms, max <100ms | superseded/미도달 분모를 함께 기록 |
| 마지막 observer→latest exact | ≤100ms | 최종 exact geometry/epoch 확인 |
| correctness/resource | stale relabel/stretch/누수 0, backlog 상한 유지 | 자동 검사와 실제 화면 검증 분리 |

채택 전에는 동일 조건 3회 모두 hard gate를 확인한다. 상대 개선만 있고 hard gate 미달이면 **개선 / PARTIAL**이다.
원인 fixture의 work count 감소 없이 한 번의 시간 단축만으로 공용 최적화를 확정하지 않는다.
실제 창 테두리 조작, trackpad, page zoom/monitor DPR, 120Hz, 한글 IME/caret/screen reader 및 Flutter 시각 동등성은
자동화로 대체할 수 없는 부분을 명시해 사용자 관찰 또는 별도 장치 검증으로 남긴다.

## 6. 이번 조사 산출물과 다음 실행의 첫 결정

이번 조사에서 확정한 것은 **큰 build 구간**, **Flutter와 다른 샘플 상태 소유 범위**, **Web semantics 전체 snapshot 처리**,
**direct work-counter 관측 누락**, **단일 Worker 동기 실행 구조**다. 각 항목이 전체 지연에 기여하는 비율은 추가 귀속이 필요하다.

다음 구현의 첫 작업은 P0 최소 계측과 P1 progress A/B 대조다. 이후 가장 큰 비용의 공용 경로를 고친다.
캐시 숫자 조정, AOT 전환, Worker 분리부터 시작하지 않는다. 기존 성능 FAIL과 사용자 체감은 그대로 기준선으로 보존한다.

문서 검증: 기존 work2의 byte-identical archive, 로컬 링크, 현재 소스/Flutter revision, 기존 dirty 파일 보존 및 whitespace를 확인한다.


## 7. 2026-09-07 실행 상태와 남은 수용

| 단계 | 이번 결과 | 남은 항목 |
| --- | --- | --- |
| P0 | direct work/profile/GC 관측, early causal capture, 최소 OFF marker, main/Worker 분리, Chrome CPU trace | 정확한 managed 함수 symbol/GC pause·Wasm heap 귀속, 모든 ingress/span의 완전성 및 cold app boot end-to-end |
| P1 | broad/local fixture, rebuild 2796→56·delegate 3→0, local progress 채택, scroll/column/state/픽셀 회귀, Flutter timeline 3회 | 나머지 control의 broad 갱신; Flutter commit/pixel endpoint와 lazy 단위가 달라 엄밀한 C 동등성은 미완료 |
| P2 | pinned source·상위 hot type·implicit owner 검토/계측 | 별도의 공용 불필요한 rebuild 결함은 미확정. dirty/key/keep-alive 계약을 우회하는 수정은 채택하지 않음 |
| P3 | 동일 노드 JSON 재사용, 2MiB/2048개 상한, 전체 snapshot·content/geometry·listener 계약 유지 | tree projection·전체 traversal/bytes O(n), 물리 접근성 검증. delta 도입 조건은 보류 |
| P4 | 새/방문 section 각각 3회, DPR1/2 grow 완료→복귀, strict resize FAIL 원본, context recovery | 새 section CPU 및 resize 지연 gate 미달. 중복 paragraph/promotion 단독 원인은 미확정이므로 캐시 확대/queue 우회는 미채택 |
| P5 | SDK/runtime 실제 속성 및 공식 문서 대조, 조건부 채택 여부 판정 | 동일 work-count native/Web 비용 비율 미분리. AOT/Worker 분리 보류, 과거 실패 보존 |
| P6 | source/Release publish/native/package 자동 검증, 한국어/영어 실행 안내, 원본·fingerprint 보존 | 물리 입력/시각/IME/120Hz 및 별도 package Web 앱의 실제 browser 수용 notVerified |

최종 수치와 실패 로그는 실행 보고를 따른다. 첫 progress·resize hard gate가 남아 있으므로
이 문서를 삭제하거나 전체 완료/PASS로 전환하지 않는다. 다음 수정은 새/방문 section 및 resize의
지연 frame에서 공용 build/layout 비용을 더 좁히고, 변경 전후 동일 작업량의 end-to-end gate로 판단한다.


## 8. 사용자 확인 후 상태 변경/resize 추가 개선

이전 progress 시작 버벅임은 사용자가 체감상 해소됐다고 확인했다. 새 요청에 따라
선택/버튼 예제를 각각 갱신하고 responsive 내용/delegate 및 navigation 구성을 재사용했다.
공용 Web semantics의 geometry-only 직렬화 경로도 줄였다.
[후속 실행 보고](history/26-09-07/web-state-resize-followup.md)에 최초 실패, 변경별 원인 대조,
최종 3회 측정과 검증을 별도로 보존했다.

- 선택 버튼 상세 대조: 508.1→80.5ms. 최종 최소 계측 3회: 78.9 / 80.9 / 78.3ms.
- 같은 열 구성 resize 최종 3회: latest exact 51.2 / 69.6 / 59.7ms.
  active tracking p95 122.7 / 126.8 / 131.5ms로 전체 실시간 수용은 여전히 PARTIAL이다.
- 변경한 State/위젯 재사용과 제어/테마/DOM 유지 회귀는 통과했다.
  strict resize DPR1/2 성능 FAIL 및 물리 입력/표시 검증 미완료를 유지한다.

## 9. 이미지 구간 진입 및 멈춘 뒤 스크롤 재시작

사용자 관찰은 어느 위치든 멈춘 뒤 재시작할 때 끊기며, 이미지 구간의 상단 진입에서
특히 심하다는 것이다. 공용 target-size 디코딩과 ResizeImage의 키/종횡비/전달 결함을
수정하고, 샘플 표시용 픽셀 수를 줄였다. geometry-only semantics 직렬화의 임시
객체도 줄였다. 실제 이미지 표시와 성능을 따로 검증하며 중간 실패를 보존한다.
[이미지 스크롤 후속 보고](history/26-09-07/web-image-scroll-followup.md)에 결과를 기록한다.
첫 구간 생성과 전체 스크롤 재시작/resize의 수용 상태는 PARTIAL로 유지한다.

## 10. 열 구성 복귀 시 오른쪽 누락 회귀

2열에서 스크롤한 뒤 1열→2열로 돌아오면 오른쪽이 회색으로 남는 오류를 재현했다.
공용 Element.activate의 nullable dependency 정리를 수정하고, 반복 부모 이동의
State/inherited 값 유지와 오른쪽 실제 픽셀 검사를 추가했다.
[열 복귀 수정 보고](history/26-09-07/web-column-return-fix.md)에 최초 실패와 검증을 보존한다.
이 correctness 수정으로 이전 성능 수용 PARTIAL을 변경하지 않는다.

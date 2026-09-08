# .NET WASM에 맞춘 Doroti 재빌드·레이아웃 구조 개편 계획

작성: 2026-09-07. 실행: 2026-09-08. 상태: **구현·실험·검증 실행, 최종 수용 PARTIAL / indexed 승격 보류**.

실행 근거와 미달 gate는 [구조 개편 실행 기록](history/26-09-08/wasm-section-structure.md)에 보존한다. 기본 eager 경로와 indexed 비교 후보를 구분하며, 원래 목표를 완화하거나 계획을 삭제하지 않는다.

## 1. 목표와 작업 범위

DorotiTestbedApp의 리사이즈, 1열↔2열 전환, 첫 섹션 진입, 버튼·탭·progress 조작에서 발생하는 긴 UI callback을 줄인다. Flutter에서 옮긴 Widget/Element/RenderObject의 공개 동작은 유지하면서, 내부 실행 단위를 .NET WASM의 호출·할당 비용에 맞춰 작게 만든다. 샘플은 재현 앱이자 첫 적용 대상이며, 재사용할 수 있는 기능은 Framework에 둔다.

최초 문서 작성 요청의 산출물은 이 작업계획이었다. 후속 요청 `work.md의 전체작업해줘`에 따라 구현과 benchmark/검증을 실행한다. renderer 기본값 변경과 배포는 별도 승격 근거 없이 하지 않는다. 실행 시작 HEAD `0cc805a4`의 작업 트리는 깨끗했고, 문서에서 미커밋으로 언급했던 텍스트 raster/cache/compositing 및 반응형 열 수정은 현재 기준선에 포함되어 있다.

최종 방향은 **변하지 않는 트리를 다시 만들지 않고, 화면에 필요한 영역만 배치하고, 이미 계산한 순수한 결과를 명시적인 의존성으로 재사용하는 구조**다. 모든 Widget을 새 엔진으로 한 번에 교체하지 않는다.

## 2. 현재 소스와 증거

| 확인한 사실 | 의미와 한계 |
| --- | --- |
| `SampleHome.build`는 theme/breakpoint/destination 등이 같으면 `_home`을 재사용한다. | 같은 열 구성의 resize에서 본문 rebuild가 0이어도 layout 비용은 남을 수 있다. `build` 횟수만으로 성능을 판단할 수 없다. |
| `BuildAnimatedHome`의 AnimatedBuilder가 매 tick 새 Scaffold와 ComponentsScreen을 만든다. | navigation 애니메이션의 변경 범위가 본문까지 넓다. 생성 횟수와 실제 update/build/layout 횟수는 각각 측정해야 한다. |
| `ComponentsState`는 section/list Widget을 캐시하지만 섹션별 `SliverToBoxAdapter`를 모두 배치한다. | paint culling과 layout 생략은 다르다. `RepaintBoundary`는 자식의 크기 재계산을 차단하지 않는다. |
| `CreateSections`는 섹션 builder 실행 때 `factory(ctx, setter)[sectionIndex]()`를 호출한다. | 한 섹션을 갱신하려고 그룹의 factory 목록을 재생성할 수 있다. 할당과 숨은 Widget 생성 비용을 P0에서 확인한다. |
| 기존 `RenderSliverList`에는 viewport/cache 범위 생성·회수와 keep-alive 경로가 있다. | 공용 primitive 재사용 가능성을 먼저 검증한다. 정확한 끝 위치 때문에 전 섹션 layout을 택했던 이유를 해결해야 한다. |
| 1열↔2열은 GlobalKey 섹션을 서로 다른 scroll subtree로 옮긴다. | State 보존은 중요하지만 대량 deactivate/activate, inherited dependency 재구독, layout 비용이 동반될 수 있다. |
| 이번 열 수정은 `TwoColumns` 하나로 목록 분할과 오른쪽 열 표시를 결정한다. | 1000 logical px 이하는 1열, 초과는 2열이다. 열 애니메이션은 제거했고 navigation 애니메이션은 유지했다. 성능 해결 판정은 아니다. |
| Web 앱은 `net10.0` / `browser-wasm`, `WasmBuildNative=true`다. | native relink와 managed AOT는 별개다. 실제 평가된 MSBuild 속성과 실행 asset을 기준선에 기록해야 한다. |

이전 측정은 현재 HEAD의 새 기준선이 아니다. workload와 renderer를 분리하여 참고한다.

| 기록 | 결과 | 해석 |
| --- | --- | --- |
| [Web retained rendering](history/26-09-07/web-sample-retained-rendering.md) | CanvasKit 신규 섹션 callback 최대 723.8ms, layout/compositing 668.5ms, raster 11.0ms | 해당 workload에서 managed UI가 큰 병목이었다. 현재 direct resize의 시간 분해로 대신 쓰지 않는다. |
| [상태/resize 개선](history/26-09-07/web-state-resize-followup.md) | 기존 Week build 446.8ms; 국소 갱신 이후 onset 약 78~81ms. 같은 열 resize에서도 callback 최대 약 125~129ms | 넓은 build 범위와 별도의 layout 병목이 모두 존재했다. strict resize gate는 미달이다. |
| [direct 기본값 실행](history/26-09-07/web-direct-default-execution.md) | direct cold progress onset 694.8~755.8ms, restart 570.9~632.5ms; sweep callback 최대 497.5ms | 비용을 최초 생성에만 귀속할 수 없다. 재시작, semantics, paint/raster admission도 다시 분해한다. 동적 호출 정적화 실험은 효과 미확인으로 이미 되돌려졌다. |
| [반응형 열 수정](history/26-09-07/sample-responsive-columns.md) | Web 기능 7 PASS, native 중간 프레임 1,040 PASS. 기존 fixed-sleep 검사는 이전 800px front를 캡처해 실패 | 기능 통과는 latency 통과가 아니다. 테스트 대기를 늘린 결과를 성능 개선으로 보고하지 않는다. |

현재 제품 비교 기준은 Web `worker-direct-webgl`이다. CanvasKit은 별도 비교군으로 유지하고 예전 문서의 renderer 기본값을 가져오지 않는다. 과거 33.3ms/50ms/100ms 계열 gate 실패 및 `PARTIAL / notQualified` 결론을 새 PASS로 덮어쓰지 않는다.

## 3. 설계 결정

### A. 상태·빌드·레이아웃·그리기의 변경 범위를 분리한다 — 우선 적용

navigation shell과 gallery body를 안정적인 자식 트리로 나눈다. rail/bar의 크기·이동만 전용 animation render object 또는 좁은 AnimatedBuilder에서 갱신한다. 실제 본문 폭이 바뀌면 본문 layout은 필요하지만, 그 이유로 본문 Widget을 다시 만들 필요는 없다.

gallery는 `SectionId`, section descriptor, 상태 저장소, mounted view로 나눈다. descriptor는 특정 섹션 하나의 builder를 직접 가리키며 그룹 factory를 매번 다시 호출하지 않는다. 상태 저장소는 값·controller·명시적 resource 수명을 소유한다. immutable descriptor에 BuildContext나 오래된 ThemeData를 캡처하지 않는다.

공통 dependency는 geometry(width, height, text scale, direction), visual(theme/brightness), content(data revision), interaction(selection/focus)으로 구분한다. 기존 MediaQuery/InheritedWidget 구독을 계속 전달한다. 새로운 전체 화면 revision 하나로 모든 캐시를 무효화하지 않는다. Theme 변경도 font/shape 등 metric 영향이 있는지 확인하며 색만 바뀐 경우와 구분한다.

### B. section 가상화와 높이 정보 저장을 함께 도입한다 — 핵심 개편

갤러리 전체를 여러 eager sliver로 구성하는 대신, 한 열당 하나의 section viewport에서 보이는 section과 제한된 앞뒤 여유 영역을 관리한다. 첫 구현은 기존 `SliverList`/child manager를 재사용한다. 가변 높이·열 이동 계약을 만족시키지 못하는 지점을 수치와 재현으로 확인한 뒤에만 별도 `SectionViewport`/render sliver를 추가한다. 이름은 잠정 내부 명칭이다.

각 section에는 `unmeasured / estimated / measured / invalidated` 높이 상태와 revision이 있다. 재사용 키는 최소 `SectionId + 실제 제약 폭 + content/metric revision + text scale + font generation + direction`을 포함한다. 단순 DPR 변경이 logical layout에 영향을 주지 않는 경로는 layout과 raster 무효화를 구분한다. 번역·font fallback·이미지 비율 변경으로 키가 달라지는 경우를 검사한다. 폭을 임의로 반올림해 줄바꿈 결과를 재사용하지 않는다.

높이 테이블과 누적 높이 인덱스로 viewport에 해당하는 항목을 찾는다. 기본 탐색은 O(log N + visible items)를 목표로 한다. 과거 높이는 미측정 항목의 추정치로만 사용하며 새 폭의 확정 geometry로 제출하지 않는다. 폭별 캐시는 최근 2개 구성을 시작점으로 측정하고, 항목 수·bytes·수명 상한을 둔다. 모든 resize 폭을 무제한 저장하지 않는다.

**미측정 가변 높이 목록에서 정확한 전체 높이와 완전한 lazy layout을 동시에 공짜로 얻을 수는 없다.** 다음 계약으로 전환한다.

- 보이는 항목은 실제 폭으로 정확히 측정하고 그린다. 전체 scroll extent는 남은 항목을 측정할 때까지 추정치일 수 있다.
- extent가 보정되면 `anchor SectionId + section 내부 offset`을 기준으로 현재 화면을 유지한다. 기존 pixel offset만 그대로 복사하지 않는다.
- End/scrollbar 끝 이동은 마지막 항목을 실제 배치하고 보정하는 명시적 경로를 둔다. 임시 추정 끝에서 영구적으로 멈추거나 모든 중간 항목을 동기 생성하지 않는다.
- keyboard traversal, ensureVisible, 접근성의 화면 밖 항목 탐색은 대상 항목의 materialization을 요청할 수 있어야 한다. viewport 밖이라는 이유로 focus 대상이나 semantics action을 없애지 않는다.
- 긴 단일 section 자체가 budget을 초과하면 section 내부를 재사용 가능한 block 단위로 나눈다. viewport 가상화만으로 그 비용이 사라진다고 가정하지 않는다.

### C. State 수명과 화면 배치 수명을 분리한다 — B의 선행 조건

열 수 변경은 stable `SectionId → column/order` 배치 정보 변경으로 표현한다. 동일 section을 두 부모에 동시에 붙이지 않고, 하나의 frame 안에서 배치 변경과 anchor 복원을 완료한다. Material의 내부 State까지 자동 직렬화할 수 있다고 가정하지 않는다.

첫 단계에서는 방문한 중요한 subtree를 재사용하되, 방문하지 않은 subtree는 만들지 않는다. 이후 가상화 eviction은 명시적으로 상태를 복원할 수 있는 section에만 허용한다. 편집 중 TextField/IME, focus, 열린 overlay, drag/selection을 소유한 subtree는 종료 또는 안전한 소유권 이전 전까지 pin한다. 일반 StatefulWidget의 State를 임의로 폐기하고 값만 복사하지 않는다.

1열 anchor는 현재 보이는 section을, 2열은 각 열의 anchor를 저장한다. 다른 열 구성으로 갔다 돌아올 때의 primary anchor 우선순위와 개별 열 복원 규칙을 문서화하고 테스트한다. GlobalKey를 일괄 제거하지 않는다. 대량 reparent를 피하는 내부 slot 소유 구조는 이 계약을 만족할 때 채택한다.

### D. .NET용 실행 경로를 추가한다 — 프로파일로 선정한 부분만 적용

Framework의 범용 Widget API 아래에 변경 이유와 계산 결과를 명시하는 작은 경로를 둔다. layout 결과는 부모가 필요로 하는 size/baseline/overflow를 함께 갖고, dependency가 같을 때만 재사용한다. 객체 참조 동일성만으로 geometry가 같다고 판단하지 않는다.

우선 후보는 section shell의 padding/constraint/column placement처럼 순수한 계산이다. 이 부분을 typed value data와 직접 호출로 표현하여 중첩 adapter, LINQ 열거, 반복 delegate 목록, boxing/dynamic dispatch를 줄인다. Material 내부 전체를 flattened tree로 바꾸는 것은 이 계획의 기본 선택이 아니다. `FrameworkWorkProfile`의 self time과 WASM 할당에서 확인된 상위 경로부터 최대 2개씩 적용한다.

같은 제약에서 기존 RenderObject layout fast path가 이미 동작하면 중복 캐시를 추가하지 않는다. 새 fast path는 intrinsic/dry layout, baseline, unbounded constraint, RTL, text scale, nested scroll, semantics transform의 기존 결과와 비교한다. 지원하지 않는 조합은 기존 정확한 경로를 사용하며 fallback 횟수도 측정한다.

pooling은 frame-local scratch buffer에 한정한다. pooled buffer가 제출된 scene, retained picture, paragraph, async task의 데이터와 alias되지 않도록 한다. Widget/Element/State/controller를 범용 pool에 넣지 않는다. 문자열·JSON 진단 생성은 캡처 시점으로 미루고 hot path는 bounded 숫자 계측을 사용한다.

### E. 프레임 budget은 안전한 작업 경계에 적용한다

입력 처리 → 최신 resize admission → 보이는 영역의 build/layout → 일관된 scene 제출 → 여유 작업 순으로 관리한다. 기존 latest pending/current frame과 input sequence 계약을 유지한다. 키·포인터 입력을 버리거나 최종 resize generation을 생략하지 않는다.

초기 실험값은 offscreen 준비 batch당 2ms, 선행 생성량은 viewport 앞뒤 각각 최대 한 화면이다. 실제 baseline 이후 조정 근거를 기록한다. 동기 widget build 하나가 이미 수백 ms라면 타이머만 추가하지 말고 B/D에서 작업 단위를 줄인다.

`performLayout`/build flush 중간에 await하거나 일부 갱신된 트리를 paint/hit-test/semantics에 노출하지 않는다. yield는 완결된 section 준비 작업 사이 또는 다음 frame 예약 지점에서만 가능하다. speculative 계산은 immutable 데이터만 사용하고 revision 변경 시 폐기한다. stale 작업 중단으로 active layout 계약이 깨지지 않도록 한다.

### F. AOT·Worker 분리는 별도의 조건부 실험이다

현재 default direct 경로는 framework와 Skia raster가 한 Worker에 있다. B/D로 UI 일을 줄여도 긴 raster가 input/resize를 막는다고 P0/P5가 입증할 때 기존 CanvasKit 분리 경로와 비교한다. 새 Worker로 live Widget/Element/Paragraph/Skia handle을 넘기지 않는다. owner/generation을 가진 immutable command/resource 계약 없이는 새 병렬 경로를 만들지 않는다.

managed AOT는 CPU 비용과 다운로드·시작 시간의 tradeoff다. full/partial AOT의 이전 실패를 보존하고 기본 interpreter/relinked 빌드에서 구조 개선을 먼저 입증한다. AOT가 실패해도 구조 개선 검증을 진행할 수 있도록 분리한다. WASM SIMD/threads는 해당 hot loop 또는 독립 계산의 이득이 증명될 때만 후속 후보로 둔다. DOM/Blazor `ShouldRender`를 Doroti Widget에 직접 적용하는 식의 대체는 하지 않는다.

Microsoft 공식 [AOT 설명](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0)은 managed AOT의 실행 성능/배포 크기 tradeoff를 설명한다. [rendering 지침](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/rendering?view=aspnetcore-10.0)의 불필요한 subtree 처리 감소 원칙은 참고하되, Blazor component 수치와 DOM 가상화 구현을 Doroti의 canvas renderer 측정값으로 사용하지 않는다.

## 4. 단계별 실행계획

각 단계는 `미착수 / 진행 / 구현완료 / 기능검증 / 성능검증 / notVerified`를 별도 기록한다. 기능 통과만으로 다음 단계의 성능 개선을 주장하지 않는다. 성능 gate 미달은 수치를 남기고 다음 원인 분석으로 연결한다.

| 단계 | 작업과 주요 파일 | 완료 조건 |
| --- | --- | --- |
| P0 기준선·비용 분해 | `FrameworkWorkCounters.cs`, `FrameworkWorkProfile.cs`, `measure-state-resize.mjs`, `measure-material-sample.mjs`, resize helpers 확장 | 현재 diff/asset/renderer/SDK/build mode를 고정하고 시나리오별 build/layout/paint/semantics/raster/queue/GC 또는 할당 데이터를 분리한 before corpus 작성 |
| P1 animation 갱신 범위 축소 | `SampleApp.cs`, 필요 시 `Framework.Widgets/transitions.cs` 또는 재사용 가능한 좁은 animation widget | navigation tick에 변화 없는 section builder 호출 0; 실제 폭 변경 layout은 수행; tab/theme/route/interaction 정상 |
| P2 section descriptor·상태 소유권 | `Components.cs`, section별 partial 파일, 재사용 가능한 section adapter | 단일 상태 변경이 해당 section만 갱신; 그룹 factory 목록 재생성 제거; focus/IME/overlay/State 수명 표와 disposal 검사 |
| P3 viewport 가상화·anchor | `Framework.Widgets/sliver.cs`, `Framework.Rendering/sliver_list.cs`, `sliver_multi_box_adaptor.cs`, 필요 시 새 section viewport; sample 적용 | visible+cache 중심 생성/layout; 1↔2열·끝 이동·동적 높이 보정·복귀 상태 보존; eager 전체 측정 의존 제거 |
| P4 typed 계산·할당 최적화 | 프로파일로 확정한 Rendering/Widgets 경로, 필요 시 `text_painter.cs`/`paragraph.cs` | 상위 hot path 후보별 WASM A/B와 정확성 비교; 이득 없는 정적화·pooling은 채택하지 않음 |
| P5 작업 budget·runtime 비교 | `Framework.Scheduler/binding.cs`, Web Worker scheduling/diagnostics, Web csproj 실험 설정 | 보이는 frame의 일관성 유지, offscreen 준비 제한, input queue 무기아; AOT/renderer 비교는 각각 별도 결과 |
| P6 통합·승격·문서 | native contracts, Web Playwright, platform build/run matrix, history/README | 기능·성능·메모리·플랫폼 증거를 분리한 최종 보고, 임시 probe 제거, 채택/미채택/잔여 gate 명시 |

의존 순서는 P0 → P1 → P2 → P3 → P4 → P5 → P6이다. P0에서 다른 병목이 우세하면 이유와 변경된 우선순위를 이 문서에 기록한다. 예를 들어 same-column resize의 build가 0이면 P1만 반복하지 않고 P3/P4 layout로 진행한다. P4/P5 후보가 이득이 없다는 결과도 실험 완료로 기록하되 성능 목표 완료와 구분한다.

## 5. 측정과 수용 기준

### 기준선과 비교 절차

1. `git status`/diff 해시, 실제 served asset manifest, SDK/runtime, evaluated MSBuild 속성, browser/GPU/DPR/viewport를 저장한다. 현재 미커밋 수정이 before와 after에 동일하게 포함되도록 isolated artifact root를 사용한다.
2. direct Release non-AOT/relinked를 주 비교군으로 한다. trimmed publish를 별도로 확인한다. CanvasKit·AOT·partial AOT는 각각 자체 before/after이며 서로의 phase 값을 섞지 않는다.
3. 후보 탐색은 동일 시나리오 3회, 채택 후보는 before/after 각각 독립 5회 이상 실행한다. build·다른 GPU benchmark와 겹치지 않는다. 계측 ON 비용 분석과 최소 marker/OFF 성능 검증을 구분한다. cold는 새 runtime, visited/restart는 동일 runtime의 별도 workload다.
4. .NET 할당은 지원되는 runtime API를 확인해 bytes/action 및 collection count를 기록한다. GC pause를 직접 관찰할 수 없으면 `notMeasured`로 둔다. Chrome process memory, JS heap, WASM memory capacity, managed live bytes를 다른 지표로 기록한다. CLR 할당 감소율을 WASM 개선율로 대체하지 않는다.
5. 원본 로그/JSON/실패/timeout을 보존한다. 측정은 retry 0, 각 test/benchmark 외부 timeout **20분**. harness 실패 표본을 임의로 0ms나 PASS로 넣지 않는다.

### 필수 workload

- cold narrow/wide first content; 390/800/999/1000/1001/1280/1500/1501 폭, DPR 1/1.25/1.5/2.
- same-column 연속 resize, breakpoint 왕복, 애니메이션 도중 재반전, 화면 높이만 변경, 최소 크기 및 grow-capacity 복귀.
- 양 열 깊은 scroll → 1열 → 추가 scroll → 2열; 신규 section/visited section을 각각 측정. 10배 section 수의 합성 fixture로 비용 증가 형태도 확인.
- 첫 선택/첫 progress, stop → 5초 idle → restart, hover/pressed/tab/focus; scroll 도중 입력.
- text scale, RTL/locale, font 로딩, 이미지 크기 확정, theme 변경, 동적 항목 추가/제거, End/ensureVisible.
- Web CDP 자동 재현과 실제 창 테두리 drag를 별도 corpus로 유지. synthetic viewport step이나 commit 알림을 물리 scan-out 증거로 쓰지 않는다.

### gate

아래는 **목표**이며 아직 달성한 수치가 아니다. P0에서 측정 정의와 표본 수를 고정한다. 목표를 변경하려면 원래 목표·실패와 변경 이유를 함께 남긴다.

| 분류 | 기준 |
| --- | --- |
| 기능 | 기존 column/selection/scroll/picker/IME/semantics/hover raster 회귀 통과. runtime error 0, 중복 key 0, 잘못된 hit target 0, 폐기 State 재사용 0 |
| 갱신 범위 | 같은 데이터/스타일의 navigation tick에서 section rebuild 0. viewport와 무관한 section layout은 측정·보정 예외를 제외하고 요청하지 않음. 항목 10배에 visible-frame 비용이 선형 증가하지 않음 |
| 구조 개선 채택 | 목표 workload의 UI callback p95가 독립 run 요약 중앙값에서 30% 이상 감소하고 아래 기능/메모리 gate 충족. 비대상 workload p95가 10% 넘게 악화되면 원인 해결 전 승격 보류. 이는 중간 채택 기준이며 최종 latency PASS와 다름 |
| UI 지속 작업 | 60Hz 대상 warm UI callback p95 ≤16.7ms, max ≤33.3ms. 비용 분해는 합산 중복 없는 self time/phase 경계 사용 |
| 입력 응답 | first selection/progress와 idle restart input→해당 sequence의 새 exact scene commit p95 ≤50ms, max ≤100ms. 렌더링 의미 변화 없는 입력은 별도 response marker 정의 |
| resize | target→caught-up-front p95 ≤33.3ms, active commit 간격 p95 ≤33.3ms, 관측 경계를 포함한 active gap <100ms, 마지막 observer→latest exact ≤50ms. 오래된 front의 CSS 확대를 정답으로 세지 않음 |
| 신규 section | 첫 노출 요청→해당 section의 정확한 content commit p95 ≤50ms, max ≤100ms. placeholder만 나온 시각과 분리. cold app boot는 별도 first-content 분포로 보고 |
| 스크롤 안정성 | 보정 후 동일 anchor의 오차 ≤1 logical px. 항목 삭제/끝 clamp는 사유 명시. 실제 줄바꿈·baseline·hit rect는 정답 경로와 비교 |
| 메모리 | cache별 item/byte/lifetime 상한을 구현. 20회 왕복 후 마지막 5회 steady window에서 live resource가 계속 증가하지 않음. transient peak/live bytes가 baseline 대비 10% 이상 증가하면 원인과 예산 해결 전 승격 보류. WASM capacity 증가만으로 leak 판정하지 않음 |
| boot/배포 | 구조 변경으로 cold first-content p95 또는 payload가 10% 이상 악화되면 분석 후 승격 보류. AOT 크기/시작시간 tradeoff는 별도 판정 |

성능 통계에는 미도달 resize target, dropped/stale generation, queue 길이와 시간가중 geometry 오차를 포함한다. 도달한 target만 골라 빠른 수치를 만드는 편향을 막는다. 모든 독립 run의 hard gate와 원본을 공개하고 pooled percentile 하나로 느린 run을 숨기지 않는다.

## 6. 호환성과 플랫폼 검증

공용 Framework를 변경하면 Windows/Web뿐 아니라 Android/Linux/Qt/MAUI/Apple host에도 영향을 준다. public Flutter형 API와 layout/State/semantics 동작을 유지한다. host 차이는 명시적 capability/예산 정책으로 두며 `isWeb` 분기로 correctness를 달리하지 않는다.

- native deterministic/mounted contracts: 생성·폐기·reparent·intrinsic/baseline·hit-test·scroll anchor·cached/direct 비교.
- Web direct + CanvasKit: DPR별 geometry/실제 PNG, input sequence, semantics identity, resize/context-loss/restart, publish 실제 asset.
- Windows: Release build, native sample, GPU intermediate-frame/hover/cache 회귀, 물리 resize/입력 확인.
- Android/Linux/Qt/MAUI: 설치된 toolchain 범위의 build 및 실행. touch, DPI, focus, lifecycle/pause-resume가 달라지는 경계 확인.
- macOS/iOS/MacCatalyst: 가능한 환경에서 build/실행 및 IME/VoiceOver/scale 확인. 실행 환경 부재는 `notVerified`이며 다른 플랫폼 PASS로 대체하지 않음.

완성 여부는 `구현`, `자동 기능`, `정량 성능`, `물리 표시/입력`, `사용자 체감` 다섯 칸으로 보고한다. unavailable 검증 때문에 구현 성과를 지우지 않되, 전체 플랫폼 검증 완료라고 쓰지 않는다.

## 7. 실험 중단·복귀 원칙

새 viewport/typed layout 경로는 내부 비교 스위치와 기존 정답 경로를 두고 단계적으로 검증한다. product UI에 구현 옵션을 노출하지 않는다. 범위가 확인되지 않은 generic layout cache, 전체 Widget 평탄화, 전면 reactive API 전환은 기본 실행 범위에서 제외한다.

한 후보가 3회 통제된 비교에서 효과가 없으면 추가 호출 정적화·pooling을 누적하지 않고 되돌린다. 대량 작업을 긴 하나의 callback에서 실행한 뒤 타이머/idle이라는 이름만 붙이지 않는다. 정확성 회귀가 생기면 그 후보만 격리하고 기존 사용자 수정과 앞서 통과한 단계는 보존한다.

P6에는 채택된 내부 구조, A/B 산출물 경로, gate별 결과, 미채택 후보의 이유와 남은 병목을 기록한다. 진행률 때문에 FAIL을 PASS로 바꾸거나 `work.md`를 지우지 않는다. 이 계획의 구현 완료 여부는 실제 코드와 검증 결과로 갱신한다.

## 8. 실행 체크리스트

- [x] P0 현재 소스/런타임 기준선 및 분리된 workload 측정 — 독립 5회 corpus, GC pause/live/transient peak 미측정은 명시
- [x] P1 navigation animation과 안정적인 본문 subtree 분리 — 기본 경로 반영, 불필요한 section rebuild 0 자동 검사
- [x] P2 section descriptor, 국소 변경, 상태/소유권 계약 — 직접 builder, 상태 수명 표, unpin/dispose 검사
- [ ] P3 section 가상화, 높이 인덱스, anchor·focus·끝 이동
- [x] P4 측정으로 선정한 typed layout/할당 후보 A/B — indexed prefix/measurement 경로와 기본 eager 별도 5회 비교; 추가 pooling/전역 dispatch 미채택
- [x] P5 안전한 frame budget 및 조건부 runtime/Worker 실험 판정 — speculative cache 0, build/layout 중 yield 없음; managed UI 병목이 남아 새 Worker/AOT 미채택
- [ ] P6 기능·성능·메모리·플랫폼 검증과 history 기록 — 이용 가능한 자동 검사/빌드와 실패·미검증 결과 기록, 전체 수용 미완료

### 실행 결과와 남은 조건

체크된 항목은 해당 구현/실험의 실행을 뜻하며 최종 latency·물리 수용 PASS를 뜻하지 않는다. P3은 공용 `SectionExtentIndex`/`SectionList`/`SectionFocusCoordinator` 후보까지 구현했다. 29/290/4096 항목, lazy 생성·End·anchor·연속 Tab/Shift+Tab, 열 이동 State, unpin/dispose, owner가 명시적으로 매핑한 동적 재정렬·추가·삭제 검사가 통과했다. 이 과정에서 공용 sliver의 앞 형제가 같은 이동에서도 index를 갱신하도록 수정했다. 화면 밖 descendant의 screen-reader 탐색, 전체 dependency/IME/overlay 수용은 아직 확인되지 않아 P3 전체 완료/기본 승격으로 표시하지 않는다.

P1/P2만 반영한 eager 경로의 resize 개선은 약 1–2%였다. 큰 개선은 `DOROTI_SAMPLE_SECTION_VIEWPORT=indexed` 비교 후보에 속한다. 마지막 공용 재정렬 수정 이후 `reorder-wwwroot` 독립 5회 corpus의 callback p95 실행별 중앙값은 같은 열 341→159.1ms, 열 전환 1699.8→351.0ms였다. 30% 구조 개선 수치와 16.7/33.3/50ms 목표 달성은 별개이며, 원래 strict gate는 미달이다. 앞선 memory/startup/progress corpus는 마지막 재정렬 수정 전 checkpoint로 구분한다.

P6에서는 Windows/Web/MAUI/Android/Linux 빌드 및 사용 가능한 자동 검사와 Linux Qt xcb 실행을 확인했다. WSLg Wayland는 이전 publish에서도 재현되는 protocol 오류로 FAIL, Android 기기와 Apple 실행 환경 및 물리 표시·IME·접근성·사용자 체감은 `notVerified`다. 전체 live/transient memory, 정확한 신규 section 지연, 실제 창 테두리 drag 등 미검증 항목을 다른 PASS로 대체하지 않는다.

원본 비교 스위치·실패 로그·timeout·이전 AOT 실패를 유지한다. 이후 작업은 P3의 남은 correctness/접근성 계약과 단일 section 내부의 synchronous layout 비용, 전체 메모리/물리 gate를 해소하는 것이며, 현재 문서를 삭제하거나 전체 완료로 재분류하지 않는다. 상세 source/artifact/검증 matrix는 [실행 기록](history/26-09-08/wasm-section-structure.md)에 있다.

### 2026-09-08 사용자 관찰 후 카로셀·스크롤 개선

사용자는 `?dorotiTestbedMode=sample&dorotiSectionViewport=indexed`에서 이전보다 약 2배 빠르게 느껴지지만 카로셀과 스크롤의 버벅임이 남는다고 보고했다. 앞선 결과의 “사용자 체감 notVerified”는 그 실행 당시 기록이며, 이번 관찰은 indexed 후보의 체감 개선으로 별도 보존한다. 물리 FPS·전체 P3/P6 수용으로 확대하지 않는다.

- [x] 카로셀·snapping·세로 스크롤의 실제 touch drag/coast를 분리하는 측정 추가
- [x] 공용 Skia 렌더러의 반복 그림을 native 명령으로 재사용하고 임시 path를 즉시 해제
- [x] 소수점 위치를 보존하고 bounds·font·context 변경 시 무효화, entry/command/byte 상한 적용
- [x] 최종 A/B·픽셀·애니메이션·열 복귀 회귀 확인 및 5088 수정본 제공

상세 비교·원시 증거·남은 긴 프레임은 [카로셀·스크롤 후속 기록](history/26-09-08/carousel-scroll-performance.md)에 분리한다. indexed는 계속 opt-in이고 기존 latency·접근성·물리 gate는 유지한다.

후속 결과: 각 조건 2회 관성 이동 A/B의 프레임 callback 중앙값은 일반 카로셀 11.7→7.6ms(35%), snapping 11.6→6.8ms(41%), 세로 스크롤 10.15→6.45ms(36%) 감소했다. 세로 스크롤 callback p95는 35.45→18.55ms였다. 계측 OFF 비교와 Web 회귀 7개 및 공용 GPU 픽셀/캐시 검사가 통과했다. 일반 카로셀의 실제 commit 간격 중앙값은 여전히 약 16.5–17ms이며 물리 FPS 상승을 주장하지 않는다. 새 섹션 진입 시 최대 158.6/247.8ms의 긴 callback은 남아 있어 worst latency 개선/전체 수용 완료로 표시하지 않는다. 총 성능 실행은 초기 진단 4회 + 관성 A/B 12회 + 계측 OFF 4회 = 20회이며 모두 보존했다.

### 2026-09-08 창 크기·열 재구성 후속 개선

사용자는 카로셀 개선은 체감하지만 창 크기에 따른 레이아웃 재구성에서는 개선을 느끼지 못한다고 보고했다. 렌더러 재사용과 별도로 resize의 재구성·소유권 비용을 측정하고 다음을 구현했다.

- [x] 공용 Scaffold의 MediaQuery 변환을 각 슬롯으로 이동해 폭만 바뀔 때 Scaffold 전체 재빌드를 방지
- [x] indexed 오른쪽 scroll owner를 숨긴 채 유지하고, 실제 필요한 섹션만 이동하도록 공용 SectionList의 일시 정지·재소유 기능 적용
- [x] 섹션 전체에 안정된 소유권 키를 적용해 깊은 스크롤 후 열 복귀 시 State·anchor 보존
- [x] 최종 Web 빌드, 공용 native 계약, 브라우저 기능 12개, 이전 카로셀 빌드 대비 최종 A/B 및 5088 수정본 제공

열 전환 상세 비교 2회의 최대 callback은 386.8→210.7ms, 377.2→192.5ms로 45.5–49.0% 감소했다. 같은 열에서 폭만 바꾸는 비교는 156.5→123ms였으나 마지막 창 크기의 정확한 화면 반영 지연은 개선되지 않았다. 상세 계측을 끈 한 쌍의 비교에서 최종 크기 반영은 열 전환 686.5→149.7ms, 같은 열 51.5→51.6ms였다. 소수의 CDP resize 실행 결과이며 물리 창 drag/FPS나 모든 크기 변경의 부드러움을 보장하지 않는다.

진단·실패·중간 후보와 최종 비교를 포함한 성능 시도는 19회다. 최초 깊은 State 복귀 실패와 잘못된 tristate 기대값으로 실패한 테스트 원본을 보존했고, 수정 후 해당 검사는 통과했다. 100ms 이상의 긴 callback, 전체 메모리·물리 표시/IME/접근성·플랫폼 gate는 여전히 남아 있다. P3/P6 전체 완료나 indexed 기본 승격으로 표시하지 않는다. 상세 표와 증거는 [레이아웃 후속 기록](history/26-09-08/responsive-layout-followup.md)에 있다.

# .NET WASM에 맞춘 Doroti 재빌드·레이아웃 구조 개편 계획

작성: 2026-09-07. 기존 실행: 2026-09-08. 기존 결과: **최종 수용 PARTIAL / indexed 승격 보류**.

2026-09-08 연구 갱신: **현재 작업계획은 [9절](#9-flutter와-같은-작업-대상을-유지하는-net-wasm-실행-구조-연구)이다. 연구·문서 작성 완료, 새 구현·성능 실험 미착수.** 사용자 요청은 “재빌드와 레이아웃 작업 타겟은 Flutter와 동일하게 잡되 .NET WASM 연산 특성에 맞춰 재구성할 수 있는지 연구하고 work.md에 작성”이다. 새 단계에서는 처리 대상·수명·실행 시점을 줄이거나 바꾸는 가상화/소유권 변경을 성능 개선에 포함하지 않는다.

아래 1–8절은 앞서 실행한 계획과 결과의 보존 기록이다. 해당 절의 가상화 확대·작업 대상 축소·추가 Worker 제안은 새 단계의 실행 지시가 아니다. 다음 구현을 진행할 때는 9절의 범위·검증 기준·실행 횟수 제한이 우선하며, 기존 FAIL/PARTIAL과 물리 검증 미완료는 그대로 유지한다.

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

## 9. Flutter와 같은 작업 대상을 유지하는 .NET WASM 실행 구조 연구

### 9.1 결론과 새 범위

**재구성할 가치가 있다. 가장 구체적인 후보는 상속 정보 맵의 전체 복사를 구조 공유로 바꾸는 작업이며, 다음은 intrinsic 캐시의 임시 어댑터/콜백, 반복적인 동적 호출과 작업 목록 처리 비용이다.** 공용 Widget/Element/RenderObject 트리와 Flutter의 갱신 규칙을 유지하면서, 같은 작업을 더 적은 복사·할당·간접 호출로 수행한다. 실제 병목 비중과 개선율은 아직 측정하지 않았으므로 후보 순위는 소스 근거에 따른 조사 순위다.

이번 요청의 산출물은 연구와 실행 가능한 계획이다. 이 절 작성 중에는 앱/Framework/컴파일러 소스, 빌드 설정, 5088 서버를 변경하지 않았고 새 성능 실험도 실행하지 않았다. 이전 결과를 새 구조의 성능 증거로 재사용하지 않는다.

새 단계에서는 다음을 고정한다.

- 동일한 논리 Widget 구성, 데이터·상태·입력 순서, constraints, cache extent, keep-alive 정책, 열 전환과 GlobalKey 이동을 사용한다.
- `setState`, `didUpdateWidget`, `didChangeDependencies`, build, layout 요청/실행, intrinsic/dry/baseline 계산의 대상·횟수와 관찰 가능한 순서를 유지한다. Flutter가 같은 frame에 처리하는 작업을 다음 frame이나 화면 진입 시점으로 미루지 않는다.
- Flutter의 dirty 처리, 같은 Widget/constraints 재사용, relayout boundary, 같은 layout 중 자식 생성·dirty 목록 병합을 그대로 따른다. 내부 map lookup·배열 복사·보조 함수 호출 수는 줄일 수 있다.
- State 수명, Focus/IME, semantics, hit-test, overlay 좌표, 부모의 자식 크기 사용 계약과 사용자 정의 override를 보존한다. 모든 플랫폼에 같은 correctness 규칙을 적용한다.
- 화면 밖 build 동결, 추가 lazy 생성/eviction, section 세분화, 부모 고정 viewport, GlobalKey 이동 제거, cache extent 축소, 프레임 생략·debounce, 전체 트리 평탄화는 이번 후보에서 제외한다. 이전 indexed 구현은 보존하지만 확대하지 않는다.

따라서 “방문한 위젯이 늘어도 비용이 일정해야 한다”를 무조건 요구하지 않는다. Flutter에서도 합법적인 대상 수가 늘면 Doroti도 같은 작업을 해야 한다. **처리 대상 일치 + 동일 작업당 비용 감소 + 사용자 workload 지연 감소**가 새 목표다.

### 9.2 현재 확인한 기준선과 비교 가능성

연구 시점 HEAD는 `a783fe20`이고 시작 시 작업 트리는 깨끗했다. 앞선 카로셀·열 전환 수정은 현재 소스에 포함되어 있다. 다음 실행에서는 HEAD/diff 및 실제 빌드 asset SHA-256을 다시 고정한다.

| 항목 | 이번에 읽어 확인한 내용 | 해석 |
| --- | --- | --- |
| SDK / 설치 runtime | .NET SDK 10.0.400, host 10.0.11, wasm-tools manifest 10.0.111 | 설치값이며 실행 중 브라우저 runtime의 활성 옵션을 대신하지 않음 |
| Release 프로젝트 평가 | `net10.0`, `browser-wasm`, `WasmBuildNative=true`, `RunAOTCompilation` 빈 값 | managed AOT 활성화 설정 없음; native relink와 AOT 구분 |
| 기타 평가값 | `WasmEnableSIMD=true`, `PublishTrimmed=true`, `Optimize=true`; `BlazorWebAssemblyJiterpreter`/`WasmEnableThreads` 빈 값 | 빈 값을 Jiterpreter 비활성화로 단정하지 않음. publish 산출물·debugger/runtime 옵션 확인 필요 |
| Release 기호 | `TRACE;DOROTI_BROWSER;RELEASE` | 앱 평가값. IL 검토 시 관련 Framework assembly의 구성도 각각 확인 |
| 생성기 검증용 Flutter lock | `56b8e1a851a594b1a154f8ea93270807dab22b9a` | reviewed Framework 파일의 `56b8e1a8` 헤더와 일치. 전체 현재 파일의 provenance는 Q0에서 목록화 |
| 설치 Flutter SDK | 3.44.9, framework `6b182d2c7585eba26d4edce0f97630effd256c33`, Dart 3.12.2 | 포팅 lock과 다른 revision. 최신 SDK 실행을 포팅 원본과 같은 것으로 표시하지 않음 |
| 참조 샘플 | `reference/flutter_sample_app/lib/src/component_screen.dart`의 SliverList/BuildSlivers/높이 캐시 | 현재 Doroti indexed와 동일한 생성·보존 정책이라고 가정할 수 없음 |
| 이전 resize 성능 | 초기 화면 비교에서 열 전환 개선; 전체 방문 후 동일 위치의 성능 corpus 없음 | 깊은 열 복귀 기능 PASS를 전체 방문 후 성능 PASS로 바꾸지 않음 |

평가 명령은 다음과 같다. 빌드나 publish를 실행하는 명령이 아니다.

```powershell
dotnet msbuild DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -p:Configuration=Release -getProperty:TargetFramework,RuntimeIdentifier,WasmBuildNative,RunAOTCompilation,BlazorWebAssemblyJiterpreter,WasmEnableSIMD,WasmEnableThreads,PublishTrimmed,Optimize,DefineConstants
```

Flutter 대응은 두 층으로 검증한다.

1. **동일 작업 fixture:** 공통 위젯·의존성·상태·스크롤 정책을 명시한 대응 fixture를 Dart/C#에 만들고, 포팅 lock revision의 Flutter와 비교한다. 현재 설치 SDK와 lock 사이에서 관련 함수가 달라졌으면 별도 버전 차이로 기록한다. 이것이 Flutter와 대상이 같은지 판단하는 기준이다.
2. **실제 sample 회귀:** 현재 eager와 indexed 각각의 before/after에서 구성·State·방문 이력·대상 집합이 그대로인지 검사한다. stock Flutter sample과 구조가 다른 indexed 결과는 workload 동등성을 증명하기 전까지 `notComparable`이며 “Flutter와 같은 작업량”이라고 보고하지 않는다.

대상 차이가 발견되면 먼저 **호환성 수정**으로 격리하고 기준선을 새로 잡는다. 대상을 줄여 얻은 개선과 같은 대상을 더 빨리 처리한 개선을 한 수치로 합치지 않는다.

### 9.3 .NET WASM 연산 특성과 설계상 의미

| 공식 자료에서 확인한 특성 | Doroti에 적용할 연구 가설 |
| --- | --- |
| 비-AOT WebAssembly는 Mono IL interpreter와 Jiterpreter를 사용한다. AOT는 publish 시 활성화하며 다운로드 크기와 실행 성능의 tradeoff가 있다. [R1] | CoreCLR의 JIT 인라이닝·탈추상화 결과를 WASM 성능으로 대신하지 않는다. 현재 비-AOT 경로에서 먼저 비교한다. |
| Jiterpreter는 interpreter opcode의 연속 구간을 Wasm으로 컴파일한다. 짧은 trace는 거부될 수 있고 일부 opcode·호출에서는 interpreter로 돌아간다. [R2] | 호출 사이에 잘게 끊어진 어댑터를 작은 typed 경로로 연결할 가치가 있다. 모든 함수 합치기나 큰 메서드화는 코드 크기·trace 제한 때문에 역효과일 수 있다. |
| 같은 설계 문서는 메모리 접근·간접 함수 호출 비용, 지역 값 재사용과 작은 코드의 중요성을 설명한다. 실제 이득에는 계측이 필요하다. [R2] | 반복되는 런타임 타입 변환, 참조 추적, map 복사, 값 boxing 후보를 실제 호출 수와 IL로 확인한다. `AggressiveInlining` 표시만으로 개선을 주장하지 않는다. |
| C# dynamic은 런타임 바인딩을 사용한다. [R3] | receiver/member 타입이 확정된 생성 코드에서 DLR 경로를 제거할 후보가 있다. Dart의 진짜 dynamic 및 사용자 override는 유지한다. |
| Flutter도 갱신 대상 선정뿐 아니라 자료구조·상수 비용을 최적화하며, 원본 PersistentHashMap은 HAMT를 사용한다. [R4][R5] | Flutter의 대상 선정 규칙을 유지하면서 .NET용 자료구조를 교체하는 것은 이번 목표에 부합한다. Dart VM에 맞춘 cast/배열 표현을 C#에 기계적으로 복사하지 않는다. |

R2는 `v10.0.0` 설계 문서이며 설치 runtime은 10.0.11이다. 문서의 publish/debugger 및 threads 관련 제한을 현재 실행의 확정 상태로 사용하지 않는다. Q0에서 실제 runtime 버전·배포 모드·옵션을 확인하고, 지원될 때만 Jiterpreter 통계(생성 trace/거부/탈출/생성 bytes)를 진단 빌드에서 수집한다. private 진단 API가 없으면 `notMeasured`로 두며, 진단을 위해 앱의 기본 runtime 옵션을 변경하지 않는다.

### 9.4 소스에 근거한 후보

#### C1. 상속 정보 맵을 경로 복사와 구조 공유로 변경 — 가장 먼저 비용 확인

현재 [PersistentHashMap](Doroti/src/Doroti.Framework.Foundation/persistent_hash_map.cs)의 `put`은 매번 `new Dictionary<TKey,TValue>(_values)`로 전체 항목을 복사한다. `remove`도 존재하는 키를 지울 때 전체 복사한다. 아래 `_CompressedNode`/`_FullNode`/`_HashCollisionNode` 선언은 실제 구현이 없는 타입이다. 이름이나 주석과 달리 현재 경로에는 HAMT 구조 공유가 없다.

[InheritedElement._updateInheritance](Doroti/src/Doroti.Framework.Widgets/framework.cs)는 상속 위젯의 변경·활성화 시 이 `put`을 사용한다. 일반 Element는 부모 맵을 공유하므로 **모든 Element가 맵 전체를 복사하는 것은 아니다.** 전환 시 활성화되는 inherited node 수, 맵당 항목 수, 복사 bytes를 측정해야 영향 범위를 알 수 있다.

재구성 후보는 `PersistentHashMap<TKey,TValue>`의 공개 계약을 유지한 .NET용 bitmap HAMT다. 수정되는 경로만 새 노드/배열로 만들고 나머지를 공유한다. 키 조회·알림 대상·알림 시점·State 재연결은 그대로 둔다. 초기 선택은 generic typed 저장소와 작은 노드이며, 임의 unsafe 메모리나 전역 Type ID 테이블을 먼저 도입하지 않는다. 작은 맵에서는 Dictionary 복사가 더 저렴할 수도 있으므로 실제 4/16/64개 키 분포 및 충돌 상황으로 비교한다.

필수 계약은 기존 snapshot 불변성, 같은 타입의 가장 가까운 ancestor 선택, 동일 해시의 서로 다른 키, 교체/삭제/없는 키/Count/열거/nullable 값, 원래 Key equality, reparent와 dispose 이후 참조 해제다. 값의 참조 동일성과 값 동등성을 혼동해 `put`을 생략하지 않는다. Doroti의 `IReadOnlyDictionary`/`remove` 확장은 Flutter 원본에 없는 부분까지 현재 호출 계약을 확인한다. 순서 의존 호출부가 있으면 열거 순서도 보존하거나 그 후보를 보류한다.

예상 효과는 맵당 전체 복사량 감소이며, 현재 resize의 주원인이라는 주장은 하지 않는다. 모든 map lookup은 계속 수행하므로 깊은 trie의 조회 비용/배열·노드 수/GC 부담까지 함께 비교한다.

#### C2. intrinsic/dry/baseline 캐시의 어댑터·콜백 비용 제거

[Rendering/box.cs](Doroti/src/Doroti.Framework.Rendering/box.cs)의 `getMin/MaxIntrinsicWidth/Height`는 호출마다 `_IntrinsicDimension__boxInterfaceAdapter`를 생성한다. 캐시 경로는 인터페이스 `memoize`와 `putIfAbsent(..., () => computer(input))`를 거친다. source-level 생성은 확인했지만 실제 IL·runtime에서 남는 할당량은 미측정이다.

1차 후보는 불변 intrinsic 종류별 어댑터 재사용과 캐시 hit에서의 콜백 생성 제거다. 2차 후보는 intrinsic/dry/baseline별 typed lookup→miss 계산→저장 경로다. 실제 `compute*` override 호출과 캐시 키·hit/miss·무효화 시점을 유지하고, 공유 어댑터에 RenderBox나 요청 상태를 저장하지 않는다.

`putIfAbsent`를 수동화할 때 재진입 계산, 예외 발생 시 미저장, null baseline을 “캐시 없음”으로 오인하지 않는 계약을 보존한다. constraints 값 동등성, double의 NaN/무한대/±0 처리, dirty이지만 같은 constraints인 경우, dry layout과 실제 layout의 분리를 검사한다. 새 폭에 과거 geometry를 재사용하거나 intrinsic 계산 자체를 줄여 얻은 결과는 이번 후보의 성공으로 세지 않는다.

#### C3. 확정된 타입의 runtime bridge를 typed C#으로 생성

[Widgets/sliver.cs](Doroti/src/Doroti.Framework.Widgets/sliver.cs)의 renderObject/parentData 접근에는 `dynamic`이 남아 있다. [DartRuntimePrimitives.ConvertValue](Doroti/src/Doroti.Runtime/DartRuntimePrimitives.cs)는 object 입력의 타입 검사와 일반 변환 fallback을 제공한다. [DartCoreAdapters.sort](Doroti/src/Doroti.Runtime/DartCoreAdapters.cs)는 Dart long 비교자를 .NET 비교자로 감싸는 경로를 갖는다.

WASM profile의 상위 호출 지점만 선정해 typed receiver, 명시적 cast, 재사용 가능한 정적 비교자로 낮춘다. **virtual 호출을 보존한 typed 호출도 DLR 제거와 구분되는 유효한 후보**다. 전역 virtual 제거·공개 클래스 sealing·subclass override 우회는 금지한다. 내부 getter를 지역 변수로 저장하는 경우에도 getter의 부작용/재진입과 수정 가능성을 먼저 증명한다. Dart int를 전역 int32로 바꾸지 않으며, 내부 collection index만 범위 증명 후 변환한다.

유효한 포팅 최적화는 `tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/`의 Dispatch/Invocations/Expressions 및 semantic index까지 반영해 재생성 후 유지한다. reviewed hand-maintained 파일은 해당 소유 방식을 따른다. 기존 `validation/virtual-dispatch`의 실제 analyzer/생성 C#/override fixture, covariant 검사·generic·mixin·named argument bridge를 재사용한다. 성능을 위해 `ConvertValue`의 null/enum/numeric 변환 의미를 바꾸지 않는다.

이전 동적 호출 정적화 실험은 효과가 입증되지 않아 되돌렸고, 직전 gesture-handler/typed-call 후보도 채택되지 않았다. “dynamic이 보인다”만으로 다시 넓게 치환하지 않는다. 이전과 다른 hot path 증거가 있을 때만 후보를 연다.

#### C4. 동일 작업 목록의 정렬·임시 데이터 처리 개선

[BuildScope](Doroti/src/Doroti.Framework.Widgets/framework.cs)는 dirty List와 `_inDirtyList` 중복 방지를 이미 사용한다. [PipelineOwner.flushLayout](Doroti/src/Doroti.Framework.Rendering/object.cs)도 두 List를 재사용한다. 따라서 중복 방지나 버퍼 재사용을 새 발견처럼 추가하지 않는다.

남은 후보는 정렬 comparer adapter, `_shouldMergeDirtyNodes` 시 `Skip(i).ToList()` 임시 목록, sliver 재조정의 keys snapshot·임시 map·반복 조회다. 같은 순서의 tail copy, typed 비교자, 필요한 용량 확보부터 검토한다. 순회 중 구조 변경을 허용하던 snapshot을 live enumeration으로 바꾸지 않는다.

나중에 작업 항목을 연속 배열로 정리하더라도 Element/RenderObject 객체 및 callback 대상은 유지한다. 깊이·owner를 캐시할 때 GlobalKey 이동과 depth 재계산을 반영해야 한다. BuildScope의 중간 재정렬/뒤로 이동, PipelineOwner의 새 dirty 합류/child owner flush, LayoutBuilder의 별도 build scope를 그대로 처리한다. 동일 깊이 노드의 순서를 바꿔 observable 결과가 달라지는 정렬/heap 교체는 채택하지 않는다. 처음부터 전체 트리를 SoA/ECS로 바꾸는 계획은 아니다.

#### 우선 제외하거나 별도 실험으로 남길 항목

- `DartRuntimePrimitives.Assert`에는 `[Conditional("DEBUG")]`가 있다. Release 직접 호출은 인자 평가를 포함해 생략되는 C# 규칙이므로, 소스의 assert/lambda 개수로 Release 비용을 계산하지 않는다. IL 확인은 허용하되 이미 제거된 코드를 지우는 작업은 후보가 아니다. [R6]
- `kReleaseMode`는 RuntimePorts getter로 매핑된다. 이를 상수로 바꾸거나 cold error 경로를 분리하는 후보는 IL에서 비용이 남고 profile에서 의미가 있을 때만 연다. 예외 처리 자체를 제거하지 않는다.
- 전면 pooling은 retained State/scene 참조 수명과 GC를 악화시킬 수 있다. 불변 맵 노드를 임의 pool로 되돌리지 않으며 frame-local scratch도 재진입·반납·최대 용량·참조 clear를 증명한 경우만 사용한다.
- SIMD는 연속 수치 계산의 비중이 확인될 때만 후보이며, 현재 SIMD 빌드 속성은 이미 true다. 트리 순회·virtual callback을 SIMD로 바꿀 수 있다고 가정하지 않는다. [R7]
- full/partial managed AOT는 기존 compiler stack overflow와 browser stack 오류를 그대로 보존한 별도 후속 비교다. 구조 후보의 선행 조건으로 두지 않는다. Worker/threads 병렬화도 이번 단계의 실행 순서 보존 범위 밖이며 기본값을 바꾸지 않는다.

### 9.5 동일 대상 검증과 프로파일 설계

**계약 trace와 성능 trace를 분리한다.** 같은 최종 PNG 또는 aggregate build count만으로 같은 작업을 했다고 판정하지 않는다.

계약 fixture에는 트리 이동 후에도 유지되는 논리 node ID를 명시한다. 런타임별 객체 주소나 이동 후 바뀌는 전체 경로를 공통 ID로 사용하지 않는다. runtime 고유 wrapper는 대응표로 따로 보존하고 임의 제외하지 않는다. 제한된 수의 scripted input과 고정 animation timestamp를 주입해 같은 논리 tick을 비교한다. 실제 브라우저 FPS 차이로 frame 수가 달라지는 실행에서는 frame 번호의 일대일 비교를 강요하지 않는다.

| 기록 | 비교할 계약 |
| --- | --- |
| dirty 원인/등록/중복/정렬, buildScope ID | 같은 입력에 같은 Element가 등록되고 같은 scope에서 처리됨 |
| `updateChild`/build/`didUpdateWidget`/dependency callback | logical ID별 대상 multiset·호출 횟수·필수 선후 관계 일치 |
| layout 진입 vs `performLayout`/`performResize` | 호출됐지만 fast return한 경우와 실제 계산을 분리; constraints/parentUsesSize/owner/dirty 원인 일치 |
| intrinsic/dry/baseline 요청·cache hit/miss·실제 compute | 재구성 전후 동일 계산 대상과 재진입/무효화 의미 유지 |
| GlobalKey activate/deactivate/dispose, dependency 등록/해제 | State ID와 inherited 값·관찰 가능한 lifecycle 순서 일치 |
| 출력 geometry/scroll extent/anchor/semantics/hit-test | 같은 결과와 입력 수신 대상; text/font/raster 차이는 별도 원인으로 분리 |

동일 구현 A/B는 결정적 fixture에서 순서까지 비교한다. Flutter와의 비교에서는 원본이 보장하는 순서를 강제하고, 보장되지 않는 같은 깊이 tie 등은 사전에 정의한 부분 순서로 검사한다. 차이가 나오면 최초 불일치 이벤트를 보존한다. 대상/호출 횟수 차이를 comparator가 느슨하게 무시하도록 수정하지 않는다.

현재 `FrameworkWorkCounters`와 `FrameworkWorkProfile`은 aggregate 중심이며 node별 대상 동등성을 입증하지 못한다. Q1에서 opt-in bounded trace를 추가할 계획이다. hot path에서 문자열/JSON을 생성하거나 매 node마다 JS interop를 호출하지 않는다. 숫자 ID/배열에 기록하고 입력 종료 후 내보낸다. 버퍼 overflow·drop은 명시해 해당 계약 검사를 무효 처리한다.

비용 분해는 callback의 build/layout/paint/semantics/encode/queue 및 다음 후보별 self time·호출 수·할당량이다. 전체 트리의 모든 함수에 타이머를 넣으면 측정값이 바뀌므로, aggregate로 좁힌 후보만 진단한다. inclusive 시간을 합산해 총 시간으로 만들지 않는다. Web profile과 emitted Release IL에서 DLR call site, boxing, adapter/delegate 생성을 대조한다. native CLR microbenchmark는 correctness·기초 할당 참고용이며 최종 채택은 browser-wasm A/B로 판정한다.

Jiterpreter 통계와 GC pause를 수집할 수 없으면 각각 `notMeasured`로 둔다. 전체 방문은 보존 객체 수뿐 아니라 runtime warm-up도 바꾸므로, “방문 후 느림=GC” 또는 “warm-up 완료”로 단정하지 않는다. managed allocated bytes/action, collection count, managed heap 추정치, WASM capacity, process memory는 별도 열로 기록한다.

### 9.6 재현 workload와 실험 예산

핵심 workload는 **전체 섹션을 실제 materialize한 뒤 처음과 동일한 화면·상태·스크롤 anchor로 돌아와 resize**하는 것이다. End 한 번으로 건너뛰거나 스크롤 횟수만 채운 것을 전체 방문으로 세지 않는다. fixture 계측에서 방문 ID가 정확히 29개이고 양 열의 대상 범위가 모두 포함됐는지 확인한다. 부분 방문은 방문 ID 수와 목록을 저장한다.

- 주 비교: 1280×900, DPR 1, 동일 anchor에서 `1240,1200,1160,1120,1160,1200,1240,1280` 폭 변경과 `980,900,800,1100,1400,900,1100,1280` 열 전환을 분리한다. CDP 간격 45ms와 실제 적용된 입력/metrics generation을 모두 기록한다.
- 부분 방문/전체 방문, 해당 화면 최초 계산/이미 계산된 화면을 구분하고 candidate 전후 동일하게 준비한다. trace 비교는 고정 tick, latency 비교는 실제 연속 입력을 사용한다.
- 회귀 계약: 기존 DPR 1/1.25/1.5/2 및 390/800/999/1000/1001/1280/1500/1501 geometry, dirty-same-constraints, nested LayoutBuilder, reparent 중 새 dirty, text scale/RTL/font/theme/이미지 크기 변경, dynamic reorder/remove, selection/focus/IME/overlay.
- 개선과 무관한 workload는 progress·carousel·스크롤 및 cold first-content 중 비용 분석에 맞는 최소 대조군을 사전에 고른다. 합성 29/290/4096 항목 계약은 실제 UI 성능과 분리한다.

각 test/benchmark의 외부 timeout은 **20분**, 자동 retry 0, GPU/browser workload와 빌드는 순차 실행한다. 새 실험은 기본 **10회 이내**에서 후보를 판단하고, 이유를 기록한 경우만 총 **20회 이내**로 확장한다. 이 제한은 앞선 5절의 독립 5회 이상 요구보다 새 단계에서 우선한다.

권장 배분은 1차 진단 4회(Flutter/Doroti × 부분/전체 방문), 상위 후보 하나의 주 workload paired A/B 6회(3쌍)로 총 10회다. 필요성이 확인되면 최소 계측으로 같은 열/열 전환 A/B 4회, 사전 선정 대조 workload 두 개의 A/B 4회까지 총 18회로 확장하고 남은 2회는 실패한 setup/재시도 예산으로 남긴다. 진단 빌드 추가 실행·warm-up 전용 실행·timeout도 이 예산에 포함해 다른 배분을 줄인다. 한 회에 수십/수백 번의 resize를 숨겨 넣지 않는다. 실행별 준비 과정과 실제 입력 수를 기록한다.

소수 실행에서는 각 raw 값과 범위를 보고하고 안정적인 p95나 통계적 확신을 주장하지 않는다. 기존 probe는 짧은 sequence에서 callback p95=max가 될 수 있다. UI 재구성 개선은 같은 대상에서의 callback 비용과 실제 최종 generation의 exact commit을 함께 본다. 오래된 progressive commit 개수나 CSS 확대를 정답 화면으로 세지 않는다. 실제 창 테두리 drag/scan-out/사용자 체감은 별도 검증이다.

### 9.7 실행 단계와 채택 조건

| 단계 | 실행 내용 / 주요 위치 | 종료 조건 | 현재 상태 |
| --- | --- | --- | --- |
| Q0 기준선·출처 고정 | HEAD/dirty/asset, evaluated props/runtime options, Flutter lock과 설치 SDK 차이, fixture 대응표 | 실행 모드와 비교 가능성 명시. 기존 sample 구조 차이는 notComparable로 분리 | 소스·설정 조사 완료; 실행 기준선 미착수 |
| Q1 동일 대상과 비용 분해 | Ui counters/profile, Widgets/Rendering opt-in trace, Dart 대응 fixture, 전체 방문 probe | 대상/횟수/순서/constraints 차이 목록과 candidate self time·allocation 순위 | 미착수 |
| Q2 후보 하나 구현 | 우선 C1 비용 확인; 실제 비중에 따라 C2/C3/C4 중 상위 하나 선택 | 기존 경로와 isolated A/B, 원본 실패 보존, local API/override/lifetime 계약 통과 | 미착수 |
| Q3 WASM 비교·회귀 | 3쌍 주 workload, 필요 시 최소 계측 및 대조군 | 동일 대상 유지와 지연·할당·메모리 tradeoff 보고. 개선 없는 후보 원복 | 미착수 |
| Q4 재생성·통합 기록 | generator lowering/virtual-dispatch 또는 reviewed source, trimmed publish, native/Web 계약 | 재생성 내구성, 플랫폼별 결과, 채택/미채택/잔여 gate 기록 | 미착수 |

후보별로 hot path 비중을 먼저 확인한다. 전체 시간의 5%인 경로를 두 배 빠르게 해도 전체 처리 시간 감소는 약 2.5%에 불과하다. 소스가 복잡해 보인다는 이유로 낮은 비중 후보를 전면 재작성하지 않는다. C1도 비용 비중이 작거나 맵 조회가 악화되면 우선순위를 내린다.

채택의 필수 조건은 **동일 대상/호출 계약 차이 0, State·geometry·입력·semantics 회귀 0, 처리 시점 지연 0**이다. 주 workload의 전체 callback에서 반복 가능한 개선을 확인하고, 할당·live/transient memory·cold 진입·비대상 workload의 회귀를 함께 판정한다. 후보 내부 시간이나 native 할당만 줄었으면 `componentOnly`로 기록하고 전체 resize 개선이라고 쓰지 않는다.

30% 전체 구조 개선 및 기존 16.7/33.3/50/100ms 계열 목표는 이전 gate로 남긴다. 새 후보의 작은 개선을 기존 gate PASS로 재분류하지 않는다. 동일 대상을 지키면서 얻은 작은 개선도 수치·복잡도·회귀 여부를 근거로 별도 후보 채택 여부를 기록할 수 있으나, 전역 최종 수용과 구분한다. 비대상 p95 10% 초과 악화, 유효 계약 trace 누락, target 불일치 또는 3쌍에서 개선 미확인 시 승격하지 않는다.

이번 방향은 공용 Foundation/Runtime/Widgets/Rendering 및 필요한 생성기 내부에 적용한다. 샘플만 위젯을 덜 만들도록 고치거나 `isWeb`에서 lifecycle을 생략해 통과시키지 않는다. available native/Windows/Web 검증과 다른 host의 build 결과를 기록하되 물리 기기·IME·접근성·전체 메모리 미확인은 `notVerified`로 유지한다.

### 9.8 공식 근거와 읽은 로컬 경로

아래 공식 자료는 2026-09-08 확인했다. Flutter 일반 API 페이지는 현재 문서이고, 버전 고정 계약은 아래 lock 소스를 기준으로 다시 대조한다. .NET v10.0.0 문서와 설치 10.0.11 사이의 차이는 Q0에서 검증한다.

- R1: Microsoft [.NET WebAssembly AOT와 interpreter](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0).
- R2: dotnet/runtime v10.0.0 [Jiterpreter 설계](https://github.com/dotnet/runtime/blob/v10.0.0/docs/design/mono/jiterpreter.md), [WASM 기능/빌드 설정](https://github.com/dotnet/runtime/blob/v10.0.0/src/mono/wasm/features.md).
- R3: Microsoft [C# dynamic과 런타임 바인딩](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic).
- R4: Flutter [Inside Flutter](https://docs.flutter.dev/resources/inside-flutter), [BuildScope](https://api.flutter.dev/flutter/widgets/BuildScope-class.html), [RenderObject.layout](https://api.flutter.dev/flutter/rendering/RenderObject/layout.html), [PipelineOwner.flushLayout](https://api.flutter.dev/flutter/rendering/PipelineOwner/flushLayout.html).
- R5: Flutter 고정 revision [PersistentHashMap 원본](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter/lib/src/foundation/persistent_hash_map.dart), [framework.dart 원본](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter/lib/src/widgets/framework.dart), [InheritedElement.notifyClients](https://api.flutter.dev/flutter/widgets/InheritedElement/notifyClients.html).
- R6: Microsoft [ConditionalAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.conditionalattribute?view=net-10.0).
- R7: Microsoft [WebAssembly runtime 성능 지침](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/webassembly-runtime-performance?view=aspnetcore-10.0).

로컬 근거: `DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj`, `DorotiTestbedApp/src/MaterialSample/Components.cs`, `Doroti/src/Doroti.Framework.Foundation/persistent_hash_map.cs`, `Doroti/src/Doroti.Framework.Widgets/framework.cs`와 `sliver.cs`, `Doroti/src/Doroti.Framework.Rendering/object.cs`와 `box.cs`, `Doroti/src/Doroti.Runtime/DartRuntimePrimitives.cs`와 `DartCoreAdapters.cs`, `Doroti/src/Doroti.Ui/FrameworkWorkCounters.cs`와 `FrameworkWorkProfile.cs`, `tools/Doroti.DartToCSharp/validation/virtual-dispatch/fixtures/flutter-baseline.json`, `reference/flutter_sample_app/lib/src/component_screen.dart`.

### 9.9 새 단계 체크리스트

- [x] 현재 소스/SDK/평가 속성/Flutter revision 차이 및 공식 자료 연구
- [x] 대상·수명·실행 시점을 고정한 .NET WASM 내부 구조 후보와 계약·예산 작성
- [ ] Q0 실행 artifact/runtime 모드 및 Flutter 대응 fixture 확정
- [ ] Q1 부분/전체 방문 동일 대상 trace와 비용 분해
- [ ] Q2 상위 후보 하나의 공용 구현 및 correctness 검사
- [ ] Q3 동일 작업 WASM A/B와 계측 최소화·회귀·메모리 확인
- [ ] Q4 생성기/소스 유지 방식, 통합 검증과 채택 결과 기록

현재 연구 결론은 **실행할 가치가 있는 구체적 후보를 확인했다**는 것이다. 추가 성능 개선, Flutter와의 실제 대상 일치, 모든 플랫폼 수용은 아직 확인하지 않았다.

# Doroti Web 일반 화면의 처리 비용 개선 계획

- 작성일: 2026-09-22
- 검토 기준: `18320304` 작업 트리의 현재 소스
- 상태: **PARTIAL — 계측·일반 화면 실측·후보 A/B 및 회귀 수행, 전체 성능 목표 미달**
- 실행 결과: [2026-09-22 실측 보고서](Doroti/validation/web-frame-cost/results-2026-09-22.md), [재현 방법](Doroti/validation/web-frame-cost/README.md)
- 추가 계획: [8. iPhone 탭 종료 대응과 Web 메모리 관리](#8-iphone-탭-종료-대응과-web-메모리-관리) — 구현·로컬 검증 결과 반영. 실기기 종료 해결·WebGPU 지연 기준은 PARTIAL.

## 1. 목표와 범위

사용자 증상은 일반 화면의 스크롤·애니메이션에서 **앱 자체가 무겁게 느껴지는 현상**이다. 특정 WebView 패널 이동이나 resize만을 최적화하는 작업으로 대체하지 않는다.

목표는 같은 화면·입력·기능을 유지하면서 프레임당 managed CPU 작업, 객체 할당, 반복적인 scene 변환·재생 비용을 줄이고, 입력 대기와 프레임 간격이 함께 개선되는지 확인하는 것이다. 현재 병목은 실행 측정으로 확정하지 않았다. 아래의 구조적 후보를 곧바로 원인으로 단정하지 않는다.

- 최초 작성 요청은 계획 문서만을 대상으로 했다. 이후 사용자의 `work.md의 전체작업해줘` 요청에 따라 2026-09-22 구현·publish·측정을 수행했다. 아래의 미충족 항목을 완료로 바꾸지는 않는다.
- 구현 시 현재의 main runtime 1개 + shared-runtime render Worker + 직접 GPU 출력 구조를 기준선으로 유지한다.
- 공용 Framework의 dirty 대상·처리 순서, layout fast path, inherited dependency, GlobalKey/State 생명주기, 입력·focus·IME·semantics를 보존한다.
- 화면 밖 상태 제거, 애니메이션 정지, 입력 샘플 생략, DPR 저하, 효과 제거로 부하를 줄이는 것은 이 계획의 개선 방법이 아니다.
- startup 다운로드·첫 부팅, WebView/Texture 전용 전송, resize 특화, 새 런타임 전환은 기본 범위 밖이다. 일반 화면 경로를 수정했을 때 필요한 회귀 검증만 포함한다.

## 2. 현재 구조 검토

### 2.1 실행 경로

```text
main: DOM 입력 / focus / IME / semantics DOM
  → MessagePort
shared-runtime JSWebWorker: 입력 처리 / scheduler / Worker rAF
  → widget build → layout → paint → scene build / semantics
  → managed Scene·Picture 명령
  → SkiaSceneRenderer: 명령 재생 / SKPicture·GPU raster cache
  → Graphite/Dawn WebGPU 또는 Ganesh/WebGL2
  → 이전에 main에서 전달한 OffscreenCanvas에 출력
```

Framework·layout·Skia 제출은 같은 render owner를 사용한다. Worker는 main DOM의 정체를 줄이지만, Worker 내부의 긴 build/layout/paint가 후속 입력과 프레임을 지연시키는 문제까지 해결하지는 않는다. 현재 `requestPresent`는 latest 슬롯을 사용하고 WebGPU 제출도 최대 2개 진행 중일 때 대기한다. 무제한 렌더 큐를 전제로 재설계할 근거는 없다.

근거: [runtime 연결](Doroti/src/Doroti.Host.Web/Web/doroti.web.managed-worker.ts), [Worker 프레임·입력·render](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts), [owner frame 요청](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs), [WebGPU capacity](Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts).

### 2.2 이미 적용된 최적화

| 영역 | 현재 구현 | 이번 작업의 의미 |
| --- | --- | --- |
| 런타임 | `net10.0`, `WasmEnableThreads=true`, 공유 runtime의 `JSWebWorker` | Worker 신규 도입이나 단순 threads 활성화를 개선안으로 제시하지 않는다. |
| 상속 데이터 | immutable bitmap HAMT의 `PersistentHashMap` | 전체 Dictionary 복사를 없애는 작업은 이미 반영되어 있다. |
| Material Components | `SectionList` + `SectionExtentIndex`, 보이는 구간 측정, 방문 State 유지, 로컬 갱신 범위 | eager 목록을 lazy 목록으로 바꾸는 작업을 다시 수행하지 않는다. |
| 화면 보관 | 방문한 오른쪽 column의 owner 유지, 숨겨진 column의 기존 `TickerMode`/focus 처리 | 전체 방문 후의 비용과 상태 복원까지 측정한다. |
| 그림 명령 | 명령 identity 기반 `SKPicture` 캐시, 두 번째 사용 시 기록 | 매 프레임 모든 그림이 C#에서 다시 재생된다고 단정하지 않는다. |
| 그림 픽셀 | GPU raster cache, warmup, promotion 개수·시간·픽셀 제한 | 캐시 추가보다 miss 원인과 실제 사용률을 먼저 확인한다. |
| semantics | 노드 JSON 캐시, geometry 전용 compact 형식 | 캐시 이후에도 남는 전체 순회·정렬·payload 조립 비용을 구분한다. |

근거: [HAMT](Doroti/src/Doroti.Framework.Foundation/persistent_hash_map.cs), [Components](DorotiTestbedApp/src/MaterialSample/Components.cs), [SectionList](Doroti/src/Doroti.Framework.Widgets/section_list.cs), [extent index](Doroti/src/Doroti.Framework.Rendering/section_extent_index.cs), [명령 캐시](Doroti/src/Doroti.Skia.Rendering/SkiaPictureCommandCache.cs), [raster cache](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs), [semantics bridge](Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs).

### 2.3 우선 조사할 구조적 후보

| 후보 | 소스에서 확인한 사실 | 아직 확인하지 않은 부분 |
| --- | --- | --- |
| H1 명령 표현의 할당 | `PathCommand`는 문자열 operation·인자 목록·별도 `HostPayload`를 갖는다. `drawRect` 등의 인자 배열과 typed payload에는 중복 정보가 있다. `SceneBuilder.AddPicture`도 일반 payload와 host payload를 함께 만든다. | 일반 스크롤에서 실제로 새 명령이 얼마나 생기는지, 할당·CPU 비중이 큰지 |
| H2 scene snapshot 복사 | `PictureRecorder.endRecording`과 `SceneBuilder.build`는 배열을 만든다. `SceneBuilder.pop`은 각 scope 구간을 별도 배열로 복사한다. 중첩 scope의 새 snapshot은 내부 명령 참조를 반복 복사할 수 있다. `addRetained`는 이미 snapshot을 참조한다. | 새 scene의 scope 깊이·복사량, 기존 retained 재사용률, 일시적 peak memory |
| H3 재생·캐시 부적격 | 캐시를 타지 않는 그림은 C#에서 명령을 순회하고 `ToPaint` 등으로 Skia 자원을 만든다. blur, shadow, 무한 경계 등은 정확성 때문에 명령 캐시를 제한한다. raster cache는 소수점 위치·변환·font surface 조건을 검사한다. | 대상 화면의 miss/bypass 사유, 명령 변환·텍스트·promotion의 self time |
| H4 Framework 반복 작업 | build/dirty sort/layout/inherited notification은 여전히 owner에서 실행된다. 많은 위젯의 비용 합계가 크더라도 부모의 inclusive time만으로 원인을 고를 수 없다. | 과도한 재빌드인지, 필요한 작업 자체의 실행 비용인지, 반복 layout/paragraph 비용인지 |
| H5 semantics·interop | `UpdateSemantics`는 retained 노드를 정렬하고 전체 전송 payload를 조립한다. wheel은 매 샘플을 전달한다. raster 진단 호출도 C#→JS 경계를 지난다. | 해당 경로의 호출 빈도·바이트·CPU 비중, 진단 OFF 상태의 잔여 비용 |
| H6 managed 실행 모드 | Web 프로젝트에는 native build와 threads 설정, AOT 실험 hook이 있지만 `RunAOTCompilation`의 현재 실행 값은 이 검토에서 평가하지 않았다. | 실제 실행 산출물의 interpreter/AOT/Jiterpreter 상태와 각 비용의 기여도 |

근거: [명령·scene·Canvas 표현](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), [PictureRecorder](Doroti/src/Doroti.Ui/PaintingTypes.cs), [그리기 재생·자원 변환](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs), [Web 프로젝트](DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj).

문자열 switch를 enum으로 바꾸거나 캐시 용량만 늘리는 변경 자체를 성능 개선으로 인정하지 않는다. 실제 할당·반복 호출·전체 프레임 비용과 연결해야 한다.

### 2.4 기존 계측의 활용과 부족한 부분

- [FrameworkWorkCounters](Doroti/src/Doroti.Ui/FrameworkWorkCounters.cs): build/layout/paint/semantics 작업량. `DOROTI_STAGE_TRACE`로 활성화된다.
- [FrameworkWorkProfile](Doroti/src/Doroti.Ui/FrameworkWorkProfile.cs): 타입별 inclusive/self time, 선택적 할당량, managed heap·GC 횟수. 중첩 시간의 합을 전체 시간으로 사용하지 않는다.
- [FrameworkWorkTrace](Doroti/src/Doroti.Ui/FrameworkWorkTrace.cs): 순서·logical identity·생명주기 계약. 상세 trace를 켠 실행은 성능 판정과 분리한다.
- [SkiaWorkDiagnostics](Doroti/src/Doroti.Skia.Rendering/SkiaRendererContracts.cs): command cache, raster promotion, paragraph 등 기존 집계.
- Web 초기화는 `dorotiResizeDiagnostics=1`에 stage trace를 묶고 `dorotiLayoutProfile`을 전달한다. `DOROTI_ALLOCATION_PROFILE`은 현재 이 초기화 경로에 전달되지 않는다. 설정을 썼다는 이유만으로 할당 계측이 켜졌다고 보고하지 않는다.
- `FrameworkWorkProfile.Snapshot`의 process 전체 누적 할당과 owner thread의 작업별 할당은 다른 값이다. export 시점의 JSON·배열 생성도 측정 구간에서 제외해야 한다.
- main의 long-task 관찰만으로 Worker의 WASM 정체를 설명할 수 없다. GPU submit 완료, DOM commit, 물리 화면 표시는 서로 다른 관측이다.

## 3. 개선 방향과 우선순위

기본 순서는 **기준선·원인 측정 → 중간 명령과 snapshot 비용 → 재생 비용 → 상위 Framework/bridge 비용 → 통합 검증**이다. P0에서 Framework 비용이 명령 처리보다 크다고 확인되면 P3를 먼저 실행한다. 각 후보는 하나씩 비교하며, 근거 없는 후보는 구현을 생략하고 그 이유를 남긴다.

공용 Framework 내부 개선은 같은 작업 대상과 순서를 유지한다. 앱의 위젯 경계 조정은 필요할 경우 별도 후보로 분리하고, 내부 작업량이 달라졌음을 명시한다. 후자를 same-work 개선으로 보고하지 않는다.

## 4. 단계별 작업

### P0. 재현 기준선과 원인 계측 — 필수 / PARTIAL

1. HEAD·dirty diff·SDK/runtime·Skia·브라우저·GPU·OS·viewport·DPR·refresh rate·전원 상태·실행 명령·served asset hash를 기록한다. 사용자 실행 조건이 불명확하면 Release publish를 주 기준으로 하고 현재 Debug 실행은 비교용으로 구분한다.
2. 평가된 `RunAOTCompilation`, `WasmBuildNative`, threads, SIMD, 명시적 runtime 옵션과 실제 로드 자산을 기록한다. native relink를 managed AOT로 간주하지 않는다. 알 수 없는 Jiterpreter 상태는 `notMeasured`로 남긴다.
3. 현재 계측을 다음 세 가지 실행으로 분리한다. 계측용 query/옵션은 bootstrap에서 한 번 전달하고 runtime 생성 후 static flag를 바꾸는 방식을 피한다.
   - 최소 계측: 숫자 timestamp·frame/input ID의 bounded buffer, 종료 후 일괄 export. 전후 성능 판정용.
   - 원인 계측: stage/layout/allocation/cache bypass 상세 집계. 비용 귀속용.
   - 계약 계측: 결정적 입력·애니메이션 tick에서 logical 대상·순서·생명주기 확인용.
4. `DOROTI_ALLOCATION_PROFILE` 전달과 실제 활성 상태 출력을 보완한다. 상세 계측 OFF일 때 불필요한 payload 작성·전체 trace serialization을 하지 않는지 확인한다. 최소 계측 자체의 비용을 OFF 실행과 대조한다.
5. 입력 도착→Worker 수신→Framework 시작/끝→scene→raster→GPU 제출을 frame/input ID로 연결한다. main/Worker의 `performance.timeOrigin` 차이와 managed clock을 정렬한다. stage가 중첩되거나 render가 동기 진입하는 경우를 고려해 시간을 중복 합산하지 않는다.
6. H1~H6별 호출 수·self time·할당량·캐시 사유를 기록하고, 주 workload CPU 또는 할당의 상위 원인 1~2개를 선택한다. GC 횟수 증가만으로 GC pause가 버벅임의 원인이라고 결론 내리지 않는다.

완료 기준: 동일 workload를 다시 실행할 수 있고, profile ON/OFF가 확인되며, 실측 근거가 있는 첫 개선 후보와 baseline이 고정되어 있다. 계측이 불완전하면 제품 구조 변경에 착수하지 않고 부족한 경로를 보완한다.

### P1. 그리기 명령·scene snapshot의 중복 비용 축소 — H1/H2가 유효할 때 / DEFERRED

대상: `GraphicsAndSemanticsContracts.cs`, `PaintingTypes.cs`, scene/picture 소비자.

- 먼저 operation별 명령 수·인자 배열 바이트·payload 수와 scope별 복사 참조 수를 측정한다. paint가 없는 retained frame과 새로 paint하는 frame을 구분한다.
- P1-A: 내부 typed command 저장 방식을 설계한다. 주요 draw/transform/clip 명령부터 작은 값 필드와 payload 참조를 사용하고, 중복 인자·진단 표현은 필요한 경계에서만 만든다. public `PathCommand`/`Picture.Commands`/`Scene.Commands` 소비자와 테스트를 조사하여 호환성을 유지한다. 단순 enum 치환만으로 끝내지 않는다.
- P1-B: scope별 독립 명령 block과 immutable child 참조 등으로 중첩 snapshot의 반복 복사를 줄이는 후보를 별도로 비교한다. `addRetained`의 generation/owner 검증과 예전 snapshot의 불변성을 유지한다.
- 작은 subtree를 유지하느라 이전 scene 전체 배열이 계속 살아 있는 설계는 피한다. read-only wrapper만 씌운 mutable buffer, 아직 pending/replay 중인 배열의 pool 반환도 금지한다.
- Picture/Scene dispose 이후에도 유효해야 하는 제출 snapshot, renderer cache, PlatformView planner, Texture 자원 참조를 포함해 소유권과 마지막 사용 시점을 정의한다.
- 저장 구조 변경 전후에 명령 순서·clip/save/restore·matrix·paint 값·이미지/텍스처 수명·픽셀 결과를 비교한다. 큰 구조 변경보다 자주 호출되는 명령부터 범위를 제한한다.

완료 기준: 같은 명령 의미와 snapshot 수명을 보존하면서 실측 비용이 감소한다. P1-A와 P1-B를 묶어 원인을 알 수 없는 단일 A/B로 제출하지 않는다.

### P2. retained 명령의 재생 비용과 캐시 효율 개선 — H3가 유효할 때 / EXPERIMENT REVERTED

대상: `SkiaPictureCommandCache.cs`, `SkiaSceneRenderer.cs`, 필요한 paragraph/path/paint 변환 경로.

- command hit/miss뿐 아니라 첫 사용·identity 교체·willChange·blur·shadow·bounds·eviction·transform 변화의 bypass 사유를 집계한다. 캐시가 없어서 느린 경우와 매번 새 picture가 생겨서 못 쓰는 경우를 구분한다.
- identity가 불필요하게 바뀌면 상위 picture 생성 경로를 먼저 수정한다. 안정된 그림은 현재 `SKPicture` 기록을 재사용하고 이동·clip은 재생 시 적용한다.
- direct replay에서 실제로 비싼 immutable path/paint/text 변환만 owner 내 bounded cache 후보로 삼는다. mutable `SKPaint`를 전역 공유하거나 shader/image 참조 수명을 생략하지 않는다.
- blur/shadow를 캐시하려면 filter outsets·cull bounds·device-space 처리와 DPR별 픽셀 동등성을 먼저 해결한다. 소수점 위치 signature를 제거해 글자를 흐리게 만드는 방식은 채택하지 않는다.
- promotion 자체의 지연도 측정한다. 기존 warmup·메모리·promotion 제한을 근거 없이 완화하지 않는다. 캐시 hit 증가는 CPU·메모리·프레임 간격 개선과 함께 판정한다.

완료 기준: 반복 스크롤/애니메이션의 재생 또는 promotion 비용 감소, 텍스트·그림자·효과·fractional transform의 회귀 없음, 반복 방문 후 캐시 자원 수가 수렴한다.

### P3. Framework와 semantics의 실제 상위 비용 개선 — H4/H5가 유효할 때 / PARTIAL

대상: `framework.cs`, Rendering의 `object.cs`/`box.cs`/실측 상위 layout, `BrowserSkiaCapabilities.cs`, 필요 시 Material sample의 소유 범위.

- P0의 self time·호출량으로 최대 두 경로를 고른다. generic dynamic 제거, LINQ 전면 제거, intrinsic cache 전면 교체를 선행하지 않는다.
- Framework 후보는 같은 dirty 대상·정렬 순서·재진입·동일 widget fast path·constraint·dependency·GlobalKey 계약을 유지하며 반복 자료구조 생성이나 중복 계산만 줄인다. 잘못된 dirty 전파가 확인되면 pinned Flutter 동작과 비교해 수정한다.
- 앱 후보는 실제로 작은 animation/state 변화가 불필요한 부모 재구성을 유발하는 경우에만 별도 분리한다. 현재 로컬 progress scope와 SectionList가 있다는 사실을 반영하고 이미 해결된 문제를 다시 고치지 않는다.
- 첫 진입만 느린 경우 section 생성·text shaping·초기 cache promotion을, 반복 진입도 느린 경우 유지된 subtree의 animation/semantics/layout 활동을 각각 분석한다. 방문 State 유지 자체를 메모리 누수로 분류하지 않는다.
- semantics가 상위 비용이면 dirty node + 삭제 + 순서 + generation을 표현하는 증분 전송을 검토한다. 현재 전체 snapshot 소비 계약을 함께 수정하고, 최초 전체 상태·reset·삭제·focus node·geometry-only 이동을 검증한다. 접근성을 끄거나 업데이트를 임의로 지연시키지 않는다.
- wheel/포인터 의미는 유지한다. 이벤트를 버려 처리량을 낮추지 않는다. batch가 필요하면 샘플 순서·timestamp·gesture 누적량을 보존한 전송 최적화로 한정한다.

완료 기준: 선택한 병목의 self time 감소와 계약 보존. 의도적으로 widget 경계를 바꾼 후보는 observable 결과가 같아도 작업량 변화와 원인을 별도 기록한다.

### P4. managed 실행 모드 확인과 조건부 AOT 비교 — P0 결과에 따라 / PARTIAL

- P0에서 실제 실행 모드를 확인하는 일은 필수다. GPU 또는 명령 생성 문제가 우세하면 AOT 실험은 근거와 함께 `notApplicable`로 정리한다.
- 필요한 C# 작업 자체의 실행 시간이 계속 지배적이고 현재 산출물이 managed AOT가 아닐 때만, 같은 소스·같은 화면으로 공식 설치 toolchain의 Mono WASM AOT publish를 비교한다.
- 기존 `DorotiPartialAot`는 Material을 해석 실행에 남기는 별도 후보이고, compiler hook이 존재한다는 사실은 정상 full AOT 성공 증거가 아니다. 빌드 실패를 숨기거나 custom compiler 수정으로 본 작업을 확장하지 않는다.
- 구조 변경과 runtime 변경은 독립 실험으로 진행한다. 프레임 CPU뿐 아니라 다운로드 크기·startup·메모리·threads·Skia interop·trim 동작을 비교한 후 default 채택 여부를 결정한다.
- CoreCLR/NativeAOT 전환, 전체 앱의 병렬 layout, Worker 추가는 별도 아키텍처 과제이며 이 계획의 완료 조건에 포함하지 않는다.

완료 기준: 실험 적용 여부와 이유가 명시되고, 수행했다면 실제 로드 자산까지 확인한 성능·비용 비교와 채택/원복 결정이 있다. 지원 불가·빌드 실패는 해당 후보의 상태로 남기고 다른 개선과 구분한다.

### P5. 통합·회귀·결과 정리 — 필수 / PARTIAL

- 채택한 후보만 최종 소스에 남긴다. 실패/원복 후보의 결과는 기록하고 실험 flag·중복 제품 경로를 정리한다.
- 공용 명령/Framework 변경은 affected 프로젝트의 Release build와 계약을 검증한다. 소스의 `doroti-reviewed-framework-source` 소유 정책을 유지한다. generator lowering을 바꾼 경우에만 그에 대응하는 변환 fixture·재생성 검증을 추가한다.
- 최종 Web 산출물의 hash를 고정하고 주 workload 최소 계측 A/B와 보조 backend 회귀를 수행한다. 구현 중 통과한 동일 소스 검사를 이유 없이 반복하지 않는다.
- 일반 sample의 scroll/state/focus/text/theme/column 복원과 DPR 변화, glyph·clip·opacity·blur/shadow, 기존 WebView/Texture의 명령 소비 경로를 변경 범위에 맞게 확인한다.
- 공용 renderer를 바꿨다면 실행 가능한 native host 한 곳의 대응 그림·수명 회귀를 확인한다. 다른 플랫폼 전체 성능을 검증한 것으로 확대하지 않는다.
- 재현 명령, before/after source·assets, 환경, metric 정의, 후보별 채택/원복 사유, `PASS/PARTIAL/notMeasured/notVerified/notComparable`를 기록한다. 물리 입력/화면 체감은 자동화 수치와 분리한다.

## 5. 측정 시나리오와 실험 규칙

### 5.1 재현 corpus

주 환경은 Chromium foreground, Release publish, `dorotiTestbedMode=sample`, 현재 기본 renderer로 한다. URL 옵션·선택 탭·초기 상태·font 준비·입력 이벤트 시각을 저장한다. 네트워크 영상과 외부 iframe은 주 corpus에서 사용하지 않는다.

| ID | 시나리오 | 구분하려는 비용 |
| --- | --- | --- |
| S0 | 정적인 작은 화면과 작은 독립 animation을 가진 비교용 화면 | 앱 고정비와 콘텐츠 크기에 따른 비용 |
| S1 | Components 첫 방문, 아직 만들지 않은 구간을 순차 스크롤 | lazy 생성·초기 text/paint 비용 |
| S2 | 같은 구간을 방문 완료한 뒤 동일 경로 왕복 | steady-state layout·retained/cache·메모리 |
| S3 | 화면에 보이는 progress/animation을 10초 실행 | 지속적인 frame 비용과 입력 없이 발생하는 부하 |
| S4 | 스크롤 중 버튼/선택/텍스트 focus 입력, 정지 후 응답 확인 | Worker 정체와 입력→제출 지연 |
| S5 | 동일 구간 왕복·다른 탭 복귀 후 10초 안정화 | idle 작업, retained State와 cache의 메모리 수렴 |

- 기본 geometry는 1280×900/DPR 1, 보조는 800×900/DPR 1과 1280×900/DPR 2로 고정한다. 고정 viewport 성능과 breakpoint 전환 회귀를 섞지 않는다.
- 각 구간의 section ID·실제 scroll 위치·내용·상태를 확인한다. 같은 wheel 횟수여도 다른 화면까지 이동했다면 비교가 성립하지 않는다.
- 첫 방문 측정 전에 전체 화면을 미리 방문하지 않는다. 반대로 steady-state 측정은 정해진 구간을 같은 순서로 준비한다.
- 애니메이션 계약은 결정적 tick fixture로 비교한다. 실시간 A/B의 프레임 수가 늘어난 것은 가능한 개선 결과이므로, 프레임 수를 같게 강제하거나 전체 할당량만 비교하지 않는다. 프레임당·초당·동일 동작당 값을 함께 기록한다.

### 5.2 지표

| 구분 | 필수 기록 | 해석 제한 |
| --- | --- | --- |
| CPU | owner active time, build/layout/paint/scene/raster의 p50·p95·max, 원인별 self time | 중첩 inclusive time 합산 금지 |
| 입력 | main ingress→Worker dispatch→해당 input을 반영한 submit/commit | GPU 완료·commit은 물리 display latency가 아님 |
| 프레임 간격 | Worker rAF와 고유 새 scene 제출 간격, 반복 replay·superseded·dropped 분리 | idle의 긴 간격은 jank로 세지 않음; submit FPS는 실제 표시 FPS와 구분 |
| 할당·GC | 구간별 owner/전체 할당 delta, B/frame·B/action·B/s, collection 횟수 | GC pause는 유효한 runtime trace가 있을 때만 수치화 |
| 메모리 | managed live estimate, WASM capacity, 가능한 process/GPU 메모리, cache entries/bytes | JS heap만으로 전체 메모리 개선 판정 금지; 미지원은 `notMeasured` |
| 작업 동등성 | deterministic trace, 구간 ID·state·scroll offset·실제 frame/scene 수 | 화면·대상 불일치는 `notComparable`, 기능 변화 후보는 별도 분류 |

### 5.3 실행 예산과 중단

- 먼저 기준선 diagnosis 1회와 최소 계측 대조를 수행한다. 상위 후보 최대 2개만 실험 대상으로 선택한다.
- 각 후보의 주 환경은 **3쌍 A/B, 순서 AB/BA/AB**로 비교한다. 한 실행에서 corpus를 순서대로 수행하며 상세 trace는 별도 실행한다. 자동 retry는 0이다.
- 보조 geometry/backend는 우선 짧은 회귀로 확인한다. 새 실패나 유의미한 불확실성 없이 전체 성능 matrix를 반복하지 않는다. 전체 브라우저 세션 예산은 30회 이내로 관리하고 실패도 횟수에 포함한다.
- build/publish/test/browser runner는 [run-with-timeout.py](Doroti/validation/run-with-timeout.py)로 감싸 **20분 process-tree timeout**을 적용한다. GPU 작업은 순차 실행한다. 기존 사용자 서버는 유지하고 작업 소유 process/listener만 종료한다.
- 기존 history에 적힌 harness·artifact 경로가 현재 checkout에 남아 있다고 가정하지 않는다. 이번 검토에서 `Doroti/validation/web-playwright`와 일부 과거 ADR/fixture 경로는 존재하지 않았다. 유효한 현재 harness를 재사용하고 부족한 일반 화면 driver만 만든다.
- 실패한 후보는 원본 로그를 보존하고 수정 이유 없이 재시도하지 않는다. 환경/작업 불일치는 먼저 교정하며 임의로 유리한 run만 선택하지 않는다.

## 6. 채택 기준과 종료 조건

아래 수치는 **이 작업의 제안 기준**이며 달성한 성능이 아니다. P0에서 기준 장치와 실제 refresh rate를 기록한 뒤 수치 변경이 필요하면 실험 시작 전에 이유와 함께 확정한다.

1. **기능·수명 필수:** 결정적 계약과 해당 픽셀·입력·state 회귀 통과, 새 runtime error/유실된 입력/잘못된 stale scene/자원 누수 없음.
2. **후보 채택:** 세 쌍에서 개선 방향이 일관되고, 대표 병목 구간 p95가 15% 이상 감소하며 전체 owner CPU/action 또는 입력→submit p95도 10% 이상 감소하는 것을 목표로 한다. 표본이 적으므로 통계적 확정 표현은 하지 않는다.
3. **메모리만 좋아진 후보:** 동일 동작당 할당이 20% 이상 감소해도 전체 지연이 개선되지 않으면 `allocationOnly`로 분류한다. 이것만으로 버벅임 해결이나 기본 채택을 결정하지 않는다. 지연·live memory의 실질 악화가 있으면 원복한다.
4. **프레임 목표:** 60Hz 기준 warm S2/S3의 owner 작업+동기 raster/submit p95는 13.3ms 이하를 목표로 하고, 16.7ms 초과율과 active 구간 50ms 초과 gap을 보고한다. 더 높은 refresh rate는 별도 결과이며 이 수치로 통과시키지 않는다. 첫 생성 S1의 spike도 별도 표로 남긴다.
5. **회귀:** 주 지표의 5% 초과 악화나 새 긴 정체는 수용하지 않는다. baseline 자체 분산으로 판별 불가하면 `inconclusive`로 남기고 예산 내에서 원인을 확인한다. cache를 키워 메모리가 늘면 비용과 상한을 별도 심사한다.
6. **전체 완료:** 원인별 결과·채택한 개선·최종 산출물과 회귀 근거가 있고 위 목표를 만족해야 범위 내 성능 PASS로 기록한다. 일부 개선만 있거나 예산 내 목표 미달이면 `PARTIAL`과 남은 상위 병목을 남긴다. 자동화 PASS와 사용자의 실제 체감 확인은 별개다.

## 7. 산출물과 진행표

구현 단계의 계획 산출물:

- `Doroti/validation/web-frame-cost/`: 일반 화면 driver, 구간·비교 분석, 최소 계약 fixture, 실행 README.
- `Doroti/validation/web-frame-cost/results-<date>.md`: source/asset identity와 before/after 핵심 수치, 한계·채택 결정. 핵심 요약은 추적되는 파일로 보존한다.
- `Doroti/artifacts/web-frame-cost/<run>/`: raw trace·screenshot·로그·환경 manifest. 삭제 가능한 로컬 산출물이며 정리 후 해당 원본이 남아 있다고 보고하지 않는다.
- 채택된 코드와 범위에 맞는 검증. runtime 또는 저장 구조의 대규모 변경이 필요하면 P0 근거와 설계 결정을 먼저 이 문서에 추가한다.

| 단계 | 현재 상태 | 결과 기록 |
| --- | --- | --- |
| 구조 검토 및 계획 | DONE | 소스·현재 존재하는 문서 확인. 측정으로 원인 확정하지 않음. |
| P0 기준선·계측 | PARTIAL | Release runtime/자산 hash, 숫자 ring, 실제 allocation flag, export 제외 할당량, scroll/animation 기준선 확보. 정확한 input→반영 scene 인과 연결, 전체 stage·명령별 비용 및 결정적 계약 fixture는 미완료. |
| P1 명령·snapshot | DEFERRED | 중복 저장·scope 복사가 상위 원인이라는 근거 미확보. 구조 변경하지 않음. `notApplicable`로 확정하지 않음. |
| P2 재생·캐시 | EXPERIMENT REVERTED | 텍스트 재생의 빈 fallback JSON 생성을 줄이는 후보를 AB/BA/AB 3쌍 비교. 일관성·채택 기준 미달로 원복. bypass 사유 전체 분류는 미완료. |
| P3 Framework·bridge | PARTIAL | 상세 진단 OFF에서 raster interop/message/payload 생성을 차단. AnimatedBuilder와 semantics의 높은 self time 확인. Framework 구조·작업 순서는 변경하지 않았으며 해당 병목 개선은 남음. |
| P4 실행 모드 | PARTIAL | 실제 Release Mono 10.0.11, threads/SIMD/native relink 확인. managed AOT 설정 없음. Jiterpreter 활성·기여도 및 AOT 독립 실험은 미측정이며 AOT 적합성을 확정하지 않음. |
| P5 통합·검증 | PARTIAL | Release publish, ring/분석 계약, native raster 계약 및 Web 회귀 근거는 결과 문서에 기록. 전체 corpus·결정적 Framework 계약·물리 체감·전체 플랫폼 성능 PASS 아님. |

### 2026-09-22 실행 판단

- 첫 후보의 S2 owner 동기 구간 합계 변화는 세 쌍에서 **−1.2%, −2.1%, +1.5%**였다. S3도 일관되게 개선되지 않았다. 좋은 실행만 골라 채택하지 않고 후보를 원복했다.
- 유지한 변경은 계측과 진단 OFF 경로의 불필요한 작업 제거다. 숫자 ring은 16,384개 기록으로 제한하고, export 할당을 managed 구간 할당에 섞지 않는다. 최종 `finish`는 같은 Worker turn에서 managed/ring snapshot을 함께 읽는다.
- corpus는 좌측 Components 스크롤, 보이는 progress 10초, 버튼 입력, 탭 복귀를 재현한다. 우측 전체 방문·독립 소형 animation·스크롤 중 focus 동시 입력·결정적 tick 계약까지 충족한 것으로 확대하지 않는다.
- 남은 우선 작업은 AnimatedBuilder/semantics 내부 self time 세분화와 정확한 scene/input 연결이다. 현재 증거로 명령 저장 구조·캐시 크기·AOT 기본값을 변경하거나 “버벅임 해결”을 선언하지 않는다.

과거 참고: [same-work 실험 기록](history/26-09-08/wasm-same-work-execution.md), [작업 계획 보관 요약](history/26-09-08/work-plans-summary.md). 과거 할당 감소나 특정 패널 raster 개선은 이번 일반 화면의 성능 증거로 재사용하지 않는다. 기준 구조는 위에서 직접 확인한 현재 소스를 따른다.

## 8. iPhone 탭 종료 대응과 Web 메모리 관리

- 추가일: 2026-09-22
- 상태: **PARTIAL — 구현·로컬 검증 수행, 실기기 종료 해결 및 WebGPU 지연 기준 미충족**
- 실행 기록: [Web 메모리 검증](Doroti/validation/web-memory/README.md), [2026-09-22 결과](Doroti/validation/web-memory/results-2026-09-22.md)
- 최초 추가 요청은 계획 작성만을 대상으로 했다. 이후 사용자의 `work.md의 추가작업 전체 해줘` 요청에 따라 제품 코드 수정·Release publish·계약·브라우저 검증을 실행한다. 계획 당시의 실행 금지 문구를 현재 요청의 제한으로 해석하지 않는다.
- 기존 P0~P5의 PARTIAL 및 미검증 경계는 유지한다. 이번 단계는 **탭 종료 방지와 메모리 상한·자원 회수**를 별도 목표로 삼는다.

### 8.1 확인한 현상과 근거의 범위

| 구분 | 확인 내용 | 아직 확정하지 않은 내용 |
| --- | --- | --- |
| 사용자 기기 관측 | iPhone 12, iOS 26, Chrome의 일반 브라우저 실행. `?dorotiTestbedMode=sample`에서 버벅임이 약간 줄었지만 탭이 종료됨. `&dorotiRenderer=worker-direct-webgl` 추가 후에는 사용자 확인 구간에서 크래시가 발생하지 않음. | 정확한 OS/Chrome 버전, 실행 자산·작업·관찰 시간 일치, jetsam/crash 원인, 장시간 안정성 |
| 기본 경로 | 현재 renderer 자동 선택은 `worker-direct-webgpu`. 사용자 URL에는 상세 진단·frame-cost 옵션이 없어 추가 계측은 OFF. | 종료가 WebKit GPU process, WebContent, WASM trap, device loss 중 어디에서 발생했는지 |
| 텍스트 자원 | `_textRenderResources`는 색·font size·weight·spacing·fallback 등의 key별 보관이며 상한이 없음. native probe에서 서로 다른 색 32/64/128/256개에 대해 GC 후에도 자원 32/64/128/256개가 유지되고 renderer Dispose 후 0개가 됨. | 이 캐시가 사용자 탭 종료에서 차지하는 비중. 같은 스타일의 반복 사용을 무한 증가로 해석하지 않음. |
| GPU 예산 | Web host의 `ResourceCacheBytes`는 256MiB. Ganesh cache limit 및 Graphite context/recorder 설정에 사용됨. picture raster 한도는 16M pixels. | 실제 GPU resident bytes와 peak. 설정 상한·context·recorder·raster 수치를 단순 합산하지 않음. |
| 출력 버퍼 | 초기 backing은 각 축에 1.5배 여유를 두고 screen 크기도 고려. Worker의 이후 크기는 grow-only. | 실제 기기의 viewport/DPR별 backing 및 회전·키보드 후 peak와 잔류량 |

WebGL 대조 결과는 **WebGPU 경로를 우선 조사할 근거**다. 공용 캐시 문제가 없다는 증거도, WebKit의 특정 버그나 OOM을 확정한 증거도 아니다. WebGL 우회 성공과 WebGPU 원인 수정 완료를 서로 다른 상태로 기록한다.

소스: [renderer 선택](Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts), [텍스트·raster 캐시](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs), [Web cache budget](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs), [Graphite 수명](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs), [초기 backing](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts), [WebGPU backing·queue](Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts). native probe 원본은 삭제 가능한 `Doroti/artifacts/iphone-memory-review/`에 있으며, 위 표에 핵심 결과를 보존한다.

### 8.2 범위와 적용 순서

**M0 원인 분리·기준선 → M1 iOS 시작 backend 안정화 → M2 텍스트 캐시 제한 → M3 GPU 예산·회수 → M4 backing 축소 → M5 통합 검증** 순서로 진행한다. backend 전환과 M2~M4를 하나의 A/B에 섞지 않는다. 공용 메모리 개선은 WebGL에서도 적용·검증하고, WebGPU 수정 여부는 WebGPU 실행으로 별도 판정한다.

- main runtime 1개 + shared-runtime render Worker 및 직접 GPU 출력 구조를 유지한다.
- 화면 상태·방문 State·focus·IME·semantics를 삭제하거나, 애니메이션을 멈추거나, DPR·효과를 낮추어 통과시키지 않는다. 폐기 가능한 렌더 캐시와 앱 상태를 구분한다.
- 강제 GC 반복, WASM maximum memory 축소, 자동 reload 반복을 해결책으로 채택하지 않는다. runtime/AOT·도구체인 교체는 이번 범위 밖이다.
- 현재 WebGPU queue의 in-flight 제한과 texture handle의 release 경로를 기준선으로 삼는다. 무제한 queue나 image cache 누수를 조사 없이 가정하지 않는다.

### M0. 동일 조건의 backend 비교와 메모리 기준선 — PARTIAL

1. iPhone 12의 정확한 iOS 26.x/build, Chrome 버전, URL, 실행 자산 hash, viewport/DPR, 화면 방향, 종료 전 동작·소요 시간, 재로드/오류 문구를 기록한다. 사용자 기존 관측은 `userObserved`로 보존한다.
2. 동일 Release 자산·입력·진단 OFF 조건에서 기본 WebGPU와 명시적 WebGL을 비교한다. 처음부터 두 경로의 시작 화면·GPU 초기화 성공·실제 선택 backend를 확인한다. 기기 재종료는 필요한 최소 재현만 수행한다.
3. 가능한 기기 로그/원격 inspector로 WebContent/GPU process 종료, jetsam memory pressure, device loss, WASM trap을 분리한다. 접근 불가한 로그는 `notVerified`; "탭이 닫힘"만으로 OOM으로 분류하지 않는다. WebKit 알려진 문제는 정확한 OS 빌드와 해당 수정 배포 여부를 대조한다.
4. managed live estimate·owner/전체 allocation delta·GC 횟수, WASM capacity, text/cache entries·보유 bytes, Graphite budgeted bytes, canvas 실제/필요 크기·추정 bytes, in-flight/retired 자원 수를 구분해 구간 경계에서 수집한다. process/GPU 메모리 미지원은 `notMeasured`로 남긴다. 가상 WASM 용량과 실제 resident memory를 동일시하지 않는다.
5. 전면 실행을 외부 driver/기기 관찰로 감시한다. 종료될 탭의 메모리 속 로그만 의존하지 않고 최소 진행 marker를 외부 기록 또는 제한 크기 로컬 기록으로 보존한다. 상세 trace·매 프레임 serialization은 안정성 비교에서 끈다.

완료 기준: 최소 재현 절차와 동일 workload의 비교 가능 여부, 실제 backend, 사용 가능한 메모리 지표·종료 분류가 고정되어 있다. 기기 연결이 없으면 desktop 결과로 실제 iPhone 종료 해결을 대신하지 않는다.

### M1. iOS WebKit의 시작 backend 안정화 — PARTIAL — 선택 구현·계약 완료 / 실기기 미확인

대상: `doroti.loader.ts`, `doroti.web.ts`의 중복 선택 경로와 bootstrap/diagnostics 계약.

- 사용자 대조 결과를 바탕으로 **iOS WebKit의 자동 선택은 우선 WebGL로 시작**하는 안정화 후보를 구현한다. iOS Chrome을 desktop Chromium과 같은 경로로 취급하지 않는다. iPad의 desktop UA 등 플랫폼 식별 한계와 실제 WebGL2/OffscreenCanvas 지원 여부를 함께 처리한다. `navigator.gpu` 존재만으로 안정성이 입증된 것으로 보지 않는다.
- 선택 규칙을 한 곳에서 계산하고 loader·host·Worker에 동일하게 전달한다. `requested/selected/reason`을 가벼운 진단 정보로 노출한다. 그 외 플랫폼의 기본 WebGPU는 유지한다.
- 기존 `dorotiRenderer=worker-direct-webgpu` / `worker-direct-webgl` 명시 옵션은 검증용 override로 보존한다. 명시적 선택 실패를 몰래 다른 backend의 PASS로 바꾸지 않는다.
- renderer 생성 전에 선택한다. 동작 중인 context를 조용히 교체하거나 framework/scene/texture를 중복 생성하는 hot fallback은 추가하지 않는다. WebGL 초기화 불가 시 이유를 보고하고 무한 초기화·재로드를 하지 않는다.
- 자동 선택·명시 override·미지원 capability를 계약 검사하고, iPhone에서 추가 query 없이 sample을 시작해 확인한다. WebGPU 재기본화 조건은 원인과 해당 OS에서의 안정성 근거가 확보되는 것으로 명시한다.

완료 기준: iOS 기본 진입의 backend가 일관되며 실기기에서 정해진 관찰 구간의 종료가 재발하지 않는다. 이는 `mitigated`이며 **WebGPU 자체 해결**로 기록하지 않는다.

### M2. 텍스트 렌더 자원의 보유 상한과 수명 — PASS — 구현·native 수명/픽셀 계약

대상: `SkiaSceneRenderer.GetTextRenderResources`, `TextRenderResources`, font 등록·cache clear·renderer dispose 경로.

- owner 안에서 LRU 등 제한 있는 보관 정책을 도입한다. entry 수와 보유 자원/추정 bytes를 함께 기록하고 최초 상한은 baseline의 작업 집합을 보고 실험 전에 고정한다. 추정 bytes를 정확한 native resident bytes로 표시하지 않는다.
- 색상만 달라질 때 font/fallback 자원을 중복 생성하는 구조를 별도 후보로 검토한다. 색과 무관한 font key 분리는 LRU 후보와 독립 비교하며, 전역 mutable `SKPaint` 공유로 대체하지 않는다.
- 활성 draw/layout 호출, SKPicture recording/replay, pending GPU recording이 참조하는 자원을 조기 Dispose하지 않도록 소유권·마지막 사용 시점을 정의한다. 필요하면 사용 중 항목을 보호하고, 해제는 owner에서 수행한다.
- 등록 font 변경 시 generation과 fallback resolution을 갱신한다. clear/dispose/예외 경로의 이중 해제와 누락을 검사한다. 한글·Latin·대체 glyph, font size/weight/spacing, theme/color 변경, fractional transform/DPR의 결과를 비교한다.
- 재현 fixture에서 상한을 넘는 서로 다른 스타일과 같은 스타일의 재방문을 수행한다. 상한 초과 후 entries가 수렴하고 퇴출 자원이 해제되며, 재생성 후에도 glyph·측정값·픽셀이 유지되는지 확인한다. GC 후 dictionary가 남는 기존 현상을 단순 "GC 미실행"으로 처리하지 않는다.

완료 기준: 살아 있는 렌더 자원의 상한과 퇴출·재사용 계약이 검증되고, 반복 theme/스타일 변경 뒤 보유량이 계속 증가하지 않는다.

### M3. Web GPU 캐시 예산과 회수 정책 — PARTIAL — WebGL 예산 채택 / WebGPU 작은 예산 기각

대상: `DorotiWebWorkerSurface.cs`, `.Graphite.cs`, `SkiaSceneRenderer`의 raster cache·GPU 자원 소유 경로.

- 현재 고정 256MiB를 모바일에 그대로 적용하는 정책을 개선한다. host가 시작 시 정하는 메모리 프로필을 Ganesh/Graphite와 renderer에 일관되게 전달한다. UA·`deviceMemory`·GPU API 존재를 실제 가용 RAM 측정치로 간주하지 않는다.
- native cache 예산 64MiB, picture raster 예산 RGBA8 환산 16MiB를 **최초 비교 후보**로 삼고, context/recorder 예산 의미와 중복 포함 여부를 설치된 Skia API로 확인한 뒤 확정한다. 이 값은 iPhone 안전 한계나 총 GPU 메모리 상한을 보증하지 않는다. entry 한도와 byte/pixel 한도를 함께 둔다.
- promotion을 할당한 뒤에만 trim하면 생기는 순간 peak를 점검한다. 활성 프레임이 참조하는 항목은 보존하고, 추가 할당을 포함한 예산 판단·회수 시점을 정의한다. cache miss/promotion 급증으로 스크롤이 나빠지는지 함께 측정한다.
- frame/recording 완료 후 회수 가능한 native resources, 정지·비가시 전환 시 버릴 수 있는 캐시를 구분한다. GPU 제출·비동기 map·texture 참조가 끝나기 전에 해제하지 않는다. 매 프레임 전체 cache flush는 하지 않는다.
- Graphite image upload cache는 설치된 구현의 기존 LRU·native 참조 계약을 먼저 확인한다. 이름이나 외부 collection 크기만으로 무제한 누수라 판단하지 않는다. GPU budget 수치가 줄어도 process 전체 peak가 줄었다고 확대하지 않는다.

완료 기준: 선택한 예산과 실제 관측값을 설명할 수 있고, 반복 작업·idle 뒤 cache/native 자원이 수렴하며 수명·화질·프레임 비용 회귀가 없다.

### M4. 과대 backing 할당과 grow-only 정책 개선 — PARTIAL — 구현·브라우저 계약 완료 / 실기기 미확인

대상: `configureDirectCanvasCapacity`, `doroti.raster.worker.ts`, `doroti.webgpu.ts`, resize generation/commit 경로.

- 모바일 최초 backing을 화면의 실제 필요 pixel 크기 기준으로 산정한다. 데스크톱 resize 여유 정책을 그대로 확장하지 않고, 여유율과 최대 초과 면적을 수치로 제한한다. DPR은 유지한다.
- 회전·주소창 변화·키보드로 viewport가 줄어든 뒤 일정 시간 안정되면 shrink하도록 hysteresis를 설계한다. 매 resize 이벤트마다 재할당하지 않는다. 구현 전 안정화 시간·축소 비율·최소 재할당 간격을 기록한다.
- transferred OffscreenCanvas의 owner에서만 backing을 변경한다. host의 CSS 크기·physical size·resize epoch·surface generation과 일치시키고, 축소 완료 전후의 frame/commit 순서를 정의한다. stale scene, crop, 빈 frame, 일시적 확대·축소가 보이지 않도록 기존 resize 계약을 유지한다.
- 회수 전 old/new surface가 공존하는 순간 peak를 포함해 계산한다. WebGL framebuffer/surface 재생성과 WebGPU current texture/native wrapper의 해제 시점을 각각 확인한다. 화면 state·focus·IME·platform view identity를 보존한다.
- portrait→landscape→portrait, 키보드 열기/닫기, 주소창 변화, background→foreground를 제한 횟수로 검증한다. 각 단계의 필요 크기·실제 backing·bytes·generation을 남긴다. 유휴 후에도 큰 한쪽 축만 남는 현상이 사라졌는지 확인한다.

완료 기준: 정해진 초과 면적 한도와 shrink 정책을 만족하고, 기존 DPR/픽셀 결과 및 resize·입력·native view 연속성이 유지된다.

### M5. 통합 검증·채택·기록 — PARTIAL

| workload | 확인할 내용 |
| --- | --- |
| 같은 화면에서 progress/animation 전면 실행 | frame당 할당·GPU 자원·queue 수렴, 탭 종료·device loss |
| Components 양쪽 방문·왕복·탭 복귀 | 유지된 State와 cache 구분, 같은 구간 재방문 후 보유량 수렴 |
| theme/색상/font 변화 후 동일 상태 재방문 | text resources 상한·퇴출·glyph/측정·cache miss 비용 |
| 방향 전환·주소창·키보드·복귀 | backing 축소, 일시적 peak, generation·focus·IME 보존 |
| 기존 WebView/Texture 혼합 화면 | backend 선택·자원 회수가 외부 view/texture 수명에 미치는 회귀 |

- 사용자 iPhone 12/iOS Chrome의 최종 자동 선택 경로를 주 안정성 환경으로 삼는다. 명시적 WebGPU는 별도 원인/수정 검증이다. desktop WebGPU/WebGL·자동화된 모바일 viewport를 실제 iPhone 증거로 대체하지 않는다.
- 각 후보의 성능은 같은 backend에서 3쌍 AB/BA/AB로 비교한다. WebGPU→WebGL 전환의 차이를 공용 cache 개선 효과로 합산하지 않는다. 성공한 동일 소스 검사는 새 변경·실패·불확실성 없이 반복하지 않는다.
- 추가 M 단계의 **브라우저/기기 실행 총예산은 실패 포함 30회 이내**, 자동 retry 0, GPU 작업은 순차 실행한다. M0에서 후보·회귀·실기기 관찰에 예산을 배분하고, 기존 P 단계 24회 기록은 별도로 보존한다. 한 실행은 20분 process-tree timeout을 적용한다. 수동 기기 관찰도 20분을 넘기지 않는다.
- 최종 전면 관찰은 기본 10분으로 시작한다. 기존 종료 시간의 두 배가 더 길면 그 시간을 목표로 하되 20분을 넘으면 이번 예산에서 안정성을 확정하지 않고 `PARTIAL`로 남긴다. foreground 관찰 시간·실제 동작·기기 조건을 기록한다. 무한 안정성을 주장하지 않는다.
- 캐시 상한 충족, 반복 같은 workload의 마지막 세 구간에서 보유량이 계속 증가하지 않는지, peak와 안정화 후 memory를 각각 확인한다. WASM capacity는 커진 뒤 즉시 줄어드는 것을 요구하지 않고 추가 성장·live estimate·resident 지표를 나눠 판단한다. 임의 강제 GC로 제품 수렴을 연출하지 않는다.
- 이번 안정성 작업은 §6의 CPU 15%/10% 개선 없이도 **수명 결함·무제한 보유 제거 또는 실제 탭 종료 완화**를 근거로 채택할 수 있다. 기능·픽셀·수명 계약은 필수이며, 같은 backend의 주요 지연 5% 초과 악화는 기본 수용하지 않는다. backend 전환의 성능 차이는 별도 공개하고 자동 선택의 안정성 판단과 구분한다. 한도 숫자만 낮춘 변경은 메모리 개선 PASS로 인정하지 않는다.
- 공용 renderer 변경 시 affected Release build·계약 및 실행 가능한 native host 한 곳의 그림·수명 회귀를 수행한다. generator lowering을 바꾼 경우에만 대응 fixture·재생성 검증을 추가한다.
- 최종 source/served asset hash, runtime/OS, backend 선택 이유, 예산, source/asset별 before/after, 실패·원복·채택 이유를 추적되는 보고서에 남긴다. 채택한 경로만 최종 소스에 남기며 실험 flag·중복 선택 로직을 정리한다.

산출물 계획: `Doroti/validation/web-memory/`의 driver·cache/resize/backend 계약·README, `results-<date>.md`의 핵심 근거, `Doroti/artifacts/web-memory/<run>/`의 삭제 가능한 raw 로그·자산 manifest·메모리 시계열·스크린샷. 기존 frame-cost 도구는 활용하되 CPU 개선과 메모리 안정성 결과를 혼동하지 않는다.

| 단계 | 상태 | 종료 시 기록할 결과 |
| --- | --- | --- |
| M0 기기·backend·메모리 기준선 | PARTIAL | 연결 기기 iOS/Chrome 및 desktop 비교 확보. 기존 URL/자산·실제 종료 원인은 미확인. |
| M1 iOS 기본 WebGL 선택 | PARTIAL | 중앙 선택·override·미지원 계약 통과. 실제 iPhone의 `mitigated` 판정은 notVerified. |
| M2 텍스트 자원 제한 | PASS | 256-entry LRU, 초과 스타일 반복·퇴출·font 등록·SKPicture 수명·glyph/측정/픽셀 계약 통과. |
| M3 GPU 예산·회수 | PARTIAL | 모바일 WebGL 64MiB native/16MiB raster 및 사전 예약·현재 프레임 보호. WebGPU 작은 예산은 기각하고 기존 값 유지. 유휴 회수 적용. |
| M4 backing 여유·축소 | PARTIAL | mobile exact backing, 1초 안정/2초 간격/25% 초과 축소, 회전 중간 면적 제한. desktop 실행·WebView DPR/resize identity 통과. 실제 IME/주소창 연속성은 미확인. |
| M5 통합·기기 검증 | PARTIAL | Release publish, native/정책 계약, 입력·양 backend Texture 픽셀/수명, WebView 계약 및 진단 OFF 기본 경로 10분 관찰 수행. 실제 iPhone 및 WebGPU 지연 5% gate 미충족. |

전체 판정에서는 **기본 경로 안정화**, **공용 메모리 관리 개선**, **WebGPU 원인 해결**을 각각 기록한다. WebGL에서 재발하지 않는다는 결과만으로 세 항목을 모두 완료 처리하지 않는다.


### 2026-09-22 추가작업 실행 판단

- [추가작업 결과 보고서](Doroti/validation/web-memory/results-2026-09-22.md)에 채택·기각 후보, 실패한 검증 준비, source/asset hash 및 재현 절차를 기록했다.
- 낮은 raster 예산만 적용한 후보는 promotion 급증으로 기각했다. 큰 단일 항목의 점유와 현재 프레임의 퇴출을 제한한 정책을 남겼다.
- WebGPU의 64MiB native/16MiB raster 후보는 일관된 지연·사용량 개선을 입증하지 못해 기존 예산으로 복구했다. WebGPU 문제 해결을 선언하지 않는다.
- 초기 계획의 desktop·자동화 결과를 실제 iPhone 증거로 대체하지 않는 기준을 유지한다. 실제 종료 URL·동일 자산·기기 foreground 관찰 및 최종 WebGPU 성능 gate는 남은 작업이다.

- 최종 자동 선택 기본 경로의 desktop iPhone-UA 관찰: 진단 OFF, 이미지 구간 왕복·theme/탭 전환 41회 / 606.553초, 오류·탭 종료 없음. 실패한 fixture 준비 2건을 포함해 browser 예산 30/30회 사용. 실제 iPhone의 성공으로 대체하지 않는다.

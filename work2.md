# Web CanvasKit 후속 개선 작업계획

작성일: 2026-09-05

상태: **검토·계획 작성 완료 / 아래 구현·검증은 미실행**

사용자는 현재 구현을 “완벽하지는 않지만 지금까지 구현 중 가장 좋다”고 평가했다. 따라서 현재 선택 후보를 보존하고, 체감이 좋아진 이유를 유지하면서 남은 지연과 화면 추종을 개선한다. 이 피드백은 선호 기준이며, 모든 방향·줄바꿈·실제 IME·물리 화면 수용 완료를 뜻하지 않는다.

이번 요청의 산출물은 이 문서다. 제품 코드, 기존 `work.md`, 결과 보고서, 원시 자료를 변경하거나 추가 native 드래그를 실행하지 않는다. 향후 구현 순서는 아래 N0→N6이며, 단계별 근거가 부족하면 다음 실험을 자동 확대하지 않는다.

## 1. 검토 근거와 현재 기준선

- 현재 상태: [work.md](work.md)의 맨 위 2부 종료 요약. 아래에 보존된 1부의 ‘미실행’ 문장은 당시 기록이다.
- 최신 측정·실패·build identity: [2부 실행 결과](history/26-09-05/web-canvaskit-redesign-v2-part2-results.md).
- 이전 비교와 기각 근거: [1부 결과](history/26-09-05/web-canvaskit-redesign-v2-results.md).
- 구현 검토: [UI Worker](Doroti/src/Doroti.Host.Web/Web/doroti.ui.worker.ts), [WidgetsBinding](Doroti/src/Doroti.Framework.Widgets/binding.cs), [MediaQuery](Doroti/src/Doroti.Framework.Widgets/media_query.cs), [BuildScope](Doroti/src/Doroti.Framework.Widgets/framework.cs), [RenderObject](Doroti/src/Doroti.Framework.Rendering/object.cs), [mapper](Doroti/src/Doroti.Host.Web/BrowserDisplayListMapper.cs), [encoder](Doroti/src/Doroti.Graphics.DisplayList/DisplayListEncoder.cs).

현재 선택 후보를 **C0**, 이번 계획으로 새로 만드는 단일 변경 후보를 **C1**이라고 부른다. C0는 현재 dirty working tree의 구현을 포함한다. 과거 HEAD나 30fps 기본 옵션으로 되돌린 것을 C0라고 부르지 않는다.

```text
?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1
```

최신 성능 표본은 Chromium 151.0.7922.34 / Radeon 780M / ANGLE D3D11 / DPI 192(200%) / **165Hz**, Release non-AOT다. 다음 실행에서 환경을 다시 기록하며, 60Hz 결과로 해석하지 않는다.

| 최신 C0 조건 | notification p95 | interval p95 | exact settle | 첫 front | active age p95 |
| --- | ---: | ---: | ---: | ---: | ---: |
| TopLeft / 150ms / reverse | 45.2ms | 23.2ms | 20.3ms | 40.45ms | 50.00ms |
| Left / 150ms / reverse | 61.0ms | 34.2ms | 38.5ms | 35.57ms | 67.00ms |
| Bottom / 600ms / expand | 41.3ms | 27.0ms | 24.4ms | 32.27ms | 45.90ms |

- 세 조건 모두 notification p95 목표 33.3ms 미달이다. Left의 개선이 가장 작았고, 과거 기본 옵션 대비 active age·settle이 나빠졌다. 첫 front도 세 조건 모두 소폭 늦어졌다.
- F3 WGC 캡처는 수집·decode가 정상이었지만 상대 boundary gap·center 기준은 FAIL이다. 중간 작은 이전 프레임과 오른쪽·아래 공백이 관찰됐으며 최종 크기만 정상이라는 이유로 통과시키지 않는다.
- 기능 회귀 19개, managed wire 계약, TypeScript는 이전 실행에서 PASS였다. latency와 capturedPresentation은 FAIL, 전체 수용은 PARTIAL / notQualified다.
- 이전 2부 native 10회는 실패한 capture 1회를 포함하여 이미 소진됐다. 이 문서 작성으로 이전 예산을 다시 열지 않는다.

## 2. 확인한 사실과 아직 가설인 부분

### 2.1 UI 동기 작업을 먼저 보되, 기존 trace 밖의 비용도 포함한다

이전 B1은 TopLeft 한 번의 active-start callback 7개다. 중앙값은 UI frame 19.2ms, build 5.6ms, layout+compositing 3.6ms, paint 2.5ms, scene 6.3ms였다. scene 안에 map 1.2ms와 encode 3.2ms가 포함된다. 포함 관계가 있는 중앙값을 더하지 않는다.

이 수치는 **B2 중복 UTF-8 변환 제거 전**의 진단이다. 현재 C0의 정확한 phase 비용이나 Left의 병목으로 그대로 인용할 수 없다. B3는 공통 encoder 수정 후 옵션 조합 비교였으므로 UTF-8 수정 자체의 개선량도 아직 분리되지 않았다.

`scheduleManagedFrame()`은 `flushPendingResize()` 뒤에 `frame-start`를 기록한다. `applyResizeMessage()`에서는 resize epoch 전달, snapshot 구성·직렬화·managed 전달이 일어난다. 또한 input·focus·lifecycle 등 다른 메시지를 처리하기 전에도 pending resize를 flush한다. 따라서 frame 내부 시간만으로 UI Worker 점유 전체를 설명할 수 없다. 이 경로가 실제로 비싼지, 같은 metrics 알림이 중복되는지는 추가 계측으로 확인해야 한다.

UI send→Raster receive 중앙값은 0.1ms였고 Raster terminal→UI 처리 대기는 17.5ms였다. 후자를 메시지 복사·전송 17.5ms로 해석하지 않는다. UI 동기 작업과 ACK 처리 대기를 나눠야 한다. 해당 7개 callback의 paragraph interop 표본 0과 active semantics 독립 표본 없음도 텍스트·semantics 비용 전체가 0이라는 뜻은 아니다.

### 2.2 이미 존재하는 최적화를 중복 구현하지 않는다

- `BuildScope._scheduleBuildFor()`에는 `_inDirtyList` 중복 방지가 있다. `_flushDirtyElements()`의 정렬·재정렬과 실제 rebuild 수, dirty 원인을 조사한다.
- `RenderObject.layout()`에는 `!_needsLayout && object.Equals(constraints, _constraints)` fast path가 있다. 같은 constraints의 재layout이 보이면 먼저 누가 왜 dirty로 만들었는지 확인한다.
- MediaQuery에는 aspect별 dependent 알림과 `_updateData()` equality 검사가 있다. 창 크기가 바뀔 때 모든 subtree가 무조건 rebuild된다고 가정하지 않는다.
- `MediaQuery.updateShouldNotifyDependent()`의 `HashSet<object>(dependencies.Cast<object>())` 생성과 `MediaQueryData`의 값 비교·목록 복사는 조사 후보다. 호출 수와 비용이 확인되기 전에는 주요 원인으로 확정하지 않는다.
- 기존 picture mapping/encoding cache가 있다. 과거 F3 picture 재사용은 대표적으로 1/29였으므로 cache 규모 확대만으로 큰 효과를 기대할 근거가 약하다.

### 2.3 우선순위

| 순서 | 개선 후보 | 착수 근거 | 구현 선택 조건 |
| --- | --- | --- | --- |
| 1 | resize 적용·metrics 알림·MediaQuery 비용 감소 | frame 앞 계측 공백, build 비용, Left 미진단 | 반복 알림·동등 값 재생성·불필요한 dependent 갱신의 실측 근거 |
| 2 | dirty 전파·layout·paint 작업량 감소 | UI 동기 구간과 ACK 대기 | 필요한 반응형 작업과 불필요한 재실행을 구분하는 최소 재현 |
| 3 | scene mapping·encoding 할당 감소 | 과거 scene 비용과 낮은 cache hit | 현재 UTF-8 수정 후에도 남은 비용 및 유효한 재사용 수명 확인 |
| 조건부 | 첫 front·종료 frame scheduling 순서 개선 | 첫 front 지연 및 Left settle 퇴행 | CPU 작업 감소 후에도 독립적인 queue/dispatch 대기가 지배적일 때 |

단일 Worker, transport credit 2, retained wire registry, AOT, 새 renderer는 이번 라운드에서 확대하지 않는다. 고정 FPS 제한, 데모 단순화, 필요한 layout 생략, CSS stretch로 지연을 가리는 변경은 개선안으로 채택하지 않는다.

## 3. 반드시 유지할 동작

- main은 DOM/input/IME/semantics/logical CSS, UI Worker는 .NET/layout, Raster Worker는 visible OffscreenCanvas/WebGL2를 소유한다. 경계에는 versioned immutable 메시지와 transferable만 전달한다.
- 기존 epoch·surface generation 검증, pending 최신값 처리, input/focus/IME/lifecycle 앞 metrics barrier, transfer terminal·resource 수명·queue 상한을 유지한다.
- 화면 크기·DPR·insets·text scale·접근성 값 변화는 필요한 dependent와 layout에 전달한다. 동일 크기라는 이유로 나머지 metrics를 버리지 않는다.
- 반전·이동 origin·첫/끝 frame·줄바꿈의 정확성을 유지한다. 정지 후 최종 크기만 맞추는 방식은 통과하지 않는다.
- 기본 `auto=document-webgl`, Windows/ANGLE/Vulkan 경로는 그대로 둔다. C1 검증 성공도 즉시 기본값 승격을 뜻하지 않는다.

## 4. 실행 단계

### N0. C0 보존과 비교 조건 고정 — native 0회

- [ ] 현재 tracked diff와 관련 untracked 파일을 함께 보존하고 source identity, 선택 URL, 빌드 모드, 실제 served inventory를 기록한다. 기존 dirty 수정은 reset·정리·덮어쓰기하지 않는다.
- [ ] C0의 재현 가능한 source/publish checkpoint를 확보한다. 현재 publish 경로는 `.doroti/publish/web-Publish-Release/wwwroot`지만 오래된 fingerprint 파일이 남을 수 있으므로 실제 boot manifest와 응답 파일 hash로 확인한다.
- [ ] C1과 C0 비교 방법을 고정한다. 안전한 국소 opt-in이면 동일 binary에서 옵션만 비교하고, 그렇지 않으면 별도 publish 디렉터리와 명시적 serve root를 사용한다. wrapper가 이를 지원하는지 먼저 확인·보완한다.
- [ ] 이후 source가 바뀌면 실제 non-AOT publish를 갱신한다. `-SkipBuild`는 source와 해당 served binary 일치가 확인된 경우에만 사용한다.
- [ ] 조건·순서·측정 포함 관계·판정 기준·실행 횟수 ledger를 후보 결과를 보기 전에 저장한다. 과거 30fps 옵션은 참고 자료이며 새 개선율의 분모는 C0다.

완료 조건: 기존 선호 구현으로 돌아갈 수 있고 각 측정의 앱·드라이버·harness identity를 구분할 수 있다.

### N1. 현재 비용과 dirty 원인 계측 — native 최대 1회

- [ ] 기존 `RecordedAtMicroseconds`, stage trace, scene sequence를 확장하여 resize generation→apply→managed metrics→build/layout/paint/scene→Raster→terminal 처리 관계를 연결한다.
- [ ] `flushPendingResize`/snapshot 직렬화/managed 적용의 시간, rAF·dispatch 대기, frame 내부 시간을 분리한다. listener barrier에서 적용된 resize도 기록하며 같은 비용을 frame 안팎에 중복 합산하지 않는다.
- [ ] build에는 enqueue 시도·중복 제거·실제 rebuild·dirty 원인·MediaQuery aspect 알림 수를, layout에는 진입·fast path·실제 layout·dirty 원인을, paint에는 dirty/repaint boundary 및 새 picture 수를 집계한다.
- [ ] 먼저 bounded counter를 쓰고 필요한 소수 대상만 상세 timing을 켠다. 매 node stack/string 생성, 무제한 로그, tree 객체의 frame 간 보관은 피한다. 할당/GC 계측은 현재 WASM 환경에서 지원과 의미를 확인한 경우에만 사용한다.
- [ ] native 없이 동일 metrics, width-only, height-only, aspect별 구독, F3 정지 크기 전환으로 계측 연결과 overhead를 확인한다. 이 결과를 native 드래그 체감 증거로 사용하지 않는다.
- [ ] C0의 **Left / 150ms / reverse**를 trace on / capture off로 최대 한 번 진단한다. 기존 TopLeft 7개 표본은 참고로 보존하고 현재 Left 측정과 동일 corpus처럼 합치지 않는다.

완료 조건: 상위 비용과 불필요한 작업의 원인을 callback/resize 단위로 설명할 수 있다. 계측으로 비용이 크게 바뀌거나 유효 표본이 부족하면 원인을 기록하고 추측에 기반한 C1을 만들지 않는다.

### N2. 근거가 있는 단일 변경 구현 — native 0회

- [ ] N1 결과로 2.3의 후보 중 **한 가지 원인**을 선택하고 예상 절감 구간·불변 조건·반증 조건을 짧게 기록한다. 여러 옵션을 묶어 효과를 비교하지 않는다.
- [ ] metrics/equality 문제라면 동일 값·독립 aspect·새 목록의 동등 내용 등을 최소 재현으로 구분한다. 알림 제거 전 필요한 resize·DPR·insets·lifecycle 전달 계약을 검증한다.
- [ ] dirty 문제라면 최초 invalidation부터 rebuild/layout/paint까지 추적한다. constraints equality, `parentUsesSize`, relayout boundary, flush 도중 다시 dirty가 되는 동작을 보존한다.
- [ ] scene 문제가 선택되면 현재 mapper/encoder의 실제 allocation·재사용을 확인한다. immutable snapshot 수명과 cache 상한·dispose/restart를 유지하고 mutable Paint/paragraph/resource를 전역 캐시하지 않는다.
- [ ] framework 수정은 reviewed source와 pinned Flutter 원본을 대조한다. [Flutter MediaQuery](reference/flutter-master/packages/flutter/lib/src/widgets/media_query.dart)와 해당 framework/rendering 원본을 사용한다. 생성 문제라면 [lowerer](tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Members.cs)·port 소유권을 확인하고 최소 회귀를 함께 수정한다. compiler-owned generated base를 직접 고치지 않는다.
- [ ] 동일한 비native fixture를 C0/C1에서 비교하여 작업 수와 해당 phase 감소를 확인한다. 진단용 계측은 performance 실행에서 끄며 C0에도 같은 harness를 적용한다.

완료 조건: 동작 보존과 국소 비용 감소에 근거가 있는 C1 하나. 감소가 없거나 문제를 단지 다른 phase로 옮겼다면 새 변경만 철회하고 C0를 유지한다.

### N3. 필요한 correctness 회귀 — native 0회

- [ ] metrics/dirty 변경에는 동일 값 반복, width-only/height-only, DPR/insets/text scale/aspect 구독, dirty-during-flush 및 boundary 관련 최소 회귀를 변경 범위에 맞게 추가한다. 구현 줄을 그대로 복제하는 테스트는 만들지 않는다.
- [ ] F3 실제 responsive text의 wrap 임계점을 fixture에서 먼저 찾고 양쪽 정지 크기에서 행 수·bounds·clipping을 확인한다. 기존 고정 폭 ImageFilter 문구 2줄을 dynamic wrap 검증으로 쓰지 않는다.
- [ ] 변경 경로의 browser golden, direct/cache/bitmap과 resize/restart, transfer/resource 계약을 필요한 범위로 실행한다. encoder를 바꾸면 strict Unicode 거부·canonical UTF-8 순서·round-trip·기존 wire hash를 확인한다.
- [ ] scheduling/metrics 변경이면 main/UI stall, maximize/restore, 실제 hidden→visible, warm input·synthetic composition을 확인한다. 기존 lifecycle harness의 Playwright focus emulation 보정을 유지한다.
- [ ] 공통 framework 변경이면 기본 document 경로와 관련 소비 경로의 최소 회귀를 포함한다. synthetic composition PASS는 실제 OS IME PASS로 확대하지 않는다.
- [ ] build/test는 모두 **20분 timeout**을 적용한다([repository 지침](.github/copilot-instructions.md)). 변경·실패·새 우려가 없으면 이미 통과한 검사를 반복하지 않는다.

완료 조건: C1 관련 correctness PASS. 실패하면 수정·필요 회귀까지 해결한 뒤 진행하고, 해결되지 않은 상태로 성능 측정을 소비하지 않는다.

### N4. C0/C1 최소 성능 비교 — native 최대 6회

trace/capture off, 동일 warm-up·창 크기·화면·DPR·주사율·실제 native 자극을 사용한다. 빌드·무거운 기능 검사·PNG 분석을 겹치지 않는다.

| 순서 | 조건 | 실행 |
| --- | --- | --- |
| 1 | Left / 150ms / reverse | C0→C1 각 1회 |
| 2 | TopLeft / 150ms / reverse | C1→C0 각 1회 |
| 3 | Bottom / 600ms / expand | C0→C1 각 1회 |

- [ ] 먼저 Left pair를 평가한다. 큰 퇴행·자극 오류·잘못된 binary이면 중단하고 원인을 정리한다. 남은 횟수를 채우기 위해 계속하지 않는다.
- [ ] notification p95/p99/max, interval, 첫 front, 첫/끝 포함 gap, exact settle, 미도달, active age, 시간 가중 geometry를 조건별로 보고한다. idle age와 active age를 분리한다.
- [ ] 절대 목표는 notification p95·interval ≤33.3ms, exact settle ≤50ms, active >100ms gap 0이다. correctness·자극·geometry coverage도 함께 판정한다.
- [ ] C0 대비 각 지표의 증가·감소를 숨기지 않는다. p95가 줄어도 첫 front·age·settle·geometry가 나빠지면 일괄 개선 PASS를 부여하지 않는다. 표본/clock 해상도로 구분하기 어려운 차이는 불확실로 남긴다.
- [ ] 조건당 한 pair로 안정적 20% 개선이나 통계적 우월성을 주장하지 않는다. 더 많은 반복이 필요하면 이번 결과를 partial로 종료하고 별도 후속 범위로 기록한다.

완료 조건: C1을 보존할 근거 또는 철회할 근거가 있다. 프로세스 exit code와 latency PASS/FAIL을 별도로 읽는다.

### N5. 실제 캡처의 추종 품질 확인 — native 최대 2회

N4에서 유지 가치가 있는 경우에만 F3 **TopLeft / 150ms / reverse**, C0/C1 각 한 번을 동일 WGC endpoint로 비교한다. 이는 이전에 gap·center FAIL과 공백이 보인 조건이다. Left 캡처와 Flutter F3 비교를 추가하여 예산을 확대하지 않는다.

- [ ] 자극 전 정지 화면에서 번역 popup·가림·초기 font/layout 안정화를 확인한다. `--disable-features=Translate`만으로 popup이 없다고 간주하지 않는다. 준비가 안 되면 드래그 전에 중단한다.
- [ ] capture-only에서 Windows grid/shape oracle가 꺼져 있고 ring/encoder/capture drop·error가 0인지 확인한다. 실패 실행도 ledger와 원시 자료에 남긴다.
- [ ] 마지막 pre-motion decoded frame으로 chrome/client 좌표를 교정한다. 모든 pre-motion generation을 새 generation 집계에서 제외하고 전체 모니터 PNG 크기를 client 크기로 쓰지 않는다.
- [ ] 기존 marker 기반 boundary gap·origin·width/height·기하학적 center를 유지하면서, 실제 content landmark 위치·노출 공백의 폭/지속·텍스트 bounds를 별도로 확인한다. marker에서 계산한 center를 독립 content 측정이라고 쓰지 않는다.
- [ ] candidate 캡처 전에 상대 gate를 고정한다. gap 증가 허용은 실제 display interval 1개, width/height 오차 증가는 실제 자극 속도×interval, 기하학적 center는 그 절반이라는 기존 정의를 사용한다. 현재 165Hz/동일 자극이면 각각 약 6.06ms/48.48px/24.24px이며 환경·단위·속도를 확인 없이 복사하지 않는다.
- [ ] active marker 전체 decode, 새 generation ≥2, 수집 손실 0, gap·geometry 상대 gate를 각각 판정한다. 공백·content 추종이 악화하면 숫자 일부가 PASS여도 전체 capturedPresentation PASS로 쓰지 않는다.
- [ ] N3에서 확인한 wrap 임계점을 실제 자극이 통과했는지 기록한다. 통과하지 않으면 dynamic wrap-in-motion은 notVerified다. capture-on latency는 N4 성능 corpus와 합치지 않는다.

완료 조건: 캡처 기준 개선/퇴행과 관측 한계가 명시된다. WGC callback rect의 표본 시각과 실제 표시 시각은 같다고 가정하지 않으며, 물리 scan-out은 미검증으로 유지한다.

### N6. 결과·채택 범위·종료 기록 — native 0회

- [ ] 다음 구현 라운드의 source/publish identity, 변경 이유, 회귀 결과, 조건별 C0/C1 표, PNG 근거, 실패와 미검증 항목을 별도 `history/<실행일>/` 보고서에 남긴다.
- [ ] correctness / stimulus / latency / capturedPresentation / manualAcceptance를 각각 판정한다. 목표 미달이면 `PARTIAL / notQualified`를 유지한다.
- [ ] C1이 퇴행하거나 근거가 불충분하면 C0를 유지하고 이번 새 변경만 선택적으로 되돌린다. 기존 dirty 작업과 실패 자료는 보존한다.
- [ ] 유지할 후보가 있으면 사용자 직접 Right/Bottom/Left/TopLeft·빠른 반전·줄바꿈 체감 확인 항목을 안내한다. 이미 받은 ‘현재 구현이 가장 좋다’는 피드백을 새 C1 수용으로 대체하지 않는다.
- [ ] 소유 서버·브라우저·capture driver를 종료하고 잔류 여부를 기록한다. 사용자 프로세스는 종료하지 않는다.

## 5. 예산과 중단 조건

향후 별도로 실행하는 한 라운드의 native 상한은 **총 10회**다: N1 진단 1 + N4 성능 6 + N5 캡처 2 + 자극/수집 실패 대응 예비 1. 최대치이며 모두 실행할 의무는 없다. 실패한 native 자극도 포함한다. 자극 전 setup 실패는 별도 기록하되 반복을 무제한 허용하지 않는다.

- 기존 matrix wrapper의 기본값은 여러 방향·시간·motion·3회 반복이므로 그대로 실행하지 않는다. 조건을 명시한 단일 실행으로 ledger를 갱신하고 hard gate 실패 시 남은 자동 실행을 멈춘다.
- 예비는 실패 원인을 해결한 경우에만 사용한다. 정상 결과를 좋게 만들기 위한 반복, 추가 방향, 옵션 탐색에 사용하지 않는다.
- 10회에 도달하면 남은 항목은 미실행으로 종료한다. 부족한 증거를 PASS로 보정하지 않는다.
- 병목이 확인되지 않거나 단일 변경의 이득이 없으면 N1/N2에서 종료할 수 있다. 이 경우에도 계측 결과와 기각 이유를 산출물로 남긴다.
- 60Hz, monitor 이동, 실제 OS IME, 물리 scan-out, Flutter F3 대응 비교는 이번 자동 라운드의 필수 완료 범위에 넣지 않는다. 전체 플랫폼 qualification은 별도 범위다.

## 6. 이번 문서 작성의 완료 범위

- [x] 현재 소스·최신 2부 결과·과거 실패를 검토했다.
- [x] 사용자 선호 C0 보존, 개선 우선순위, 계측 공백, 단일 변경 선택 기준을 정했다.
- [x] 단계별 선행 조건, 회귀·성능·캡처 판정, 최대 10회 예산과 중단 조건을 작성했다.
- [ ] N0~N6 구현·실행 — 이번 요청에서는 진행하지 않음.

문서 검증은 경로와 diff 확인으로 한정한다. 이 계획의 체크 완료를 제품 성능·화면 품질 개선 완료로 해석하지 않는다.

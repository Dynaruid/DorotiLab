# Web SkiaSharp direct 기본 전환 및 갱신 시작·실시간 resize 개선 계획

- 작성일: 2026-09-07
- 상태: **검토·계획 작성 완료 / 구현 미시작**
- 저장소: `C:\Users\parti\Labo\DorotiLab`
- 검토 HEAD: `207010a48eb7040bed4475ace53c5363d819de6f` + 현재 미커밋 변경 전체.
- 이번 요청의 산출물은 이 문서다. 기본값·런타임 코드·기존 `work.md`는 이번 계획 작성에서 변경하지 않는다.
- 실행 시 기존 변경과 증거를 보존하고, 실제 시작 시점의 소스/차이를 다시 확인한다. HEAD만으로 현재 동작을 재현할 수 없다.

## 1. 목표와 범위

사용자는 동일 Material 샘플에서 **SkiaSharp direct가 훨씬 부드럽다**고 직접 확인했다.
이 판단을 전환 방향으로 채택하여 다음 세 작업을 수행한다.

1. Web 기본 렌더러를 `worker-direct-webgl`로 전환한다.
2. 브라우저 창을 움직여 크기를 바꾸는 **도중에도** 현재 viewport에 맞는 새 레이아웃이 계속 표시되도록 한다.
3. 정지 상태에서 스크롤/애니메이션을 시작할 때, 그리고 스크롤로 새 구간에 진입할 때 발생하는 큰 초기 지연을 줄인다.

공용 host·scheduler·Skia renderer에서 원인을 수정한다. 샘플은 재현과 회귀 검증에 사용한다.
CanvasKit은 명시적 URL 선택으로 유지한다. CanvasKit 패키지 제거, 다른 OS의 기본 GPU 변경,
대규모 Worker 분리 재설계, AOT/threads 도입은 이번 기본 범위에 포함하지 않는다.
공용 Skia 코드 변경이 Windows 등에도 영향을 줄 수 있으므로 해당 변경에 맞는 회귀 검증은 포함한다.

## 2. 현재 확인된 근거와 미확인 사항

| 항목 | 현재 판정 | 근거 / 의미 |
| --- | --- | --- |
| direct의 체감 개선 | 사용자 확인 | 이번 전환 방향의 근거. 모든 화면·입력·주사율의 성능 검증 완료를 뜻하지 않는다. |
| live resize 추종과 시작 지연 | 사용자 문제 보고 | 현재도 해결되지 않은 작업으로 기록한다. |
| Web 기본값 | 소스 확인 | `doroti.loader.ts`의 생략/auto/미인식 선택과 target manifest는 아직 CanvasKit이다. |
| direct 구조 | 소스 확인 | 하나의 .NET Worker가 framework UI/layout/paint와 Skia/WebGL2 raster를 함께 실행한다. CanvasKit DisplayList 변환·전송 경로는 거치지 않는다. |
| 기존 진행표시기 비교 | warm 자동 측정만 완료 | 약 6초: CanvasKit/direct 359/360 제출, Worker 제출 간격 p95 19.9/18.5ms, 최대 33.9/23.9ms. 한 번씩 측정했으며 실제 표시 FPS가 아니다. |
| 최초 애니메이션 프레임 | 기존 측정에서 제외 | `measure-material-sample.mjs --progress`는 시작 버튼 클릭 후 **3초 대기 뒤** 측정한다. `--cold`도 이 3초를 제거하지 않는다. |
| 신규 구간 생성 | direct 원인·수치 미확인 | 이전 CanvasKit sweep의 최대 723.8ms, managed layout/compositing 668.5ms는 조사 단서이며 direct 측정값으로 재사용하지 않는다. |
| 이전 direct 수정 | 현재 소스에 존재 | sample 모드를 시작/재시작 모두 전달하고, 진단 DOM 기록을 100ms로 묶는 수정이 이미 있다. 재구현하지 않는다. |
| 이전 direct 검증 | 제한된 자동 PASS | 진행표시기 픽셀 변화/정지, Worker 재시작 후 sample 유지. 초기 재시작 테스트의 viewport 선택자 FAIL과 수정 후 PASS는 이력에 보존되어 있다. |

참조:

- [최근 샘플 렌더링 조사와 자동 증거](history/26-09-07/web-sample-retained-rendering.md)
- [과거 direct 구현/resize 기록](history/26-08-30/web-worker-direct-renderer-summary.md)
- [이전 샘플 작업 요약·원문 보관](history/26-09-07/material-sample-work-summary.md)

과거 direct 기록에는 176.5ms gate 실패, 이후 제한된 빠른 resize 개선, 미완료 물리 검증이 함께 있다.
그 당시 `auto=document-webgl` 정책은 현재 정책이 아니다. 과거 gate를 소급 PASS로 바꾸거나,
사용자가 이번에 정한 direct 전환 방향을 과거 정책으로 취소하지 않는다.
**이번 검토에서는 새 브라우저 재현·성능 측정을 실행하지 않았다. 아래 원인 후보는 소스 검토 결과다.**

## 3. 코드 검토: 우선 추적할 경계

### 3.1 기본값과 선택 정책

- `Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts`: `selectRendererMode()`의 기본 반환값.
- `Doroti/src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json`: `defaultRenderer`.
- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`: 기존 `presenterPolicy()`, Worker bootstrap, diagnostics의 requested/selected 값 일관성.
  현재 Worker presenter diagnostics의 `requestedMode`는 `offscreen-worker`로 고정되어 있어 direct 선택 표시도 점검 대상이다.
- `Doroti/validation/web-playwright/tests/default-renderer.spec.ts`: 생략/auto를 CanvasKit으로 기대한다.
- `Doroti/docs/adr/ADR-020-web-typescript-bootstrap.md`, 앱 한국어/영문 README, 실행/검증 문서와 실제 template/package 소비 경로를 검색한다.
- `run-web-renderer-ab.ps1`는 아직 document/direct 비교와 `selectedAutoMode=document-webgl`을 기록한다.
  현재 기본값을 검증한 것처럼 사용할 수 없다. 기본 정책과 비교 대상/실험 라벨을 명시하도록 정리한다.
- `run-web-playwright.ps1 -FastResize`는 CanvasKit 전용이다. 기본값 문자열만 바꿔 direct 검증으로 간주하지 않는다.
- `DorotiWebWorkerRunner.cs`의 CanvasKit identity 분기는 backend 초기화 분기다. default 문자열 변경과 혼동하여 일괄 치환하지 않는다.

### 3.2 resize: viewport, CSS, GPU capacity의 일관성

현재 경로:

`ResizeObserver / visualViewport resize → commitObservedResize → typed admission-target →
adoptAdmissionResizeEpoch → BrowserHostAdapter.ApplyResizeEpoch → framework frame →
SkiaSceneRenderer.PaintCore → DorotiWebWorkerSurface.RenderFrame → direct-commit`.

확인된 구조 및 원인 후보:

1. `commitObservedResize()`는 `commitDirectCanvasLogicalSize()`로 canvas CSS를 viewport 크기로 쓴다.
   반면 초기화와 `direct-commit`에서는 `configureDirectCanvasCapacity()`가 CSS를 **capacity / DPR**로 쓴다.
   두 함수가 같은 CSS 크기를 다른 기준으로 갱신한다. grow-only backing과 `object-fit: cover`가 결합되므로
   프레임 전후의 실제 배율·clip·빈 영역을 검사해야 한다. 이 충돌이 사용자 증상의 주원인인지는 아직 미확인이다.
2. main→Worker에는 snapshot 한 개 + latest, typed admission 최대 4개 + latest가 이미 있다.
   ACK 대기, 중복 snapshot/typed metrics, Worker 이벤트 루프 점유가 최신 크기 도착을 막는지 분리 측정한다.
3. Worker rAF와 resize 때 최대 2회의 조기 task wake가 이미 있다.
   native resize 중 rAF 지연인지, 긴 managed 작업인지 확인한다. 무제한 timer나 frame 요청 추가는 하지 않는다.
4. `SkiaSceneRenderer.PaintCore()`는 현재 epoch와 정확히 맞지 않는 scene을 거부한다.
   최신 크기가 계속 바뀔 때 superseded만 반복되어 완료 프레임이 굶는지 확인한다.
   검사 해제로 오래된 scene을 최신 크기라고 보고하는 수정은 허용하지 않는다.
5. Worker backing은 capacity 초과 때 1.5배로 증가하고 `EnsureSurface()`는 크기/문맥 변경 시 surface를 재생성한다.
   capacity 내 resize와 capacity 증가·DPR 변경을 별도로 측정하여 allocation/context reset/flush 지연을 구분한다.
6. `CoalesceGeometryDuringActiveMetrics`는 semantics geometry 정책이다.
   이를 화면 layout 전체를 지연시키는 debounce로 오인하여 제거하지 않는다.

### 3.3 시작 지연: 첫 입력부터 첫 변경 프레임까지

- 단일 Worker에서 framework와 raster가 직렬 실행되므로 긴 레이아웃·캐시 생성 중 새 input/resize 메시지도 대기할 수 있다.
- `SkiaSceneRenderer.DrawPictureLayer()`는 **두 번째 사용부터** GPU surface 생성, picture replay, flush,
  snapshot을 동기 수행한다. 여러 picture가 한 프레임에서 동시에 승격되면 시작 몇 프레임에 비용이 몰릴 수 있다.
- 현재 raster cache는 24 entries/16M pixels, 개별 4M pixels 제한이다. 크기·선형 transform 변화는 재생성을 유발하고
  translation은 key에서 제외된다. 동적 picture는 `WillChangeHint`로 제외한다. 이 기존 의미를 보존한다.
- `_pictureRasterWarmups`는 성공적으로 cache를 만들면 제거되지만, 한 번만 사용하고 버려진 key는
  전체 clear까지 남을 수 있다. metadata의 수명/상한도 조사한다.
- `GetTextRenderResources()`는 크기·색상·font 등으로 key를 만들며 dictionary에 저장한다.
  `Layout()`은 run 측정과 paragraph layout을 수행한다. 첫 font/fallback/glyph 생성, 연속 크기·색상 변화에 따른
  cache 증가, 측정 중복이 실제 긴 프레임과 상관있는지 계측한다. unbounded cache 확대는 해결책으로 사용하지 않는다.
- 샘플은 29개 section을 lazy 생성하고 방문한 section을 keep-alive한다.
  이미 방문한 구간에서의 재시작과 새 구간 첫 생성은 다른 workload다. shared rebuild/layout/paint invalidation,
  paragraph 측정, image decode/upload, runtime effect 생성, 관리 힙 allocation/GC를 각각 확인한다.

## 4. 단계별 실행 계획

### P0. direct 기준선과 재현 계측

- [ ] 현재 dirty 상태와 실행 asset/build identity, renderer/GPU, viewport/DPR/zoom, 브라우저·OS·주사율을 기록한다.
- [ ] 명시적 direct URL로 먼저 측정하여 default 변경 전후의 선택 혼동을 막는다.
- [ ] 기존 계측에 direct의 input/resize 수신, frame 요청/시작/끝, build/layout/paint/semantics,
  cache 승격/eviction, paragraph/font, surface 생성/flush, Worker commit을 연결한다.
  input sequence·resize generation·framework frame·request ID로 연결하고 모든 Worker 시간은 같은 epoch 기준으로 환산한다.
- [ ] 짧은 bounded trace와 누적 counter를 사용한다. synchronous GPU query, 매 이벤트 전체 JSON/DOM 기록을 추가하지 않는다.
  진단 OFF 대조군도 유지하여 관측 비용을 확인한다.
- [ ] 측정 시작을 버튼 클릭/첫 wheel **이전**으로 옮긴 onset 모드를 추가한다.
  첫 변경 프레임, 첫 100/500/1000ms, 이후 5초를 각각 보고한다. warm-up 결과로 onset을 대체하지 않는다.
- [ ] cold load 후 첫 조작, 5초 idle 후 재시작, 방문 구간 재스크롤, 새 section 진입을 분리한다.
- [ ] resize는 정지 화면/진행표시기 실행 중, 양방향 빠른 resize/느린 resize/방향 반전,
  breakpoint 횡단, capacity 내/초과, DPR 1/2, page zoom을 재현한다.
- [ ] 각 시나리오 최소 3회. FPS만 집계하지 않고 첫 반응 지연·긴 프레임 수·content age·geometry 오차를 보존한다.

완료 조건: 두 증상을 재현하는 실패 증거와 비용 상위 단계가 있고, 자동 재현 불가 항목은 `notVerified`로 남긴다.
계측으로 확인되지 않은 원인을 확정하지 않는다.

### P1. SkiaSharp direct를 Web 기본값으로 전환

- [ ] loader와 manifest의 기본값을 `worker-direct-webgl`로 일치시킨다.
  생략/`auto`/미인식 값의 현재 선택 규칙은 유지하면서 결과만 direct로 변경한다.
- [ ] 명시적 `worker-canvaskit-webgl`, direct 및 기존 backend 선택은 보존한다.
  direct 초기화 실패 시 다른 backend로 조용히 넘어가지 않고 실제 실패를 표시한다.
- [ ] 기본 선택 테스트를 direct의 ownership/main .NET 0·Worker .NET 1·visible Offscreen WebGL2,
  first content, diagnostics identity 기준으로 갱신한다. 생략/auto/오타/각 명시적 override를 검증한다.
- [ ] sample/diagnostics, reload, runtime replacement, source 실행 및 publish/package 소비 앱에서도 선택이 일치하는지 확인한다.
- [ ] README/ADR/활성 실행 문서와 기본 검증 경로를 갱신한다. 과거 결과·CanvasKit 전용 시험은 이름과 범위를 유지한다.

완료 조건: 사용자가 쓰는 `?dorotiTestbedMode=sample`이 실제 direct로 시작한다.
전환은 이번 사용자 결정으로 진행하며, P2/P3 미완료를 성능 문제 해결로 보고하지 않는다.

### P2. live resize 경로 수정

- [ ] P0 증거에 따라 CSS 크기 소유 규칙을 통일한다. 우선 후보는 root가 viewport를 clip하고
  canvas가 capacity/DPR 배율을 유지하는 방식이다. 관측 viewport와 완료 프레임의 크기를 별도로 보존한다.
  CSS 확대·축소만으로 새 viewport 렌더링을 대신하거나 물리 backing을 main에서 변경하지 않는다.
- [ ] typed admission과 snapshot의 generation 역행/중복을 제거하고 최신 metrics를 framework frame 시작 전에 반영한다.
  transport window와 scene current/latest는 각각 상한을 유지한다.
- [ ] 실제 rAF throttling이 확인된 경우에만 제한된 wake 정책을 조정한다. idle 이후에는 정상 vsync·무작업 상태로 복귀한다.
- [ ] superseded 연속 발생 시 원래 scene 크기/epoch를 보존하는 완료 정책을 검토한다.
  frame descriptor를 바꿔 exact로 위장하지 않고, active resize 동안 새 layout 결과가 주기적으로 표시되게 한다.
- [ ] allocation이 주원인일 경우 capacity와 surface 재사용을 수정한다. grow-only 용량의 최대치·DPR 전환·문맥 재생성·자원 해제를 함께 검증한다.

완료 조건: resize 도중의 프레임 갱신·최신 크기 추종과 종료 후 exact가 모두 통과한다.
끝난 뒤 한 번 맞아지는 결과만으로 PASS하지 않는다.

### P3. 스크롤·애니메이션 시작 비용 분산 및 무효화 수정

- [ ] cache 승격이 원인이라면 frame별 생성 수/픽셀/시간 예산을 두고 제한한다.
  예산 소진 시 정상 picture replay로 현재 프레임을 그리며, 이후 안정된 프레임에 승격한다.
  기존 warm-up 횟수만 올려 동일한 비용을 몇 프레임 뒤로 옮기는 방식은 채택하지 않는다.
- [ ] cache key의 내용·DPR·scale·font/image generation 무효화와 translation 재사용을 검증한다.
  일회성 warm-up metadata를 정리하고 cache thrash/메모리 증가를 제한한다.
- [ ] 첫 입력에 불필요한 전역 rebuild/layout/paint가 발생하면 공용 dirty 전파와 retained layer 경계에서 수정한다.
  텍스트·이미지·shader 비용은 P0에서 확인한 항목만 수정하고 정확한 font/내용/폭 무효화 계약을 유지한다.
- [ ] 새 section 생성이 지배적이면 공용 sliver layout/측정 중복을 먼저 수정한다.
  추가 prefetch가 필요할 때만 제한된 ahead-of-viewport 준비를 검토한다.
  전체 샘플 선생성, 강제 장시간 warm-up, 초기 로딩으로 비용 전가를 완료 조건으로 삼지 않는다.
- [ ] state·focus·OverlayPortal·semantics를 보존하고 방문 구간 왕복/테마·폰트·이미지 변경에서 stale 화면을 방지한다.

완료 조건: onset 창에서 실제 긴 프레임과 첫 반응 지연이 감소하고 warm animation/idle/메모리가 회귀하지 않는다.
한 프레임의 blocking 작업 자체가 큰 경우 예산 검사만으로 해결했다고 표시하지 않는다.

### P4. 통합 검증과 문서 정리

- [ ] Release 빌드·TypeScript·변경한 scheduler/cache/resize 계약 검증. 모든 테스트 프로세스 timeout **20분**.
- [ ] 기본 URL과 명시적 direct의 동일 동작, CanvasKit override 시작, default-renderer/worker-protocol,
  progress pixel-change/stop, wheel, resize-continuity/pixel marker, DPR/zoom, context loss/restart 검증.
- [ ] Material sample의 breakpoint 전환, selection/keep-alive, 입력 초점·한글 IME·caret,
  테마/이미지/폰트·clip/overlay를 검증한다. 공용 Skia 변경에는 관련 native 계약과 host build를 추가한다.
- [ ] direct 전용 resize 측정에 기존 `resize-following` 분석을 연결한다.
  `-FastResize`의 CanvasKit 전용 검사를 통과했다고 direct PASS로 보고하지 않는다.
- [ ] P0와 같은 조건에서 최소 3회 재측정하고 각 run 원본/최악값/오류/누락 frame을 보존한다.
- [ ] 사용자의 실제 브라우저 창 drag와 첫 scroll/animation 체감을 확인한다.
  자동 증거와 사람 관찰을 별도 기록하며 관찰이 없으면 `notVerified`다.
- [ ] `history/26-09-07/`의 별도 실행 보고서에 최종 diff/명령/결과/남은 항목을 기록하고,
  한국어·영문 README의 기본/비교 URL과 알려진 제한을 최종 상태에 맞춘다.

## 5. 판정 기준

아래 수치는 이번 작업의 **60Hz 환경 목표**이며 현재 달성 수치가 아니다.
P0에서 측정 방식과 환경을 고정한다. 실패 후 통과를 위해 기준을 낮추지 않는다.
120Hz 등은 별도 refresh budget과 실제 환경으로 평가하며 60Hz 결과를 그대로 승격하지 않는다.

| 범위 | 완료 목표 |
| --- | --- |
| 기본값 | 옵션 없는 sample/diagnostics와 auto가 direct. 명시적 CanvasKit은 CanvasKit. restart/publish도 동일. |
| resize 연속성 | native drag 도중 boundary 포함 commit gap p95 ≤33.4ms, max <100ms. 100ms 초과 무갱신 구간 0. |
| resize 추종 | observer→해당 target 이상을 반영한 실제 새 scene p95 ≤50ms, max <100ms. 최종 target 후 exact ≤100ms. superseded/unreached target을 별도 집계. |
| resize geometry | 완료 scene과 CSS/backing/DPR 관계 일치, 왜곡·검은 band·좌표 어긋남 없음. 실제 viewport와 front 크기 오차/내용 age를 시간별 보고. |
| 시작 반응 | input→첫 변경 scene p95 ≤50ms, max <100ms. 각 시작 후 첫 1초의 50ms 초과 framework+raster task 0을 목표로 한다. |
| 지속 애니메이션 | commit 간격 p95 ≤20ms, max ≤33.4ms를 3회 측정. 픽셀 변화·정지 상태도 별도 확인. |
| cache/수명 | 명시된 entry/pixel/metadata 상한 준수. 반복 조작·resize/테마 변경 후 계속 증가하는 자원 없음. 짧은 성능 측정 외 10분 반복 검증. |
| correctness | 역행 front/잘못된 exact/미종결 request/복구 후 유실된 입력·sample 모드/새 런타임 오류 0. |
| 사용자 관찰 | 실제 resize 추종과 시작 지연 개선 확인. 자동 submit 수치를 화면 FPS 또는 물리 scan-out으로 표현하지 않음. |

평균 FPS만으로 판정하지 않는다. 첫 입력 이전부터 측정하고, 시작/종료 경계를 포함한 gap,
첫 변경 scene의 input sequence, 새 content인지 여부를 검사한다. 이전 picture를 반복 제출해 카운트를
늘려도 첫 반응·최신 크기 추종 개선으로 인정하지 않는다.

## 6. 실행 및 비교 주소

저장소 루트에서:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release
```

- 전환 검증: <http://127.0.0.1:5088/?dorotiTestbedMode=sample>
- direct 명시: <http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiRenderer=worker-direct-webgl>
- CanvasKit 대조: <http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiRenderer=worker-canvaskit-webgl>

실행 순서는 **P0 → P1 → P2 → P3 → P4**다. 기본 전환과 성능 해결 상태를 각각 보고한다.
이 문서 작성 시점에는 소스·기존 이력만 검토했으며, 위 구현/새 계측/자동·물리 검증은 모두 미실행이다.

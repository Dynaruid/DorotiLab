# Web interactive resize를 Flutter식 transaction으로 재구성하는 작업계획

## 0. 문서 목적과 작업 경계

- 작성일: 2026-08-24
- 대상: `Doroti.Host.Web`의 브라우저 창/컨테이너 resize, DPR/zoom 전환, WebGL context 복구
- 문서 성격: **검토 결과와 ordered 구현계획**이다. 이 문서 작성만으로 구현·브라우저 실행·visible acceptance를 수행한 것으로 간주하지 않는다.
- 최초 계획 작성 단계에서는 `work3.md`만 작성했고 구현은 시작하지 않았다. 이후 관련 상태 문서 갱신에서도 기존 `Program.cs`와 validator의 미커밋 변경은 보존한다.
- Flutter는 새 runtime build/capture 대상이 아니라 **resize protocol의 source-only reference**로 사용한다.
- active automated smoke는 기존 합의대로 **40 samples**만 사용한다.
- `work2.md`의 Web hard stop은 당시 Windows gate 상태를 기록한 것이다. 2026-08-24 사용자의 명시적 요청은 Web **검토와 계획 작성**을 재개한 것이며, 아직 Web 구현이나 milestone PASS를 승인·증명한 것은 아니다.

현재 결론은 다음과 같다.

> Web도 Windows와 마찬가지로 geometry 변경 자체보다 **현재 visible front, 다음 exact raster, visible commit의 소유권이 분산된 것**이 핵심 위험이다. 다만 Web에서는 브라우저 window geometry를 앱이 동기적으로 보류할 수 없으므로 Flutter의 bounded blocking wait를 복사하지 않는다. 대신 `ResizeObserver`에서 이전 GPU front를 즉시 1:1 복원하고, 하나의 rAF transaction이 latest epoch의 exact-content raster를 준비·검증·commit하도록 비동기 상태기계로 번역한다.

현재 Web 구현은 이미 retained front와 exact staging FBO를 갖고 있으므로 전면 교체 대상이 아니다. 우선순위는 다음 두 구조적 결함 후보를 제거하는 것이다.

1. `ensureStaging()`이 크기가 달라질 때 staging FBO/texture/renderbuffer를 폐기·재할당해 interactive resize 중 allocation/GPU stall을 만들 수 있다.
2. root resize sampling, managed frame request, `queueMicrotask` presenter drain, default-framebuffer commit이 여러 scheduler에 나뉘어 한 browser paint에 current/latest transaction이 하나만 존재한다는 보장이 약하다.

## 1. Windows에서 확인한 교훈과 Web 번역

| Windows에서 유효했던 원칙 | Web에 적용하는 형태 | 그대로 복사하지 않는 것 |
| --- | --- | --- |
| 동시에 보이는 visible surface는 하나만 둔다 | DOM visible canvas 하나와 default framebuffer 하나만 유지하되 hidden prepared slot은 허용 | 두 visible canvas를 동시에 표시하거나 CSS opacity/z-index로 독립 교대 |
| buffer capacity와 exact-content extent를 분리한다 | offscreen front/staging FBO는 grow-only capacity, 각 slot은 별도 exact content extent 보유 | canvas intrinsic backing을 큰 capacity로 고정해 CSS로 축소 표시 |
| 확대 전에 기존 raster가 새 영역을 덮을 cover를 준비한다 | `ResizeObserver` callback에서 target-sized default framebuffer를 만들고 이전 front overlap을 1:1 blit, 나머지는 theme background | 이전 전체 frame stretch, `SetSourceSize` 유사 CSS scale |
| 축소 target raster를 미리 준비하고 exact commit만 visible하게 한다 | rAF staging raster가 끝난 뒤 generation이 여전히 latest일 때만 exact rect를 default framebuffer에 blit | main thread를 100ms 동기 block, GPU 완료를 scan-out ACK로 해석 |
| current + latest만 유지한다 | presenter queue depth 최대 2, 중간 epoch는 `superseded` terminal | FIFO replay와 모든 ResizeObserver event raster |
| matching Present 뒤 platform wait를 풀고 flush는 presenter owner가 수행한다 | exact WebGL command submission 뒤 managed terminal을 완료하고 browser compositor 결과는 `browser-present-unverified`로 분리 | `gl.flush()`, Promise 완료, rAF return을 실제 display ACK로 기록 |

Web에서 canvas의 CSS display size와 intrinsic drawing-buffer size가 다르면 브라우저가 전체 bitmap을 scale할 수 있다. 따라서 grow-only capacity는 **offscreen FBO에만** 적용하고, visible canvas backing은 매 committed epoch의 exact physical size를 유지한다.

## 2. 현재 repository에서 확인한 사실

### 2.1 보존할 현재 구조

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
  - root `ResizeObserver`와 DPR watcher가 logical/physical size를 하나의 `ResizeEpoch`으로 게시한다.
  - stale `devicePixelContentBoxSize`와 새 DPR을 섞지 않고 1px 초과 불일치 시 logical×DPR pair로 되돌린다.
  - `applyProvisionalEpoch()`은 canvas backing을 target physical size로 reset하고 이전 front overlap을 `NEAREST`, 1:1로 즉시 복원한다.
  - `CanvasPresenter`는 `current + latest`, exact staging raster, stale generation rejection, exactly-once `terminalRecorded`를 가진다.
  - context restore는 `contextGeneration`을 증가시키고 front/staging을 새 context에서 재구축한다.
- `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`
  - managed metrics/raster/terminal 경계를 소유하며 JS presenter와 generation을 교환한다.
- `Doroti/eng/validate-web-resize-continuity-live.ps1`
  - Chrome/Edge, 기본 40 samples, visible mode, compatibility matrix, context loss/restore를 지원한다.

### 2.2 기존 증거의 정확한 범위

- deterministic contract와 Web Release build/publish는 과거 PASS했다.
- Chrome 40-sample compatibility matrix와 Edge 40-sample baseline은 blank/stale/GL/browser error 0으로 PASS했다.
- 이는 CDP/state-machine 호환성 증거이며 실제 browser compositor scan-out이나 사용자가 보는 border-drag 부드러움의 증거가 아니다.
- 실제 mouse border drag, Firefox, 고주사율 visible acceptance는 `notVerified`다.
- 이번 `work3.md` 작성에서는 위 결과를 재실행하지 않았으므로 현재 상태도 `notVerified`/과거 evidence 유지다.

### 2.3 2026-08-24 Windows Arm N 후속 evidence의 Web 의미

Windows Arm N은 hidden composition front에 latest exact frame을 먼저 latch한 뒤, visible child 교체와 offset/clip을 같은 DirectComposition commit으로 제출하도록 수정됐다. 2ms cadence transaction smoke 3회가 PASS했고, 사용자는 수정된 final binary의 고속 좌측·상단 확대에서 기존 front/border와 창 경계 어긋남 및 떨림이 사라졌음을 직접 확인했다. 이 결과는 해당 Windows 회귀에 대한 **scoped manual PASS**이며 Windows 전체 G2나 Web visible PASS는 아니다.

Web에 옮길 원칙은 다음과 같다.

- “visible surface 하나”는 동시에 표시되는 front가 하나라는 뜻이다. hidden staging/prepared slot은 허용한다.
- visible canvas extent나 front metadata를 먼저 확장하지 않는다.
- latest exact staging의 generation/context/DPR/extent를 final gate에서 확인한 뒤, exact blit과 visible front metadata 채택을 하나의 browser paint transaction에서 수행한다.
- 준비 중 superseded된 hidden slot은 visible commit 없이 폐기한다.

이 문서의 Web 구현·브라우저 runtime 상태는 여전히 `notRun`/`notVerified`이며, Windows manual PASS로 자동 승격하지 않는다.

### 2.4 우선 검증할 결함 후보

1. **exact-size staging 재할당 churn**
   - 현재 `ensureStaging()`은 width/height가 달라지면 기존 staging GPU objects를 삭제하고 새로 만든다.
   - front/staging을 교대하므로 빠른 왕복 resize에서 texture/renderbuffer allocation과 driver synchronization이 반복될 수 있다.
2. **scheduler ownership 분산**
   - host sampling rAF, managed render 요청, `queueMicrotask(runPresenter)`, Promise continuation이 별도 scheduling 경계를 만든다.
   - 한 paint interval에 여러 exact raster가 시작되거나, obsolete raster가 main thread/GPU 시간을 점유한 뒤 reject될 수 있다.
3. **destructive visible backing reset과 provisional restore 사이의 관측 공백**
   - 코드상 같은 callback에서 즉시 복원하지만 실제 브라우저 paint 전 완료되는지, resize 방향·DPR·browser별로 계측해야 한다.
4. **CPU/managed latency가 최신 epoch보다 늦게 끝나는 경우**
   - stale present는 막더라도 오래 걸린 current raster가 latest 시작을 지연할 수 있다.
5. **context/DPR transition의 resource epoch 혼합**
   - old context slot이나 old DPR content extent가 복구 후 visible commit에 재사용되지 않는지 증명해야 한다.

## 3. 목표 불변조건

구현 완료 시 다음 계약이 항상 성립해야 한다.

### 3.1 visible ownership

- visible DOM canvas는 정확히 하나다.
- canvas CSS logical size와 intrinsic physical backing은 committed latest epoch와 일치한다.
- visible default framebuffer에는 CPU readback 없이 WebGL GPU blit만 사용한다.
- resize 중 이전 전체 frame을 새 client 전체로 stretch하지 않는다.
- 확대의 새 영역은 theme background이며, 이전 front의 유효 overlap만 1:1로 복사한다.
- 축소는 이전 front를 top-left 기준 1:1 crop하고 exact frame이 준비되면 교체한다.

### 3.2 resource ownership

- GPU slot은 `front`, `staging` 최대 2개다. context 복구 중 임시 bootstrap slot이 필요하면 high-water 3을 넘지 않는다.
- 각 slot은 다음 값을 분리해 가진다.
  - allocation capacity width/height
  - exact content physical width/height
  - logical width/height와 DPR
  - resize generation과 context generation
  - state: `free`, `rasterizing`, `prepared`, `front`, `retired`
- shrink에서는 texture/renderbuffer를 재할당하지 않는다.
- expansion은 필요한 축의 capacity만 grow한다. 동일 drag 중 grow 횟수와 allocation duration을 evidence에 기록한다.
- front는 exact commit과 slot swap이 끝나기 전에 삭제·resize·staging으로 재사용하지 않는다.

### 3.3 transaction ownership

- root `ResizeObserver`/DPR watcher는 target 관측과 provisional restore만 담당한다.
- exact frame lifecycle은 하나의 `BrowserFramePump` rAF가 담당한다.
- 한 rAF에는 최대 하나의 latest epoch만 managed metrics → raster → final gate → blit/terminal로 진행한다.
- queue는 `current + latest`만 허용하며 depth high-water는 2 이하다.
- target이 바뀌면 current는 GPU/managed 작업 완료 후 `superseded`로 정확히 한 번 terminal되고 latest만 다음 rAF에 진행한다.
- Promise, `gl.flush()`, WebGL fence submission, rAF return은 `submitted`일 뿐 scan-out ACK가 아니다.

### 3.4 금지 사항

- `readPixels`, `getImageData`, `toDataURL`, bitmap CPU round-trip
- `preserveDrawingBuffer=true`
- CSS transform/mask/opacity로 resize 결함 가리기
- 두 visible canvas 또는 DOM screenshot overlay 교대
- full-frame provisional stretch나 non-uniform scale
- 모든 ResizeObserver event FIFO replay
- debounce 시간을 늘려 stale/blank를 숨기기
- CDP screenshot PASS를 최종 visible PASS로 승격
- 새 Flutter runtime instrumentation

## 4. 목표 시간축

```text
T0 browser layout / ResizeObserver or DPR signal
  -> coherent logical/physical/DPR ResizeEpoch 게시
  -> visible canvas backing을 target physical size로 정확히 commit
  -> retained front overlap을 같은 callback에서 1:1 crop/blit
  -> expansion remainder는 theme background
  -> latest exact work만 BrowserFramePump에 예약

T1 one sampling/presenter rAF
  -> current가 없으면 latest를 current로 승격
  -> capacity staging slot 확보(grow only)
  -> exact content extent로 managed metrics/build/raster
  -> generation/context/DPR final gate
     stale -> superseded terminal, visible front 유지
     exact -> staging exact rect를 default framebuffer에 1:1 blit
  -> front/staging metadata swap
  -> submitted terminal
  -> 다음 latest가 있으면 다음 rAF 하나만 예약

T2 browser paint/compositor
  -> 앱은 scan-out을 ACK하지 않음
  -> external/manual observer가 visible acceptance를 별도 판정
```

핵심은 `canvas.width/height` reset 직후 blank default buffer가 browser paint까지 남지 않게 하는 것과, exact raster가 끝나기 전까지 retained front metadata/resource를 건드리지 않는 것이다.

## 5. Ordered 구현계획

각 단계는 앞 단계 PASS 후에만 진행한다. FAIL이면 해당 단계의 failure branch를 기록하고 뒤 단계를 시작하지 않는다.

### W3-0 — baseline과 provenance 동결

작업:

- 현재 Web source fingerprint, .NET/SkiaSharp 버전, Chrome/Edge/Firefox 버전, GPU/driver, OS, monitor refresh를 기록한다.
- 기존 40-sample Chrome/Edge smoke를 변경 전 baseline으로 각각 1회 실행한다.
- `-Visible`로 실제 브라우저를 열고 최소 다음을 직접 관찰한다.
  - 좌/우/상/하 및 모서리 border drag
  - 빠른 확대/축소 왕복
  - maximize/restore
  - browser zoom과 monitor/DPR 전환
- trace에서 target, backing reset, stable-front restore, managed raster, front commit의 동일 timebase를 확인한다.

추가 evidence:

- default backing reset → stable-front restore duration
- ResizeObserver signals / rAF count / managed raster count
- resize 방향별 target generation count
- GPU surface create/delete/resize count와 duration
- current/latest queue high-water
- exact commit까지 걸린 browser refresh 수

PASS:

- baseline build/contract/smoke가 재현되고 source fingerprint가 evidence에 연결된다.
- 직접 관찰 결과는 좋고 나쁨과 관계없이 방향별로 기록된다.

FAIL/stop:

- bootstrap, DPI, source fingerprint, browser version 또는 validator cleanup이 불명확하면 구현을 시작하지 않는다.

### W3-1 — evidence schema와 deterministic contract 보강

작업:

- `GpuSurface`에 allocation capacity와 exact content extent를 분리한 diagnostic field를 먼저 추가한다.
- trace phase를 다음처럼 명시한다.
  - `epoch-observed`
  - `visible-backing-reset-start/end`
  - `retained-restore-start/end`
  - `slot-grow-start/end`
  - `exact-raster-start/end`
  - `final-generation-gate`
  - `exact-visible-submit`
  - `slot-adopt/retire`
- deterministic fixture에 expansion, shrink, mixed-axis, DPR-only, A→B→C supersede, context loss 중 current/latest를 추가한다.
- terminal duplicate/unterminated, stale visible submit, slot premature reuse를 0으로 강제한다.

PASS:

- generated frame 전부 exactly-once terminal
- queue high-water ≤ 2
- stale/mismatched visible submit 0
- slot illegal transition/reuse 0

### W3-2 — grow-only offscreen capacity slot

대상:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- 필요 시 `DorotiWebGlSurface.razor`와 managed descriptor

작업:

- `ensureStaging(exactWidth, exactHeight)`의 exact mismatch delete/recreate를 제거한다.
- slot capacity가 target 이상이면 기존 FBO/texture/renderbuffer를 그대로 사용한다.
- 부족한 축만 새 capacity로 grow하고, 교체 전 front resource는 유지한다.
- Skia backend render target에는 allocation capacity가 아니라 exact content width/height를 전달한다.
- exact raster 전 viewport/scissor/color/depth/stencil state를 target extent 기준으로 초기화한다.
- commit blit source/destination rect는 exact content extent만 사용한다.
- shrink와 같은-capacity expansion에서 allocation/delete count가 증가하지 않게 한다.

PASS:

- 40 resize pulse 후 slot 수 ≤ 2, shrink allocation 0
- front/staging exact content metadata mismatch 0
- framebuffer complete, GL error 0
- 전체 frame scale 1.0/1.0

FAIL branch:

- Skia/WebGL이 larger attachment에 exact-sized backend target wrapping을 안전하게 지원하지 않으면 이 단계만 rollback한다.
- fallback은 2~3개 bucketed capacity slot 비교 실험이며 exact-size 매 event 재할당으로 바로 돌아가지 않는다.

### W3-3 — 단일 BrowserFramePump로 scheduler 통합

작업:

- presenter의 exact drain owner를 하나의 rAF로 고정한다.
- `queueMicrotask`는 rAF 예약 coalescing에만 사용하거나 제거하고, microtask가 직접 managed raster를 시작하지 않게 한다.
- host sampling rAF와 presenter rAF가 중복이면 하나의 frame pump에서 target snapshot과 present drain을 순서대로 수행한다.
- rAF 시작 시 latest를 snapshot하고, managed callback 뒤 final generation/context gate를 다시 확인한다.
- current가 실행 중일 때 새 target은 latest 하나만 교체하고 replaced latest를 즉시 `superseded` terminal한다.
- exact commit 후 latest가 남아 있을 때만 다음 rAF를 하나 예약한다.

PASS:

- 한 browser frame interval에서 exact-raster start 최대 1
- rAF scheduled/entered 균형, orphan callback 0
- queue high-water ≤ 2, duplicate terminal 0
- resize event 수가 증가해도 managed raster는 coalesced latest 수를 넘지 않음

### W3-4 — direction-aware provisional restore와 exact commit

작업:

- 현재 `applyProvisionalEpoch()` 정책을 보존하되 방향별 trace와 contract를 분리한다.
- expansion:
  - target-sized visible backing을 먼저 확정
  - front overlap만 top-left 1:1 `NEAREST` blit
  - 새 영역은 theme background
- shrink:
  - old front의 target overlap을 1:1 crop
  - exact staging이 준비되기 전 old frame을 scale/reflow하지 않음
- mixed-axis resize는 축별로 expansion background와 shrink crop을 결합한다.
- exact staging commit은 latest exact content rect만 1:1 blit하고 같은 transaction에서 front metadata를 adopt한다.
- retained restore와 exact commit 사이에 CSS size/backing size가 다시 바뀌면 commit하지 않고 supersede한다.

PASS:

- blank/white/black full-frame sample 0
- provisional scaleX/scaleY 항상 1
- source/destination rect overflow 0
- exact front commit generation이 latest와 일치
- right/bottom edge stripe와 uniform raster-size oscillation 0

### W3-5 — DPR/zoom/context/lifecycle 복구

작업:

- ResizeObserver physical box와 DPR coherence 규칙을 새 slot metadata에도 적용한다.
- DPR-only epoch는 logical size가 같아도 새 exact physical content로 raster한다.
- context loss 시 current/latest terminal을 정리하고 old-context slot을 visible/staging으로 재사용하지 않는다.
- restore 후 context generation을 올리고 latest epoch의 background → exact front 순서로 재구축한다.
- hidden/frozen tab에서 rAF가 중단된 동안 latest만 유지하고 resume 시 한 transaction만 실행한다.
- dispose 시 rAF, observer, DPR listener, JS interop callback, GPU slots를 순서대로 정리한다.

PASS:

- context generation 증가, old-context access 0
- restore 뒤 latest exact front 1회 재구축
- DPR/zoom mismatch와 uniform scale failure 0
- leaked listener/rAF/GPU object/unterminated frame 0

### W3-6 — automated 40-sample gate

실행:

```powershell
pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1 -Shard Contract
dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release
pwsh -NoProfile -File Doroti/eng/validate-web-resize-continuity-live.ps1 -Browser Chrome -SampleCount 40 -ExerciseCompatibilityMatrix
pwsh -NoProfile -File Doroti/eng/validate-web-resize-continuity-live.ps1 -Browser Edge -SampleCount 40
git diff --check
```

모든 명령은 repository 규칙에 따라 20분 안에 종료한다.

Chrome/Edge PASS:

- blank/stale/GL/browser error 0
- queue high-water ≤ 2
- failed/unterminated/duplicate terminal 0
- context loss/restore PASS
- exact latest front 존재
- shrink allocation 0, slot high-water ≤ 2
- scale 1:1, overflow 0

Firefox:

- Chromium CDP validator를 억지로 재사용하지 않는다.
- repository에 이미 사용 가능한 Firefox WebDriver/BiDi 경로가 있으면 동일 40-sample schema로 추가한다.
- 새 tooling 설치가 필요하면 별도 승인 전까지 `notVerified`로 둔다.

### W3-7 — 실제 visible acceptance

자동 smoke PASS 뒤에만 실행한다.

필수 matrix:

- Chrome와 Edge 실제 mouse border drag
- 좌/우/상/하/네 모서리
- 빠른 확대, 빠른 축소, 왕복, maximize/restore
- 100%, 125%, 150%, 200% browser zoom
- 사용 가능한 60/120/144/165Hz monitor
- foreground/background 복귀와 4× CPU slowdown
- Firefox는 tooling 또는 직접 관찰 가능한 범위에서 별도 기록

visible PASS 조건:

- full-frame white/black/transparent flash 0
- 이전 frame uniform stretch 0
- content raster 크기 왕복 떨림과 right/bottom stripe 0
- border와 exact content가 지속적으로 2 browser paints 이상 분리되지 않음
- drag 입력이 멈춘 뒤 latest exact frame 잔여 mismatch 0
- 사용자 직접 관찰 PASS

CDP screenshot, DOM geometry, GL trace만으로 이 단계를 PASS하지 않는다. 필요하면 120fps 이상 외부 촬영을 diagnostic evidence로 추가하되 사용자 직접 관찰과 구분한다.

### W3-8 — 제품 정리와 문서 승격

G4 visible PASS 뒤에만 진행한다.

- obsolete exact-size-per-event staging allocation path와 dead diagnostics를 제거한다.
- package/template의 Web host가 같은 frame pump와 slot contract를 사용하는지 확인한다.
- active docs의 source identity, commands, evidence path, PASS/FAIL/notVerified를 최신화한다.
- experimental flag가 남으면 기본값과 rollback 조건을 문서화한다.
- unrelated Windows 작업과 evidence는 수정·삭제하지 않는다.

## 6. Acceptance 상태를 나누는 방법

| 범주 | 필요한 증거 | 현재 상태 |
| --- | --- | --- |
| source/contract | deterministic state machine, diff | `notRun` |
| build/publish | Release build/publish | `notRun` |
| GPU resource correctness | slot/capacity/context/GL counters | `notImplemented` |
| automated browser compatibility | Chrome/Edge 40-sample | 과거 PASS, 새 구조는 `notRun` |
| Firefox | WebDriver/BiDi 또는 직접 실행 | `notVerified` |
| visible resize | 실제 border drag와 사용자 관찰 | `notVerified` |
| browser scan-out | qualified external observation | `notVerified` |

한 행의 PASS로 다른 행을 대신하지 않는다.

## 7. 즉시 중단 조건

- visible canvas가 둘 이상 필요해짐
- canvas CSS/intrinsic size mismatch를 scale로 보정함
- shrink에서 GPU allocation/delete가 계속 발생함
- stale generation이나 old context가 visible default framebuffer에 blit됨
- queue depth가 2를 넘거나 terminal이 중복/누락됨
- managed raster를 main thread blocking wait로 감쌈
- `preserveDrawingBuffer`, CPU readback, CSS mask/transform가 필요해짐
- 40-sample smoke에서 blank/stale/GL error가 한 번이라도 발생함
- direct visible test에서 Windows에서 제거한 것과 같은 uniform scale, edge stripe, white/black gap이 재현됨

중단 시 다음 단계로 넘어가지 않고 raw evidence, source fingerprint, 정확한 실패 epoch와 resume point를 기록한다.

## 8. 예상 변경 파일

우선 범위:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`
- `Doroti/eng/validate-resize-continuity.ps1`
- `Doroti/eng/validate-web-resize-continuity-live.ps1`
- `Doroti/validation/resize-contract/*` 중 Web fixture
- 실행 결과를 기록할 active 작업 문서

변경하지 않을 기본 범위:

- Windows D3D12 presenter와 observer
- Flutter reference/runtime artifacts
- CPU bitmap renderer
- unrelated platform hosts

공통 transaction type 변경이 필요하면 먼저 Web-only adapter로 해결 가능한지 확인하고, 불가능할 때만 common contract를 최소 변경한다.

## 9. 완료 정의와 정확한 시작점

이 작업은 다음을 모두 만족해야 완료다.

1. grow-only offscreen capacity + exact content extent가 구현되고 shrink allocation이 0이다.
2. 하나의 BrowserFramePump가 latest-only exact raster/commit을 소유한다.
3. provisional restore와 exact commit이 모든 방향에서 1:1이며 blank/stretch/overflow가 0이다.
4. DPR/zoom/context/lifecycle contract가 PASS한다.
5. Chrome/Edge 40-sample gate가 PASS한다.
6. 실제 border drag visible matrix와 사용자 직접 관찰이 PASS한다.
7. Firefox와 scan-out처럼 실행하지 못한 범주는 `notVerified`로 정확히 남긴다.

현재 시작점은 **W3-0 baseline/provenance 동결**이다. 이번 문서 작성 단계에서는 구현과 runtime validation을 수행하지 않았다.

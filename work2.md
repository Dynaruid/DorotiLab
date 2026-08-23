# Windows interactive resize 근본 원인 재검증 및 구조 수정 계획

## 0. 문서 목적과 현재 결론

- 전면 재검토일: 2026-08-23
- 대상: Windows interactive resize의 border/content phase, present cadence, visible continuity
- 입력: 현재 repository source, 기존 `work2.md` 실행 기록, `idea.md`, 저장된 app/WGC evidence
- 문서 성격: **후속 구현 계획**이다. 이번 문서 수정 자체는 구현, build, runtime 또는 visible acceptance를 의미하지 않는다.
- Flutter는 고정 source-only protocol reference로만 유지한다. 새 Flutter runtime build/capture/renderer A/B는 수행하지 않는다.
- Web은 Windows의 size/present ownership 결론과 제품 G2가 확정되기 전까지 재개하지 않는다. 재개할 때 active smoke는 합의된 40-sample 절차만 사용한다.

재검토의 핵심 결론은 다음과 같다.

> 공통 근본 원인은 Skia, MAUI, child HWND 또는 Composition 중 하나가 단독으로 느린 것이 아니라, **DWM이 소유한 top-level/non-client geometry와 앱이 소유한 client surface/present가 하나의 원자적 transaction으로 변경되지 않는 것**이다.

현재까지 동일하게 보인 `right gap`에는 서로 다른 현상이 섞여 있었다.

1. **실제 앱 pipeline cadence 저하**
   - MAUI Composition candidate는 10초 동안 target 161, presented ACK 69, app present 약 6.9Hz였다.
   - 이는 실제 제품 후보의 scheduling/dispatcher/composition 병목이다.
2. **DXGI target/backbuffer epoch mismatch**
   - pure top-level N0는 `WM_SIZE` 380회와 present 380회가 일치했는데도 WGC gap이 남았다.
   - `Scaling.None`에서 작은 새 backbuffer가 아직 큰 presentation target에 놓이면 남는 영역이 background로 보인다.
3. **WGC 관측기 자체의 size/cadence phase**
   - N0 WGC는 165Hz 환경에서 약 39Hz만 수집했고 callback backlog p50이 약 30.4ms였다.
   - callback 안의 synchronous GPU readback과 frame-pool `Recreate` 때문에 현재 수치로 2-refresh acceptance를 판정할 수 없다.

따라서 기존 계획의 다음 가정은 폐기한다.

- `WGC gap = 앱 content가 현재 창보다 늦다`
- `capture error 0 + encoder drop 0 = WGC frame loss 0`
- `WGC callback에서 나중에 읽은 GetWindowRect = 해당 frame 시점 geometry`
- `N0가 pure top-level에서도 strict FAIL했으므로 host ownership 가설이 기각됐다`
- `n0-arm-a-420`이 실제 420×300 logical gate였다

새 계획은 먼저 관측기를 교정하고, 그다음 direct top-level control에서 **geometry commit 전 준비와 drag 중 stable surface**를 검증한다. Composition/MAUI 제품 경로는 이 인과 실험에서 유효한 protocol이 나온 뒤에만 다시 연다.

## 1. 기존 실행 결과 재분류

기존 evidence는 삭제하거나 수치를 고쳐 쓰지 않는다. 해석과 acceptance 상태만 다음과 같이 재분류한다.

| 단계/증거 | 확인된 사실 | 재검토 상태 |
| --- | --- | --- |
| 기존 M0/G0 | epoch/terminal/correctness 계약과 기존 evidence | `PASS` 유지, 이번에 재실행하지 않음 |
| 기존 W0 native child HWND | contract/build/runtime correctness | `PASS` 유지 |
| 기존 W0 WGC visual/cadence | WGC texture에서 gap 관측 | `diagnosticOnly`; 실제 scan-out 및 refresh phase는 `notVerified` |
| R0 provenance | paired backend와 source fingerprint 고정 | `PASS` 유지 |
| C0 Composition GPU bridge | D3D11On12 GPU-only bridge, 3-slot lifetime, teardown | `PASS` 유지; device removal은 `notVerified` |
| C1 attached visual | 최소 visual/input/semantics/close smoke | `PASS` 유지; IME/minimize 전체 matrix는 `notVerified` |
| 기존 C2 candidate run | target 161, presented ACK 69, app present 6.9Hz | **app-internal cadence `FAIL`**; visible/scan-out acceptance는 `invalidated`/`notVerified` |
| 기존 C2 raw-child pair | 같은 WGC 도구에서 gap/title failure 관측 | `diagnosticOnly`; absolute visible FAIL 판정은 철회 |
| N0 Arm A 구조 | 하나의 top-level HWND, child/XAML/MAUI/CPU copy 0, WM_SIZE=present 380 | 구조/correctness `PASS` |
| `n0-arm-a-420-wgc.json` | WGC gap과 size-transition pattern | `diagnosticOnly`; strict gate `invalid` |
| N0 420×300 크기 | DPI 192에서 실제 initial outer 1680×1200px | 840×600 logical이므로 기존 420×300 판정 `invalid` |
| N0 Arm B | lifted `Microsoft.UI.Composition` 1.8에 desktop target interop 부재 | 해당 API arm `unavailable` 유지 |
| C3/N1/G2 | 제품 통합과 전체 acceptance | `notStarted` / `notVerified` |
| Web B0~B2/G3~G4 | Windows hard gate 뒤 단계 | `notStarted` / `notVerified` |

### 1.1 N0 frame에서 확인된 방향

N0 첫 전환은 다음과 같다.

| frame | WGC `ContentSize.Width` | app content right gap | 해석 |
| --- | ---: | ---: | --- |
| 0 | 1658 | 0 | target/backbuffer 일치 |
| 1 | 1658 | 8 | app content는 이미 더 작은 크기 |
| 2 | 1650 | 0 | WGC target size가 다음 frame에 따라옴 |

frame 1의 gap 8px은 `1658 - 1650`과 같다. 첫 80 frame에서도 gap이 있는 72개 중 38개가 바로 다음 `ContentSize` 감소량과 정확히 일치했다.

이는 적어도 N0의 대표 transition에서 `content가 늦었다`가 아니라 다음 순서가 발생했음을 뜻한다.

```text
new smaller app backbuffer/source
  -> old larger DWM/WGC presentation target
  -> DXGI_SCALING_NONE background strip
  -> next capture target size catches up
```

이 pattern은 강한 원인 단서지만 실제 monitor scan-out과 동일하다고 아직 주장하지 않는다.

### 1.2 현재 evidence가 증명하지 못하는 것

- WGC gap이 실제 monitor에 같은 frame/duration으로 표시됐는지
- WGC에서 계산한 최대 91/183/283 refresh phase가 실제 scan-out phase인지
- `encoderDroppedFrames=0`이 WGC frame-pool overwrite/discard까지 포함하는지
- `GetWindowRect`와 WGC `SystemRelativeTime`이 같은 시점을 나타내는지
- standard non-client chrome에서 public API만으로 geometry와 client present를 원자 commit할 수 있는지

이 항목들은 아래 M0/M1/H0 gate 전까지 모두 `notVerified`다.

## 2. 새 원인 모델과 시간축

모든 trace와 acceptance는 다음 여섯 시간축을 분리한다.

| 축 | owner | authoritative event/data | 의미 |
| --- | --- | --- | --- |
| T0 input intent | user/input driver | cursor QPC, `WM_SIZING` proposed RECT | 아직 commit되지 않은 미래 target |
| T1 window geometry | USER32/DWM | `WM_WINDOWPOSCHANGING/CHANGED`, `WM_SIZE`, sampled rect | OS가 적용한 top-level/client geometry |
| T2 framework/layout | WinUI/MAUI 또는 direct host | XAML host layout epoch 또는 direct client rect | 앱 content metrics authority |
| T3 surface | app/DXGI | backing/source/backbuffer size와 generation | 렌더된 pixel extent |
| T4 present | DXGI/compositor | present ID, submit QPC, frame statistics | 앱 제출 및 compositor queue 상태 |
| T5 observation/output | WGC/Desktop Duplication/camera | `SystemRelativeTime`, output frame | 외부에서 관측된 composed/display 결과 |

잘못된 결론을 막기 위해 다음 규칙을 적용한다.

- T0의 proposed target을 T1의 committed geometry로 재라벨하지 않는다.
- T1 이후에 렌더를 시작한 frame을 `geometry와 동시에 준비됨`으로 기록하지 않는다.
- T3 exact size만으로 T4/T5 표시 완료를 주장하지 않는다.
- `Present`, `DwmFlush`, `RequestCommitAsync`, commit completion을 scan-out ACK라고 부르지 않는다.
- WGC callback QPC와 frame `SystemRelativeTime`을 섞지 않는다.
- 한 frame 뒤에 읽은 window rect로 이전 captured bitmap의 phase를 판정하지 않는다.

### 2.1 공통 structural race

현재 direct N0도 다음 순서다.

```text
WM_SIZING: count only
  -> OS/DWM geometry change
  -> WM_SIZE
  -> ResizeBuffers
  -> exact render/copy
  -> GPU fence waits
  -> Present queue
  -> DWM consumes the new front
```

`WM_SIZE`를 authority로 쓰는 한 앱은 geometry가 적용된 뒤에야 exact frame 준비를 시작한다. 반대로 shrink 중 새 작은 buffer/source를 먼저 제출하면 DWM이 아직 더 큰 target을 소비하는 transition이 생길 수 있다.

따라서 새 구조의 목표는 `WM_SIZE를 더 빨리 처리`하는 데 그치지 않는다.

1. `WM_SIZING`에서 미래 exact target을 미리 계산한다.
2. visible front와 독립된 backing에 미래 frame을 준비한다.
3. drag 중 visible swap-chain capacity는 바꾸지 않는다.
4. committed geometry 시점에는 allocation/raster가 아니라 bounded source switch/copy/present만 남긴다.
5. standard chrome과 surface 전환이 여전히 분리되면 custom chrome control로 ownership 한계를 판정한다.

### 2.2 cadence 병목

N0는 165Hz input 1651 samples에 `WM_SIZE`/present가 각각 약 380회, 즉 약 38Hz다. current WndProc는 size 변화마다 `ResizeBuffers`, backing 재생성, raster, GPU copy와 fence wait를 직렬 수행한다.

이 구조는 다음 두 문제를 만든다.

- WndProc/input loop가 GPU/resource lifetime을 기다리며 raw input을 coalesce한다.
- geometry가 적용된 뒤 시작한 work가 다음 geometry input까지 막아 window와 cursor cadence 자체를 떨어뜨린다.

새 hot path에서는 WndProc가 GPU fence와 `ResizeBuffers`를 기다리지 않도록 한다. waitable swap-chain object를 사용하더라도 render worker가 frame 시작 전에 기다리며, WndProc에서 present 완료를 기다리는 용도로 사용하지 않는다.

## 3. 보존할 구현 계약

재검토는 기존 correctness work를 되돌리는 것이 아니다. 다음 계약은 그대로 유지한다.

- `DorotiResizeEpoch`, build token, exact matcher, `FrameTransaction`, exactly-once terminal ledger
- logical/physical/root/surface dimensions와 scale을 하나의 immutable epoch에 고정
- framework/raster mailbox의 bounded `current + latest`
- stale target의 pre-raster/pre-flush/pre-present rejection과 명시적 `Superseded`
- scene/backing identity의 submit-time relabel 금지
- GPU-only copy; CPU readback/GDI/bitmap round-trip 금지
- exact front가 visible인 동안 destructive `Resize`/`BeginDraw` 금지
- Composition path의 최대 3-slot pool, retirement 전 front 재사용 금지
- device loss/shutdown에서 open draw, checked-out resource, fence/callback의 terminal 정리
- correctness, app cadence, visible composition, scan-out, input/lifecycle을 별도 상태로 기록

기존 C0/C1 experimental code와 default raw-child rollback path는 새 product gate가 결정될 때까지 보존한다. 기존 evidence와 unrelated worktree 변경을 정리하거나 삭제하지 않는다.

## 4. 새 ordered gate

```text
D0: 기존 evidence 재분류와 provenance 동결
  -> M0: DPI/timebase/capture pipeline 교정
      -> M1: observer qualification
          FAIL -> architecture 판정 금지, 측정기에서 hard stop
      -> H0-A: direct top-level current ordering control
      -> H0-B: WM_SIZING prebuild + stable-capacity/no-ResizeBuffers control
          PASS -> H1-S: standard chrome protocol candidate
          FAIL -> H0-C: same-surface custom chrome control
              PASS -> H1-C: custom chrome/shell candidate
              FAIL -> ownership 가설 미해결, product/Web hard stop
      -> C2R: H0에서 PASS한 protocol의 MAUI/Composition 재검증(조건부)
      -> P0: Windows product owner 선택과 통합
      -> G2: Windows product acceptance
      -> Web B0~B2/G3~G4 재개
```

공통 hard rule:

- M1이 PASS하기 전에는 WGC gap/refresh 수치로 presenter를 PASS/FAIL하지 않는다.
- H0-B를 H0-A와 동일 renderer/scene/input/observer로 비교한다.
- H0-C는 제품 결정이 아니라 standard chrome ownership 한계를 판정하는 control이다.
- H0에서 유효한 protocol을 찾기 전에는 C2R scheduling을 다시 튜닝하지 않는다.
- 앞 gate가 FAIL이면 뒤 단계는 `notStarted`/`notVerified`로 남긴다.

## 5. D0 — evidence와 provenance 동결

상태: `PASS` — 이 문서에서 재분류 완료

### 보존할 evidence

- paired raw-child: `win-rsz-default-left-20260823-230345-7243be9e`
- Composition candidate: `win-rsz-default-left-20260823-230500-43bd9077`
- paired source fingerprint: `fca0d7e0e9b34b9e4c39fe9a53d2f315cf91e2f2e654a477f7e83df1ed4bbb53`
- N0 app: `n0-arm-a-420-app.json`
- N0 WGC diagnostic: `n0-arm-a-420-wgc.json` 및 frame sequence
- C0 반복 evidence:
  - `c0-composition-20260823-225738-b840851dd3804336b`
  - `c0-composition-20260823-225753-28170a3d574b472f8`
  - `c0-composition-20260823-225812-53f84fc0557c467d9`

### D0 규칙

- historical execution source HEAD는 `6557bd79cb870b9699da010e59d55b376485076e`이며, 구현/evidence는 이 HEAD 위 scoped dirty worktree로 보존한다.
- historical JSON/PNG를 새 schema로 덮어쓰지 않는다.
- 기존 summary의 `FAIL` 문자열을 수정하지 않고 새 decision record에서 invalidated 이유를 남긴다.
- source commit, dirty-file fingerprint, binary hash, backend flag, OS/GPU/driver/DPI/refresh를 새 run마다 기록한다.
- 비교 run은 같은 binary/source fingerprint와 candidate flag만 다르게 한다.
- `n0-arm-a-420` 이름은 그대로 보존하되 실제 logical size mismatch를 metadata correction으로 기록한다.

## 6. M0 — 관측기와 DPI 계약 교정

상태: `notStarted`

M0의 목표는 app을 고치는 것이 아니라 `실제로 어느 owner가 먼저/늦게 바뀌는지`를 refresh 단위로 판정할 수 있는 observer를 만드는 것이다.

### 6.1 DPI-aware window sizing

- validator/capture/input process를 모두 Per-Monitor V2 DPI-aware로 시작한다.
- historical gate의 420×300, 640×360, 1000×600은 **logical outer-window size**로 유지한다. logical 값을 DPI-unaware `SetWindowPos`에 넘기거나 physical outer size를 다시 DPI scaling하지 않는다.
- DPI-aware `SetWindowPos`, `GetWindowRect`, `DwmGetWindowAttribute`, 실제 `GetClientRect`를 함께 사용해 requested outer와 derived client를 모두 검증한다. client-size 전용 control이 필요할 때만 `AdjustWindowRectExForDpi`로 outer extent를 계산한다.
- run 시작 전 다음을 evidence에 기록하고 requested/actual mismatch가 있으면 run을 `invalid`로 즉시 종료한다.
  - requested logical outer-window size
  - actual logical/physical outer-window size와 client size
  - outer rect와 DWM extended frame bounds
  - window/monitor DPI와 raster scale

허용 오차:

- physical outer width/height: requested DPI conversion 대비 각 1px 이하
- logical outer round-trip: 각 0.5 logical px 이하
- actual client extent는 oracle/surface authority로 별도 기록하며 outer size와 혼용하지 않음
- mismatch run을 다음 size gate의 evidence로 재사용하지 않는다.

### 6.2 WGC callback hot path

현재 `FrameArrived` 안의 staging texture 생성, synchronous `Map`, full memcpy, pixel oracle, PNG queue, `GetWindowRect`, per-size `Recreate`를 제거하거나 callback 밖으로 이동한다.

새 callback은 다음만 수행한다.

```text
TryGetNextFrame
  -> frame SystemRelativeTime/ContentSize/capture index 기록
  -> preallocated GPU readback ring slot으로 CopySubresourceRegion
  -> ring fence/query 기록
  -> frame dispose/return
```

- D3D11 staging resource는 최대 capture extent에 맞춰 미리 할당한다.
- single-monitor M1에서는 frame pool도 monitor work-area를 포함하는 고정 최대 physical size로 한 번 만든다.
- drag 중 `Direct3D11CaptureFramePool.Recreate`를 호출하지 않는다.
- frame `ContentSize`가 pool capacity를 넘으면 clip을 숨기지 말고 run을 `invalid`로 끝낸다.
- `Map`, pixel scan, PNG encoding은 capture callback과 분리된 analyzer worker에서 fence/query 완료 뒤 수행한다.
- ring full이면 별도 `captureRingDroppedFrames`를 증가시키고 해당 run을 acceptance에서 제외한다.

### 6.3 same-timebase trace

모든 event는 raw timestamp와 clock identity를 함께 남긴다.

- input/window/app: QPC counter + frequency
- WGC: `SystemRelativeTime` 원본 100ns 값
- callback: callback-entry QPC와 callback-exit QPC
- present: submit QPC, present ID/count, 가능한 frame statistics
- Desktop Duplication: acquire QPC와 frame metadata

run 시작/종료에서 QPC↔100ns calibration pair를 기록한다. phase 계산은 captured frame의 `SystemRelativeTime`으로 수행하고 callback-entry QPC는 delivery delay 진단에만 사용한다.

`GetWindowRect`는 다음 두 경로로 분리한다.

- input driver가 일정 cadence로 sampling한 T0/T1 window trace
- app WndProc가 `WM_WINDOWPOS*`, `WM_SIZE`에서 기록한 committed geometry trace

WGC callback이 readback을 끝낸 뒤 읽은 rect를 captured frame의 geometry로 사용하지 않는다.

### 6.4 실제 output 교차 관측

WGC window capture만으로 strict visible acceptance를 판정하지 않는다.

우선순위:

1. DXGI Desktop Duplication으로 같은 monitor output을 캡처하고 실제 screen coordinate의 window crop을 분석
2. DXGI present statistics/PresentMon 또는 ETW로 present ID와 displayed/composed timing 연결
3. WGC `ContentSize`와 window-local pixel oracle를 진단용으로 병행
4. WGC와 Desktop Duplication이 모순되면 240fps 이상 외부 촬영 또는 육안 high-refresh acceptance로 판정

Desktop Duplication도 scan-out의 완전한 하드웨어 ACK라고 과장하지 않는다. 다만 window-target WGC의 frame-pool resize artifact와 전체 desktop composition을 분리하는 필수 control로 사용한다.

### M0 PASS gate

- exact logical outer-window size 생성과 DPI round-trip PASS
- callback 안 synchronous `Map`/pixel analysis/PNG encoding/`GetWindowRect` 0
- drag 중 frame-pool `Recreate` 0
- preallocated capture ring이 bounded이고 ring drop 0
- WGC frame timestamp, callback delay, app/window QPC가 별도 필드로 저장됨
- analyzer가 `ContentSize` 밖 undefined pixel을 oracle에 포함하지 않음
- Desktop Duplication 또는 동등한 output-level observer가 동일 run에 연결됨
- capture/app/driver process와 mouse button/priority/timer cleanup PASS

### M0 FAIL gate

- 165Hz test에서 callback 자체가 readback/analysis에 block됨
- pool capacity/resize transition 때문에 crop extent를 신뢰할 수 없음
- DPI-aware requested/actual outer-window size와 derived client extent를 고정할 수 없음
- app present와 output frame을 같은 timebase로 연결할 수 없음

M0 FAIL이면 presenter/ownership 실험으로 진행하지 않는다.

## 7. M1 — observer qualification

상태: `notStarted`

M1은 새 observer가 짧은 transition을 놓치거나 만들어내지 않는지 검증한다.

### 7.1 qualification scene

direct top-level validation app에서 resize와 독립적으로 다음 marker를 표시한다.

- frame마다 증가하는 GPU-rendered binary/Gray-code frame ID
- 고정 1px border와 right-edge color marker
- 고정 높이 AppBar, checkerboard, circle
- surface/source size generation을 나타내는 color patch

scripted sequence:

1. static size 5초
2. surface content만 165Hz 또는 가능한 최대 cadence로 5초 변경
3. window geometry만 단계적으로 변경하고 content는 고정
4. content/source와 window geometry를 알려진 0/1/2/4 refresh offset으로 변경
5. 420×300 logical outer-window left-edge triangle drag 10초

### 7.2 observer metrics

- rendered frame ID 대비 WGC/Desktop Duplication observed ID 누락/중복
- capture delivery interval과 callback duration
- capture-ring drop, encoder drop, WGC timestamp discontinuity
- known injected phase 대비 측정 phase 오차
- static size에서 false gap/blank/title/circle failure
- window-only 변화에서 WGC `ContentSize`와 Desktop Duplication bounds의 상대 phase

### M1 PASS gate

- callback duration p95 ≤ 1ms, p99 ≤ 2ms
- capture-ring drop 0, capture error 0
- known 0/1/2/4-refresh injected phase의 방향을 모두 올바르게 식별
- phase magnitude 오차 p95 ≤ 1 refresh
- static false gap/blank/non-uniform stretch 0
- WGC가 2-refresh gate를 샘플링할 cadence를 제공하지 못하면 WGC를 strict judge에서 자동 제외하고 Desktop Duplication/외부 observer가 그 역할을 담당
- 3회 연속 qualification이 같은 판정을 냄

### M1 FAIL 처리

- threshold, PNG stride, sample 수를 조정해 PASS로 만들지 않는다.
- WGC가 구조적으로 충분한 cadence를 제공하지 않으면 `diagnosticOnly`로 강등한다.
- output-level observer도 qualification을 통과하지 못하면 strict refresh acceptance를 중단하고 외부 고속 촬영 setup이 준비될 때까지 뒤 gate를 `notStarted`로 둔다.

## 8. H0 — direct top-level 인과 분리 실험

상태: `notStarted`

H0는 MAUI/XAML/child HWND를 제외한 하나의 top-level HWND에서 renderer, scene, input, observer를 고정하고 size/present ordering만 바꾼다.

공통 조건:

- 200% DPI, 165Hz 내장 panel 우선
- 정확한 420×300 logical outer-window부터 시작하고 실제 client extent를 surface authority로 기록
- left-edge 10초 triangle drag
- same source fingerprint/binary에서 arm flag만 변경
- `Scaling.None`, fixed bar/circle/checker/right-edge oracle
- CPU readback/GDI/bitmap path 0
- app frame generation과 present ID를 output marker에 인코딩

### 8.1 H0-A — current ordering control

목적: 기존 N0 결과를 올바른 DPI와 observer로 다시 측정한다.

```text
WM_SIZE
  -> ResizeBuffers exact
  -> raster/copy
  -> fence wait
  -> Present(0)
```

필수 기록:

- `WM_SIZING`, `WM_WINDOWPOS*`, `WM_SIZE` rate와 duration
- WndProc blocked time
- `ResizeBuffers`, raster, copy, fence wait, Present duration
- committed client size, backbuffer size, present ID, WGC/desktop frame ID
- shrink/expansion 방향별 target-minus-source extent

H0-A는 baseline control이며 PASS 후보가 아니다. 여기서 기존 gap pattern이 재현되지 않아도 observer와 DPI 변화 때문에 historical evidence를 삭제하지 않는다.

### 8.2 H0-B — prebuild + stable-capacity source handoff

목적: geometry commit 뒤의 `ResizeBuffers/raster/fence`가 common lag의 주원인인지 검증한다.

#### surface/resource 구조

- monitor/work-area와 product maximum policy 안의 bounded capacity swap chain을 한 번 생성한다.
- interactive drag 중 `ResizeBuffers`를 호출하지 않는다.
- exact offscreen backing은 `current + latest` 두 generation만 유지한다.
- `WM_SIZING` proposed outer rect를 DPI-aware client size로 변환해 future epoch을 게시한다.
- render worker가 future exact backing을 미리 raster/flush하고 ready fence를 남긴다.
- current visible source region/front는 committed geometry가 바뀌기 전까지 유지한다.

#### commit hot path

```text
WM_SIZING proposed target
  -> future exact backing prebuilt asynchronously

WM_WINDOWPOSCHANGED / WM_SIZE committed target
  -> committed epoch을 present worker에 게시하고 WndProc는 즉시 복귀
  -> worker가 matching ready generation lookup
  -> GPU copy into stable-capacity backbuffer
  -> SetSourceSize(committed exact client extent)
  -> Present
```

- matching future frame이 없으면 old front를 non-uniform stretch하지 않는다.
- shrink는 committed bounds에서 crop하고 expansion의 uncovered area는 명시적 background로 남긴다.
- WndProc는 raster, GPU copy, fence wait, `ResizeBuffers`, `Present`를 수행하지 않는다.
- GPU readiness를 기다려야 하면 최대 1 refresh의 bounded wait arm을 한 번만 비교한다. timeout을 반복 튜닝하지 않는다.
- `SetSourceSize`는 이 H0 실험에서만 capacity-preserving source handoff의 인과 control로 허용한다.
- 과거 capacity-buffer overflow 때문에 H0/G2 visible proof 없이 제품 경로로 승격하지 않는다.

#### H0-B가 답해야 할 질문

1. `WM_SIZE` rate가 기존 약 38Hz보다 올라가는가?
2. WndProc blocked time에서 `ResizeBuffers`/raster/fence가 제거되는가?
3. committed geometry에 matching prebuilt generation이 몇 % 존재하는가?
4. standard chrome과 source/present가 실제 output에서 같은 refresh에 전환되는가?
5. shrink의 `Scaling.None` background strip과 expansion clip이 사라지는가?

### 8.3 H0-C — same-surface custom chrome control

실행 조건: M1 PASS 후 H0-B가 internal cadence/correctness는 PASS하지만 actual output에서 standard border/content phase만 남을 때.

목적: public standard non-client chrome의 별도 DWM ownership이 마지막 원인인지 판정한다.

- borderless top-level window 한 개를 사용한다.
- resize border/title/caption oracle와 app content를 같은 swapchain/backing에 그린다.
- custom hit-test로 left/right/top/bottom/corner resize intent를 만든다.
- OS outer geometry trace는 계속 기록하되 visible border/content는 같은 presented frame ID를 공유한다.
- full product input/accessibility/IME를 넣지 않고 ownership control만 검증한다.

판정:

| H0-B standard chrome | H0-C custom chrome | 결론 |
| --- | --- | --- |
| PASS | 미실행 | standard chrome에서도 stable-surface protocol 가능 |
| FAIL | PASS | 남은 root는 standard non-client geometry와 app surface의 분리 |
| FAIL | FAIL | surface/present scheduling 또는 observer 가설이 아직 불완전 |

### 8.4 H0-B/H0-C PASS gate

H0-A는 비교용 control이므로 아래 gate로 product candidate가 되지 않는다. 아래 absolute gate는 H0-B와 조건부 H0-C에 적용한다.

correctness:

- stale/mismatched present, illegal transition, unterminated transaction 0
- committed client/source/render extent mismatch 0
- drag 중 `ResizeBuffers` 0(H0-B/C)
- capacity/source overflow, out-of-bounds copy, non-uniform stretch 0
- final exact frame과 size mismatch 0

cadence:

- `WM_SIZE`/committed geometry handler p95 ≤ 1 refresh, p99 ≤ 2 refresh
- matching ready frame의 geometry-commit→Present submit p95 ≤ 1 refresh
- inter-present interval >2 refresh episode가 연속 2회 이상 발생하지 않음
- final target→matching present ≤ 2 refresh

visible/output:

- Desktop Duplication/qualified observer에서 blank/title/AppBar/circle/right-edge failure 0
- newly committed visible border와 content frame ID의 phase >2 refresh episode 0
- final gap 0
- WGC와 output observer가 다르면 output observer와 직접 관찰을 우선하고 discrepancy를 별도 기록
- 정확한 420×300, 640×360, 1000×600 logical outer-window에서 각 3회 연속 PASS

fail-fast는 M1이 PASS한 뒤에만 적용한다. 420×300 absolute gate가 유효하게 FAIL하면 640/1000은 `notStarted`로 남긴다.

## 9. H1 — Windows visible ownership 결정

상태: `notStarted`

H0 결과로 제품 후보를 다음처럼 제한한다.

### H1-S — standard chrome + stable surface

선택 조건: H0-B PASS.

- `WM_SIZING` future epoch prebuild
- drag 중 stable capacity
- committed geometry에서 bounded source switch/present
- standard title/caption 유지

이 경우 direct HWND와 MAUI Composition 중 어느 host에서도 같은 protocol을 재현할 수 있는지 C2R로 비교한다.

### H1-C — custom chrome + single visible surface

선택 조건: H0-B FAIL, H0-C PASS.

- standard non-client chrome과 exact content의 원자 전환을 요구하지 않는다.
- border/title/content를 하나의 presented surface와 frame generation으로 소유한다.
- pure Win32 shell migration의 input/IME/UIA/packaging 비용을 별도 product decision으로 승인받기 전 N1 전체 구현을 시작하지 않는다.

### hard stop

H0-B와 H0-C가 모두 valid FAIL이면 다음을 하지 않는다.

- 새 Composition API arm 추가
- ANGLE DLL/EGL surface 재시도
- timer/debounce/timeout 반복 조정
- Web milestone 재개

이 경우 M1 observer와 render/present timebase를 다시 검토하고 원인 미확정으로 기록한다.

## 10. C2R — MAUI/WinUI Composition 재검증

상태: `notStarted`

실행 조건:

- M1 PASS
- H0-B standard-chrome protocol이 valid PASS
- MAUI/XAML host를 유지할 제품 가치가 있음

기존 C0/C1은 다시 만들지 않고 보존된 bridge/lifetime을 사용한다. 다만 기존 C2의 visible FAIL 판정은 재사용하지 않는다.

### 10.1 먼저 해결할 app-internal cadence

기존 candidate의 target 161 / presented ACK 69 / app present 6.9Hz는 WGC와 무관한 internal FAIL이다.

- host layout target publication rate와 UI dispatcher queue delay를 분리한다.
- Composition commit pending 동안 latest 하나만 유지하되 completed front retirement가 다음 frame 전체를 막지 않게 한다.
- WndProc/XAML layout path에서 synchronous GPU wait와 `DwmFlush`를 제거한다.
- surface preparation, UI mutation, commit request/action, front adoption을 별도 duration으로 기록한다.
- `DwmFlush`는 atomicity 해법으로 사용하지 않는다. 필요하면 H0 protocol PASS 후 단 한 번의 OFF/resize-only ON diagnostic pair만 허용한다.

internal cadence gate:

- authoritative host layout target rate가 committed geometry rate의 90% 이상
- presented ACK rate가 admitted target rate의 90% 이상 또는 coalesced target마다 정확한 latest terminal 존재
- inter-present p95 ≤ 2 refresh
- pending surface/commit 때문에 2-refresh 초과 연속 stall 0
- transaction/surface leak 0

이 gate가 FAIL하면 strict visible capture를 실행하지 않고 C2R FAIL로 끝낸다.

### 10.2 H0 protocol 이식

H1-S가 선택된 경우:

- top-level `WM_SIZING` proposed rect로 future composition surface를 prebuild한다.
- actual XAML host layout은 committed content epoch authority로 유지한다.
- visible front는 host commit 전 destructive resize하지 않는다.
- committed host size와 matching prepared surface가 있을 때 surface/visual size/clip을 같은 compositor mutation turn에 교체한다.

H1-C가 선택된 경우:

- standard MAUI chrome/XAML composition으로 같은 ownership을 재현할 수 없다고 본다.
- C2R을 건너뛰고 pure Win32/custom chrome product decision으로 이동한다.

### C2R PASS gate

- H0와 동일한 qualified observer와 exact logical size matrix 사용
- candidate OFF direct/raw control과 같은 source fingerprint pair
- raw render child HWND, HWND swap chain, `SwapChainPanel` attachment 0
- app-internal cadence gate PASS
- output visible gate와 final gap PASS
- pointer/key/focus/hidden text input/semantics minimum smoke PASS
- minimize/restore, IME, full accessibility는 G2 전까지 `notVerified`로 명시

C2R absolute FAIL을 baseline 대비 상대 개선으로 덮지 않는다.

## 11. P0 — 제품 경로 통합

상태: `notStarted`

### 11.1 MAUI Composition 제품 경로

선택 조건: C2R PASS.

- `DorotiWindowsDxgiHost`: XAML layout/input/focus owner
- `WindowsCompositionSurfacePresenter`: visible surface/pool/commit owner
- common resize protocol: proposed target prebuild, committed host layout admission, latest-only present
- hidden `Entry`/`Editor`, semantics overlay, pointer/capture를 중복 owner 없이 유지
- `ContentIsland`는 concrete input/accessibility failure가 있을 때만 별도 gate로 검토

### 11.2 pure Win32/custom chrome 제품 경로

선택 조건: H0-C PASS이고 standard chrome path가 valid FAIL.

- 새 `Doroti.Host.Win32`: window/message/input/DPI/lifecycle/custom chrome owner
- 새 Windows target package identity
- framework session/view/render common host 추출
- TSF/IMM32 Korean IME bridge
- UIA fragment root와 semantics bridge
- clipboard, cursor, pointer/touch/pen, focus, shutdown
- packaging/startup/title/caption replacement acceptance

N1은 presenter 교체가 아니라 shell migration이다. H0-C control PASS와 명시적 product decision 없이 scaffold/package/template부터 만들지 않는다.

### 11.3 삭제와 cutover

다음은 P0 구현 PASS가 아니라 G2 PASS 뒤에만 삭제한다.

- raw `STATIC` child HWND와 parent/child dual subclass
- `WindowsHwndD3D12Presenter` legacy product path
- 비활성 ANGLE spike
- `SwapChainPanel` dual exact presenter와 unused presenter
- experimental Composition/N0 feature flags와 rollback wiring

G2 전까지 default path와 rollback 가능성을 보존한다.

## 12. Windows G2 product acceptance

상태: `notStarted` / `notVerified`

### visual/cadence matrix

- left/right/top/bottom/네 corner 각 10초
- 420×300, 640×360, 1000×600 logical outer-window
- 100/125/150/200% DPI와 monitor 이동
- 가능한 장비에서 60/120/144/165Hz
- 느린 drag, 빠른 drag, 방향 반전, edge/corner 전환

### lifecycle/device matrix

- minimize/restore, maximize/restore, occlusion/unocclusion
- DPI change 중 drag와 monitor crossing
- device loss/recovery와 presenter reset
- unload/reload, close/reopen, cold/warm startup
- packaged/installer relevant launch path

### input/accessibility matrix

- mouse hover/click/capture, cursor
- touch/pen
- keyboard/focus/tab traversal
- Korean IME composition/candidate/commit
- hidden text input lifetime
- semantics/UIA screen-reader path

### G2 완료 규칙

- correctness, cadence, visible output, input/accessibility, lifecycle을 각각 PASS/FAIL로 기록한다.
- 한 범주의 build/smoke PASS로 다른 범주를 대신하지 않는다.
- 직접 visible acceptance 또는 qualified output observer가 없는 장비 범주는 `notVerified`다.
- 모든 required product 범주가 PASS하기 전 Windows 완료로 표시하지 않는다.

## 13. Web 재개 조건

Web B0~B2/G3~G4는 다음 조건을 모두 만족한 뒤에만 재개한다.

1. Windows H0에서 size authority/surface/present ownership의 인과 결론 확정
2. 선택된 Windows 제품 경로 G2 PASS
3. common epoch/transaction 변경이 Web contract를 깨뜨리지 않음

Web에서는 기존 방향을 유지한다.

- retained GPU front + exact staging FBO
- root `ResizeObserver` + DPR watcher + one sampling rAF
- `current + latest`, stale rejection, exactly-once terminal
- CPU readback, `preserveDrawingBuffer`, CSS masking, FIFO replay, full-frame provisional stretch 금지
- active browser smoke는 40 samples
- CDP screenshot/geometry PASS를 browser compositor scan-out proof로 확대하지 않음

## 14. 공통 evidence schema

새 Windows run은 최소 다음 필드를 가진다.

### provenance

- run ID, source commit, dirty fingerprint, binary hash
- backend/arm flags
- OS build, GPU/driver, monitor, DPI, refresh
- requested/actual logical/physical outer-window와 derived client rect

### T0/T1 geometry

- input QPC/cursor/intended rect
- `WM_SIZING` proposed rect와 edge
- `WM_WINDOWPOSCHANGING/CHANGED`, `WM_SIZE`, client rect
- handler enter/exit와 blocked duration

### T2/T3 app/surface

- framework/host layout epoch
- render generation, backing/source/backbuffer size
- prebuild ready QPC와 fence
- `ResizeBuffers`/`SetSourceSize` count와 duration
- admitted/superseded/failed terminal reason

### T4 present

- present ID/count, submit/return QPC
- present flags/sync interval/scaling mode
- present queue/frame statistics when available
- app-presented ACK를 scan-out ACK와 구분

### T5 observation

- WGC `SystemRelativeTime`, `ContentSize`, callback entry/exit
- WGC pool capacity, recreate count, ring/encoder drop
- Desktop Duplication frame metadata/QPC
- observed frame/generation marker
- gap/blank/AppBar/circle/title/right-edge oracle
- WGC/output observer discrepancy

### acceptance status

- `valid`, `invalid`, `PASS`, `FAIL`, `notStarted`, `notVerified`, `diagnosticOnly`
- invalid reason은 DPI mismatch, capture drop, input cleanup, focus loss, process failure 등으로 구체화
- raw evidence path와 summary fingerprint

## 15. validation과 실행 규칙

- 모든 test/build/runtime 명령은 repository 지침에 따라 최대 20분 timeout을 사용한다.
- 각 구현 stage는 관련 Release build와 contract를 실행하지만, build PASS를 visible PASS로 기록하지 않는다.
- documentation-only 단계에서는 문서 diff/link/status 검증만 수행한다.
- Web source를 변경하지 않는 M0~C2R에서는 Web runtime validation을 실행하지 않는다.
- process를 시작하기 전 기존 동일 binary/validator process와 command line을 확인한다.
- 실패/예외에서도 mouse up, `WM_CANCELMODE`, timer resolution, process priority, capture session, frame pool, GPU ring, child process를 정리한다.
- unrelated worktree 변경을 stage/commit/delete하지 않는다.

기본 source validation:

```powershell
dotnet run --project Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release
dotnet build DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj -c Release
pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1
git diff --check
```

M0 이후 live validator는 qualified observer가 준비되기 전 기존 strict WGC exit code를 제품 gate로 사용하지 않는다.

## 16. 중단 조건과 금지 사항

### 즉시 중단 조건

- requested/actual DPI/client size mismatch
- WGC/Desktop observer qualification FAIL
- capture ring/drop/timebase 오류
- WndProc에서 unbounded GPU/resource wait
- drag 중 stable-surface arm에서 `ResizeBuffers` 발생
- exact source extent 밖 copy 또는 overflow
- stale/mismatched generation present
- front retirement 전 reuse 또는 unbounded allocation
- product candidate rollback 뒤 startup/input/title/caption 미복구

### 하지 않을 것

- 현재 WGC gap count만으로 app lag 방향 판정
- callback QPC/current `GetWindowRect`를 이전 WGC frame timestamp로 사용
- `encoderDroppedFrames=0`을 WGC frame loss 0으로 해석
- invalid `n0-arm-a-420` evidence를 420×300 gate로 재사용
- M1 전 2-refresh strict gate 실행
- timer/debounce/sample 수/PNG stride로 failure 숨기기
- `DwmFlush`를 geometry+surface atomic transaction으로 취급
- `Present`, commit completion, app ACK를 scan-out ACK로 기록
- visible current front의 destructive resize
- old front full-client/non-uniform stretch
- CPU readback/GDI/bitmap round-trip 제품 경로
- H0 evidence 없이 capacity buffer + `SetSourceSize` 제품 복귀
- H0 PASS 전 새 shell/product scaffolding
- G2 PASS 전 legacy path 삭제
- 새 Flutter runtime instrumentation
- Windows G2 전 Web milestone 승격

## 17. 완료 정의와 현재 상태

이 계획은 다음 조건을 모두 만족해야 완료다.

1. M0/M1에서 observer와 DPI/timebase가 qualification PASS한다.
2. H0-B 또는 H0-C가 valid visible/cadence absolute gate를 PASS한다.
3. H1에서 standard/custom chrome ownership 결론이 확정된다.
4. 선택된 MAUI Composition 또는 pure Win32 제품 경로가 G2를 PASS한다.
5. legacy/experimental path와 package/template/validator identity가 정리된다.
6. active decision 문서가 실제 evidence 상태와 일치한다.
7. 미실행 gate는 구체적인 `notStarted`/`notVerified`로 남는다.

2026-08-23 재검토 직후 상태:

| 단계 | 상태 |
| --- | --- |
| D0 evidence 재분류 | `PASS` — 기존 파일 보존, acceptance 해석 교정 |
| M0 observer/DPI 교정 | `notStarted` |
| M1 observer qualification | `notStarted` / `notVerified` |
| H0-A current control 재측정 | `notStarted` |
| H0-B stable-capacity/prebuild | `notStarted` |
| H0-C custom chrome control | `notStarted` |
| H1 ownership 결정 | `notStarted` |
| 기존 C0 | `PASS` — device removal `notVerified` |
| 기존 C1 | `PASS` — minimum smoke only |
| 기존 C2 | internal cadence `FAIL`; visible verdict `invalidated`/`notVerified` |
| 기존 N0 Arm A | 구조/correctness `PASS`; strict visible verdict `invalid` |
| C2R | `notStarted` |
| P0/G2 | `notStarted` / `notVerified` |
| Web | `notStarted` / `notVerified` |

현재 정확한 재개점은 **M0 DPI-aware sizing과 non-blocking capture ring 설계/구현**이다. Composition presenter tuning, N1 shell migration, Web 작업부터 재개하지 않는다.

## 18. 근거

### 현재 repository

- Windows host/presenters: `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`
- Composition candidate: `Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs`
- direct top-level control: `Doroti/validation/windows-top-level-presentation/Program.cs`
- WGC observer: `Doroti/validation/windows-resize-capture/main.cpp`
- Composition bridge validation: `Doroti/validation/windows-composition-surface/`
- common epoch/transaction: `Doroti/src/Doroti.Ui/ResizeLifecycle.cs`, `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
- renderer: `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- validators: `Doroti/eng/validate-resize-continuity.ps1`, `Doroti/eng/validate-resize-continuity-live.ps1`

### local reference

- Flutter resize protocol: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- Flutter native window ownership: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc`
- Avalonia Composition target/surface: `reference/Avalonia-main/src/Windows/Avalonia.Win32/WinRT/Composition/`
- Avalonia DirectComposition fallback: `reference/Avalonia-main/src/Windows/Avalonia.Win32/DComposition/`

### Microsoft 공식 문서

- [WM_SIZE](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-size)
- [WM_SIZING](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-sizing)
- [Window Features — size and position messages](https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features)
- [DXGI_SCALING](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling)
- [IDXGISwapChain::Present](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present)
- [IDXGISwapChain2::SetSourceSize](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setsourcesize)
- [Reduce latency with DXGI 1.3 swap chains](https://learn.microsoft.com/en-us/windows/uwp/gaming/reduce-latency-with-dxgi-1-3-swap-chains)
- [DwmFlush](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)
- [Windows Graphics Capture](https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture)
- [Desktop Duplication API](https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/desktop-dup-api)
- [Composition native interoperation with DirectX and Direct2D](https://learn.microsoft.com/en-us/windows/apps/develop/composition/composition-native-interop)
- [ICompositionDrawingSurfaceInterop.BeginDraw](https://learn.microsoft.com/en-us/windows/win32/api/windows.ui.composition.interop/nf-windows-ui-composition-interop-icompositiondrawingsurfaceinterop-begindraw)
- [Compositor.RequestCommitAsync](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositor.requestcommitasync?view=windows-app-sdk-1.8)

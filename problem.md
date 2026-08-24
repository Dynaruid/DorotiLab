# Windows 좌측 interactive resize 잔여 문제와 근본 원인

작성일: 2026-08-24

대상: `Doroti/validation/windows-top-level-presentation`의 standard-chrome raw top-level HWND + D3D12/DXGI control

상태: **Arm S/C diagnostic과 custom non-client Arm N 구현 완료, Arm C 필수 3회 observer qualification FAIL, Arm N transaction smoke PASS, 실제 mouse visible acceptance notVerified/기존 M1 FAIL 유지**

## 1. 현재 증상

사용자가 실제 창을 빠르게 조작해 확인한 증상은 다음 순서로 변했다.

1. 초기에는 좌측 edge로 확장할 때 우측에 흰 영역이 나타났고, 좌측 edge로 축소할 때 창이 좌우로 떨렸다.
2. DXGI/GDI background ownership을 명시한 뒤 흰 영역은 사라졌다.
3. 좌측 축소를 동기 `Present`/`DwmFlush` 경로에서 제외한 뒤 축소 떨림은 거의 사라졌다.
4. 그러나 좌측 edge로 조절할 때 content가 창 geometry를 한 박자 늦게 따라오며 우측에 암색/검은 영역이 남는다.
5. 우측 edge는 같은 속도로 조절해도 geometry와 content가 거의 실시간으로 맞는다.

따라서 현재 실패는 “blank 색을 무엇으로 칠할 것인가”가 아니라 **좌측 edge의 화면 좌표 변화와 visible frame의 적용 시점이 일치하지 않는 것**이다.

## 2. 결론

### 2.1 X/Y 산술 오류는 아니다

`WM_SIZING`은 화면 좌표 `RECT`를 제공하고, 현재 control은 이를 authoritative drag rectangle로 사용한다. 좌측 edge에서는 `Left`와 `Width`가 함께 바뀌고 `Right`는 고정되며, 우측 edge에서는 `Left`가 고정되고 `Right`/`Width`만 바뀐다. 이 계산 자체는 정상이다.

현재 남은 문제는 다음 두 상태가 하나의 atomic transaction에 속하지 않는다는 것이다.

- USER32/DWM이 소유하는 standard non-client border와 top-level HWND의 screen-space geometry
- 애플리케이션 render worker가 소유하는 D3D12 backing, DXGI `Present`, compositor-visible frame

즉, 근본 원인은 **좌표값 오류가 아니라 좌표 기준(anchor)과 presentation transaction ownership의 분리**다.

### 2.2 현재 구조에는 동시에 만족할 수 없는 두 선택지가 있다

좌측처럼 window origin이 움직이는 edge에서 현재 raw `CreateSwapChainForHwnd` 경로는 다음 중 하나를 선택한다.

1. exact frame을 `Present`하고 `DwmFlush`까지 기다린 뒤 geometry를 허용한다.
   - uncovered 영역은 줄어든다.
   - window procedure가 GPU/compositor를 기다리므로 포인터보다 창이 늦게 움직이고 미세한 떨림이 생긴다.
2. geometry를 즉시 허용하고 exact frame을 비동기로 따라오게 한다.
   - border와 포인터의 결합은 좋아진다.
   - 이전 frame이 새 origin에 먼저 재배치되므로 고정돼야 할 우측 끝에 uncovered 영역이 생긴다.

지금까지의 수정은 이 trade-off의 양쪽을 오간 것이다. background 색, overscan, latest-only queue는 노출 형태나 stale frame 수를 줄일 수 있지만 두 상태를 atomic하게 만들지는 못한다.

## 3. 좌측에서만 크게 보이는 수학적 이유

좌측 확장을 단순화하면 이전 window는 다음과 같다.

```text
previous outer = [L0, R]
previous width = W0 = R - L0
```

사용자가 좌측 edge를 왼쪽으로 이동하면 새 geometry는 다음과 같다.

```text
new outer = [L1, R], L1 < L0
new width = W1 = R - L1
```

LTR HWND에서 `DXGI_SCALING_NONE`은 back buffer와 target의 **왼쪽 edge를 정렬**한다. exact `W1` frame이 아직 없고 이전 `W0` frame이 새 origin `L1`에 붙으면 이전 content의 화면상 오른쪽 끝은 다음 위치가 된다.

```text
old content right = L1 + W0
required right     = R
right gap          = R - (L1 + W0) = L0 - L1
```

즉, 한 resize sample 동안 좌측 edge가 움직인 거리만큼 우측 gap이 생길 수 있다. 좌측 edge의 invariant는 화면상 `R`인데, `Scaling.None`의 invariant는 target의 왼쪽 origin이다.

우측 edge resize에서는 `Left`와 client origin이 고정된다. 같은 left-aligned back buffer를 사용하므로 이전 frame과 새 target이 같은 origin을 공유한다. 게다가 현재 구현은 fixed-origin expansion을 동기 pre-present하므로 좌측보다 더 정확하게 보인다.

## 4. 현재 코드가 좌우를 다르게 처리하는 지점

`Program.cs`의 현재 control 정책은 의도적으로 비대칭이다.

- `PreparedResizeTarget.RequiresSynchronousPrepresent`
  - `RequiresExpandedCoverage && !OriginMoves`일 때만 `true`다.
- 우측/하단처럼 origin이 고정된 expansion
  - exact frame을 `CommitRenderWindow(...)`로 준비한다.
  - `Present`와 `DwmFlush` 뒤 geometry를 허용한다.
- 좌측/상단처럼 origin이 움직이는 resize
  - `QueueLatestRenderWindow(...)`로 비동기 게시한다.
  - geometry는 GPU/compositor를 기다리지 않고 즉시 허용한다.
- 축소
  - 이전 큰 front를 crop하고 새 exact raster를 비동기로 게시한다.

따라서 “우측은 실시간인데 좌측은 늦고 우측에 검은 영역이 보인다”는 사용자 관찰은 현재 분기와 정확히 일치한다. 이 차이는 우연한 driver 현상이나 측정 노이즈만으로 설명할 수 없다.

## 5. 배제된 원인

### 5.1 단순 outer/client 좌표 계산

- `WM_SIZING` suggested outer rect와 `WM_WINDOWPOSCHANGING` target을 함께 추적한다.
- outer rect, non-client extent, content size exact matcher가 존재한다.
- 최근 control run에서 prepared outer mismatch와 timeout은 0이었다.
- 오른쪽 edge가 안정적이라는 사실도 공통 width/height 산술 오류 가능성을 낮춘다.

판정: **주원인 아님**.

### 5.2 `ResizeBuffers` hot-path stall

- swap chain/backing capacity는 monitor work-area 이상으로 유지된다.
- ordinary interactive resize마다 `ResizeBuffers`하지 않는다.

판정: **현재 control의 반복 증상에 대한 주원인 아님**. 제품 host의 exact-size surface에서는 별도 위험으로 남는다.

### 5.3 부모/child HWND 간 z-order 또는 size race

- 현재 control은 render child 없이 standard-chrome top-level HWND 하나가 geometry와 swap chain을 함께 소유한다.
- 그런데도 좌측 phase 차이가 남는다.

판정: **현재 control에서는 배제됨**. 다만 향후 제품의 top-level + render child 구조에서는 다시 검증해야 한다.

### 5.4 흰색 기본 background 또는 미초기화 픽셀

- Win32 class brush와 `WM_ERASEBKGND`를 명시했다.
- `IDXGISwapChain1.BackgroundColor`도 같은 암색으로 지정했다.
- backing capacity 전체를 clear하고 retained-front overscan도 유효한 배경으로 채운다.
- 흰 영역은 사라졌지만 frame 추종 지연은 남았다.

판정: **흰색 증상의 직접 원인이었지만 transaction 문제의 근본 원인은 아님**.

### 5.5 stale FIFO replay

- resize target은 latest-only로 coalesce한다.
- render 도중 더 최신 target이 오면 중간 frame을 visible present하지 않도록 했다.
- resize publish는 `Present(0)`을 사용한다.
- 그 뒤에도 사용자가 좌측 지연/gap을 재현했다.

판정: **증폭 요인이었지만 근본 원인은 아님**.

### 5.6 Skia draw 속도

- 과거 측정에서 raster 자체보다 target-to-ACK와 surface/present 구간이 더 컸다.
- 최신 control의 ordinary animation은 약 165fps를 유지했고 blank/timeout/mismatch는 0인 run이 있었다.
- 사용자에게 보이는 실패는 edge 방향과 origin 이동 여부에 강하게 종속된다.

판정: **주원인 아님**.

## 6. 지금까지의 완화책이 끝까지 해결하지 못한 이유

| 변경 | 개선된 것 | 남은 한계 |
|---|---|---|
| outer X/Y 추적 | target rect mismatch 감소 | geometry와 present는 여전히 별도 제출 |
| `DwmFlush` before geometry | frame-before-border 가능성 증가 | caller를 다음 composition까지 막아 pointer lag/jitter 발생 |
| monitor-sized stable capacity | `ResizeBuffers` 제거 | 이전 content의 screen-space anchor는 바뀌지 않음 |
| single top-level HWND | cross-HWND ordering 제거 | USER32 geometry와 DXGI flip은 여전히 서로 다른 상태 전이 |
| explicit DXGI/GDI background | 흰 영역 제거 | uncovered phase가 암색으로 보일 뿐임 |
| async shrink/origin-moving publish | 좌측 축소 떨림 감소 | geometry-before-content gap 허용 |
| latest-only + `Present(0)` | stale intermediate frame/추가 refresh wait 제거 | 최신 frame도 DWM geometry와 같은 atomic commit은 아님 |
| valid retained-front overscan | 단색 fallback 노출 감소 | 실제 layout/right-edge oracle는 exact frame 전까지 이전 세대 |

### 6.1 자동 observer가 해결을 선언할 수 없는 이유

최근 ad hoc qualification에서는 앱 counter가 PASS이고 blank/timeout/mismatch가 0이어도, 같은 코드에서 phase 판정이 run마다 PASS/FAIL로 바뀌었다. WGC right-gap 진단도 최대 8px 수준을 계속 보고했지만 WGC callback/frame-pool 위상 자체가 strict output oracle로 확정되지 않았다. 따라서 `work2.md`의 M1은 여전히 FAIL이고 WGC는 `diagnosticOnly`다.

반대로 사용자는 실제 standard border drag에서 동일한 좌측 지연과 우측 암색 영역을 반복 재현했다. 현재 단계에서는 자동 PASS 한 번보다 이 visible 재현이 더 강한 acceptance evidence다.

## 7. API 계약과 현재 한계

Microsoft 문서상 다음 사실은 현재 판정과 일치한다.

- [`WM_SIZING`](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-sizing)은 screen-coordinate drag `RECT`를 제공한다. 이는 geometry 제안이지 DXGI frame commit token이 아니다.
- [`DXGI_SCALING_NONE`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling)은 LTR HWND에서 back buffer와 target의 왼쪽 edge를 맞추며, target 밖 영역은 background color로 채운다. 우측 고정 left-edge resize에 필요한 right-edge anchor 계약이 아니다.
- [`DwmFlush`](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)는 호출 애플리케이션의 queued DirectX 변경이 화면에 그려질 때까지 caller를 막는다. USER32 window rect와 DXGI present를 하나의 transaction으로 묶는 API는 아니다.
- [DirectComposition architecture](https://learn.microsoft.com/en-us/windows/win32/directcomp/architecture-and-components)는 visual 변경을 `Commit` batch로 모아 composition frame 시작 시 atomic하게 적용한다. 현재 direct-HWND swap-chain control에는 edge-dependent visual transform/clip과 exact content swap을 묶는 이런 별도 visual transaction owner가 없다.

따라서 raw standard-chrome HWND의 geometry 변경과 `CreateSwapChainForHwnd`의 `Present` 순서만 조정해서는 아래 세 조건을 동시에 보장할 수 없다.

1. 좌측 border가 포인터를 지연 없이 따라간다.
2. content의 화면상 우측 edge가 매 sample 고정된다.
3. provisional stretch나 uncovered background가 한 frame도 없다.

## 8. 근본 해결 방향

### 8.1 현재 경로에서 중단해야 할 조정

다음 변경은 같은 trade-off를 반복하므로 근본 해결로 간주하지 않는다.

- `DwmFlush` 위치나 timeout만 조정
- 더 많은 background/overscan/mask 추가
- resize event debounce
- FIFO depth 변경
- outer rect에 임의의 1~8px 보정값 추가
- 자동 observer PASS만으로 visible PASS 선언

### 8.2 선택지 A: transient scaling을 명시적으로 허용

`DXGI_SCALING_STRETCH` 또는 동등한 compositor provisional scale로 이전 front를 새 client 전체에 즉시 맞추고 exact frame이 준비되면 교체한다.

장점:

- uncovered gap 없이 standard HWND geometry를 즉시 따라가기 쉽다.
- 구현 위험이 가장 낮다.

단점:

- exact frame 전 한두 frame 동안 비등방 왜곡이 생길 수 있다.
- 현재 1:1/no-stretch 목표와 충돌한다.

이 선택지는 “gap 없음”을 “transient distortion 없음”보다 우선할 때만 채택한다.

### 8.3 선택지 B: composition visual이 provisional anchor를 소유

swap chain/retained front를 composition visual에 붙이고 resize direction별 transform과 clip을 명시한다.

- 좌측 expansion에서는 이전 front의 화면상 오른쪽 edge가 `R`에 고정되도록 translate/clip한다.
- 새로 생긴 영역은 정의된 resize background로 채운다.
- exact frame, visual transform, clip 변경을 하나의 compositor batch로 교체한다.
- 우측 expansion은 left anchor, 좌측 expansion은 right anchor를 사용한다.

이 방향은 current `Scaling.None`의 고정 left-anchor를 edge-aware visual transaction으로 대체한다. 다만 standard non-client HWND geometry 자체는 USER32/DWM 소유이므로, 실제 spike에서 geometry와 visual batch의 phase가 충분히 결합되는지 visible gate로 확인해야 한다.

### 8.4 선택지 C: custom non-client resize까지 전체 composition owner로 이동

1:1 content, no gap, no transient stretch, pointer coupling을 모두 강한 요구사항으로 유지한다면 가장 근본적인 방향이다.

- standard non-client live resize에 의존하지 않는다.
- 앱이 resize hit-test/capture와 chrome visual을 소유한다.
- border/chrome/content의 provisional geometry를 같은 composition tree에서 갱신한다.
- exact frame 준비 뒤 실제 HWND placement를 안정된 경계에서 정리한다.

비용은 가장 크며 accessibility, snap layouts, system menu, DPI, keyboard sizing, touch, maximize/restore를 모두 재구현·검증해야 한다.

## 9. 권고 결정

현재 control에 추가 timer/flush/background 패치를 계속하지 않는다. 이 control은 다음 사실을 입증한 diagnostic으로만 유지한다.

- 단일 HWND와 stable-capacity D3D12 front만으로도 white blank와 resize allocation은 제거할 수 있다.
- 그러나 standard non-client origin-moving geometry와 left-aligned `Scaling.None` present는 strict left-edge continuity를 보장하지 못한다.

다음 구현 spike는 두 arm을 명시적으로 비교해야 한다.

1. **Arm S:** transient `STRETCH`를 허용해 gap/lag가 모두 사라지는지 확인한다.
2. **Arm C:** composition visual의 edge-aware translate/clip + exact swap transaction을 구현한다.

Arm C가 strict 1:1 visual gate를 통과하면 목표 구조로 채택한다. 통과하지 못하고 Arm S만 사용자가 수용 가능하면 no-gap 우선 정책으로 요구사항을 변경한다. 둘 다 실패하면 custom non-client owner가 필요하다고 판정한다.

## 10. 필요한 acceptance evidence

자동 counter나 build만으로 해결을 선언하지 않는다. 최소한 다음을 모두 확인한다.

- 실제 mouse로 left/right/top/bottom 및 네 corner를 빠르게 왕복
- expansion과 shrink를 분리해 판정
- 100%, 150%, 200% DPI
- 60Hz, 120Hz 이상 refresh 환경
- 좌측 edge의 screen-space right invariant 오차
- border position과 content right edge의 같은 output-frame timestamp
- uncovered white/black/solid-background frame 0
- transient non-uniform scale frame 0 또는 정책상 명시된 허용치
- pointer-to-border lag와 역방향 jitter 0
- 세 번 연속 valid observer + 사용자 visible acceptance

현재 `work2.md`의 M1/G2 상태는 그대로 유지한다. 이 문서는 원인 판정이며 제품 구현 PASS나 migration 승인을 의미하지 않는다.

## 11. 2026-08-24 구현 결과와 정확한 재개점

### 11.1 구현한 두 arm

`Doroti/validation/windows-top-level-presentation`에 기존 Arm A 기준선을 보존하고 다음 두 경로를 추가했다.

- `--arm S`
  - direct-HWND swap chain을 `DXGI_SCALING_STRETCH`로 만든다.
  - monitor-sized backing capacity는 유지하고 exact content rect를 `IDXGISwapChain2.SetSourceSize()`로 게시한다.
  - geometry는 `Present`/`DwmFlush`를 기다리지 않으며 다음 exact source가 오기 전 이전 source의 transient stretch를 명시적으로 허용한다.
- `--arm C`
  - `CreateSwapChainForComposition` + native DirectComposition target/visual을 사용한다.
  - origin-moving left/top edge는 반대편 screen edge가 실제로 고정된 경우에만 이전 front를 translate한다. 단순 window move와 임의 `SetWindowPos`는 edge anchor로 오인하지 않는다.
  - 이전 content와 새 client의 교집합만 1:1 clip하고, exact frame 뒤 offset/clip을 초기화하는 DirectComposition batch를 commit한다.
  - composition swap chain에는 `SetSourceSize()`를 사용하지 않는다. capacity 전체로의 uniform scale이 재현되었기 때문에 monotonic backing을 자연 크기 1:1로 유지하고 visual clip만 사용한다.
  - provisional/exact commit은 visual epoch로 결합하며 stale exact commit은 visible present 전에 거부한다.
  - composition swap chain의 `Present(1)`이 caller를 cadence에 맞춰 block하지 않으므로 frame-latency waitable object를 explicit pacing owner로 사용한다.

interop은 repository의 기존 Vortice 계열과 같은 `3.8.3`으로 `Vortice.DirectComposition`을 정확히 pin했다. 이는 Windows App SDK 제품 migration을 대신하지 않는다. 제품 host의 Windows App SDK 기준은 사용자의 결정대로 최신 stable `2.4.0` exact pin이며, 현재 diagnostic control은 SDK version과 독립된 transaction 검증용이다.

`validate-windows-presentation-observer.ps1`은 `-Arm A|S|C`를 받고 arm별 evidence/summary를 분리한다.

### 11.2 drag 속도 변경

사용자 요청에 따라 native capture와 PowerShell live validator의 triangle-wave 주기를 모두 2초에서 1초로 줄였다.

- 이동 폭: horizontal 260px, vertical 140px 유지
- input cadence: active display refresh 기준 유지
- 방향 전환: 약 0.5초마다
- 전체 확대→축소 왕복: 약 1.0초
- 결과: 이전보다 pointer 이동 속도 정확히 2배

최신 Arm C raw evidence는 10초 interactive-left-drag에서 165Hz, 1651 drag samples, 19 direction reversals를 기록했고 최초 전환들은 0.51s, 1.01s, 1.51s, 2.01s였다.

### 11.3 현재 evidence

구조/build:

- `dotnet build ...windows-top-level-presentation... -c Release --no-restore`: **PASS**, 0 warnings, 0 errors
- native `windows-resize-capture` Release build: **PASS**
- A/S/C startup + repeated `SetWindowPos` resize smoke: **PASS**, process failure 0
- Arm C smoke: provisional 4, exact 4, stale exact reject 0

observer:

- Arm S 1회: **FAIL**
  - source 165.179fps, static false failure 0, Desktop Duplication qualified
  - phase 7/8 PASS, zero-offset content-before-geometry 1건 실패
  - summary: `win-observer-m1-arm-s-summary-20260824-173953.json`
- Arm C isolated 1회: **PASS**
  - source 165.011fps, static false failure 0, phase 8/8
  - provisional/exact 1/1, stale reject 0
  - summary: `win-observer-m1-arm-c-summary-20260824-174448.json`
- Arm C required 3회: **FAIL** (`PASS`, `FAIL`, `FAIL`)
  - run 2: phase 7/8, zero-offset content-before-geometry 실패
  - run 3: phase 7/8 + static failure 58, provisional/exact 391/381, stale reject 1
  - summary: `win-observer-m1-arm-c-summary-20260824-174645.json`
- 2배 drag 적용 후 Arm C 1회: **FAIL**
  - source 165.060fps, static false failure 0, Desktop Duplication qualified
  - phase 6/8, provisional/exact 1/1, stale reject 0
  - summary: `win-observer-m1-arm-c-summary-20260824-174844.json`
- 최종 source fingerprint와 일치하는 2배 drag Arm C 단일 회귀: **PASS**
  - source 164.954fps, static false failure 0, Desktop Duplication qualified
  - phase 8/8, provisional/exact 1/1, stale reject 0
  - summary: `win-observer-m1-arm-c-summary-20260824-175318.json`

WGC strict judge는 계속 `diagnosticOnly`이고 Desktop Duplication만 해당 run에서 `qualified`였다. isolated PASS 하나는 3회 연속 조건이나 visible acceptance를 대체하지 않는다.

### 11.4 판정과 재개점

- Arm S는 transient distortion을 허용하는 정책 비교군으로 구현됐지만 자동 phase gate부터 FAIL했다.
- Arm C는 구조적으로 실행되고 isolated PASS도 만들었으나 3회 observer 결과가 재현 가능하지 않다.
- 최종 2배 속도 단일 회귀에서 process crash, stale exact visible commit, cadence 이탈은 관측되지 않았지만 실제 사용자 visible acceptance는 아직 수행하지 않았다.
- 이 단일 PASS는 앞서 실패한 필수 3회 연속 qualification을 대체하지 않는다.
- 따라서 M1은 **FAIL — hard stop**, G2는 `notVerified`, 제품 migration 승인은 하지 않는다.

정확한 재개점은 다음 두 가지다.

1. Arm C에서 content-before-geometry offset 0/1 phase가 run마다 반대로 관측되는 원인을 raw Desktop Duplication timestamp와 DirectComposition commit epoch로 좁힌다.
2. 수정 뒤 같은 환경 3회 연속 qualified PASS를 확보하고, 1초 triangle wave로 실제 mouse left/right/top/bottom/corner visible matrix를 사용자가 확인한다.

그 전에는 custom non-client Arm으로 확대하거나 A0-V 이후 제품 구현을 시작하지 않는다.

## 12. 2026-08-24 좌측 떨림 후속 수정

### 12.1 확인한 구현 결함

Arm C의 visual epoch가 latest인지 확인하는 것만으로는 충분하지 않았다. `WM_SIZING`에서 exact raster를 미리 queue한 뒤 render worker가 HWND geometry 적용 전에 `PresentPrepared(epoch)`를 실행할 수 있었고, 이때 provisional right-anchor offset/clip이 0으로 초기화됐다. 즉 같은 epoch 안에서도 다음 순서가 허용됐다.

```text
provisional visual commit
  -> exact raster ready
  -> exact Present + offset reset
  -> matching HWND geometry admission
```

좌측 edge에서는 exact reset 전후로 content screen origin이 바뀌므로 사용자가 본 좌우 떨림을 직접 만들 수 있다. 기존 stale check는 다른 epoch의 exact commit만 막았고 **같은 epoch의 geometry-before-admission commit**은 막지 못했다.

추가로 다음 두 결함을 확인했다.

- `SetWindowPos`가 `SWP_NOSIZE`를 명시하지 않았다는 이유만으로 실제 width/height가 같은 pure move까지 resize visual epoch로 만들었다. pure move에는 matching `WM_SIZE`가 없으므로 admission이 닫힌 채 남을 수 있었다.
- platform thread의 `ApplyProvisionalVisual()`이 render worker의 GPU wait/copy/present 전체와 같은 `_presentGate`를 사용했다. left drag의 `WM_SIZING`이 GPU resource wait 뒤에 막힐 수 있는 구조였다.

### 12.2 수정한 transaction

`Doroti/validation/windows-top-level-presentation/Program.cs`를 다음처럼 수정했다.

1. Arm C visual epoch에 `geometryAdmittedEpoch`를 추가했다.
2. matching outer/client rect를 확인한 `WM_SIZE`만 해당 epoch를 admit한다.
3. early exact raster는 허용하지만 admission 전 `Present`와 provisional offset reset은 거부한다.
4. `WM_SIZE` admission 뒤 target version을 강제로 다시 게시해, admission 전에 준비가 끝난 frame도 최신 geometry에서 visible publish를 재시도한다.
5. pure move는 outer width/height 비교로 resize preparation에서 제외한다.
6. DirectComposition state 전용 `_compositionGate`를 두고 GPU wait/copy와 분리했다. final visible boundary에서 epoch/admission을 다시 확인한 뒤 `Present + CommitExact`만 짧게 직렬화한다.
7. admission 때문에 보류된 animation frame은 presented cadence로 세지 않고 bounded wait한다.
8. evidence에 provisional/exact 외에 pre-admission reject, geometry admission, admission reject counter를 추가했다.

이제 허용되는 순서는 다음과 같다.

```text
WM_SIZING provisional commit + exact raster prepare
  -> matching WM_SIZE geometry admission
  -> latest/admitted epoch 재확인
  -> exact Present + provisional offset reset
```

### 12.3 현재 검증 결과

구조/build:

- Release `dotnet build --no-restore`: **PASS**, 0 warnings, 0 errors
- `git diff --check`: **PASS**
- final source에서 pure move가 orphan visual epoch를 만들지 않음

최종 3회 observer:

- summary: `win-observer-m1-arm-c-summary-20260824-181313.json`
- verdict: **FAIL** (`PASS`, `FAIL`, `FAIL`)
- source cadence: 세 run 모두 약 165fps
- prepared outer mismatch: 세 run 모두 0
- geometry admission reject: 세 run 모두 0
- interactive left drag blank frame: 세 run 모두 0
- Desktop Duplication right gap diagnostic maximum: 세 run 모두 8px
- run 2/3 실패: synthetic `content-before-geometry` 0/1-refresh phase oracle 변동

앱 counter는 admission 계약이 동작했음을 보였지만 observer 3회 연속 조건은 통과하지 못했다. 또한 자동 triangle wave는 실제 mouse에서 사용자가 느낀 떨림 제거를 대신 증명하지 않는다.

따라서 현재 판정은 다음과 같다.

- 같은 epoch의 **exact-before-geometry** 경쟁과 pure-move orphan epoch는 코드에서 수정됨
- Release build와 transaction counter는 PASS
- 필수 3회 observer는 FAIL
- 실제 mouse 좌측 edge visible acceptance는 `notVerified`
- M1은 계속 **FAIL — hard stop**, G2는 `notVerified`

정확한 다음 확인은 final binary Arm C에서 실제 mouse로 좌측 확대/축소를 빠르게 왕복해 떨림이 사라졌는지 사용자가 확인하는 것이다. 떨림이 남으면 admission 이후의 exact reset이 아니라 `WM_SIZING` provisional commit 자체와 standard non-client geometry 사이의 output-frame phase가 원인이므로, 현재 raw top-level standard-chrome Arm C를 더 조정하지 않고 custom non-client owner 판단으로 넘어간다.

## 13. 2026-08-24 좌측·상단 떨림 재현 후 custom non-client Arm N

### 13.1 추가 관찰과 판정

사용자가 final Arm C에서도 좌측 떨림을 재현했고, 상단을 위로 키울 때도 같은 떨림을 확인했다. 좌측과 상단은 모두 outer origin이 움직이는 경로이며 `ApplyProvisionalVisual()`이 exact frame 전에 각각 X/Y offset을 commit한다. 따라서 이 관찰은 좌측 산술이나 right-edge oracle 문제가 아니라 다음 공통 순서가 output frame 사이에 노출된다는 판정과 일치한다.

```text
WM_SIZING: provisional visual offset/clip commit
  -> 별도 USER32 standard non-client geometry admission
  -> WM_SIZE: exact visual reset
```

Arm C의 geometry admission gate는 exact reset이 geometry보다 앞서는 경쟁은 막지만, 첫 provisional DirectComposition commit과 standard HWND geometry를 하나의 compositor transaction으로 만들 수는 없다. 상단 재현으로 Y축에서도 같은 한계가 확인됐으므로 Arm C의 offset/flush/timer를 더 조정하지 않았다.

### 13.2 구현한 ownership 변경

`Doroti/validation/windows-top-level-presentation`에 `--arm N`을 추가했다.

- 실제 top-level HWND는 현재 monitor work area 크기의 투명한 `WS_POPUP + WS_EX_NOREDIRECTIONBITMAP` envelope로 고정한다.
- 사용자가 보는 border, title 영역과 content만 하나의 DirectComposition visual에 둔다.
- envelope 바깥과 visual 바깥은 `HTTRANSPARENT`이고, 보이는 surface의 8-DIP 안쪽 border만 앱이 hit-test/capture한다.
- 좌측·상단·우측·하단과 corner drag는 실제 HWND rect를 바꾸지 않고 app-owned rect만 갱신한다.
- render worker는 latest exact raster를 준비하고, visible boundary에서 swap-chain `Present`와 visual offset/clip을 같은 composition owner 아래 commit한다.
- intermediate target은 latest-only로 supersede하며 stale visual epoch는 visible commit하지 않는다.
- surface 전체 border도 content와 같은 Skia front에 그리므로 border와 content가 서로 다른 owner에서 이동하지 않는다.
- top title band drag는 같은 visual rect를 이동하고 `Alt+F4`로 종료한다.

이 구조에서는 좌측/상단 drag 중 USER32 standard border geometry가 존재하지 않는다. 따라서 Arm C에서 떨림을 만든 `standard HWND origin change ↔ provisional visual offset`의 두-owner phase 자체가 제거된다.

### 13.3 현재 검증 결과

구조/build:

- Release `dotnet build --no-restore`: **PASS**, 0 warnings, 0 errors
- `git diff --check`: **PASS**
- Arm N startup/automatic close: **PASS**, exit code 0

Arm N opt-in transaction smoke (`--owned-smoke`) 3회:

- 좌측 expansion/shrink → 상단 expansion/shrink → 좌상단 corner expansion/shrink
- target input: 119
- render publish: 119
- composition-owned visible commit: 세 run 모두 118
- superseded intermediate frame: 각 run 1
- stale epoch reject: 0~1, stale frame visible commit 0
- latest epoch/committed epoch: 세 run 모두 120/120
- drain timeout: 세 run 모두 0
- process failure: 세 run 모두 0
- verdict: **PASS/PASS/PASS**

외부 pointer 주입은 실행 환경 정책에서 허용되지 않아 자동 smoke는 실제 mouse input을 대체하지 않는다. 실제 mouse로 좌/우/상/하와 네 corner를 빠르게 왕복하는 visible acceptance는 `notVerified`다.

### 13.4 제품 승격 전 남은 경계

Arm N은 transaction ownership을 검증하는 diagnostic이며 현재 standard Windows chrome 기능을 완성한 제품 host가 아니다. 다음 항목은 아직 구현·검증하지 않았다.

- Snap Layouts, system menu, maximize/restore, keyboard sizing
- accessibility와 standard caption/button semantics
- DPI/monitor 전환과 multi-monitor envelope migration
- touch resize, taskbar/foreground/z-order 정책
- 실제 mouse에서 pointer lag, 역방향 jitter, blank/gap 0 확인
- 100/150/200% DPI와 60/120Hz 이상 visible matrix

따라서 Arm N smoke PASS로 `work2.md` M1이나 G2를 승격하지 않는다. 현재 M1은 계속 **FAIL — hard stop**, G2는 `notVerified`다. 정확한 다음 단계는 Arm N final binary를 실제 mouse로 먼저 확인하는 것이며, visible PASS 뒤에만 custom chrome의 Windows 기능 계약을 제품 migration 계획에 포함한다.

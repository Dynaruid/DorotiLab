# Windows Composition surface 기반 interactive resize 후속 작업계획

## 0. 문서 상태와 검토 결론

- 작성일: 2026-08-23
- 기준 checkout: `753a8df4`
- 입력 문서: 현재 `idea.md`, `work.md`
- 검토 범위: 현재 Windows host/source와 package graph, 고정 Flutter source, 로컬 Avalonia source, Microsoft 공식 Composition/D3D11On12 API 문서
- 상태: **계획 작성만 완료**. 이 문서의 C0~C3, N0~N1 구현, build, runtime, WGC, 제품 acceptance는 모두 `notStarted`/`notVerified`다.
- 기존 증거는 `work.md`에서 인용하되 재실행한 것으로 쓰지 않는다. `work.md`의 기준 checkout은 `21320c77`, 현재 checkout은 `753a8df4`다.
- 이 문서는 `work.md`의 실패 기록을 덮어쓰지 않는다. `work.md`의 M0/G0 PASS, W0 runtime correctness PASS, D3D12 ordering/ANGLE 및 G1 visual/cadence FAIL을 선행 사실로 둔다.

`idea.md`의 핵심 방향은 채택한다. 다음 Windows 후보는 raw child HWND의 resize 순서를 더 조정하는 경로가 아니라, **보이는 content를 WinUI composition visual 하나로 모으고 exact staging surface만 교체하는 경로**다. 이 경로가 strict WGC gate를 통과하지 못하면 **순수 Win32 top-level ownership A/B**로 이동한다.

다만 다음 사항은 `idea.md`보다 엄격하게 수정한다.

| 검토 항목 | 판정 | 작업계획 반영 |
| --- | --- | --- |
| raw child HWND와 `SwapChainPanel`을 후보 visible path에서 제거 | 채택 | C1부터 opt-in 후보 경로에서 제거한다. 기존 경로는 paired baseline/rollback용으로 G1 통과 전까지 보존한다. |
| `CompositionDrawingSurface` exact staging 후 surface 교체 | 채택 | visible front는 그 surface가 front인 동안 `Resize`/`BeginDraw`하지 않는다. |
| visual property와 surface 교체의 한 commit | 조건부 채택 | 같은 compositor commit cycle에 넣되, DWM non-client geometry와 WinUI commit까지 원자적이라고 주장하지 않는다. WGC만 border/content phase를 판정한다. |
| `BeginDraw` texture를 바로 D3D12 resource로 unwrap | 선행 증명 필요 | app이 만든 동일 D3D11On12 device/queue 위 resource인지, 실제 `UnwrapUnderlyingResource`가 성공하는지 C0에서 증명한다. 성공을 문서만으로 가정하지 않는다. |
| front/staging 즉시 역할 교환 | 수정 | old front가 visual tree에서 detach됐다고 확인하기 전 staging으로 재사용하지 않는다. bounded surface pool과 retirement 신호가 성립하지 않으면 C0 FAIL이다. |
| `WM_SIZE`가 MAUI content의 exact size authority | 수정 | MAUI 경로의 exact content epoch은 XAML host의 실제 layout size와 rasterization scale로 만든다. `WM_SIZING`/top-level `WM_SIZE`는 edge/provisional intent 관찰용이며 실제 `WM_SIZE`는 WinUI에 항상 전달한다. |
| `RequestCommitAsync` completion을 화면 표시 ACK로 사용 | 거부 | `CommitRequested`/`CommitActionCompleted` 내부 telemetry로만 기록한다. `VisibleSurfaceCommitted`도 app-side ownership 단계이며 scan-out 증거가 아니다. |
| `ContentIsland`를 초기 spike에 포함 | 보류 | C0/C1은 `ElementCompositionPreview.SetElementChildVisual`만 사용한다. `ContentIsland`는 C2 통과 뒤 실제 input/accessibility 경계가 필요할 때만 별도 C3 후보로 검토한다. |
| Flutter runtime baseline 재실행 | 제외 | Flutter는 고정 source와 `work.md`의 과거 evidence만 참고한다. 새 Flutter instrumentation/build/capture/A-B는 수행하지 않는다. |

Ordered hard gate는 다음과 같다.

```text
R0: source/evidence/baseline 고정
  -> C0: Composition surface + GPU bridge feasibility
      FAIL -> composition branch 중단, 이후 단계 notStarted
  -> C1: MAUI attached visual 최소 연결
      API/attachment FAIL -> N0 가능성만 별도 판정
  -> C2: MAUI composition strict WGC
      PASS -> C3: MAUI 제품 통합 + Windows G2
      FAIL -> rollback -> N0: pure Win32 top-level A/B
  -> N0 PASS -> N1: Win32 shell 제품 통합 + Windows G2
  -> C3 또는 N1의 Windows G2 PASS 뒤에만 Web 재개
```

앞 단계가 FAIL이면 뒤 단계는 실행하지 않고 `notStarted`/`notVerified`로 남긴다.

## 1. 보존할 사실과 폐기할 ownership

### 1.1 보존할 구현과 계약

- `DorotiResizeEpoch`, build token, exact-size admission, `FrameTransaction`, exactly-once terminal ledger를 유지한다.
- scene은 build 시작 시 캡처한 logical/physical size, scale, target generation, metrics generation을 끝까지 유지한다.
- submit/present 직전에 최신 target으로 scene이나 backing-store identity를 재라벨하지 않는다.
- framework/raster mailbox는 `current + latest` bound를 유지한다.
- exact D3D12/Skia offscreen backing store와 GPU-only path를 재사용한다. renderer 교체는 이번 후보의 첫 수단이 아니다.
- startup visibility와 mouse hover/click/capture 회귀 수정은 보존한다. composition 후보에서도 최소 smoke로 재확인한다.
- `EndDraw`, GPU fence, `RequestCommitAsync`, `Present`, `DwmFlush`를 실제 scan-out ACK라고 부르지 않는다.

### 1.2 후보에서 폐기할 현재 visible ownership

```text
WinUI/MAUI top-level HWND + XAML compositor
  -> raw STATIC child HWND
     -> CreateSwapChainForHwnd + D3D12 Present
```

현재 raw child는 `DorotiWindowsDxgiSurface.cs`의 `WindowsClientResizeSource`가 생성하고, parent/child subclass, child geometry, mouse input을 함께 소유한다. `WindowsHwndD3D12Presenter`는 같은 child HWND에 flip-model swap chain을 연결한다.

이 구조에서는 다음 사건이 한 transaction이 아니다.

1. DWM의 top-level border/client geometry 변경
2. WinUI의 XAML layout/composition 처리
3. app의 child HWND 이동/확장
4. child HWND swap-chain exact buffer present

C 후보는 3번과 4번의 별도 HWND boundary를 제거한다. 그러나 1번과 WinUI visual commit까지 하나의 API transaction이 되는 것은 아니다. 따라서 C 후보는 **원자성 보장으로 채택하는 것이 아니라, cross-HWND ownership 제거 가설을 strict WGC로 검증하는 것**이다.

## 2. C 후보 — MAUI/WinUI Composition surface presenter

### 2.1 visible graph

```text
DWM top-level/non-client geometry
  -> WinUI XAML layout
     -> DorotiWindowsDxgiHost UIElement
        -> attached ContainerVisual
           -> SpriteVisual
              -> CompositionSurfaceBrush
                 -> immutable exact front CompositionDrawingSurface
```

- `DorotiWindowsDxgiHost`는 layout, pointer/key/focus 연결 지점이다.
- `SpriteVisual` 하나만 Doroti scene의 visible owner다.
- `ElementCompositionPreview.SetElementChildVisual(host, visual)`로 연결한다.
- `SetElementChildVisual`은 visual을 host visual tree의 마지막 child로 올리므로, semantics overlay/hidden text input과의 실제 z-order 및 hit-test를 C1에서 확인한다.
- C0/C1에서는 `ContentIsland`, `ChildSiteLink`, 별도 accessibility root를 만들지 않는다.

### 2.2 size authority와 좌표 계약

MAUI 경로에서는 window intent와 content exact target을 분리한다.

- `WM_SIZING`
  - 현재 drag edge/corner와 suggested top-level rect만 기록한다.
  - provisional anchor 방향을 정하는 입력이다.
  - framework scene의 exact metrics로 직접 승격하지 않는다.
- top-level `WM_SIZE`/`WM_DPICHANGED`
  - intent/DPI 변경을 알리되 원래 메시지를 삼키지 않고 WinUI에 전달한다.
  - XAML layout이 아직 반영되지 않은 값을 scene descriptor로 재라벨하지 않는다.
- `DorotiWindowsDxgiHost` 실제 layout + `XamlRoot` scale
  - candidate content의 authoritative logical size와 rasterization scale이다.
  - `physicalWidth/Height = round(logicalWidth/Height * scaleX/Y)`를 한 epoch에 고정한다.
  - 0×0, unloaded, minimize target은 terminal reason을 남기고 surface를 만들지 않는다.

좌표는 다음 세 층을 trace에 별도 기록한다.

| 층 | 단위 | 필수 필드 |
| --- | --- | --- |
| XAML host/visual | logical/effective pixels | logical size, visual size, offset, clip |
| drawing surface | physical pixels | surface size, BeginDraw update rect/offset, DXGI format |
| screen/WGC | physical pixels | window/client/content rect, monitor DPI, refresh |

physical surface를 logical visual에 표시하기 위한 DPI transform은 허용한다. 금지하는 것은 **이전 front를 새 logical target 전체에 맞추는 provisional stretch**다. C0에서 brush `Stretch`, scale/transform, pixel rounding을 고정하고 1px border/checkerboard/circle oracle로 1:1 screen mapping을 확인한다.

### 2.3 surface state와 bounded lifetime

surface slot의 상태는 다음 순서만 허용한다.

```text
Free
  -> Drawing(BeginDraw)
  -> GpuWorkQueued
  -> DrawEnded(EndDraw)
  -> PendingVisualCommit
  -> Front
  -> Retired
  -> Free
```

필수 조건:

- 한 `CompositionGraphicsDevice`에서 active `BeginDraw`는 하나만 둔다.
- `BeginDraw`가 돌려준 pointer는 `EndDraw` 뒤 저장하거나 사용하지 않는다.
- visible `Front`에는 `Resize`/`BeginDraw`를 호출하지 않는다.
- `PendingVisualCommit`은 동시에 하나만 허용한다. 그동안 들어온 target은 `latest` 하나로 coalesce한다.
- old front는 visual이 새 surface로 전환됐다는 **검증된 retirement signal** 전에는 `Free`가 아니다.
- `RequestCommitAsync` 호출 또는 반환만으로 retirement를 가정하지 않는다. C0에서 resolved SDK의 동작과 API contract를 확인해 안전한 signal을 고정한다.
- 안전한 bounded retirement를 만들 수 없으면 surface를 generation마다 계속 할당하지 않고 C0 FAIL로 끝낸다.
- 초기 pool은 최대 3 slots를 상한으로 한다. pool이 모두 front/pending/retired이면 새 allocation 대신 latest target만 보존한다.
- stale epoch의 `DrawEnded` surface는 brush에 연결하지 않고 `Superseded` terminal로 끝낸다.
- device loss/shutdown 때 active draw, checked-out D3D11On12 resource, pending fence, commit callback을 모두 terminal 처리한다.

### 2.4 GPU bridge — C0에서 결정할 두 경로

현재 backing store는 D3D12 `R8G8B8A8_UNorm`, sample count 1이다. Composition destination의 실제 format과 D3D11On12 ownership을 확인하기 전에는 `CopyResource` 가능성을 확정하지 않는다.

우선순위 A:

```text
CompositionDrawingSurface.BeginDraw(ID3D11Texture2D)
  -> returned texture가 app의 동일 D3D11On12 device 소유인지 확인
  -> UnwrapUnderlyingResource(same D3D12 command queue)
  -> destination COMMON -> COPY_DEST
  -> backing RENDER_TARGET -> COPY_SOURCE
  -> exact CopyResource 또는 compatible CopyTextureRegion
  -> 두 resource를 필요한 최종 state로 복귀
  -> queue fence signal
  -> ReturnUnderlyingResource(fence, value)
  -> EndDraw
```

우선순위 B는 A의 destination unwrap이 API/resource ownership상 불가능할 때만 C0 안에서 허용한다.

```text
existing D3D12 backing을 D3D11On12 wrapped source로 노출
  -> BeginDraw의 ID3D11Texture2D destination과 같은 D3D11 device/context에서
     GPU-only CopyResource/CopySubresourceRegion
  -> wrapped source release/flush와 EndDraw 순서를 명시적으로 완료
```

공통 hard requirement:

- Composition graphics device, D3D11On12 device, D3D12 device/queue는 같은 adapter/ownership graph를 사용한다.
- destination texture description, format, size, sample count를 매 draw 검증한다.
- `R8G8B8A8_UNorm` exact copy가 지원되지 않으면 candidate backing을 `B8G8R8A8_UNorm`/`SKColorType.Bgra8888`로 만들 수 있는지 먼저 검증한다.
- format conversion이 필요하면 GPU shader/blit 후보를 별도 측정하며, CPU swizzle/readback은 허용하지 않는다.
- `BeginDraw` offset을 copy destination origin에 반영한다. full update에서도 offset이 0이라고 가정하지 않는다.
- D3D12 work가 destination을 `COMMON`으로 돌리고 fence를 signal한 뒤 `ReturnUnderlyingResource`한다.
- checked-out 동안 해당 resource에 D3D11On12 translation-layer work를 섞지 않는다.
- device removal/error에서 `BeginDraw`가 실패했다면 `EndDraw`를 호출하지 않는다.

A/B가 모두 안전하게 성립하지 않으면 pure Win32 composition도 같은 bridge를 재사용할 수 없으므로 C0에서 중단한다. 이 실패를 CPU copy, GDI, bitmap encode, 다른 ANGLE DLL로 우회하지 않는다.

### 2.5 visible commit

exact staging이 준비된 뒤 compositor dispatcher의 한 turn에서 다음 값을 함께 바꾼다.

- `CompositionSurfaceBrush.Surface`
- `SpriteVisual.Size`, `Offset`, `Clip`
- brush DPI mapping transform/scale
- epoch/transaction을 식별하는 diagnostics state

내부 trace는 최소한 다음 시점을 분리한다.

```text
ExactBackingStoreReady
BeginDrawStarted
GpuCopyQueued
GpuFenceSignaled
ResourceReturned
EndDrawCompleted
VisualMutationQueued
CommitRequested
CommitActionCompleted
FrontAdopted
Presented | Superseded | Failed
```

- `EndDrawCompleted`는 surface pixels가 composition에 사용 가능해진 시점이다.
- `CommitRequested`/`CommitActionCompleted`는 compositor submission 관찰값이다.
- `FrontAdopted`/기존 `VisibleSurfaceCommitted`는 app-side visible owner가 바뀐 단계일 뿐 scan-out ACK가 아니다.
- WGC/PresentMon/ETW 같은 외부 관측만 strict visual/cadence evidence다.

### 2.6 provisional 표시

exact surface가 준비되기 전에는 old front를 새 client 전체로 확대하지 않는다.

| drag edge | old front provisional anchor |
| --- | --- |
| left | right 고정, logical `offsetX = newWidth - oldWidth` |
| right | left 고정, `offsetX = 0` |
| top | bottom 고정, logical `offsetY = newHeight - oldHeight` |
| bottom | top 고정, `offsetY = 0` |
| corner | X/Y 규칙 조합 |

- shrink는 새 host bounds로 crop한다.
- expansion으로 드러난 영역은 `DorotiMauiSurface` root background만 보인다.
- old surface pixels는 유지하고 visual offset/clip만 바꾼다.
- exact commit에서 surface, size, clip, DPI mapping을 교체하고 provisional offset을 0으로 되돌린다.
- host layout과 edge intent가 일시적으로 불일치하면 stretch하지 않고 더 보수적인 crop/background를 택한다.

### 2.7 scheduling

- MAUI 후보에서는 synchronous `WM_SIZE` 100ms wait를 제거한다.
- Win32 message handler는 immutable intent/edge를 게시하고 즉시 복귀한다.
- actual XAML host layout이 authoritative content epoch을 게시한다.
- framework/raster는 `current + latest`; compositor에는 pending visible commit 하나만 둔다.
- exact staging이 latest gate를 통과했을 때만 visible commit한다.
- UI dispatcher가 blocked인 동안 XAML layout/compositor callback이 진행된다고 가정하지 않는다.
- `DwmFlush`는 C2에서 기본 OFF와 commit 뒤 resize-only ON을 제한적으로 A/B할 수 있다. 한 번의 paired A/B 뒤 strict gate가 좋아지지 않으면 OFF로 고정하고 반복 조정하지 않는다.

## 3. 구현 경계

### 3.1 C0 validation 전용

- 새 `Doroti/validation/windows-composition-surface/`
  - 작은 WinUI/Win32 executable과 checkerboard/bar/circle/color-patch scene
  - Composition interop projection, D3D11On12 bridge, surface pool, teardown만 검증
  - MAUI/framework 제품 연결 금지
- managed projection이 필요한 native interface를 안전하게 노출하지 못하면 좁은 native bridge를 사용한다.
- `Vortice.Direct3D11` 추가가 필요하면 central package version과 Windows lock file만 scoped 변경한다. package 추가 자체를 C0 성공으로 간주하지 않는다.

### 3.2 C1/C2 MAUI 후보

- `DorotiWindowsDxgiHost`
  - 후보 path에서 `SwapChainPanel Presenter`를 제거한다.
  - attached visual root와 XAML input/focus owner를 제공한다.
- `WindowsTopLevelResizeSource` 신설 또는 책임 분리
  - parent `WM_SIZING`/`WM_SIZE`/DPI/edge 관찰만 소유한다.
  - child HWND 생성, child subclass, native mouse routing을 포함하지 않는다.
- `WindowsCompositionSurfacePresenter` 신설
  - compositor, graphics device, surface pool, brush/visual, GPU bridge, commit/retirement를 소유한다.
- D3D12 device/backing 책임 분리
  - 현재 `WindowsHwndD3D12Presenter` 안의 adapter/device/queue/Skia context/backing-store 책임을 candidate가 재사용할 수 있는 좁은 owner로 추출한다.
  - C0 PASS 전에는 대규모 제품 refactor를 하지 않는다.
- `DorotiWindowsDxgiSurface`
  - candidate flag 아래에서 raw child를 만들지 않는다.
  - exact host layout epoch, framework/raster mailbox, composition presenter 연결만 조정한다.
- `DorotiMauiSurface`
  - hidden `Entry`/`Editor`, semantics overlay, root background를 유지한다.
  - candidate visual이 semantics/input z-order를 가리지 않는지 확인한다.

### 3.3 삭제 시점

다음 코드는 C2 PASS가 아니라 C3/G2 PASS 뒤에만 삭제한다.

- raw `STATIC` child HWND와 parent/child dual subclass
- `WindowsHwndD3D12Presenter` 제품 경로
- 비활성 `DOROTI_WINDOWS_ANGLE_SPIKE`
- `SwapChainPanel` dual exact presenter와 사용되지 않는 `WindowsD3D12Presenter`
- 후보 선택용 feature flag와 baseline-only wiring

삭제 전까지 기본 제품 path를 바꾸지 않는다. 후보 FAIL 시 candidate wiring/package만 제거하고 현재 raw-child baseline으로 복귀한다.

## 4. R0 — provenance와 paired baseline 고정

상태: `notStarted`

### 작업

- source HEAD, dirty files, resolved Windows App SDK/D3D package, OS/GPU/driver/DPI/refresh를 기록한다.
- `work.md`의 기존 evidence ID와 수치를 historical evidence로 고정하고 재작성하지 않는다.
- candidate와 같은 source/binary의 feature flag로 current raw-child baseline을 실행할 수 있게 한다.
- C2 직전에 raw-child baseline을 같은 장비/해상도/input driver에서 paired 실행한다.
- Flutter는 고정 source protocol과 기존 `flutter-rsz-default-20260823-112152-4dd6a255` 기록만 참고한다. 새 Flutter build/capture/instrumentation은 실행하지 않는다.

### gate

- baseline/backend identity와 source fingerprint가 trace에 남는다.
- WGC가 실제 candidate/raw content와 title/caption을 캡처하는지 확인된다.
- 포커스 상실, input cleanup 실패, capture error/drop run은 acceptance에서 제외한다.

## 5. C0 — Composition surface/GPU bridge feasibility

상태: `notStarted` / runtime `notVerified`

### 작업

1. resolved Windows App SDK 1.8에서 compositor, graphics device, drawing surface, native interop을 생성한다.
2. 동일 D3D12 device/queue 위 D3D11On12 device를 만들고 우선순위 A/B bridge를 순서대로 검증한다.
3. odd/even size, 420/640/1000 logical의 200% physical size, repeated resize에서 full-surface copy를 실행한다.
4. BeginDraw offset, format, color channel, alpha, row/pixel geometry를 checkerboard/color patch로 확인한다.
5. surface pool을 최대 3 slots로 제한하고 pending commit 동안 latest coalescing을 확인한다.
6. repeated create/draw/commit/resize/teardown과 forced presenter reset을 실행한다.
7. device removal/replacement를 자동으로 유발할 수 없으면 `notVerified`로 명시하고, 최소한 checked-out/open-draw cleanup의 deterministic failure injection을 수행한다.

### PASS gate

- Release build 경고 0/오류 0.
- `BeginDraw`/GPU copy/return/`EndDraw` 반복 성공.
- CPU readback/GDI/bitmap encode 0.
- source/destination adapter, format, size, sample count mismatch 0.
- open `BeginDraw`, checked-out resource, pending fence/callback, leaked slot 0.
- surface slot 수가 상한을 넘지 않고 front 재사용-before-retirement 0.
- device/reset failure가 exactly-once terminal로 닫힌다.

### FAIL gate

- A/B GPU-only bridge가 모두 안전하지 않다.
- app-owned D3D11On12 graph로 BeginDraw texture를 사용할 수 없다.
- safe bounded front retirement를 정의할 수 없다.
- full-surface update가 CPU copy 또는 unbounded allocation을 요구한다.
- format/alpha/color correctness를 GPU-only로 유지할 수 없다.

C0 FAIL이면 C1/C2/N0/N1을 진행하지 않는다.

## 6. C1 — MAUI attached visual 최소 연결

상태: `notStarted` / visual/input `notVerified`

### 작업

- opt-in candidate flag에서 raw child HWND와 `SwapChainPanel` 없이 attached visual을 연결한다.
- root background, fixed AppBar, circle, right-edge, checkerboard를 composition surface로 표시한다.
- host layout epoch과 physical surface size/DPI mapping을 trace로 연결한다.
- left/right/top/bottom/corner provisional anchor를 synthetic size sequence로 검증한다.
- unload/reload, 0×0, minimize/restore, presenter reset에서 surface state를 terminal 처리한다.
- XAML host pointer/key/focus와 hidden text input/semantics overlay가 candidate visual 위에서 계속 접근 가능한지 최소 smoke한다.

### PASS gate

- startup 첫 화면 visible, title/caption visible, blank 0.
- raw render child HWND 0, HWND swap chain 0, `SwapChainPanel` attachment 0.
- surface/visual/DPI mapping mismatch와 non-uniform stretch 0.
- mouse hover/click/capture smoke와 resize 뒤 click PASS.
- candidate OFF 시 current raw-child baseline이 동일 binary에서 동작한다.
- C0 surface lifetime/terminal 조건을 그대로 유지한다.

### 실패 분기

- GPU bridge/lifetime 문제가 재발하면 C0 FAIL로 되돌아가고 N0로 우회하지 않는다.
- 실패가 WinUI attached-visual/XAML integration에 한정되고 desktop target에서는 같은 presenter를 사용할 수 있다는 근거가 있으면 C2 없이 N0로 이동할 수 있다.
- `ContentIsland`를 visual failure를 숨기는 즉시 fallback으로 넣지 않는다.

## 7. C2 — MAUI composition strict WGC

상태: `notStarted` / G1 `notVerified`

### fail-fast 순서

1. 420×300 logical, 200% DPI, 165Hz, left edge 10초
2. 640×360 logical, 같은 조건
3. 1000×600 logical, 같은 조건

앞 크기가 absolute gate를 실패하면 다음 크기로 진행하지 않는다. 각 크기는 동일 source fingerprint에서 3회 연속 valid run이 PASS해야 한다. 한 번의 좋은 run만으로 안정적 개선을 주장하지 않는다.

### 기록할 evidence

- actual input samples, `WM_SIZING`, delivered top-level `WM_SIZE`, host layout epochs
- framework scenes, exact backing ready, BeginDraw/EndDraw, GPU fences
- visual mutation, commit request/action completion, front adoption, terminal 수
- WGC window/client/content rect, blank, title/caption, AppBar height, circle aspect, right gap, final gap
- border-content phase p50/p95/max와 2-refresh 초과 episode 수/지속 시간
- surface pool high-water mark, supersede 수, device/capture/framework error
- current raw-child paired baseline은 진단 비교로 기록하되 candidate의 absolute failure를 상대 개선으로 덮지 않는다.

### strict PASS gate

- app-presented geometry mismatch, overflow, blank, non-uniform stretch 0.
- title/caption/AppBar/circle/capture error/drop/final gap failure 0.
- provisional background/crop은 허용하지만 border-content phase가 2 refresh intervals를 넘는 episode 0.
- stored Flutter evidence에 비교 가능한 phase p95가 있으면 한 refresh 이상 나쁘지 않아야 한다. 새 Flutter run은 요구하지 않는다.
- 3회 연속 run 모두 transaction leak, stale front adoption, checked-out resource, front mutation 0.
- commit telemetry 성공만이 아니라 WGC-visible content가 같은 epoch/geometry로 확인된다.

### FAIL 처리

- timer, timeout, sample 수, frame-latency wait를 반복 조정하지 않는다.
- 한 번의 제한된 `DwmFlush` OFF/resize-only ON paired A/B 외에는 cadence knob를 확장하지 않는다.
- candidate feature flag를 OFF로 되돌리고 default raw-child path가 시작/input 동작을 유지하는지 확인한다.
- 실패가 DWM/WinUI layout과 visual commit의 phase boundary로 남으면 N0로 이동한다.
- W1/W2식 제품 통합, 구 경로 삭제, Web milestone은 시작하지 않는다.

## 8. N0 — pure Win32 top-level ownership A/B

상태: `notStarted` / C2 실패 전 실행 금지

C2가 strict gate를 통과하지 못했지만 C0 presenter/bridge 자체는 유효할 때만 실행한다.

```text
WS_OVERLAPPEDWINDOW top-level HWND
  -> one WndProc owns client size/DPI/input/lifecycle
  -> same Doroti epoch/framework/D3D12 backing
  -> Arm A 또는 Arm B visible owner
```

### 두 control arm

1. **Arm A — top-level HWND direct DXGI**
   - top-level HWND client에 단 하나의 `CreateSwapChainForHwnd` presenter
   - raw render child HWND 없음
   - exact offscreen backing → 1:1 GPU copy → Present
2. **Arm B — desktop Composition target**
   - `ICompositorDesktopInterop.CreateDesktopWindowTarget(topLevelHwnd, ...)`
   - C0의 동일 `WindowsCompositionSurfacePresenter`
   - top-level client root visual 하나와 exact drawing surface

### 작업 경계

- 고정 bar/circle/right-edge scene와 동일 WGC/input driver만 먼저 연결한다.
- MAUI/XAML, `ContentIsland`, hidden Entry/Editor, full semantics를 N0 spike에 넣지 않는다.
- one WndProc가 `WM_SIZE`, `WM_SIZING`, `WM_DPICHANGED`, close/minimize만 소유한다.
- compositor arm은 current thread DispatcherQueue와 desktop target 수명을 명시적으로 소유한다.
- Arm A/B는 renderer/backing/evidence 조건을 같게 하고 visible presenter만 바꾼다.

### gate

- C2와 같은 420 → 640 → 1000 strict WGC fail-fast matrix를 적용한다.
- 한 arm만 PASS하면 그 arm을 선택한다.
- 둘 다 PASS하면 더 단순하고 commit/lifetime 단계가 적은 arm을 선택한다.
- 둘 다 FAIL하면 host ownership 가설만으로 해결되지 않은 것으로 기록하고 N1/Web을 시작하지 않는다.

## 9. C3 또는 N1 — 선택된 Windows 제품 경로 통합

상태: `notStarted` / C2 또는 N0 PASS 전 실행 금지

### C3 — MAUI Composition 제품 통합

- `DorotiWindowsDxgiHost`가 XAML layout/input owner, `WindowsCompositionSurfacePresenter`가 visible owner가 되도록 책임을 고정한다.
- pointer/keyboard/Korean IME/focus/hidden text input/semantics/accessibility를 중복 owner 없이 연결한다.
- `ContentIsland`는 기존 XAML 경계로 제품 요구를 충족할 수 없다는 구체적 failure가 있을 때만 별도 opt-in으로 검증한다.
- `ChildSiteLink.ActualSize`, transform, input/accessibility state를 XAML placement와 동기화해야 하므로 단순 visual 교체와 같은 milestone으로 묶지 않는다.

### N1 — pure Win32 shell migration

- 새 `Doroti.Host.Win32`
  - window class/message loop, size/DPI, pointer/touch/pen, keyboard/focus, clipboard, shutdown 소유
- 새 `Doroti.Target.Windows.Win32.win-x64`
  - Windows target manifest, runtime assets, package identity 소유
- common host 추출
  - `MauiFrameworkHost`의 session/view/render 연결을 toolkit-independent service로 이동
- native text/accessibility
  - TSF/IMM32 bridge와 UIA fragment root를 명시적으로 구현
- template/validator
  - 실제 `Win32/DXGI` 또는 `Win32/Composition` backend identity와 packaging/startup을 검증

N1은 presenter 교체가 아니라 Windows shell migration이다. N0 strict PASS 없이 scaffold/package/template부터 만들지 않는다.

### Windows G2 product acceptance

- left/right/top/bottom/네 corner 각 10초
- 100/125/150/200% DPI와 monitor 이동
- 가능한 장비에서 60/120/144/165Hz
- minimize/restore, maximize/restore, 빠른 방향 전환, occlusion, device loss/recovery
- pointer/mouse capture, touch/pen, keyboard, Korean IME, focus, cursor, semantics/accessibility
- cold/warm startup, close/reopen, title/caption, installer/packaged launch relevant path
- correctness, visual, cadence, input/accessibility, lifecycle을 별도 PASS/FAIL로 기록

위 범주 중 하나라도 실패하면 Windows 완료로 표시하지 않는다. Web B0~B2/G3~G4는 Windows G2 PASS 전까지 `notStarted`/`notVerified`다.

## 10. 공통 validation과 실행 규칙

- 모든 test/build/runtime 명령에는 repository 지침에 따라 최대 20분 timeout을 적용한다.
- C0 이후 각 stage에서 최소 다음 gate를 실행한다.

```powershell
dotnet run --project Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release
dotnet build DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj -c Release
pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1
git diff --check
```

- common contract/package 변화가 다른 target에 영향을 주면 해당 Qt/macOS/Android compile gate를 추가한다.
- Web source를 변경하지 않는 C0~C2에서는 Web runtime validation을 실행하지 않는다.
- evidence마다 source commit/fingerprint, candidate flag/backend, OS/GPU/driver, DPI/refresh, input rate, raw trace, WGC를 기록한다.
- build/contract PASS, API call 성공, commit callback, app trace를 visible/cadence PASS로 확대 해석하지 않는다.
- WGC가 composition visual을 실제 포함하는지 color/checkerboard marker로 먼저 확인한다.
- validator가 실패/예외로 끝나도 mouse up, `WM_CANCELMODE`, timer/priority 복구, 관련 process 종료를 보장한다.

## 11. 중단 조건과 금지 사항

다음 중 하나가 발생하면 해당 후보를 FAIL로 기록하고 ordered branch만 따른다.

- UI dispatcher를 기다리는 synchronous `WM_SIZE`로 deadlock/reentrancy가 생긴다.
- exact staging 준비 전에 visible front를 resize/mutate해야만 API가 동작한다.
- D3D11On12 resource를 안전하게 return할 수 없거나 fence/state ownership이 불명확하다.
- old front retirement 전에 재사용하거나 surface pool이 unbounded로 증가한다.
- composition commit telemetry는 성공하지만 WGC에서 surface/epoch/geometry가 일치하지 않는다.
- small-window phase 또는 2-refresh 초과 episode가 strict gate를 반복 실패한다.
- XAML과 ContentIsland/native HWND가 input/IME/accessibility를 동시에 소유해야만 동작한다.
- candidate rollback 뒤 startup/input/title/caption baseline이 복구되지 않는다.

명시적으로 하지 않을 것:

- raw `STATIC`/`CS_OWNDC` child HWND ANGLE 재실험
- `WM_SIZING` pre-present ordering 재조정
- timeout/debounce/sample 수로 failure 숨기기
- capacity buffer + `SetSourceSize` 복귀
- visible front의 선행 `Resize`/`ResizeBuffers`/`BeginDraw`
- old front의 full-client stretch나 non-uniform scale
- `SwapChainPanel.SetSwapChain` per-frame 교체
- CPU readback, GDI copy, PNG/bitmap round-trip
- `RequestCommitAsync`, `Present`, `DwmFlush`, commit completion을 scan-out ACK로 기록
- C2/N0 strict PASS 전 제품 cutover, C3/N1 G2 PASS 전 구 경로 삭제
- 새 Flutter runtime build/capture/renderer A/B
- Windows G2 전 Web milestone 승격

## 12. 완료 정의와 상태 표기

이 계획은 다음 조건을 모두 만족해야 완료다.

1. C0에서 GPU bridge, surface lifetime, bounded retirement가 PASS한다.
2. C2 또는 N0 중 하나가 strict WGC absolute gate를 안정적으로 PASS한다.
3. 선택된 C3 또는 N1 제품 경로가 Windows G2를 PASS한다.
4. 실패/구 presenter와 opt-in wiring을 삭제하고 package/lock/template/validator identity를 정리한다.
5. `research.md`와 active work 문서를 현재 구조와 실제 evidence 기준으로 갱신한다.
6. 모든 미실행 gate는 구체적으로 `notStarted`/`notVerified`로 남긴다.

계획 작성 시점 상태:

| 단계 | 상태 |
| --- | --- |
| 기존 M0/G0 | `PASS` — `work.md`의 기존 evidence 인용, 이번에 재실행하지 않음 |
| 기존 W0 D3D12 runtime correctness | `PASS` — 기존 evidence 인용 |
| 기존 W0 ordering/ANGLE, G1 visual/cadence | `FAIL` — 기존 evidence 인용 |
| R0 | `notStarted` |
| C0 | `notStarted` / build/runtime `notVerified` |
| C1 | `notStarted` / visual/input `notVerified` |
| C2 | `notStarted` / strict WGC `notVerified` |
| N0 | `notStarted` / strict WGC `notVerified` |
| C3/N1, Windows G2 | `notStarted` / `notVerified` |
| Web B0~B2/G3~G4 | `notStarted` / `notVerified` |

## 13. 근거

### 현재 repository

- Windows host/presenters: `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`
- MAUI surface/input/semantics: `Doroti/src/Doroti.Host.Maui/DorotiMauiSurface.cs`
- common epoch/transaction: `Doroti/src/Doroti.Ui/ResizeLifecycle.cs`, `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
- renderer: `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- Windows package lock: `DorotiDemoApp/windows/packages.lock.json`
- current validation: `Doroti/eng/validate-resize-continuity.ps1`, `Doroti/eng/validate-resize-continuity-live.ps1`

### local reference

- Flutter resize protocol: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- Flutter native window ownership: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc`
- Avalonia desktop composition target: `reference/Avalonia-main/src/Windows/Avalonia.Win32/WinRT/Composition/WinUiCompositedWindow.cs`
- Avalonia drawing surface BeginDraw/EndDraw: `reference/Avalonia-main/src/Windows/Avalonia.Win32/WinRT/Composition/WinUiCompositedWindowSurface.cs`
- Avalonia DirectComposition fallback: `reference/Avalonia-main/src/Windows/Avalonia.Win32/DComposition/DirectCompositedWindow.cs`

### Microsoft 공식 문서

- [Composition native interoperation with DirectX and Direct2D](https://learn.microsoft.com/en-us/windows/apps/develop/composition/composition-native-interop)
- [CompositionGraphicsDevice.CreateDrawingSurface2](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositiongraphicsdevice.createdrawingsurface2?view=windows-app-sdk-1.8)
- [ICompositionDrawingSurfaceInterop.BeginDraw](https://learn.microsoft.com/en-us/windows/win32/api/windows.ui.composition.interop/nf-windows-ui-composition-interop-icompositiondrawingsurfaceinterop-begindraw)
- [ElementCompositionPreview.SetElementChildVisual](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.hosting.elementcompositionpreview.setelementchildvisual?view=windows-app-sdk-1.8)
- [CompositionSurfaceBrush.Stretch](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositionsurfacebrush.stretch?view=windows-app-sdk-1.8)
- [Compositor.RequestCommitAsync](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositor.requestcommitasync?view=windows-app-sdk-1.8)
- [ContentIsland](https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island)
- [ID3D11On12Device2.UnwrapUnderlyingResource](https://learn.microsoft.com/en-us/windows/win32/api/d3d11on12/nf-d3d11on12-id3d11on12device2-unwrapunderlyingresource)
- [ID3D11On12Device2.ReturnUnderlyingResource](https://learn.microsoft.com/en-us/windows/win32/api/d3d11on12/nf-d3d11on12-id3d11on12device2-returnunderlyingresource)
- [CreateSwapChainForHwnd](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd)
- [DwmFlush](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)

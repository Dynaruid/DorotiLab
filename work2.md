# Doroti WindowsAppSdk raw HWND + ContentIsland/SiteBridge 전환 계획

## 0. 문서 목적과 최종 결정

- 전면 개편일: 2026-08-24
- 대상: Doroti Windows 제품 호스트, interactive resize, D3D12 presentation, 입력·IME·접근성·lifecycle
- 목표 제품 호스트: `Doroti.Host.WindowsAppSdk`
- 목표 창 구조: **raw Win32 top-level HWND + Windows App SDK `ContentIsland`/SiteBridge**
- 그래픽 경로: Doroti가 직접 소유하는 D3D12/DXGI visible surface
- Windows App SDK 기준: **최신 stable `2.4.0`을 정확히 고정**
- 문서 성격: ordered 구현 계획, 현재 구현 결과, 기존 증거의 재분류 기록

이 문서의 최종 구조 결정은 다음과 같다.

> Windows 제품 창의 크기·DPI·non-client·lifecycle 권한은 raw top-level HWND 하나가 가진다. Doroti 프레임은 같은 resize epoch의 geometry와 결합해 준비한 뒤 visible surface에 커밋한다. Windows App SDK는 `ContentIsland`/SiteBridge를 통해 입력, focus, 접근성, WinRT content 환경을 제공하지만 Doroti raster 크기와 presentation timing의 권한을 갖지 않는다.

Windows 제품 경로에서는 MAUI/WinUI `Window`, XAML layout, `SwapChainPanel.SizeChanged`를 크기 권한으로 사용하지 않는다. MAUI는 Android, iOS, macOS 등 기존 비-Windows target을 위해 유지한다. Windows에서 XAML control이 실제로 필요한 경우에도 `DesktopWindowXamlSource`는 한정된 보조 island로만 허용하며 제품 presentation root가 될 수 없다.

2026-08-24 기준 Microsoft 공식 stable 최신판은 `2.4.0`이고, repository의 현 pin `1.8.260508005`는 maintenance line이며 2026-09-09 servicing 종료 예정이다. 따라서 새 host는 `2.4.0`으로 시작한다. 다만 “항상 최신” floating reference를 쓰지는 않는다. 각 qualification run은 하나의 정확한 package/runtime version으로 재현 가능해야 하며, 이후 stable patch 승격은 별도 dependency gate를 통과해야 한다.

이 결정은 목표 아키텍처를 확정한 것이지 runtime acceptance를 통과했다는 뜻은 아니다. M1은 계속 FAIL이지만, 2026-08-24 사용자가 현재 정상인 Arm N을 제품 후보로 명시적으로 선택하고 WindowsAppSdk 전환 구현을 요청했다. 따라서 아래 제품 후보 구현은 M1을 소급 PASS하지 않는 **명시적 사용자 override branch**로 진행했으며, visible/G2 acceptance는 계속 별도다.

## 1. 완료 정의와 증거 해석 원칙

이 전환의 완료는 단순 build 성공이 아니다. 아래 축을 각각 독립적으로 판정한다.

1. `correctness`: 같은 resize epoch의 metrics, scene, backing store, geometry, present가 일치한다.
2. `visible continuity`: 확대·축소·방향 전환 때 검은 영역, 흰 영역, 색 띠, stale edge, 전체 프레임 stretch가 보이지 않는다.
3. `cadence`: drag 중 frame/present cadence가 목표 refresh와 합의된 허용 범위를 만족한다.
4. `input`: pointer, keyboard, focus, capture, cursor, drag가 render HWND와 island 사이에서 일관된다.
5. `text`: 한국어 IME 조합, 후보창, caret 위치, clipboard가 정상이다.
6. `accessibility`: UIA tree, hit test, focus, bounds가 committed geometry와 일치한다.
7. `lifecycle`: minimize/restore, maximize, snap, DPI/monitor 이동, device removal 복구가 정상이다.
8. `deployment`: packaged/unpackaged 실행과 Windows App Runtime 선택이 재현 가능하다.

판정값은 `PASS`, `FAIL`, `notVerified`, `diagnosticOnly`만 사용한다.

- source/contract/build PASS는 visible PASS가 아니다.
- WGC/observer 수치 PASS는 실제 화면 또는 scan-out PASS가 아니다.
- `DwmFlush` 반환은 scan-out ACK나 geometry+frame atomic commit 증거가 아니다.
- 자동화되지 않은 사용자 관찰은 중요한 diagnostic evidence이지만 전체 matrix PASS로 승격하지 않는다.
- 실패한 observer로 얻은 시각 결과는 qualification 판정에 사용하지 않는다.
- 최신 package로 restore/build가 성공해도 1.8→2.4 runtime behavior가 호환된다는 뜻은 아니다.

## 2. 현재 상태와 hard stop

| 단계 | 상태 | 현재 증거와 의미 |
|---|---|---|
| D0 기존 evidence 재분류 | PASS | correctness, cadence, visible acceptance를 분리했다. |
| M0 observer/DPI/capture pipeline 정리 | PASS | 관측기 자체 계약과 provenance를 정리했다. |
| M1 output observer qualification | **FAIL** | 3회 연속 qualification 실패. 자동 승격 gate는 계속 닫혀 있다. |
| 2026-08-24 Arm S/C control | diagnosticOnly | S/C 구현과 최신 2배 drag 단일 PASS는 확보했지만 필수 C 3회가 PASS/FAIL/FAIL이다. 전체 matrix는 `notVerified`이다. |
| 2026-08-24 Arm N custom non-client control | **PASS** | dual composition front smoke 3회 PASS 뒤 사용자가 수정본에서 고속 좌측·상단 확대를 재현해 front/border와 창 경계의 어긋남 및 떨림이 사라졌음을 확인했다. 이 PASS는 해당 수동 회귀 범위에 한정되며 전체 방향·DPI·monitor·표준 창 기능 matrix는 `notVerified`이다. |
| A0 목표 아키텍처 결정 | PASS | `Doroti.Host.WindowsAppSdk` + raw HWND + ContentIsland/SiteBridge + WinAppSDK `2.4.0`으로 확정했다. 구현 PASS가 아니다. |
| A0-R non-client ownership 재결정 | PASS | 사용자 지시로 Arm N custom non-client + dual-front를 제품 후보 ownership으로 선택했다. 선택 PASS이며 Windows 기능 matrix PASS는 아니다. |
| A0-V 2.4 dependency/API preflight | PASS | scoped exact `2.4.0`, self-contained unpackaged raw HWND, DispatcherQueue, attached SiteBridge create/connect/close가 PASS했다. root 중앙 pin은 rollback host 때문에 `1.8`로 유지한다. |
| A1 Arm N 제품 presenter 추출 | diagnosticOnly | `ArmNDualFrontPresenter`로 제품 소유 presenter를 분리했다. validation과 동일 binary를 공유하는 원래 A1 조건 및 H0-V는 미충족이다. |
| A2 raw HWND 제품 shell | diagnosticOnly | STA entry, fixed work-area raw HWND envelope, app-owned region, first exact frame before show와 content와 분리된 custom titlebar/caption button 픽셀 캡처가 PASS했다. DPI/startup 100회 matrix는 `notVerified`다. |
| A3 exact resize transaction | diagnosticOnly | custom chrome 이후 좌측 8ms smoke에서 geometry/commit `146/146`, submitted/presented/superseded/failed/dropped `146/133/13/0/0`. visible continuity는 `notVerified`다. |
| A4 ContentIsland/SiteBridge 통합 | diagnosticOnly | root `ContentIsland`/`DesktopAttachedSiteBridge` 연결과 raw pointer ownership은 runtime PASS. 2.4에서 site keyboard processing 비활성화는 지원되지 않는다. |
| A5 framework integration | diagnosticOnly | Doroti session/dispatcher/Skia renderer와 새 host가 정적·좌측 resize smoke에서 프레임을 terminal 처리했다. wheel animation은 DWM-vsync latest-only callback으로 유한 종료했다. 다중 DPI/cadence matrix는 `notVerified`다. |
| A6 input/focus/cursor | diagnosticOnly | 실제 USER32 입력 smoke에서 caption move `3`, edge resize `3`, wheel `1`, resize cursor PASS, pointer add/remove `1/1`, geometry/commit `7/7`, failed/superseded/dropped `0/0/0`. touch/pen/drag-drop/monitor matrix는 미실행이다. |
| A7 IME/text/clipboard | notVerified | IMM32 composition/result/caret와 Win32 clipboard 경로는 구현. SiteBridge keyboard processing과 raw WndProc translation 중 최종 단일 owner가 아직 검증되지 않았고 한국어 후보창 실사용도 미실행이다. |
| A8 semantics/UIA | notVerified | Doroti semantics 수신 경계만 존재하며 HWND UIA provider/pattern은 아직 구현되지 않았다. |
| A9 lifecycle/DPI/device recovery | diagnosticOnly | custom caption의 minimize, maximize/restore, close를 실제 USER32 click으로 확인했다. Snap/system menu/mixed-DPI/device-loss는 미구현 또는 미실행이다. |
| P0 Windows target cutover | diagnosticOnly | Demo/workspace/template 기본 Windows 경로와 target/runner/package를 전환했고 기존 MAUI runner는 rollback으로 보존·빌드 PASS. CI/install/packaged identity는 미검증이다. |
| G2 Windows 제품 acceptance | notVerified | 제품 후보는 생겼지만 전체 visible/input/text/UIA/lifecycle/deployment matrix를 실행하지 않았다. |
| Web 계획 재개 | PASS | 별도 사용자 요청으로 `work3.md`의 검토·ordered plan을 작성했다. 구현 PASS가 아니다. |
| Web 구현 | notStarted | Windows 결과가 Web 구현·browser milestone PASS를 대신하지 않는다. `work3.md`의 독립 gate를 따른다. |

현재 자동 acceptance의 정확한 재개점은 **M1 output observer를 strict 판정에 쓸 수 있게 고치거나, 240fps 이상 외부 카메라를 qualification oracle로 확보하는 것**이다. 구현 branch의 다음 경계는 Arm N 제품 후보를 실제 화면에서 우측·하단·네 corner, DPI/monitor, 표준 창 기능까지 확대하고 A6~A9를 완성하는 것이다. 사용자 override는 구현 시작 권한일 뿐 M1/H0-V/G2 PASS 증거가 아니다. Web은 `work3.md`에 따라 독립 판정한다.

### 2.1 2026-08-24 WindowsAppSdk 제품 후보 구현 evidence

- `Microsoft.WindowsAppSDK`는 새 host/preflight/runner에만 exact `2.4.0`으로 고정했다. 기존 MAUI Windows target의 중앙 `1.8.260508005` pin은 rollback을 위해 바꾸지 않았다.
- preflight: `ContentIsland.CreateForSystemVisual` + `DesktopAttachedSiteBridge.CreateFromWindowId`가 `connected=True`, pointer owner `raw-hwnd`, keyboard owner `island`, exit 0으로 종료했다.
- 2.4 constraint: `ProcessesKeyboardInput=false`는 connect 전 `Microsoft.UI.Input.dll` fail-fast `0xC0000602`, connect 후 `0x80131509` 예외로 거부된다. 따라서 이를 우회하거나 숨기지 않고 root island를 keyboard owner로 고정했다.
- product static smoke: `islandWasConnected=True`, Arm N adapter `AMD Radeon 780M Graphics`, geometry/commit `1/1`, presented `2`, failed `0`, exit 0.
- product left-resize smoke: 8ms request, geometry/commit `306/306`, submitted `307`, presented `303`, replayed `1`, superseded `4`, failed/dropped `0`, exit 0.
- custom chrome capture: 200% DPI의 physical desktop capture에서 title, minimize/maximize/close button, 분리된 72px physical titlebar와 1440×1280 content가 한 Arm N front 안에 표시됐다.
- actual USER32 interaction smoke: caption move `3`, left-edge resize `3`, wheel signal `1`, pointer add/remove `1/1`, geometry/commit `7/7`, submitted/presented `10/10`, failed/superseded/dropped `0/0/0`, exit 0.
- caption function smoke: left-right resize cursor가 `IDC_SIZEWE`와 일치했고 maximize region `0,0,2560,1504`, restore region `560,76,2000,1428`, minimize `IsIconic=True`, close exit `0`을 확인했다.
- wheel terminal smoke: notch 1회 전후 physical crop에서 sampled pixels `45,586/109,800`이 변경됐다. frame request/vsync callback/submitted/presented/superseded `106/105/105/91/14`, failed/dropped `0/0`으로 유한 종료했다. 즉 이전 즉시 재귀 scheduling의 `6,506` submitted/`6,363` superseded 폭주는 제거됐다.
- runner graph: `Windows | WindowsAppSdk | Win32-WinMain | win-x64 | Doroti.Target.Windows.WindowsAppSdk.win-x64`.
- `doroti.ps1 build --platform windows`, new unpackaged publish/launch, legacy `DorotiDemoApp/windows` rollback build, host/target/template pack가 모두 PASS했다.
- 이 evidence는 composition/transaction/runtime contract다. 사람이 본 제품 resize, scan-out continuity, UIA, 한국어 IME, mixed-DPI, Snap/maximize는 `notVerified`다.

## 3. 보존해야 할 기존 증거

### 3.1 2026-08-23 observer evidence

- M0는 PASS했다.
- M1은 3회 연속 valid FAIL이었다.
- WGC는 callback backlog와 frame-pool resize phase 때문에 strict visual judge로 사용할 수 없어 `diagnosticOnly`이다.
- Desktop Duplication strict output judge도 M1을 통과하지 못했다.
- 따라서 당시 H0 이후 architecture/product/Web 단계는 시작하지 않았다.

### 3.2 2026-08-24 D3D12/Flutter-handshake diagnostic spike

다음 실험은 폐기한다.

- monitor 크기 capacity + `SetSourceSize`: raster scaling 이상과 edge line 때문에 폐기
- drag 중 destructive `ResizeBuffers`를 쓰는 single child: 확대/축소 때 검은·흰 uncovered 영역 때문에 폐기
- 서로 독립된 두 visible surface를 동시에 두고 z-order/geometry로 교대하는 방식: 흰 띠와 flicker 때문에 폐기

현재 가장 좋은 control은 다음 구조다.

- standard-chrome top-level HWND 1개
- D3D12 direct-present용 `WS_CHILD` render HWND 1개
- monotonic swap-chain capacity
- `SetSourceSize` 미사용
- exact-content backing store
- 확대는 geometry 노출 전 cover 준비
- 축소는 geometry 축소 전 exact raster 준비
- platform ACK 후 raster 측 `DwmFlush`

사용자 관찰 결과 검은 영역은 제거되었고, 흰 영역도 제거되었으며, 축소 떨림은 거의 사라졌다. 마지막 오른쪽 보라색 띠는 qualification oracle로 취급했다. 하지만 방향·DPI·monitor·refresh 전체 matrix를 실행하지 않았으므로 visible 상태는 `diagnosticOnly/notVerified`이다.

보존할 observer 실행 기록:

- summary: `win-observer-m1-summary-20260824-005217.json`
- child window count: 1
- handshake count: 356
- timeout count: 1
- maximum timeout: 108.1093ms
- source cadence: 163.391fps
- device failure: 0
- Desktop Duplication interval p95: 7.5653ms
- 결론: 수치가 개선되어도 M1 전체 판정은 **FAIL**이며 H0/A1/P0 PASS로 승격하지 않는다.

### 3.3 2026-08-24 Arm S/C transaction spike

`problem.md`의 권고대로 diagnostic control에 두 arm을 구현했다.

- Arm S: direct-HWND `Scaling.Stretch` + exact `SetSourceSize`; transient distortion 허용 비교군
- Arm C: `CreateSwapChainForComposition` + DirectComposition edge-aware offset/clip + exact visual commit
- Arm C는 capacity backing을 1:1로 사용하며 `SetSourceSize`를 금지한다. composition chain에서 사용했을 때 capacity 전체 uniform scale이 재현됐다.
- Vortice DirectComposition은 다른 Vortice package와 동일한 `3.8.3` exact pin이다.
- Windows App SDK 제품 baseline은 사용자 결정대로 최신 stable `2.4.0` exact pin이며 이 native diagnostic spike와 별도다.
- interactive drag triangle wave는 2초에서 1초 왕복으로 바뀌어 기존보다 정확히 2배 빠르다.

현재 evidence:

- Arm S 1회 `FAIL`, phase 7/8
- Arm C isolated 1회 `PASS`, source 165.011fps, phase 8/8
- Arm C required 3회 `PASS/FAIL/FAIL`
- 2배 drag 적용 뒤 Arm C 1회 `FAIL`, source 165.060fps, phase 6/8
- 최종 source fingerprint와 일치하는 2배 drag 단일 회귀 `PASS`, source 164.954fps, phase 8/8
- Arm C provisional/exact smoke 4/4, stale reject 0
- Release build와 native capture build는 PASS
- 실제 mouse visible matrix는 `notVerified`

따라서 M1은 계속 **FAIL — 자동 승격 hard stop**이다. 당시 isolated observer PASS나 build PASS만으로 A0-V/H0-V/A1을 시작하지 않았다. 이후 제품 후보 구현은 2.1에 기록한 사용자 override이며 M1 판정을 바꾸지 않는다.

### 3.4 2026-08-24 Arm N dual-front transaction과 scoped manual PASS

Arm C의 standard non-client geometry와 composition visual이 서로 다른 output frame에 노출되는 한계를 제거하기 위해 custom non-client Arm N을 diagnostic control로 구현했다.

- monitor work-area 크기의 고정 `WS_POPUP + WS_EX_NOREDIRECTIONBITMAP` envelope 안에서 app-owned visible rect만 변경한다.
- idle 시 window region을 visible rect로 제한하고 resize capture 중에만 envelope를 연다.
- monitor-sized composition swap chain 두 slot 중 현재 보이지 않는 slot에 latest exact frame을 copy/`Present(0)`한다.
- hidden front의 frame-latency signal과 epoch/extent final gate를 통과한 뒤 root child visual 교체와 offset/clip을 하나의 DirectComposition commit으로 제출한다.
- 이전 front와 새 front를 동시에 표시하지 않으며, superseded hidden front는 visible switch 없이 폐기한다.

현재 evidence:

- Release build: PASS, warnings 0, errors 0
- 2ms cadence, 239-target Arm N transaction/input-region smoke 3회: PASS/PASS/PASS
- front/geometry mismatch, region wait timeout, composition drain timeout, process failure: 모두 0
- latest/visible-front/visual commit epoch: 각 run `240/240/240`
- 사용자 직접 확인: 수정된 final binary에서 고속 좌측·상단 확대 시 첨부 이미지와 같은 front/border와 창 경계 어긋남 및 떨림이 사라짐 — **PASS (scoped manual)**

이 manual PASS는 사용자가 보고한 정확한 회귀의 해결 증거다. 그러나 우측·하단·네 corner, 확대/축소 전체 조합, 100/150/200% DPI, 60/120Hz 이상, multi-monitor, Snap Layouts, system menu, maximize/restore, keyboard/touch sizing, accessibility는 아직 `notVerified`다. Arm N 제품 후보의 visible 판정은 여전히 scoped diagnostic이며, 기존 M1 observer qualification은 **FAIL**, G2는 `notVerified`로 유지한다.

### 3.5 과거 경로의 재분류

| 경로 | 보존 상태 | 새 계획에서의 위치 |
|---|---|---|
| W0 contract/build | PASS | transaction 계약 참고. visible acceptance는 아님. |
| W0 WGC visual | diagnosticOnly | observer qualification 전 strict 근거로 사용 금지. |
| C0 Composition bridge | PASS | bridge construction evidence만 보존. device removal은 `notVerified`. |
| C1 minimum smoke | PASS | 최소 실행 evidence. IME/minimize 전체 matrix는 `notVerified`. |
| C2 internal cadence | FAIL | 기존 MAUI/Composition 제품 경로를 선택하지 않는 근거. visible은 무효 또는 `notVerified`. |
| N0 Arm A pure top-level | 구조/correctness PASS | raw HWND 방향의 참고 evidence. strict visual은 observer 문제로 무효. |
| N0 Arm B desktop target interop | unavailable | 재사용하지 않는다. |
| C3/N1/G2 | notStarted/notVerified | 새 A0-V~P0 단계로 대체한다. |

## 4. 목표 아키텍처

raw HWND + ContentIsland/SiteBridge라는 상위 ownership 결정은 유지한다. 2026-08-24 사용자 선택으로 제품 후보는 **Arm N custom non-client + fixed work-area HWND envelope + app-owned region + dual hidden/visible composition slot**으로 확정했다. standard chrome + direct child HWND는 제품 baseline이 아니라 기존 비교/rollback 경로다. 이 선택은 Snap, system menu, maximize/restore, keyboard/touch sizing, caption UIA를 Doroti가 구현해야 한다는 뜻이며, 해당 기능은 아직 `notVerified`다.

### 4.1 창과 presentation 소유권

raw top-level HWND가 다음 항목의 유일한 authority다.

- custom non-client 영역과 app-owned hit-test region
- fixed work-area HWND envelope 내부의 committed geometry epoch
- committed content pixel size와 screen/client coordinate 변환
- `WM_DPICHANGED`와 suggested rect 적용
- minimize, maximize, restore, snap, monitor 이동
- close/destroy와 thread-affine USER32 lifecycle

`AppWindow`는 raw HWND에서 `WindowId`를 얻고 DispatcherQueue를 연결하는 얇은 wrapper로만 사용한다. 현재 Arm N geometry는 실제 HWND outer rect를 drag마다 바꾸지 않고 fixed envelope 안에서 움직인다. custom caption의 move/minimize/maximize/restore/close는 scoped smoke를 통과했지만 표준 DWM caption, Snap Layouts, system menu 계약은 현재 구현 증거에 포함되지 않는다.

### 4.2 Doroti visible surface

Doroti raster의 visible owner는 raw top-level HWND에 붙은 하나의 DirectComposition target이다.

- 두 composition swap-chain slot은 work-area capacity로 고정하고 한 slot만 visible front가 된다.
- 다음 slot을 hidden 상태에서 Present하고 frame-latency latch를 기다린 뒤, visible visual/offset/clip을 하나의 DirectComposition commit으로 교체한다.
- XAML layout이나 ContentIsland layout 결과로 swap chain을 resize하지 않는다.
- drag 중 `ResizeBuffers`, `SetSourceSize`, CPU readback, full-frame provisional stretch를 사용하지 않는다.
- 동일 window에 두 front를 동시에 표시하지 않는다. visible front와 geometry/clip은 같은 commit에서 교체한다.
- stale epoch frame은 visible present 전에 폐기한다.

### 4.3 ContentIsland/SiteBridge 역할

Windows App SDK content 계층은 graphics size authority가 아니라 platform integration 계층이다.

- `DesktopAttachedSiteBridge`: top-level HWND에 root `ContentIsland`를 연결한다.
- root island: site keyboard processing, focus/navigation, coordinate environment, accessibility 연결점 제공
- `DesktopChildSiteBridge`: IME helper, popup, overlay처럼 명시적으로 제한된 보조 island에만 사용
- root island는 fixed HWND envelope 크기를 유지하며 Doroti visible geometry를 역으로 변경하지 않는다.
- `ResizePolicy`가 Doroti client size를 역으로 결정하게 하지 않는다.
- full-client 투명 island가 pointer hit test를 가로채지 않도록 region과 z-order를 명시한다.
- raw HWND가 pointer owner다. Windows App SDK 2.4가 attached bridge의 keyboard processing 비활성화를 거부하므로 site processing은 enabled로 유지한다. raw WndProc keyboard/IME translation과의 최종 단일 ownership은 A7에서 확정해야 한다.

XAML control이 불가피할 때만 `DesktopWindowXamlSource`를 제한적으로 허용한다. 이 경우에도 XAML island는 보조 child이며 top-level, render HWND, Doroti frame scheduling을 소유하지 않는다.

### 4.4 thread 모델

| thread | apartment | 책임 |
|---|---|---|
| platform/window thread | STA | WinMain, raw HWND, USER32 message pump, DispatcherQueue, SiteBridge/ContentIsland, region admission |
| framework thread | 기존 Doroti 계약 | UI state, layout 요청, exact frame scheduling |
| raster thread | MTA/graphics thread | D3D12 recording, backing store 준비, hidden Present, DirectComposition geometry/front commit |
| observer process/thread | 독립 | capture, geometry sampling, cadence/visual 판정 |

platform thread는 `DispatcherQueueController.CreateOnCurrentThread()`로 Windows App SDK dispatch 환경을 만들고 USER32 pump와 함께 운용한다. resize handshake를 기다리는 동안 임의 message pumping이나 framework callback 재진입을 허용하지 않는다. bounded wait가 끝나면 timeout terminal state를 기록하고 현재 front를 유지한다.

### 4.5 process startup과 shutdown

startup 순서:

1. Windows App SDK 2.4 bootstrap/package identity와 실제 runtime version 확인
2. `[STAThread]` entry와 COM apartment 초기화
3. platform `DispatcherQueueController` 생성
4. raw top-level HWND class 등록 및 `CreateWindowExW`
5. HWND에서 `WindowId`, 필요 시 `AppWindow` 획득
6. root `ContentIsland`와 `DesktopAttachedSiteBridge` 연결
7. Doroti engine/framework/raster thread 시작과 Arm N presenter lazy 생성
8. hidden slot의 first exact frame과 DirectComposition commit 완료
9. app-owned region을 적용한 뒤 raw HWND 표시

shutdown은 반대 방향으로 수행한다.

1. 새 input/frame/resize epoch 수락 중지
2. outstanding transaction을 terminal `cancelled`로 종료
3. auxiliary child bridge와 island 닫기
4. presenter/GPU queue drain 및 resource 해제
5. root SiteBridge와 ContentIsland 닫기
6. top-level HWND destroy
7. DispatcherQueue shutdown 완료
8. COM/Windows App SDK bootstrap 해제

## 5. exact resize transaction 계약

### 5.1 epoch 상태

각 interactive resize target은 immutable epoch를 가진다.

```text
observedTarget
  -> metricsDelivered
  -> sceneBuiltForSameEpoch
  -> exactBackingStoreReady
  -> geometryAdmitted
  -> visibleSurfaceCommitted
  -> presented | timedOut | superseded | cancelled
```

필수 값:

- `windowId`, `resizeEpoch`, `direction`, `dpiEpoch`
- target outer rect와 target client pixel size
- exact content pixel size와 swap-chain capacity
- scene generation, raster generation, prepared fence value
- geometry admission, visible commit, present/terminal timestamp

같은 epoch가 아닌 scene, backing store, geometry, present를 결합하지 않는다. current+latest queue를 사용하며 중간 epoch FIFO replay는 하지 않는다.

### 5.2 확대 transaction

1. `WM_SIZING` target을 새 epoch로 관찰한다.
2. 현재 visible content를 새 target을 덮을 수 있는 capacity/cover에 준비하고 present한다.
3. cover present 뒤 platform-side `DwmFlush`를 사용해 geometry admission 전 ordering point를 둔다.
4. parent의 현재 clip 안에서 render child cover를 확장한다.
5. framework/raster가 미래 target의 exact scene과 backing store를 준비하되 아직 visible commit하지 않는다.
6. prepared fence와 epoch가 일치하면 top-level geometry를 admit한다.
7. 해당 `WM_SIZE`에서 `PresentPrepared(epoch)`를 실행한다.
8. platform이 visible commit ACK를 기록한다.
9. raster 측 ordering point 이후 child rect를 exact client rect로 정리한다.

### 5.3 축소 transaction

1. `WM_SIZING` target을 새 epoch로 관찰한다.
2. 기존 capacity 안에 작은 target의 exact scene/backing store를 준비한다.
3. prepared fence와 epoch가 일치할 때 parent geometry 축소를 admit한다.
4. 해당 `WM_SIZE`에서 `PresentPrepared(epoch)`를 실행한다.
5. platform ACK와 raster ordering point 후 render child를 exact client rect로 축소한다.

축소 시 큰 이전 frame을 매 tick 비율 변경해 보여주지 않는다. exact 새 content가 준비되지 않으면 current geometry/front를 유지하거나 사전에 정의한 safe cover를 유지한다.

### 5.4 timeout과 방향 전환

- wait는 bounded이며 timeout 값과 이유를 evidence에 남긴다.
- timeout 시 target geometry를 무조건 admit하지 않는다.
- current visible front와 current geometry를 유지하고 가장 최신 pointer target으로 재시도한다.
- 이미 geometry가 OS에 의해 강제로 적용된 경우 safe cover만 허용하며 stale exact frame을 stretch하지 않는다.
- drag 방향이 바뀌면 이전 epoch를 `superseded`로 닫고 current+latest 기준으로 새 transaction을 시작한다.
- timeout/supersede 뒤 늦게 끝난 GPU work는 present하지 않는다.

### 5.5 atomicity의 정확한 표현

Win32, DXGI, DWM 사이에 앱이 사용할 수 있는 단일 진짜 atomic API가 있다는 주장을 하지 않는다. 이 계획의 “한 번에 커밋”은 다음을 뜻한다.

- 앱 내부에서는 한 epoch의 exact frame과 geometry만 함께 admission한다.
- geometry가 먼저 노출되는 시간을 cover와 ordering으로 최소화한다.
- user-visible mismatch window를 observer와 고속 촬영으로 검증한다.
- `DwmFlush`는 ordering 보조 수단이며 scan-out ACK로 기록하지 않는다.

## 6. 프로젝트와 파일 구성 계획

### 6.1 새 host project

새 project:

`Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj`

예정 파일:

- `DorotiWindowsAppSdkApplication.cs`: public host entry와 engine lifecycle
- `WindowsAppSdkWindowHost.cs`: raw HWND class, WndProc, AppWindow wrapper
- `WindowsAppSdkHostAdapter.cs`: Doroti platform contract adapter
- `WindowsAppSdkFrameworkHost.cs`: framework thread와 exact-frame 요청 연결
- `WindowsAppSdkD3D12Presenter.cs`: render HWND, swap chain, prepared present
- `WindowsAppSdkResizeCoordinator.cs`: epoch state machine과 geometry admission
- `WindowsAppSdkContentIslandHost.cs`: ContentIsland/SiteBridge 생성·배치·shutdown
- `WindowsAppSdkInputBridge.cs`: pointer/keyboard/focus/capture/cursor
- `WindowsAppSdkTextInputBridge.cs`: TSF/IME/caret/clipboard
- `WindowsAppSdkSemanticsBridge.cs`: UIA provider와 island accessibility 연결
- `WindowsAppSdkDiagnostics.cs`: ETW/JSONL/provenance/terminal evidence

### 6.2 shared presenter core

현재 `Doroti/validation/windows-top-level-presentation/Program.cs`의 성공한 control protocol을 제품 host에 복사하지 않는다. 다음을 framework-independent shared core로 추출한다.

- immutable resize epoch
- monotonic capacity policy
- exact backing store preparation
- expand/shrink admission protocol
- prepared fence/ACK/timeout state
- D3D12 device-loss boundary
- structured diagnostics

validation app과 제품 host가 동일한 core를 참조해야 한다. test 전용 branch와 product branch의 timing logic이 갈라지면 해당 gate를 FAIL 처리한다.

### 6.3 새 Windows target

새 target project:

`Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64`

target descriptor의 의도:

- `HostKind=WindowsAppSdk`
- `NativeEntryKind=Win32-WinMain`
- `UseMaui=false`
- `Microsoft.WindowsAppSDK=2.4.0` central exact pin
- D3D12/DXGI/direct HWND swap-chain dependency 명시
- packaged/unpackaged startup 조건과 runtime selection 기록

Demo/template entry는 `[STAThread] static int Main(...)` 또는 동등한 native entry를 사용한다. 기존 `Doroti.Target.Windows.Maui.win-x64`, `App.xaml`, MAUI Windows package는 G2 PASS 전까지 제거하지 않는다.

### 6.4 기존 코드의 처리 원칙

- `DorotiWindowsDxgiSurface.cs`의 재사용 가능한 graphics code는 A1에서 ownership-neutral core로 이동한다.
- MAUI/WinUI-specific window/layout callbacks는 새 host에 복사하지 않는다.
- 기존 Windows MAUI target은 비교와 rollback을 위해 P0 완료 전까지 보존한다.
- 비-Windows MAUI path는 변경하지 않는다.
- runner SDK, product solution, template, packaging 기본값 변경은 P0에서만 수행한다.

## 7. ordered 실행 단계

### D0 — 기존 evidence 재분류

상태: **PASS**

- build/contract/runtime/visible/scan-out evidence를 분리했다.
- 과거 WGC visual 결과를 observer qualification 이전 strict PASS/FAIL로 사용하지 않는다.
- 기존 C0/C1/W0/N0 결과의 의미를 이 문서 3장처럼 제한한다.

### M0 — observer 기반 정리

상태: **PASS**

- capture source, output, monitor, DPI, geometry timestamps를 evidence에 기록한다.
- dropped frame, callback backlog, pool recreation을 측정한다.
- app telemetry와 observer telemetry를 별도 clock/provenance로 유지한다.

### M1 — output observer qualification

상태: **FAIL — hard stop**

작업:

1. callback에서 synchronous GPU readback을 제거한다.
2. capture callback과 analysis queue를 분리한다.
3. frame-pool recreate 구간을 qualification sample에서 명시적으로 제외하거나 continuity를 증명한다.
4. capture frame 시점의 geometry를 별도 timeline으로 결합한다.
5. Desktop Duplication의 monitor/output crop과 client edge mapping을 검증한다.
6. 가능하면 240fps 이상 외부 카메라를 독립 oracle로 사용한다.

PASS 조건:

- 동일 환경에서 3회 연속 qualified PASS
- observer 자체 backlog/drop/recreate가 합의된 한계 안에 있음
- synthetic edge oracle의 위치와 색을 방향별로 정확히 검출
- app telemetry 없이도 black/white/color band와 stale edge를 판정 가능

FAIL/stop 조건:

- 세 번 중 한 번이라도 observer validity가 깨지면 M1은 PASS가 아니다.
- M1 PASS 전 자동 승격으로 A0-V 이후를 시작하지 않는다. 명시적 사용자 override는 별도 branch/evidence로 기록하고 M1을 소급 PASS하지 않는다.

### A0-R — non-client ownership 재결정

상태: **PASS — ownership 선택에 한정**

원래 선행 조건: M1 PASS와 Arm N 전체 visible/function matrix 확보. 2026-08-24 사용자가 현재 정상인 Arm N 기반 전환을 명시적으로 요청해 선택 gate만 override했다.

판정 대상:

- standard chrome + direct child HWND baseline을 유지할지 결정
- custom non-client + dual composition front Arm N 구조를 제품 ownership으로 채택할지 결정
- Arm N 선택 시 Snap Layouts, system menu, maximize/restore, keyboard/touch sizing, UIA/caption semantics, DPI/monitor migration의 구현 책임과 acceptance를 A2~A9에 명시

선택 결과는 4.1~4.2와 제품 host에 단일 Arm N 구조로 반영했다. 다만 이 PASS는 제품 후보를 고르는 의사결정이며, Snap/system menu/maximize/DPI/monitor/UIA를 포함한 기능 acceptance는 A6~A9/G2에서 계속 `notVerified`다.

### A0-V — Windows App SDK 2.4 dependency/API preflight

상태: **PASS — exact 2.4 self-contained unpackaged/attached-root scope**

선행 조건: A0-R 선택 PASS. M1은 사용자 override로 건너뛰었으며 소급 PASS하지 않는다.

목적: 현 `1.8.260508005`에서 최신 stable `2.4.0`으로 major upgrade할 때 raw HWND/ContentIsland 경계가 실제로 성립하는지 제품 코드 전에 확인한다.

작업:

1. 새 host/preflight/runner에서 `Microsoft.WindowsAppSDK`를 정확히 `2.4.0`으로 pin한다. 기존 MAUI rollback target 때문에 root 중앙 pin은 바꾸지 않는다.
2. transitive WinUI, Windows SDK BuildTools, bootstrap/runtime package graph를 lock-file로 비교한다.
3. `ContentIsland`, `DesktopAttachedSiteBridge`, `DesktopChildSiteBridge`, `DispatcherQueueController`, `Win32Interop`, `AppWindow` API surface를 2.4 metadata와 공식 sample로 확인한다.
4. C#에서 custom `[STAThread] Main`과 generated XAML main 비활성화 여부를 검증한다.
5. 이번 지원 mode인 self-contained unpackaged raw-HWND smoke와 publish launch를 만든다. package identity install은 P0/G2에 남긴다.
6. 설치된 runtime이 아닌 실제 선택된 Windows App Runtime version을 로그에 남긴다.
7. 1.8과 2.4를 같은 process에 혼합하거나 runtime fallback하지 않도록 contract를 둔다.

PASS 조건:

- restore/build가 exact `2.4.0` dependency graph로 재현됨
- raw HWND + DispatcherQueue + attached/child SiteBridge 최소 create/close smoke PASS
- packaged/unpackaged 중 지원할 deployment mode별 startup/shutdown PASS
- API 또는 generated-entry breaking change가 문서화되고 대응 방법 확정
- M1 observer와 validation toolchain이 dependency 변경 때문에 회귀하지 않음

승격 정책:

- 구현 기간 중 `2.4.x` 이상 새 stable이 나와도 자동 승격하지 않는다.
- 새 version은 release notes, API diff, restore/build, bridge lifecycle, H0-V 축소 smoke를 별도 실행한 뒤 pin을 변경한다.
- preview/experimental package와 Limited Access Feature는 제품 baseline에서 금지한다.

### H0-V — shared direct protocol 재검증

선행 조건: A0-V PASS

대상: `Doroti/validation/windows-top-level-presentation`

작업:

- 현재 best control을 shared-core 후보로 정리한다.
- expand/shrink/reverse/corner transaction 로그를 epoch 단위로 남긴다.
- right-edge 보라색 oracle을 포함해 모든 edge oracle을 대칭화한다.
- timeout, supersede, device removal을 fault injection한다.

PASS 조건:

- 8방향 × 10초 drag에서 visible artifact 0
- 빠른 확대/축소/즉시 방향 전환에서 stale epoch present 0
- timeout이 발생해도 uncovered region 0
- 내부 transaction invariant violation 0
- qualified observer 3회 연속 PASS

### A1 — presentation core 추출

선행 조건: H0-V PASS

- validation `Program.cs`에서 WinMain/sample UI와 presenter protocol을 분리한다.
- 공용 core가 USER32, D3D12, epoch state를 명확한 interface로 노출하게 한다.
- validation과 product가 같은 binary/source core를 참조하게 한다.
- old experimental code를 삭제하지 않고 comparison fixture로 남긴다.

PASS 조건:

- validation app이 추출 전과 동일한 evidence contract를 유지한다.
- shared-core contract test와 Release x64 build PASS
- H0-V의 축소 smoke 재실행 PASS

### A2 — `Doroti.Host.WindowsAppSdk` raw shell

선행 조건: A1 PASS

- Windows App SDK 2.4 bootstrap, STA entry, DispatcherQueue를 구성한다.
- raw top-level HWND와 standard chrome을 만든다.
- HWND→WindowId→AppWindow wrapper를 연결한다.
- `ShowWindow` 전 first exact frame gate를 둔다.
- minimize/restore/close ordering의 기본 telemetry를 추가한다.

PASS 조건:

- `Microsoft.UI.Xaml.Window`/MAUI 없이 top-level 실행
- top-level HWND가 유일한 client-size authority임을 contract test로 확인
- 100/125/150/200% DPI에서 initial client pixels 일치
- clean startup/shutdown 100회에서 leak/crash 0

### A3 — 제품 exact resize transaction

선행 조건: A2 PASS

- render child HWND와 D3D12 presenter를 제품 shell에 연결한다.
- `WM_SIZING` target, prepared fence, geometry admission, `WM_SIZE` commit을 하나의 epoch로 연결한다.
- `PlatformDispatcher.RequestExactFrame`과 prepared-frame API를 연결한다.
- timeout/current+latest/supersede/device-loss 정책을 구현한다.

PASS 조건:

- framework metrics와 raster scene이 같은 epoch임을 로그로 증명
- drag 중 `ResizeBuffers`, `SetSourceSize`, full-frame stretch 0
- stale epoch present 0
- H0-V와 동일한 qualified visual test 3회 연속 PASS

### A4 — ContentIsland/SiteBridge 통합

선행 조건: A3 PASS

- root ContentIsland와 `DesktopAttachedSiteBridge`를 raw HWND에 연결한다.
- 필요한 보조 surface만 `DesktopChildSiteBridge`로 만든다.
- island rect update를 committed epoch 이후로 제한한다.
- render HWND와 island의 z-order, hit-test region, focus transfer를 명시한다.
- SiteBridge close와 DispatcherQueue shutdown ordering을 fault test한다.

PASS 조건:

- island 생성 전후 resize visual/cadence가 회귀하지 않는다.
- island가 render extent를 변경하거나 resize feedback loop를 만들지 않는다.
- render 영역 pointer가 투명 island에 가로채이지 않는다.
- root/child bridge 반복 생성·종료에서 crash/leak 0

### A5 — Doroti framework integration

선행 조건: A4 PASS

- engine startup, platform dispatcher, vsync/frame scheduling을 새 host에 연결한다.
- logical size, device pixel ratio, physical pixels의 단일 변환 지점을 둔다.
- exact-frame request와 ordinary frame request의 우선순위/병합 규칙을 고정한다.
- framework work가 WndProc를 block하지 않도록 queue ownership을 검증한다.

PASS 조건:

- sample scene의 layout/raster pixel size가 모든 DPI에서 일치
- resize epoch와 ordinary animation frame 간 starvation 없음
- frame queue가 무한 증가하지 않음
- app present cadence와 observer cadence가 합의된 범위 안에서 일치

### A6 — input/focus/cursor

선행 조건: A5 PASS

- pointer/mouse/touch/pen, wheel, keyboard, modifier를 Doroti events로 변환한다.
- raw HWND, render HWND, island 사이 focus path를 명시한다.
- pointer capture, leave tracking, cursor update, drag-and-drop 경계를 구현한다.
- resize border hit test와 content hit test를 분리한다.

PASS 조건:

- resize 중 pointer capture loss/중복 event 0
- Alt+Tab, child island focus, render focus 왕복 정상
- click/scroll 좌표가 committed DPI/geometry와 일치
- 100/125/150/200% DPI와 monitor 이동 후 hit test 정상

### A7 — 한국어 IME, text, clipboard

선행 조건: A6 PASS

- TSF 또는 Windows App SDK content input 경로의 실제 owner를 하나로 정한다.
- composition start/update/commit/cancel을 Doroti text model에 연결한다.
- caret/selection rect를 committed screen coordinate로 제공한다.
- IME candidate window, clipboard, dead key, surrogate pair를 검증한다.

PASS 조건:

- 한국어 두벌식 연속 조합, backspace, selection replacement 정상
- resize/DPI 이동 중 후보창과 caret 위치 정상
- focus island↔render 왕복 후 조합 유실/중복 0
- clipboard text round-trip 정상

### A8 — semantics/UI Automation

선행 조건: A7 PASS

- Doroti semantics tree를 HWND/UIA provider와 연결한다.
- root island accessibility와 Doroti provider의 parent/child boundary를 고정한다.
- focus, bounds, hit test, Invoke/Value/Scroll pattern을 구현한다.
- bounds는 committed epoch만 게시한다.

PASS 조건:

- Accessibility Insights에서 중복 root/끊긴 tree 없음
- Narrator focus와 keyboard focus 일치
- resize/DPI 변경 뒤 UIA bounds와 화면 pixels 일치
- 보조 island control과 Doroti node 사이 탐색 정상

### A9 — lifecycle, DPI, monitor, device recovery

선행 조건: A8 PASS

- minimize/restore, maximize, snap, fullscreen, display change를 검증한다.
- per-monitor-v2 DPI와 suggested rect 적용 순서를 고정한다.
- swap-chain/device removal 때 current transaction을 terminal로 닫고 resource를 재생성한다.
- sleep/resume, RDP attach/detach, monitor disconnect를 가능한 범위에서 검증한다.

PASS 조건:

- lifecycle 전환 뒤 stale size/frame/input 없음
- DPI 변경 시 logical/physical size가 한 epoch에서 일치
- device removal injection 후 black/white uncovered 없이 복구
- shutdown 중 outstanding GPU/bridge callback으로 인한 crash 0

### P0 — Windows target/runner/package cutover

선행 조건: A9 PASS와 제품 후보 G2 preflight PASS

- `Doroti.Target.Windows.WindowsAppSdk.win-x64`를 product solution에 추가한다.
- runner SDK와 target discovery에 `HostKind=WindowsAppSdk`를 추가한다.
- DemoApp/template/package manifest가 새 native entry를 생성하게 한다.
- CI Release build, publish, packaged/unpackaged launch를 추가한다.
- rollback flag로 기존 MAUI Windows target을 선택할 수 있게 한다.

PASS 조건:

- clean checkout에서 restore/build/publish/install/launch PASS
- Windows App SDK `2.4.0` package/runtime provenance 일치
- package identity가 있는 실행과 없는 실행의 지원 범위 문서화
- 기존 non-Windows target 회귀 0
- rollback 절차 검증

기존 Windows MAUI target 삭제는 P0 PASS만으로 하지 않는다. G2 전체 PASS와 사용자 제품 수용 이후 별도 cleanup 변경으로 수행한다.

### G2 — Windows 제품 acceptance

선행 조건: P0 PASS

테스트 matrix:

- outer size: 420×300, 640×360, 1000×600
- DPI: 100%, 125%, 150%, 200%
- refresh: 60, 120, 144, 165Hz에서 사용 가능한 조합
- drag: slow, fast, expand, shrink, immediate reverse, four corners, four edges
- display: same-monitor, cross-monitor, mixed-DPI 이동
- lifecycle: minimize/restore, maximize, snap, fullscreen, device removal
- input: mouse, touch/pen 가능 환경, keyboard, focus, capture
- text: 한국어 IME 조합/후보창/caret/clipboard
- accessibility: Narrator/Accessibility Insights
- deployment: packaged/unpackaged 및 runtime 부재/업데이트 경로

PASS 조건:

- correctness, visible continuity, cadence, input, text, accessibility, lifecycle, deployment가 각각 PASS
- 각 qualified visual run 10초 이상, 방향별 3회 연속 PASS
- 검은 영역, 흰 영역, 색 띠, edge line, raster jitter, full-frame stretch 0
- stale/mismatched epoch present 0
- device failure 0 또는 검증된 recovery
- 사용자가 실제 빠른 drag에서 visible acceptance 확인

하나라도 미실행이면 해당 축은 `notVerified`이며 G2 전체 PASS로 쓰지 않는다.

## 8. 자동화와 evidence 규격

모든 run은 최소한 다음 provenance를 남긴다.

- git commit/dirty status
- build configuration와 RID
- Windows version/build
- Windows App SDK NuGet version과 실제 Windows App Runtime version
- package identity/bootstrap mode
- GPU/driver와 adapter LUID
- monitor/output, refresh rate, DPI
- window HWND, render HWND, WindowId
- observer 종류와 version
- run ID, start/end timestamp, input trace 또는 drag mode

epoch JSONL event:

- `resizeObserved`, `metricsDelivered`, `sceneBuilt`, `backingReady`
- `coverPresented`, `geometryAdmitted`, `wmSizeObserved`
- `preparedPresented`, `platformAck`, `dwmOrderingPoint`
- `transactionTerminal`, `deviceLost`, `deviceRecovered`

요약에는 count뿐 아니라 p50/p95/p99/max와 worst epoch ID를 기록한다. app, observer, camera evidence는 별도 원본을 보존하고 합성 summary가 원본 provenance를 잃지 않게 한다.

## 9. source/contract 검증 항목

구현 전에 다음 금지 조건을 정적 검사에 추가한다.

- 새 Windows product host에서 `Microsoft.UI.Xaml.Window` 생성 금지
- 새 Windows target에서 MAUI startup 사용 금지
- render size authority로 XAML `SizeChanged` 사용 금지
- drag hot path의 `ResizeBuffers` 금지
- `IDXGISwapChain::SetSourceSize` 금지
- visible continuity를 위한 CPU readback 금지
- full-frame provisional stretch 금지
- 동시에 두 front를 표시하거나 독립된 z-order/geometry로 교대하는 구성 금지. hidden prepared slot과 single-commit child 교체는 허용
- resize transaction 중 arbitrary nested message pump 금지
- stale epoch present 금지
- floating/pre-release Windows App SDK package version 금지

추가 contract:

- top-level HWND는 정확히 1개
- direct render child HWND는 정확히 1개
- root attached SiteBridge는 정확히 1개
- auxiliary child SiteBridge는 등록된 purpose/rect가 있을 때만 존재
- client geometry authority는 `WindowsAppSdkWindowHost` 하나
- presenter와 framework는 geometry를 요청할 수 있지만 직접 변경하지 못함
- build-time package version과 runtime-selected version이 evidence에 함께 존재

## 10. 실패 시 중단·rollback 규칙

- M1 FAIL이면 자동으로 A0-V 이후를 시작하지 않는다. 이번 사용자 override처럼 명시적으로 시작한 경우에도 M1/H0-V/G2 상태를 올리지 않는다.
- A0-V에서 2.4 API/deployment 문제가 나면 1.8 제품 구현으로 우회하지 않고 원인과 최소 지원 OS/deployment 결정을 기록한다.
- H0-V에서 artifact가 재현되면 timer/debounce tuning으로 제품 단계에 진입하지 않고 transaction/observer 문제로 돌아간다.
- A3가 H0-V보다 cadence 또는 visible continuity가 나쁘면 shell integration을 원인 분리할 때까지 A4로 가지 않는다.
- A4에서 island 추가 후 회귀하면 SiteBridge rect/z-order/input ownership을 수정한다. render path를 XAML-owned path로 되돌리지 않는다.
- IME/UIA 때문에 full-client XAML root가 필요해 보이면 A7/A8을 FAIL로 기록하고 별도 최소 spike로 원인을 분리한다.
- P0/G2 전에는 기존 Windows MAUI target을 삭제하거나 기본 target을 영구 전환하지 않는다.
- Web 제품 구현은 별도 승인과 `work3.md` gate 없이 Windows 결과만으로 자동 재개하지 않는다.

rollback 단위:

1. runtime flag로 WindowsAppSdk host와 기존 MAUI Windows host를 선택
2. shared presentation core의 last accepted version 고정
3. package/template 기본값은 마지막에 전환
4. Windows App SDK pin upgrade는 dependency commit 단위로 되돌릴 수 있게 분리
5. rollback이 source revert 없이 가능함을 release candidate에서 확인

## 11. 이번 문서 개편 이후의 정확한 다음 작업

1. 새 product runner를 사용자가 실제 화면에서 좌측·상단·우측·하단·네 corner로 확인한다.
2. M1 observer qualification을 수정하거나 외부 고속 카메라 oracle을 확보해 visible continuity를 정식 판정한다.
3. 100/125/150/200% DPI와 mixed-monitor 이동에 맞춰 fixed envelope/region/metrics를 per-monitor authority로 확장한다.
4. Arm N custom chrome에 남은 system menu, Snap Layouts, keyboard/touch sizing과 caption UIA를 구현한다.
5. island keyboard owner와 Doroti text model을 연결해 한국어 IME 후보창/caret/selection/clipboard를 실사용 검증한다.
6. HWND UIA provider와 Doroti semantics tree를 연결하고 Accessibility Insights/Narrator matrix를 실행한다.
7. device loss, sleep/RDP/display change, repeated bridge shutdown fault matrix를 실행한다.
8. clean checkout CI, package identity가 있는 설치, runtime 부재/업데이트, rollback launch를 검증한다.
9. 위 항목 뒤에만 P0/G2를 PASS로 승격하고 기존 MAUI Windows target 제거 여부를 별도 결정한다.

현재 상태를 다음처럼 오해하면 안 된다.

- repository 전체 중앙 package가 이미 `2.4.0`이다: **아님** — 새 WindowsAppSdk 경로만 scoped exact `2.4.0`이다.
- `Doroti.Host.WindowsAppSdk` 제품 후보가 존재한다: **맞음**
- raw HWND 제품 shell이 acceptance를 통과했다: **아님**
- ContentIsland/SiteBridge create/connect/close와 Arm N frame commit이 scoped runtime smoke를 통과했다: **맞음**
- 2026-08-24 validation control이 제품 G2를 통과했다: **아님**
- 기존 MAUI Windows target을 지금 제거해도 된다: **아님**

## 12. 공식 기준 source

Windows App SDK 공식 문서를 구현 시 기준으로 사용한다.

- 최신 stable 다운로드: <https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads>
- release channel과 servicing: <https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/release-channels>
- Windows App SDK 2.0/2.4 release notes: <https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/release-notes/windows-app-sdk-2-0?pivots=stable>
- HWND interop: <https://learn.microsoft.com/en-us/windows/apps/develop/ui/retrieve-hwnd>
- `Win32Interop.GetWindowIdFromWindow`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/microsoft.ui.interop/nf-microsoft-ui-interop-getwindowidfromwindow>
- `AppWindow.GetFromWindowId`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindow.getfromwindowid>
- Content namespace: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content>
- ContentIsland overview: <https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island>
- `DesktopAttachedSiteBridge`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.desktopattachedsitebridge>
- `DesktopChildSiteBridge`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.desktopchildsitebridge>
- `DispatcherQueueController.CreateOnCurrentThread`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.dispatching.dispatcherqueuecontroller.createoncurrentthread>
- `DesktopWindowXamlSource` fallback boundary: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.hosting.desktopwindowxamlsource>
- Windows App SDK Islands sample: <https://github.com/microsoft/WindowsAppSDK-Samples/blob/main/Samples/Islands/README.md>

repository 기준점:

- 기존 MAUI Windows App SDK pin: `Doroti/Directory.Packages.props`
- 새 scoped 2.4 host: `Doroti/src/Doroti.Host.WindowsAppSdk`
- 새 Windows target: `Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64`
- 새 API/runtime preflight: `Doroti/validation/windowsappsdk-24-preflight`
- 새 Demo runner: `DorotiDemoApp/windowsappsdk`
- 현 Windows MAUI host: `Doroti/src/Doroti.Host.Maui`
- 현 Windows MAUI target: `Doroti/src/Doroti.Target.Windows.Maui.win-x64`
- exact-frame API: `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
- direct top-level control: `Doroti/validation/windows-top-level-presentation/Program.cs`
- 현 D3D12 Windows surface experiment: `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`

이 문서의 gate와 상태가 active 기준이다. 구현 중 새 evidence가 생기면 해당 단계의 `PASS/FAIL/notVerified`와 exact resume point를 함께 갱신한다.

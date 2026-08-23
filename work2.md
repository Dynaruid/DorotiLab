# Doroti WindowsAppSdk raw HWND + ContentIsland/SiteBridge 전환 계획

## 0. 문서 목적과 최종 결정

- 전면 개편일: 2026-08-24
- 대상: Doroti Windows 제품 호스트, interactive resize, D3D12 presentation, 입력·IME·접근성·lifecycle
- 목표 제품 호스트: `Doroti.Host.WindowsAppSdk`
- 목표 창 구조: **raw Win32 top-level HWND + Windows App SDK `ContentIsland`/SiteBridge**
- 그래픽 경로: Doroti가 직접 소유하는 D3D12/DXGI visible surface
- Windows App SDK 기준: **최신 stable `2.4.0`을 정확히 고정**
- 문서 성격: ordered 구현 계획과 기존 증거의 재분류 기록

이 문서의 최종 구조 결정은 다음과 같다.

> Windows 제품 창의 크기·DPI·non-client·lifecycle 권한은 raw top-level HWND 하나가 가진다. Doroti 프레임은 같은 resize epoch의 geometry와 결합해 준비한 뒤 visible surface에 커밋한다. Windows App SDK는 `ContentIsland`/SiteBridge를 통해 입력, focus, 접근성, WinRT content 환경을 제공하지만 Doroti raster 크기와 presentation timing의 권한을 갖지 않는다.

Windows 제품 경로에서는 MAUI/WinUI `Window`, XAML layout, `SwapChainPanel.SizeChanged`를 크기 권한으로 사용하지 않는다. MAUI는 Android, iOS, macOS 등 기존 비-Windows target을 위해 유지한다. Windows에서 XAML control이 실제로 필요한 경우에도 `DesktopWindowXamlSource`는 한정된 보조 island로만 허용하며 제품 presentation root가 될 수 없다.

2026-08-24 기준 Microsoft 공식 stable 최신판은 `2.4.0`이고, repository의 현 pin `1.8.260508005`는 maintenance line이며 2026-09-09 servicing 종료 예정이다. 따라서 새 host는 `2.4.0`으로 시작한다. 다만 “항상 최신” floating reference를 쓰지는 않는다. 각 qualification run은 하나의 정확한 package/runtime version으로 재현 가능해야 하며, 이후 stable patch 승격은 별도 dependency gate를 통과해야 한다.

이 결정은 목표 아키텍처를 확정한 것이지 runtime acceptance를 통과했다는 뜻은 아니다. 현재 실행 gate는 여전히 M1에서 멈춰 있다.

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
| M1 output observer qualification | **FAIL** | 3회 연속 qualification 실패. 현재 hard stop이다. |
| 2026-08-24 direct D3D12 control | diagnosticOnly | 사용자 관찰에서 검은/흰 uncovered 영역은 제거되고 거의 완벽한 수준까지 접근했다. 전체 matrix는 `notVerified`이다. |
| A0 목표 아키텍처 결정 | PASS | `Doroti.Host.WindowsAppSdk` + raw HWND + ContentIsland/SiteBridge + WinAppSDK `2.4.0`으로 확정했다. 구현 PASS가 아니다. |
| A0-V 2.4 dependency/API preflight | notStarted | M1 PASS 뒤 가장 먼저 수행한다. |
| A1 공용 direct presenter 추출 | notStarted | M1 hard stop 때문에 시작하지 않는다. |
| A2 raw HWND 제품 shell | notStarted | 선행 gate 미통과. |
| A3 exact resize transaction | notStarted | 선행 gate 미통과. |
| A4 ContentIsland/SiteBridge 통합 | notStarted | 선행 gate 미통과. |
| A5~A9 framework/input/IME/UIA/lifecycle | notStarted | 선행 gate 미통과. |
| P0 Windows target cutover | notStarted | 제품 G2 전에는 기존 Windows target을 제거하지 않는다. |
| G2 Windows 제품 acceptance | notVerified | 제품 후보가 아직 없다. |
| Web 재개 | notStarted | Windows G2 전에는 재개하지 않는다. |

현재 정확한 재개점은 **M1 output observer를 strict 판정에 쓸 수 있게 고치거나, 240fps 이상 외부 카메라를 qualification oracle로 확보하는 것**이다. M1에서 3회 연속 qualified PASS가 나오기 전에는 A0-V 이후 제품 구현, 기존 shell migration, Web 작업을 시작하지 않는다.

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
- 두 visible front를 교대하는 방식: 흰 띠와 flicker 때문에 폐기

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

### 3.3 과거 경로의 재분류

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

### 4.1 창과 presentation 소유권

raw top-level HWND가 다음 항목의 유일한 authority다.

- `WM_NCCALCSIZE`, non-client 영역과 standard chrome
- `WM_SIZING`, `WM_WINDOWPOSCHANGING/CHANGED`, `WM_SIZE`
- client pixel size와 screen/client coordinate 변환
- `WM_DPICHANGED`와 suggested rect 적용
- minimize, maximize, restore, snap, monitor 이동
- close/destroy와 thread-affine USER32 lifecycle

`AppWindow`는 raw HWND에서 `WindowId`를 얻어 title, icon, presenter, placement 같은 Windows App SDK 기능을 설정하는 얇은 wrapper로만 사용한다. `AppWindow`와 HWND가 서로 다른 resize authority가 되어서는 안 된다.

### 4.2 Doroti visible surface

Doroti raster의 visible owner는 top-level의 client에 붙은 하나의 `WS_CHILD` render HWND다.

- render HWND가 D3D12/DXGI swap chain을 직접 소유한다.
- swap-chain capacity는 drag 중 단조 증가시킬 수 있지만 visible content extent는 항상 committed epoch의 exact size다.
- XAML layout이나 ContentIsland layout 결과로 swap chain을 resize하지 않는다.
- drag 중 `ResizeBuffers`, `SetSourceSize`, CPU readback, full-frame provisional stretch를 사용하지 않는다.
- 동일 window에 동시에 보이는 두 front를 두지 않는다.
- stale epoch frame은 visible present 전에 폐기한다.

### 4.3 ContentIsland/SiteBridge 역할

Windows App SDK content 계층은 graphics size authority가 아니라 platform integration 계층이다.

- `DesktopAttachedSiteBridge`: top-level HWND에 root `ContentIsland`를 연결한다.
- root island: focus/navigation, input routing, coordinate environment, accessibility 연결점 제공
- `DesktopChildSiteBridge`: IME helper, popup, overlay처럼 명시적으로 제한된 보조 island에만 사용
- island의 pixel rect는 host가 committed epoch 이후 명시적으로 `MoveAndResize`한다.
- `ResizePolicy`가 Doroti client size를 역으로 결정하게 하지 않는다.
- full-client 투명 island가 pointer hit test를 가로채지 않도록 region과 z-order를 명시한다.
- render HWND가 기본 pointer owner이며 island 입력 구역은 별도 등록한다.

XAML control이 불가피할 때만 `DesktopWindowXamlSource`를 제한적으로 허용한다. 이 경우에도 XAML island는 보조 child이며 top-level, render HWND, Doroti frame scheduling을 소유하지 않는다.

### 4.4 thread 모델

| thread | apartment | 책임 |
|---|---|---|
| platform/window thread | STA | WinMain, raw HWND, USER32 message pump, DispatcherQueue, SiteBridge/ContentIsland, geometry commit |
| framework thread | 기존 Doroti 계약 | UI state, layout 요청, exact frame scheduling |
| raster thread | MTA/graphics thread | D3D12 recording, backing store 준비, present |
| observer process/thread | 독립 | capture, geometry sampling, cadence/visual 판정 |

platform thread는 `DispatcherQueueController.CreateOnCurrentThread()`로 Windows App SDK dispatch 환경을 만들고 USER32 pump와 함께 운용한다. resize handshake를 기다리는 동안 임의 message pumping이나 framework callback 재진입을 허용하지 않는다. bounded wait가 끝나면 timeout terminal state를 기록하고 현재 front를 유지한다.

### 4.5 process startup과 shutdown

startup 순서:

1. Windows App SDK 2.4 bootstrap/package identity와 실제 runtime version 확인
2. `[STAThread]` entry와 COM apartment 초기화
3. platform `DispatcherQueueController` 생성
4. raw top-level HWND class 등록 및 `CreateWindowExW`
5. HWND에서 `WindowId`, 필요 시 `AppWindow` 획득
6. D3D12 device, render HWND, direct presenter 생성
7. root `ContentIsland`와 `DesktopAttachedSiteBridge` 연결
8. Doroti engine/framework/raster thread 시작
9. first exact frame 준비 후 창 표시

shutdown은 반대 방향으로 수행한다.

1. 새 input/frame/resize epoch 수락 중지
2. outstanding transaction을 terminal `cancelled`로 종료
3. auxiliary child bridge와 island 닫기
4. root SiteBridge와 ContentIsland 닫기
5. presenter/GPU queue drain 및 resource 해제
6. render HWND와 top-level HWND destroy
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
- M1 PASS 전 A0-V 이후를 시작하지 않는다.

### A0-V — Windows App SDK 2.4 dependency/API preflight

선행 조건: M1 PASS

목적: 현 `1.8.260508005`에서 최신 stable `2.4.0`으로 major upgrade할 때 raw HWND/ContentIsland 경계가 실제로 성립하는지 제품 코드 전에 확인한다.

작업:

1. `Doroti/Directory.Packages.props`의 후보 branch에서 `Microsoft.WindowsAppSDK`를 정확히 `2.4.0`으로 pin한다.
2. transitive WinUI, Windows SDK BuildTools, bootstrap/runtime package graph를 lock-file로 비교한다.
3. `ContentIsland`, `DesktopAttachedSiteBridge`, `DesktopChildSiteBridge`, `DispatcherQueueController`, `Win32Interop`, `AppWindow` API surface를 2.4 metadata와 공식 sample로 확인한다.
4. C#에서 custom `[STAThread] Main`과 generated XAML main 비활성화 여부를 검증한다.
5. packaged와 unpackaged 최소 raw-HWND smoke를 각각 만든다.
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
- two visible fronts 금지
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

- M1 FAIL이면 A0-V 이후를 시작하지 않는다.
- A0-V에서 2.4 API/deployment 문제가 나면 1.8 제품 구현으로 우회하지 않고 원인과 최소 지원 OS/deployment 결정을 기록한다.
- H0-V에서 artifact가 재현되면 timer/debounce tuning으로 제품 단계에 진입하지 않고 transaction/observer 문제로 돌아간다.
- A3가 H0-V보다 cadence 또는 visible continuity가 나쁘면 shell integration을 원인 분리할 때까지 A4로 가지 않는다.
- A4에서 island 추가 후 회귀하면 SiteBridge rect/z-order/input ownership을 수정한다. render path를 XAML-owned path로 되돌리지 않는다.
- IME/UIA 때문에 full-client XAML root가 필요해 보이면 A7/A8을 FAIL로 기록하고 별도 최소 spike로 원인을 분리한다.
- P0/G2 전에는 기존 Windows MAUI target을 삭제하거나 기본 target을 영구 전환하지 않는다.
- Web은 G2 PASS 전 재개하지 않는다.

rollback 단위:

1. runtime flag로 WindowsAppSdk host와 기존 MAUI Windows host를 선택
2. shared presentation core의 last accepted version 고정
3. package/template 기본값은 마지막에 전환
4. Windows App SDK pin upgrade는 dependency commit 단위로 되돌릴 수 있게 분리
5. rollback이 source revert 없이 가능함을 release candidate에서 확인

## 11. 이번 문서 개편 이후의 정확한 다음 작업

1. M1 observer qualification 실패 원인을 수정한다.
2. 같은 환경에서 3회 연속 qualified PASS를 확보한다.
3. A0-V에서 Windows App SDK `2.4.0` API/deployment preflight를 통과한다.
4. H0-V에서 2026-08-24 best control을 전체 방향 matrix로 재검증한다.
5. 그 뒤에만 A1 shared presenter core를 추출한다.
6. A2에서 `Doroti.Host.WindowsAppSdk` raw HWND shell을 만든다.
7. A3 direct presenter, A4 ContentIsland/SiteBridge 순으로 결합한다.

현재 상태를 다음처럼 오해하면 안 된다.

- repository package가 이미 `2.4.0`이다: **아님**
- `Doroti.Host.WindowsAppSdk`가 이미 존재한다: **아님**
- raw HWND 제품 shell이 acceptance를 통과했다: **아님**
- ContentIsland/SiteBridge integration이 검증됐다: **아님**
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

- 현재 Windows App SDK pin: `Doroti/Directory.Packages.props`
- 현 Windows MAUI host: `Doroti/src/Doroti.Host.Maui`
- 현 Windows MAUI target: `Doroti/src/Doroti.Target.Windows.Maui.win-x64`
- exact-frame API: `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
- direct top-level control: `Doroti/validation/windows-top-level-presentation/Program.cs`
- 현 D3D12 Windows surface experiment: `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`

이 문서의 gate와 상태가 active 기준이다. 구현 중 새 evidence가 생기면 해당 단계의 `PASS/FAIL/notVerified`와 exact resume point를 함께 갱신한다.

# Windows Vulkan 좌·상단 live-resize 떨림과 Acrylic 재질

## 현재 판정

- 수정 전 사용자 관찰: **FAIL-observed (2026-09-03)** — 우·하단은 괜찮지만 좌·상단 resize에서 내부 raster가 떨리고, Vulkan Acrylic은 blur 없이 단순 반투명처럼 보였다.
- 현재 구현: **repaired-pending-user-recheck** — visible Vulkan raster와 viewport clip을 top-level HWND 하나에 결합했고, origin 이동 중에는 USER32를 raster 완료로 막지 않고 current+latest frame만 전달한다.
- 자동 판정: **PASS-automated-partial** — source-built `TopLeft`, 같은 binary 반복 2회, 별도 `Left`/`Right`, Vulkan+Acrylic aggregate와 DemoApp current-monitor capture가 통과했다.
- 사람 체감/물리 scan-out: **notVerified** — WGC, Desktop Duplication과 Presentation receipt는 사람이 실제로 끄는 순간의 scan-out 및 재질 만족도를 대신하지 않는다.
- 기본 presenter는 계속 `AngleD3D11`; Vulkan은 `DOROTI_WINDOWS_PRESENTER=Vulkan` opt-in이다.

이 문서가 현재 source의 권위 있는 결론이다. `work.md` 0~22절의 retained visible child, edge anchor/source translation, ContentIsland 및 DWM transient-backdrop 실험은 실패 이력이며 현재 구조가 아니다.

## 근본 원인

### 좌·상단 raster 떨림

좌·상단 계열 resize는 client extent와 top-level HWND의 screen origin을 동시에 바꾼다. 이전 구현은 raster를 retained child HWND에 붙여 top-level 비클라이언트와 별도 DWM commit으로 만들었다. Source rect를 full capacity로 유지하고 child를 미리 배경색으로 채워도 `TopLeft` 캡처에서 새 top-level client와 이전 child subtree 사이에 실제 wallpaper가 18 px 노출됐다. 즉 버퍼 크기 문제가 아니라 두 visible geometry timeline의 결합 문제였다.

또한 origin이 움직일 때 모든 `WM_SIZE`를 exact raster/CompositionFrame까지 동기 대기하면 USER32 창 이동이 raster cadence에 묶여 창 전체가 계단식으로 움직인다. Proposed layout을 geometry 전에 표시하거나 edge별 offset을 주는 방식은 내부 content와 창 이동을 두 번 보이게 한다.

현재 구현은 native topmost `IDCompositionTarget`을 top-level HWND에 직접 붙인다. Native child는 숨겨진 backing-capacity 통지용 handle일 뿐 pixel이나 screen position을 소유하지 않는다. 따라서 shell frame, viewport clip과 visible raster가 같은 HWND geometry를 사용한다.

### Acrylic이 단순 반투명으로 보인 이유

Premultiplied alpha surface와 투명 app 배경만으로는 Acrylic blur가 생기지 않는다. 현재 경로는 전용 `Windows.System.DispatcherQueueController` thread에서 `Windows.UI.Composition.DesktopWindowTarget`을 top-level HWND에 non-topmost로 만들고, `DesktopAcrylicController.SetTarget(WindowId, CompositionTarget)`으로 system Acrylic을 실제 target에 연결한다. Top-level에는 `DWMWA_USE_HOSTBACKDROPBRUSH`를 설정한다.

Native topmost Vulkan Presentation surface는 `DXGI_ALPHA_MODE_PREMULTIPLIED`로 controller target 위에 놓인다. 이 구조에는 ContentIsland나 raw wallpaper alpha passthrough가 없으며, kind/theme/tint/luminosity runtime option도 controller에 적용한다.

## 현재 표시 구조

```text
top-level HWND (USER32 geometry/input/client-clip authority)
  ├─ non-topmost Windows.UI.Composition DesktopWindowTarget
  │   └─ DesktopAcrylicController system backdrop (Acrylic일 때)
  └─ native topmost DirectComposition target
      └─ Windows Presentation surface
          └─ full-capacity Vulkan app raster, identity transform

hidden native child HWND
  └─ monitor-sized backing-capacity authority only

Skia/Vulkan retained backing
  → exact-LUID D3D11 shared texture 3개
  → Windows Presentation available buffer
  → top-level HWND client가 exact viewport를 clip
```

- 매 frame 전체 retained capacity를 app-owned pixel로 갱신하고 exact logical viewport를 `(0,0)`에 그린다.
- Presentation source rect는 항상 full capacity이고 transform은 identity다.
- Edge별 source translation, visible child-position correction, stretch, 고정 FPS limiter를 사용하지 않는다.
- Opaque 모드는 top-level HWND의 app background plane도 유지해 Presentation 초기화/복구 전의 client를 app 색으로 채운다.
- Acrylic 모드는 opaque fill 대신 active Desktop Acrylic target이 backdrop을 소유한다.

## resize transaction

`WM_SIZING`은 USER32 proposed `RECT`를 수정하거나 proposed layout을 먼저 표시하지 않는다. 필요한 backing capacity만 보장한다. 실제 `WM_SIZE`가 authoritative client extent를 발행한다.

### Origin이 움직이는 `Left`/`Top` 계열

```text
WM_SIZING
  → capacity만 확인
USER32 actual WM_MOVE/WM_SIZE
  → top-level target과 현재 raster가 같은 HWND로 함께 이동
  → actual metrics 발행
  → raster worker가 running 1 + latest pending 1로 coalesce
  → 새 full-capacity frame을 같은 (0,0)에 비동기 교체
WM_EXITSIZEMOVE
  → 마지막 actual generation만 bounded exact settle
```

해당 edge는 `Left`, `Top`, `TopLeft`, `TopRight`, `BottomLeft`다. Interactive 각 step에서 platform thread는 raster/CompositionFrame을 기다리지 않는다. 이 때문에 geometry cadence가 GPU cadence에 양자화되지 않고, 오래된 generation은 latest로 재명명하지 않고 `superseded` terminal로 끝난다.

### Origin이 고정되는 `Right`/`Bottom` 계열

Origin이 고정되는 edge는 실제 `WM_SIZE` 뒤 matching exact generation과 CompositionFrame을 bounded wait한다. 기존 우·하단의 정확한 맞춤을 유지하면서 moving-origin 경로와 동일한 full-capacity/top-level raster owner를 사용한다.

## 실제 Acrylic 계약

- 최소 OS: Windows 11 24H2 build 26100.
- `DesktopAcrylicController.IsSupported()`가 참이어야 한다.
- `DesktopAcrylicController.SetTarget(WindowId, DesktopWindowTarget)` 성공과 controller state `Active`를 요구한다.
- `DWMWA_USE_HOSTBACKDROPBRUSH = TRUE`를 요구한다.
- `ContentIslandConnected=false`, `DesktopWindowTargetConnected=true`가 의도된 구조다.
- top Vulkan Presentation surface는 `DXGI_ALPHA_MODE_PREMULTIPLIED`다.
- attach/host-backdrop/premultiplied 중 하나라도 실패하면 explicit Vulkan+Acrylic 요청을 명시적으로 실패시킨다. ANGLE이나 단순 transparency로 조용히 바꾸지 않는다.
- runtime kind/theme/tint/luminosity는 전용 composition thread에서 last-wins로 controller에 적용한다.

## SkiaSharp 4.152.0 RC1 전환

- 모든 SkiaSharp 계열 중앙 버전과 runner 기본값을 실제 RC1 패키지 `4.152.0-rc.1.26426.14`로 통일했다.
- Windows host가 같은 버전의 `SkiaSharp.Vulkan.Silk.NET`을 직접 참조한다.
- 수동 raw-handle `GRVkBackendContext` 조립 대신 `GRSilkNetBackendContext`, typed Silk.NET instance/device/queue와 `GRVkExtensions.Initialize` callback으로 Skia Vulkan context를 만든다.
- Windows validator는 `SkiaSharp`, `SkiaSharp.NativeAssets.Win32`, `SkiaSharp.Vulkan.Silk.NET`의 요청/해결 버전이 정확히 일치하는지 fail-closed로 검사한다.
- 강제 restore 뒤 `Doroti.Host.Maui` Release의 Windows/Android/iOS/macOS/MacCatalyst 전체 TFM과 `DorotiDemoApp` Windows runner가 warning/error 0으로 빌드됐다. Windows에서 전체 solution build의 남은 실패는 macOS 전용 외부 도구 `sips` 부재뿐이다.

## 최신 자동 evidence

### Resize

- 실패 원인 확인: `.doroti/evidence/windows-vulkan-parent-local-child-fill-topleft-r3-20260904/manifest.json` — retained child를 배경색으로 즉시 채운 뒤에도 84 frame 중 1 frame에서 실제 wallpaper 18 px 노출.
- 최종 source-built: `.doroti/evidence/windows-vulkan-top-level-current-latest-topleft-final2-20260904/manifest.json` — `TopLeft` reverse 600 ms PASS, 71 capture frame, 93 outer changes, motion 중 92 presents, 155.50 Hz, present-gap p95/max 12.66/14.57 ms, accepted→next-present p95/max 8.02/9.50 ms, gap 0, marker regression 0, wait/timeout 0, device loss 0. source-to-binary 대응과 실행 중 source/repository/binary 안정성도 PASS다.
- 동일 binary 반복: `r5`, `r6` — 각각 160.57/158.88 Hz, gap 0, marker regression 0, device loss 0.
- 별도 회귀: `left-r7`, `right-r8` — 둘 다 gap 0/final exact/resource PASS. `Left`는 165.64 Hz와 wait 0, `Right`는 fixed-origin CompositionFrame wait 53/timeout 0을 기록했다.

### Vulkan+Acrylic 및 수명주기

- `.doroti/evidence/windows-vulkan-top-level-acrylic-capability-final2-20260904/manifest.json` — 최종 source-built aggregate `PASS`, gate `Vulkan-Composition-PASS-automated-partial`, source-to-binary 및 실행 중 source/repository/binary 안정성 PASS.
- 제품 report: effective `Vulkan/Composition-Swapchain + experimentalAcrylic`, `DesktopAcrylicController`, state `Active`, backdrop target/desktop target/host backdrop true, ContentIsland false, premultiplied alpha, Presentation buffer 3, active WSI 0.
- Exact resize 10, device reset 10, minimize/restore 10, start/close 10/10, synthetic device-loss ordering, 자동 input/IME/UIA와 package graph가 통과했다.
- `.doroti/evidence/windows-vulkan-top-level-acrylic-visual-final-20260904/desktop.capture.json` — WGC 29 frame/오류 0, Desktop Duplication 193 frame/오류 0. `monitor-000180.png`에서 창 밖의 선명한 wallpaper와 달리 창 안 배경의 세부 윤곽이 확산·억제됐다. 이는 캡처상 system material 증거이며 사람의 재질 만족도는 아니다.

## 검증 경계

- 최신 binary의 physical `Left`/`TopLeft` drag smoothness와 Acrylic 질감은 사용자 재확인 전까지 `notVerified`다.
- NVIDIA/Intel, 125/150/200% DPI, 60/120/144 Hz, mixed-DPI/Snap/maximize/occlusion은 `notVerified`다.
- 물리 한글 IME, Narrator/Accessibility Insights 및 장시간/실제 device-removal 검증도 `notVerified`다.
- 이전 topology의 실패 evidence는 삭제하거나 현재 PASS로 재분류하지 않는다.

## 결론

현재 결론은 **repaired-pending-user-recheck / PASS-automated-partial**이다. 좌·상단 떨림은 visible child를 보정하는 대신 visible raster와 window clip을 top-level HWND 하나에 결합하고 moving-origin resize를 current+latest 비동기 교체로 바꿨다. Vulkan Acrylic은 단순 alpha가 아니라 active `DesktopAcrylicController` target 위에 premultiplied Vulkan raster를 합성한다. 최종 사람 체감 PASS는 최신 binary를 직접 확인한 뒤에만 부여한다.

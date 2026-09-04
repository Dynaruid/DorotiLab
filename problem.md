# Windows Vulkan 좌·상단 live-resize 떨림과 Acrylic 재질

## 현재 판정

- 최신 사용자 관찰: **FAIL-observed (2026-09-04)** — top-level DirectComposition target/current+latest 변경도 흰색·검은색 영역과 좌·상단 내부 raster 떨림을 전혀 해결하지 못했다. 이전 자동 PASS는 이 관찰을 뒤집지 않는다.
- 현재 구현: **repaired-pending-user-recheck** — 실패한 top-level raster owner를 제거하고, ANGLE과 같은 exact-size visible child HWND에 Vulkan Presentation target을 붙였다. 실제 child `WM_SIZE`가 같은 generation의 present 제출까지 bounded wait한다.
- 자동 판정: **PASS-automated-partial** — 새 exact-child topology의 source-built `TopLeft`와 Vulkan+Acrylic aggregate, DemoApp Release build가 통과했다.
- 사람 체감/물리 scan-out: **notVerified** — WGC, Desktop Duplication과 Presentation receipt는 사람이 실제로 끄는 순간의 scan-out 및 재질 만족도를 대신하지 않는다.
- 기본 presenter는 계속 `AngleD3D11`; Vulkan은 `DOROTI_WINDOWS_PRESENTER=Vulkan` opt-in이다.

이 문서가 현재 source의 권위 있는 결론이다. `work.md` 0~23절의 retained oversized child, edge anchor/source translation, ContentIsland, DWM transient-backdrop 및 top-level current+latest target은 실패 이력이며 현재 구조가 아니다.

## 근본 원인

### 좌·상단 raster 떨림

좌·상단 계열 resize는 client extent와 top-level HWND의 screen origin을 동시에 바꾼다. 이전 구현은 raster를 retained child HWND에 붙여 top-level 비클라이언트와 별도 DWM commit으로 만들었다. Source rect를 full capacity로 유지하고 child를 미리 배경색으로 채워도 `TopLeft` 캡처에서 새 top-level client와 이전 child subtree 사이에 실제 wallpaper가 18 px 노출됐다. 즉 버퍼 크기 문제가 아니라 두 visible geometry timeline의 결합 문제였다.

또한 origin이 움직일 때 모든 `WM_SIZE`를 exact raster/CompositionFrame까지 동기 대기하면 USER32 창 이동이 raster cadence에 묶여 창 전체가 계단식으로 움직인다. Proposed layout을 geometry 전에 표시하거나 edge별 offset을 주는 방식은 내부 content와 창 이동을 두 번 보이게 한다.

직전 top-level target 구현도 물리 검증에서 실패했다. Top-level client clip의 USER32 geometry와 `IPresentationManager::Present`로 교체되는 내부 raster가 같은 HWND에 있더라도 서로 다른 commit 시점에 보일 수 있었고, moving-origin을 비동기로 두면 그 차이가 좌·상단에서 내부 떨림으로 드러났다.

현재 구현은 ANGLE 경로의 ownership/order를 따른다. Native top-level `WM_SIZE` 안에서 visible child를 실제 client 크기로 `SetWindowPos`하고, 중첩된 child `WM_SIZE`가 metrics를 발행한 뒤 matching Vulkan Presentation 제출 terminal까지 기다린다. Parent origin, child clip, admitted raster가 한 USER32 resize transaction 안에 놓이며 `WM_SIZING` proposed geometry를 미리 raster하지 않는다.

### Acrylic이 단순 반투명으로 보인 이유

Premultiplied alpha surface와 투명 app 배경만으로는 Acrylic blur가 생기지 않는다. 현재 경로는 전용 `Windows.System.DispatcherQueueController` thread에서 `Windows.UI.Composition.DesktopWindowTarget`을 top-level HWND에 non-topmost로 만들고, `DesktopAcrylicController.SetTarget(WindowId, CompositionTarget)`으로 system Acrylic을 실제 target에 연결한다. Top-level에는 `DWMWA_USE_HOSTBACKDROPBRUSH`를 설정한다.

Native topmost Vulkan Presentation surface는 `DXGI_ALPHA_MODE_PREMULTIPLIED`로 controller target 위에 놓인다. 이 구조에는 ContentIsland나 raw wallpaper alpha passthrough가 없으며, kind/theme/tint/luminosity runtime option도 controller에 적용한다.

## 현재 표시 구조

```text
top-level HWND (USER32 geometry/input authority)
  ├─ non-topmost Windows.UI.Composition DesktopWindowTarget
  │   └─ DesktopAcrylicController system backdrop (Acrylic일 때)
  └─ visible exact-size native child HWND (client clip authority)
      └─ native topmost DirectComposition target
          └─ Windows Presentation surface
              └─ full-capacity Vulkan app raster, identity transform

Skia/Vulkan retained backing
  → exact-LUID D3D11 shared texture 3개
  → Windows Presentation available buffer
  → exact child HWND가 viewport를 clip
```

- 매 frame 전체 retained capacity를 app-owned pixel로 갱신하고 exact logical viewport를 `(0,0)`에 그린다.
- Presentation source rect는 항상 full capacity이고 transform은 identity다.
- Edge별 source translation, visible child-position correction, stretch, 고정 FPS limiter를 사용하지 않는다.
- Opaque 모드는 child background plane도 유지해 Presentation 초기화/복구 전의 client를 app 색으로 채운다.
- Acrylic 모드는 opaque fill 대신 active Desktop Acrylic target이 backdrop을 소유한다.

## resize transaction

`WM_SIZING`은 USER32 proposed `RECT`를 수정하거나 proposed layout을 먼저 표시하지 않고 sizing edge만 기록한다. 실제 top-level `WM_SIZE`가 visible child를 exact client extent로 바꾸며, 중첩된 child `WM_SIZE`가 authoritative metrics와 raster generation을 발행한다.

```text
WM_SIZING
  → edge만 기록, proposed raster 없음
top-level actual WM_MOVE/WM_SIZE
  → child SetWindowPos(0, 0, exact client width/height)
  → nested child WM_SIZE
      → actual metrics 발행
      → matching retained Vulkan frame을 Presentation에 제출
      → bounded exact terminal까지 wait
  → parent/child resize transaction 반환
WM_EXITSIZEMOVE
  → 마지막 actual geometry 확인
```

이 ordering은 `Left`/`Top` 계열과 `Right`/`Bottom` 계열에 동일하다. Wait 범위는 CPU copy fence와 `IPresentationManager::Present` 제출 terminal까지이며 CompositionFrame/DwmFlush/scan-out은 기다리지 않는다. 따라서 exact ordering은 유지하면서 modal sizing loop를 DWM refresh에 직렬화하지 않는다.

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

- 새 source-built exact-child: `.doroti/evidence/codex-vulkan-exact-child-topleft-r2-20260904/manifest.json` — `TopLeft` reverse 600 ms `PASS-automated-partial`, capture 84, outer change 70, motion present 69, 118.29 Hz, present-gap p95/max 9.75/15.58 ms, accepted→next-present p95/max 4.41/7.39 ms, validation background right gap 0, final exact true, platform/CompositionFrame timeout 0. Visible owner는 `exact child HWND DirectComposition Vulkan Presentation target`, ordering은 `exact-child-wm-size-bounded-present`다.
- 직전 top-level evidence의 자동 PASS들은 **2026-09-04 사용자 FAIL-observed로 물리적으로 기각**되었다. 아래 기록은 삭제하지 않지만 현재 수정의 성공 근거로 사용하지 않는다.
- 실패 원인 확인: `.doroti/evidence/windows-vulkan-parent-local-child-fill-topleft-r3-20260904/manifest.json` — retained child를 배경색으로 즉시 채운 뒤에도 84 frame 중 1 frame에서 실제 wallpaper 18 px 노출.
- 최종 source-built: `.doroti/evidence/windows-vulkan-top-level-current-latest-topleft-final2-20260904/manifest.json` — `TopLeft` reverse 600 ms PASS, 71 capture frame, 93 outer changes, motion 중 92 presents, 155.50 Hz, present-gap p95/max 12.66/14.57 ms, accepted→next-present p95/max 8.02/9.50 ms, gap 0, marker regression 0, wait/timeout 0, device loss 0. source-to-binary 대응과 실행 중 source/repository/binary 안정성도 PASS다.
- 동일 binary 반복: `r5`, `r6` — 각각 160.57/158.88 Hz, gap 0, marker regression 0, device loss 0.
- 별도 회귀: `left-r7`, `right-r8` — 둘 다 gap 0/final exact/resource PASS. `Left`는 165.64 Hz와 wait 0, `Right`는 fixed-origin CompositionFrame wait 53/timeout 0을 기록했다.

### Vulkan+Acrylic 및 수명주기

- 새 exact-child topology: `.doroti/evidence/codex-vulkan-exact-child-acrylic-20260904/manifest.json` — source-built aggregate `PASS`, gate `Vulkan-Composition-PASS-automated-partial`; Acrylic controller `Active`, target/desktop target/host backdrop true, ContentIsland false, premultiplied alpha, exact resize/device reset/minimize-restore/start-close 반복 PASS.
- `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows -Configuration Release` — warning 0, error 0.
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

현재 결론은 **repaired-pending-user-recheck / PASS-automated-partial**이다. 직전 top-level current+latest 해법은 사용자의 2026-09-04 물리 FAIL로 폐기했다. 새 구현은 ANGLE처럼 exact visible child의 실제 `WM_SIZE` 안에서 matching Vulkan Presentation 제출까지 끝낸다. Vulkan Acrylic은 top-level active `DesktopAcrylicController` 뒤에 exact-child premultiplied raster를 합성한다. 최종 사람 체감 PASS는 이 새 binary를 직접 확인한 뒤에만 부여한다.

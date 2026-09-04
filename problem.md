# Windows Vulkan Acrylic resize: standard-HWND moving-origin ownership limit

## 현재 판정

- 사용자가 2026-09-05에 moving-origin pre-submit 후보와 40 FPS pacing 후보를 차례로 직접 확인했지만 `Left`/`Top` 떨림이 모두 남았다. 두 후보는 **FAIL-observed for physical acceptance**다.
- 40 FPS는 raster만이 아니라 `WM_SIZING` geometry admission까지 함께 제한했는데도 떨림을 없애지 못했다. 따라서 원인은 처리량 부족이나 cadence가 아니며, high-resolution pacing과 v10 계약은 현재 source에서 제거했다.
- 현재 source는 40 FPS 전 v9 ordering으로 돌아갔다. `Right`/`Bottom`은 사용자가 거의 완벽하다고 확인한 fixed-origin 경로를 유지하지만, 표준 top-level HWND가 origin/extent를 소유하는 `Left`/`Top`은 accepted 상태가 아니다.
- 고정 envelope와 custom non-client를 사용하는 app-owned geometry 구조는 사용자의 결정에 따라 채택하지 않는다. 따라서 현재 승인된 다음 구현 후보는 없으며 `Left`/`Top` 물리 acceptance는 미해결 상태다.

## 원인

직전 exact-child 구조에는 shell/top-level geometry와 visible child raster라는 두 개의 표시 geometry 소유자가 있었다. Matching Present를 child `WM_SIZE` 안에서 기다려도 두 HWND subtree의 DWM commit을 하나의 원자적 변경으로 만들 수 없었다. 이 때문에 좌·상단처럼 창 origin과 extent가 동시에 바뀌는 resize에서 이전 child 범위 밖의 top-level plane이 먼저 노출될 수 있었다.

Presentation target을 top-level HWND로 되돌리면 geometry 소유자는 하나가 되지만, 별도 USER32 redirection bitmap은 target-size commit이 늦는 순간 흰색을 드러냈다. `WS_EX_NOREDIRECTIONBITMAP`만 적용하면 같은 구간이 투명/검은색으로 바뀌었을 뿐 원인은 사라지지 않았다.

Top-level target으로 합친 뒤에도 표준 HWND의 origin/extent는 USER32/DWM transaction이고 Presentation frame/clip은 DirectComposition transaction이다. 둘은 동일 compositor tick에 합쳐질 수는 있어도 하나의 원자적 commit으로 묶을 수 없다. 우측·하단은 screen origin이 고정되어 이 차이가 거의 보이지 않지만, 좌측·상단은 origin과 content 좌표계가 함께 바뀌므로 한 transaction이 먼저 보이면 잔상 같은 이중 이동이 된다. FPS 제한, `DwmFlush`, 배경 채우기, 제출 순서 변경은 이 ownership 경계를 제거하지 못한다.

Acrylic에서는 premultiplied Vulkan surface의 retained capacity가 현재 logical viewport 밖에서 투명하다. 따라서 창 geometry와 raster commit 사이에 남는 pixel을 app의 임의 색으로 칠하지 않고, 기존 `DesktopAcrylicController`와 같은 HWND 범위의 system material로 채울 underlay가 필요하다.

Universal Skia-first의 raster/copy/Present **제출** 순서는 필요하지만, 모든 방향에서 DWM 표시 완료까지 기다린 것이 `Left`/`Top`의 이중 이동을 만들었다. Proposed layout을 이전 screen origin에서 강제로 한 번 scan-out한 뒤 HWND origin이 이동하기 때문이다. 반대로 `Present` 자체를 actual `WM_SIZE` 뒤로 미루면 새 client와 이전 raster 사이에 material tail이 생긴다. 따라서 moving-origin은 geometry 전 제출은 유지하되 pre-geometry display wait만 제거해야 한다.

## 현재 ownership

```text
top-level HWND
  ├─ shell geometry, client clip, input/focus/IME/UIA
  ├─ DWM transient-window backdrop underlay (Acrylic only)
  ├─ non-topmost DesktopWindowTarget + active DesktopAcrylicController
  └─ native topmost DirectComposition Vulkan Presentation target
       └─ full-capacity identity source, premultiplied for Acrylic

hidden child HWND
  └─ monitor/DPI retained-capacity probe only; visible pixels/input 없음
```

ANGLE은 계속 기본 presenter이고 이 변경은 명시적 `DOROTI_WINDOWS_PRESENTER=Vulkan` 경로에만 적용된다. Vulkan에서 ANGLE로 조용히 fallback하지 않는다.

## 방향별 resize ordering

```text
Right / Bottom / BottomRight
WM_SIZING(proposed window RECT)
  → proposed client extent 계산
  → hidden capacity가 작으면 먼저 grow
  → proposed viewport metrics 발행
  → Skia가 exact proposed-size scene을 retained backing에 raster
  → Vulkan copy 완료
  → IPresentationManager::Present
  → matching Composition frame / DwmFlush 경계 관찰
  → 성공한 뒤 WM_SIZING 반환

USER32
  → top-level geometry와 Acrylic HWND 범위를 proposed RECT로 적용

actual WM_SIZE
  → 이미 같은 metrics면 두 번째 raster 없음

Left / Top / TopLeft / TopRight / BottomLeft
WM_SIZING(proposed window RECT)
  → proposed viewport metrics 발행
  → exact Skia raster → Vulkan copy
  → IPresentationManager::Present 제출
  → CompositionFrame/DwmFlush로 이전 origin 표시를 강제하지 않고 즉시 반환
USER32
  → 바로 이어 top-level origin/extent를 proposed RECT로 적용
actual WM_SIZE
  → pre-submitted metrics와 exact match면 두 번째 raster/Present 없음
  → programmatic/mismatch resize만 bounded exact render fallback
WM_EXITSIZEMOVE
  → final actual geometry에서 DwmFlush 한 번으로 마지막 제출/geometry settle
```

우측·하단의 이미 양호한 선행 Present+DWM wait는 유지한다. 좌측·상단의 40 FPS pacing은 물리 실패 후 제거했다. 별도의 100 ms wait는 GPU raster/copy/Present 제출이 영원히 UI transaction을 막지 않게 하는 fail-safe이며 cadence limiter가 아니다. Source translation, stretch, edge별 scene 이동, child-position 보정은 사용하지 않는다.

## Acrylic 계약

- Windows 11 24H2 build 26100 이상과 `DesktopAcrylicController.IsSupported()`를 요구한다.
- non-topmost `DesktopWindowTarget`에 `DesktopAcrylicController.SetTarget(WindowId, CompositionTarget)`을 적용하고 `DWMWA_USE_HOSTBACKDROPBRUSH=TRUE`를 요구한다.
- `DWMWA_SYSTEMBACKDROP_TYPE=DWMSBT_TRANSIENTWINDOW`는 target-lag pixel을 같은 system material로 채우는 HWND-wide resize underlay다. 기존 active controller가 정상 viewport의 Acrylic 재질 소유자다.
- topmost Vulkan Presentation surface는 `DXGI_ALPHA_MODE_PREMULTIPLIED`다.
- `WS_EX_NOREDIRECTIONBITMAP`으로 별도 흰색 USER32 backing plane을 제거한다.
- attach/controller/host backdrop/system underlay/premultiplied 조건이 실패하면 explicit Vulkan+Acrylic 요청을 fail-fast한다.
- kind/theme/tint/luminosity option은 기존 controller에 last-wins로 적용된다.

## 현재 자동 evidence

- `.doroti/evidence/codex-vulkan-moving-40fps-hires-opaque-topleft-20260905/manifest.json` — `PASS-automated-partial`; 87 capture, strict gap 0 frame/0 px, outer/accepted/present 24/23/22, 37.99 presentations/s, present gap p95/max 27.659/29.277 ms, accepted-to-next-present p95/max 5.726/6.753 ms, moving pre-submit/display-wait 28/0, final exact/resource PASS다.
- `.doroti/evidence/codex-vulkan-moving-40fps-hires-acrylic-topleft-20260905/manifest.json` — transport/marker/final exact/resource와 outer/accepted/present 24/23/23, 39.43 presentations/s, moving pre-submit/display-wait 28/0, Composition wait/observed/timeout 2/2/0은 PASS다. 전체 status는 strict material-tail 31/73 frame, 최대 59 px 때문에 `FAIL`로 유지한다.
- high-resolution timer 전 두 후보는 strict gap 0이었지만 33.53 fps와 30.22 fps까지 과도하게 낮아졌다. Windows 일반 sleep 양자에 의존한 결과이므로 현재 40 FPS 후보로 사용하지 않는다.
- `.doroti/evidence/codex-vulkan-moving-40fps-hires-capability-r2-20260905/manifest.json` — 폐기된 v10/40 FPS source-built aggregate의 `PASS-automated-partial` 기록이다. 물리 확인 실패 후 current source acceptance로 사용하지 않는다. 당시 native SHA-256은 `8b787b3375c16a2715e1891f343d649cedae616556e9fd7ab7bd9e873d73eb60`이다.
- 첫 v10 aggregate `.doroti/evidence/codex-vulkan-moving-40fps-hires-capability-20260905`는 기존 ANGLE baseline의 stale-input copy 1개가 supersede되어 `Presents 24 != GpuCopies 25`로 중단됐다. 재실행 PASS가 이 간헐 실패를 지우지 않는다.
- `.doroti/evidence/codex-vulkan-moving-submit-final-acrylic-topleft-20260904/manifest.json` — 전체 status `FAIL` 보존. Acrylic strict app-background coverage가 23/82 frame, 최대 9 px를 검출했다. Transport, marker, final exact, resource, top-level owner는 PASS다. 94 outer changes, 93 accepted/presented target, 157.19 presentations/s, accepted-to-next-present p95/max 5.773/7.170 ms, platform wait timeout 0이다.
- 같은 Acrylic run의 방향 contract는 moving-origin pre-geometry Presentation 제출 97, 해당 단계 display wait 0이다. 전체 CompositionFrame wait/observed/timeout 2/2/0은 초기/정확 fallback 경계이며 moving-origin interactive pre-submit을 직렬화하지 않는다.
- `.doroti/evidence/codex-vulkan-moving-submit-final-opaque-topleft-20260904/manifest.json` — final exact/resource와 moving pre-submit/display-wait 106/0은 PASS지만, 67 capture 중 2 frame에서 최대 18 px exact app-raster gap이 검출돼 전체 `FAIL`이다. 저장 anomaly PNG에서도 좁은 오른쪽 tail을 확인했다. 직전 `.doroti/evidence/codex-vulkan-moving-submit-opaque-topleft-20260904/manifest.json`의 0 frame/0 px PASS는 이 간헐 실패를 지우지 않는다.
- `.doroti/evidence/codex-vulkan-moving-submit-acrylic-right-20260904/manifest.json` — fixed-origin 회귀의 transport/marker/final/resource와 53 pre-geometry admissions, DWM wait/observed/timeout 55/55/0은 PASS다. 전체 status는 strict Acrylic sentinel 5/69 frame, 최대 27 px 때문에 `FAIL`을 유지한다.
- `.doroti/evidence/codex-vulkan-moving-submit-final-acrylic-capability-r4-20260904/manifest.json` — 현재 복귀한 v9 ordering의 이전 aggregate `PASS-automated-partial` 기록이다. Source를 다시 build하더라도 이 자동 결과는 물리 left/top acceptance를 뜻하지 않는다.
- 첫 aggregate `.doroti/evidence/codex-vulkan-moving-submit-acrylic-capability-20260904`는 ANGLE baseline의 stale-input copy 1개가 supersede되어 `Presents 24 != GpuCopies 25`로 중단됐다. 재실행 PASS가 이 간헐 실패를 지우지 않는다.
- Windows Release build는 warning 0/error 0이고 native ABI fixture는 PASS다.

## 보존하는 실패 기록

- exact-child + per-frame Presentation/DWM Acrylic: `.doroti/evidence/codex-vulkan-exact-child-dwm-commit-acrylic-topleft-20260904/manifest.json` — 28/78 gap frame, 최대 43 px.
- top-level synchronous target + redirection bitmap: `.doroti/evidence/codex-vulkan-top-level-sync-acrylic-topleft-20260904/manifest.json` — 28/71 gap frame, 최대 42 px, 흰색 노출.
- top-level + `WS_EX_NOREDIRECTIONBITMAP`만 적용: `.doroti/evidence/codex-vulkan-top-level-sync-noredir-acrylic-topleft-20260904/manifest.json` — 4/81 gap frame, 최대 35 px, 검은색 노출.
- universal Skia-first + underlay는 빈 영역을 제거했지만 사용자의 물리 확인에서 `Left`/`Top` 잔상 같은 떨림이 남아 현재 방향별 ordering으로 supersede했다.
- moving-origin two-phase `Prepared → post-geometry Present/DWM`도 사용자의 다음 물리 확인에서 떨림과 material 영역을 남겨 FAIL-observed로 supersede했다.
- 직전 자동 PASS들은 삭제하거나 현재 물리 PASS로 재분류하지 않는다.

## 40 FPS 후보의 검증 경계 (물리 FAIL로 superseded)

현재 결론은 **40fps-paced-pending-user-recheck / mixed automated evidence**다. 새 moving-origin 25 ms geometry+raster pacing의 불투명 gap/final/resource 계약은 자동 PASS지만 Acrylic strict app-raster pixel gate는 FAIL 상태를 유지한다. 실제 모니터에서 40 FPS가 떨림을 줄이는지, 또는 단순히 더 큰 step의 choppiness로 보이는지는 동일한 실행 명령으로 직접 확인해야 한다.

NVIDIA/Intel, 다른 AMD driver, 125/150/200% DPI, 60/120/144 Hz, mixed-DPI/Snap/maximize/occlusion, 물리 IME/accessibility와 실제 device-removal은 계속 `notVerified`다.

## 최종 물리 판정과 제외한 구조

사용자의 재확인으로 40 FPS 후보도 `Left`/`Top` 물리 acceptance에서 `FAIL`이다. High-resolution pacing과 v10 계약은 current source에서 제거하고 v9 ordering으로 복귀했다. 자동 gap/final/resource 결과는 이 물리 실패를 뒤집지 않는다.

구조 분석상 좌·상단까지 없애려면 resize 중 top-level HWND origin을 USER32에 계속 맡기는 현재 구조가 아니라, 고정 envelope 내부의 border/chrome/content/Skia geometry를 app이 한 Composition transaction으로 소유해야 한다. 그러나 사용자는 이 Arm N/fixed-envelope/custom-non-client 방향을 채택하지 않기로 했다. 관련 구현은 남기지 않으며 표준 HWND v9 경로의 `Left`/`Top` 떨림은 해결되었다고 주장하지 않는다.

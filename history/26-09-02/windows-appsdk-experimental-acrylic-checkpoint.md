# Windows App SDK `experimentalAcrylic` 체크포인트

- 기록일: 2026-09-02
- 성격: 기존 `work.md`를 Silk.NET Vulkan 활성 계획으로 교체하기 전의 상태 보존
- 기본 경로: opaque `HwndExactCpp` + managed ANGLE/EGL-D3D11
- 실험 경로: `experimentalAcrylic` + ContentIsland + ANGLE D3D11 Composition Swapchain

## 보존 판정

| 항목 | 판정 | 근거 |
|---|---|---|
| opaque 기본 경로 | PASS-current baseline | Release, native ABI, opaque before/after, empty-PATH/package 검사가 통과했다. 기본 renderer는 계속 ANGLE/EGL-D3D11이다. |
| Acrylic 제품 구조/API | PASS-implemented | pre-show topology/fallback, Composition Swapchain presenter, runtime kind/theme/tint snapshot과 terminal 계약이 구현됐다. |
| strict P0.5 top-HWND | **FAIL** | Acrylic backdrop은 보였지만 candidate WGC에서 Doroti scene marker가 검출되지 않았다. |
| strict P1-CS visible | **FAIL** | buffer 500회/3-slot 자체는 통과했으나 matched frame 236개 중 160개에서 presented buffer extent와 client extent가 달랐다. |
| 2026-09-02 TopLeft 3초 | PASS-current-capture | active/inactive max 9/0px, frame-id coverage 약 100%, settle 29.16ms였다. |
| 2026-09-02 TopLeft 600ms | **FAIL** | active max 37px, matched coverage 약 74%, accepted cadence 38.17fps, uncropped app-bar 1/title 6, gap 23/13px였다. |
| 전체 자동 matrix | notRun | 8방향, 전체 속도, DPI/refresh/monitor/device-loss 3회 matrix를 완료하지 않았다. |
| 실물 acceptance | notVerified | 실제 scan-out, 사람의 border drag, 한국어 IME, Narrator/Accessibility Insights는 자동 WGC로 증명되지 않는다. |

## 마지막 구현 체크포인트

- `ResizeContentToParentWindow`가 bridge HWND geometry를 소유하고 native `WM_SIZE`는 top-client viewport만 발행한다.
- 성공 callback은 buffer, source rect, identity transform을 한 번에 present하며 full-frame stretch와 256px dark overscan을 제거했다.
- 12px transparent guard와 stationary-edge alignment로 retained frame을 1:1 crop/clip한다.
- Composition `WM_SIZE`/`WM_EXITSIZEMOVE`의 WndProc은 terminal, fence, event, `DwmFlush`를 기다리지 않는다.
- 수정 뒤에도 빠른 TopLeft 600ms hard gate는 실패했다. 다음 원인 후보는 explicit resize-edge 전달과 interactive slot 획득의 nonblocking current+latest 전환이었다.

## 대표 evidence

- `.doroti/evidence/experimental-acrylic-20260902-083717-a6463018bc/manifest.json`
- `.doroti/evidence/experimental-acrylic-wrapup-20260902/manifest.json`
- `.doroti/evidence/acrylic-p05-20260901-162530-02a5d0bb70f3/manifest.json`
- `.doroti/evidence/acrylic-p1cs-20260901-163103-883fe1bbe050/manifest.json`
- [P0/P1 gate 결과](../26-09-01/windows-appsdk-acrylic-p0-p1-gate-results.md)
- [P0.5/P1-CS gate 결과](../26-09-01/windows-appsdk-acrylic-p05-p1cs-gate-results.md)

## 후속 경계

이 기록은 Acrylic을 PASS 또는 stable로 승격하지 않는다. 새 Silk.NET Vulkan 계획은 opaque HWND의 별도 presenter 후보이며 `experimentalAcrylic`의 fallback, hidden presenter, 또는 Composition 경로로 사용하지 않는다. Acrylic 작업을 재개할 때는 위 FAIL/notRun/notVerified 상태에서 이어간다.

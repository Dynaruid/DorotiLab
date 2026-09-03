# Windows Vulkan live-resize 표시 불일치

## 현재 판정

- 사용자 시각 판정: **PASS-observed (2026-09-03)**
- 구현 판정: **accepted** — 현재 shared runtime 수정을 이 문제의 해법으로 채택
- 엄격 자동 판정: **PARTIAL** — 정식 반복 세트는 matrix 12회 중 11회, case 24개 중 22개 통과; 이후 최종 source-built matrix는 별도 PASS
- 대상: `HwndExactCpp` runner의 opt-in `DOROTI_WINDOWS_PRESENTER=Vulkan`
- 관찰 환경: AMD Radeon 780M 선택, 96 DPI, 165 Hz

사용자가 화면에 표시된 반복 Left/TopLeft headed test를 직접 보고, 검은 띠나
resize 거동이 더 이상 사용을 막을 정도가 아니라고 확인했다. 따라서 이 문제의
사람 체감 acceptance는 `PASS-observed`로 닫는다.

다만 이것을 자동 검출까지 100% 통과했다는 뜻으로 확대하지 않는다. 12번째까지의
반복 중 7회차에서 Left와 TopLeft 각각 한 프레임의 validation-background gap이
검출됐다. 아래에 그 결과를 `PARTIAL`로 그대로 남긴다.

## 근본 원인

초기 Vulkan 경로는 화면 표시가 다음 두 수명주기로 갈라져 있었다.

1. USER32/DWM이 top-level HWND의 origin, extent와 client clip을 갱신했다.
2. 별도의 visible child HWND와 Vulkan WSI swapchain이 app raster를 표시했다.

Retained oversized child로 매 resize step의 `vkCreateSwapchainKHR` 비용은
없앴지만, top-level geometry와 child WSI buffer를 하나의 표시 transaction으로
만들 수는 없었다. 특히 Left/TopLeft drag는 origin과 extent가 함께 변하므로
geometry가 먼저 보이는 순간 검정/흰 영역과 이전 raster의 늦은 이동이 두드러졌다.

`WM_SIZE`에서 exact frame을 기다리는 방식과 `DwmFlush`만으로는 이미 변경된
USER32 geometry와 별도 WSI present를 원자적으로 묶을 수 없었다.

## 채택한 구조

현재 opt-in Vulkan 제품 경로는 visible Vulkan WSI를 사용하지 않는다.

```text
Skia/Vulkan retained offscreen backing
  → Vulkan copy
  → same-LUID D3D11 BGRA shared texture 3개
  → Windows Presentation API
  → retained child HWND의 native DirectComposition target
  → top-level client clip
```

핵심 계약은 다음과 같다.

- top-level HWND는 USER32의 window geometry와 input authority로 남는다.
- visible content는 monitor work-area 이상으로 유지되는 child HWND의
  DirectComposition target 하나가 소유하고, parent가 `WS_CLIPCHILDREN`으로
  현재 client 영역만 표시한다.
- parent에는 `WS_EX_NOREDIRECTIONBITMAP`을 적용해 parent redirection bitmap과
  child DComp tree가 서로 다른 frame에 합성되는 경로를 만들지 않는다.
- Vulkan 장치와 같은 adapter LUID의 D3D11 device가 만든 BGRA shared texture
  3개를 Vulkan에 dedicated external memory로 import한다.
- Skia backing의 현재 logical viewport를 retained-frame image에 갱신하고, 나머지
  capacity는 마지막 app-owned pixels로 유지한다. 매 Present에는 이 full-capacity
  guard를 복사해 새 geometry가 드러나도 미소유 검정 영역이 나타나지 않게 한다.
- Vulkan copy fence가 완료된 뒤에만 `IPresentationManager::Present`를 호출한다.
  buffer 재사용과 teardown은 Presentation availability event가 결정한다.
- `PRESENTATION_ERROR_LOST`처럼 결과가 불확정하면 context를 poison하고
  fail-fast한다.

## resize 순서

최종 구현은 호스트 창의 크기나 위치를 임의 FPS로 제한하지 않는다.

```text
WM_SIZING proposed RECT
  → RECT를 수정하지 않고 proposed client 크기 계산
  → retained child capacity 확인
  → proposed logical viewport generation 발행
  → exact Vulkan frame Present
  → 같은 present-id/tag의 CompositionFrame 통계 관찰
  → WM_SIZING 반환
  → USER32가 원래 proposed RECT를 commit
  → WM_SIZE에서 matching prepared frame 승인
     또는 mismatch/timeout이면 actual client 크기로 exact fallback
```

즉 USER32가 geometry의 유일한 소유자이고, Doroti는 다음 geometry를 대신
`SetWindowPos`하지 않는다. 대신 그 geometry에 대응하는 app frame이 DWM
composition frame에 들어간 것을 먼저 확인하는 pre-geometry frame gate만 둔다.

## 호스트 resize 제한안을 채택하지 않은 이유

검토한 제한안은 `WM_SIZING`의 proposed RECT를 마지막 승인 크기로 되돌리고,
posted message 또는 timer가 준비된 크기를 `SetWindowPos`하는 방식이었다.
실험에서는 USER32가 나중에 이전 proposal을 다시 적용하면서 cursor와 창 geometry가
어긋났고, 간헐적 gap이 오히려 커졌다.

따라서 다음 항목은 최종 코드에서 제거했다.

- host-owned pending sizing rectangle
- cursor-anchor 기반 geometry 재계산
- posted/timer `SetWindowPos` commit
- 30/60 FPS 같은 고정 window-resize limiter

## 검증 결과

### 사용자 관찰

반복 자동 pointer resize가 실제 화면에 표시되는 동안 사용자가 직접 Left와
TopLeft 동작을 확인했다. 현재 보이는 결과를 수용 가능하다고 판단했으므로
이 문제의 최종 시각 판정은 `PASS-observed`다.

이 판정은 현재 AMD/96 DPI/165 Hz 환경에 한정한다. 사용자가 보지 않은 GPU,
DPI, refresh-rate와 mixed-monitor 조합까지 통과했다는 뜻은 아니다.

### focused 반복 자동 검증

- evidence: `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-probe1`
  부터 `probe12`
- matrix: 11/12 `PASS-automated-partial`
- case: 22/24 `PASS`
- source에서 직접 build한 최초 matrix: `probe1`
- `probe2`~`probe12`: 동일 binary 반복 관찰을 위한 `-SkipBuild`

유일한 strict 실패인 `probe7`:

| case | gap 검출 | 나머지 계약 |
|---|---:|---|
| Left | 1 frame, 최대 8 px | transport/resource/final exact PASS, CompositionFrame 30/30, timeout 0 |
| TopLeft | 1 frame, 최대 60 px | transport/resource/final exact PASS, CompositionFrame 28/28, timeout 0 |

`probe1`과 `probe2`~`probe6`, `probe8`~`probe12`에서는 두 case가 모두
통과했다. 따라서 strict pixel oracle은 `PARTIAL`이며, 일회성 검출을 삭제하거나
완전한 자동 PASS로 재분류하지 않는다.

### 최종 source-built 회귀 검증

- live resize: `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-final2`
  — `PASS-automated-partial`, source→binary correspondence PASS
- Left: gap 0, outer change 26, accepted/presented 25/26,
  CompositionFrame 30/30, timeout 0, final exact/resource PASS
- TopLeft: gap 0, outer change 26, accepted/presented 25/26,
  CompositionFrame 31/31, timeout 0, final exact/resource PASS
- capability/lifecycle: `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-capability-final2`
  — aggregate PASS, product/exact-resize/device-reset/minimize-restore/start-close 10회 PASS,
  Composition buffer 3, active WSI 0, source/repository/binary endpoint 안정성 PASS

## 범위 밖 또는 후속 확인

- NVIDIA/Intel 선택 경로
- 125/150/200% DPI
- 60/120/144 Hz 및 다른 monitor
- mixed-DPI 이동, Snap, maximize/restore, alt-tab/occlusion
- 물리 IME와 accessibility acceptance
- `PRESENTATION_ERROR_LOST` 뒤 manager/import graph 자동 재생성

이 항목들은 `notVerified`지만 현재 사용자가 승인한 검은 띠 문제를 다시
미해결로 만들지는 않는다. 별도 환경에서 같은 증상이 관찰될 때 해당 matrix의
새 결함으로 재개한다.

## 결론

현재 결론은 **PASS-observed / automated PARTIAL**이다. visible Vulkan WSI의
two-owner 문제를 제거하고, retained child DirectComposition target과
pre-geometry CompositionFrame gate를 적용했다. 사용자가 직접 본 headed
Left/TopLeft 결과는 수용 기준을 통과했으며, 반복 자동 검출의 1회성 두 프레임은
남은 계측 경계로 정직하게 보존한다.

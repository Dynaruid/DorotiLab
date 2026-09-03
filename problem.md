# Windows Vulkan live-resize 표시 불일치

## 현재 판정

- 상태: **FAIL-observed — 이전보다 미세하게 개선됐지만 해결되지 않음**
- 대상: `HwndExactCpp` runner의 실험적 `DOROTI_WINDOWS_PRESENTER=Vulkan` 경로
- 주요 장치: AMD Radeon 780M
- 가장 민감한 동작: 창의 왼쪽 및 왼쪽 위 border를 잡고 확대·축소
- 자동 상태: focused `TopLeft` probe는 `PASS-automated-partial`
- 물리 상태: 사용자의 실제 border drag에서 흰색/검정 영역과 내부 raster 떨림이 계속 관찰됨

자동 검증 통과는 프레임 요청과 Vulkan present가 정상적으로 끝났다는 뜻일 뿐, DWM이 실제 화면에 어떤 중간 상태를 표시했는지는 증명하지 않는다. 이 문제에서는 사람에게 보이는 결과를 최종 판정으로 사용한다.

## 재현 방법

```powershell
$env:DOROTI_WINDOWS_PRESENTER = 'Vulkan'
$env:DOROTI_WINDOWS_VULKAN_DEVICE = 'AMD'
Remove-Item Env:DOROTI_DEMO_EXPERIMENTAL_ACRYLIC -ErrorAction SilentlyContinue

pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform windows `
  -Configuration Release
```

실행된 창에서 다음을 확인한다.

1. 왼쪽 border를 빠르게 확대·축소한다.
2. 왼쪽 위 corner를 대각선으로 확대·축소한다.
3. 내부 내용이 새 창 크기에 늦게 맞춰지는지 관찰한다.
4. 새로 생긴 영역이나 가장자리에 흰색 또는 검정 영역이 순간적으로 나타나는지 관찰한다.

## 관찰되는 증상

### 1. 흰색 또는 검정 영역

창의 visible client 영역이 변하는 도중, 아직 같은 geometry에 대응하는 앱 프레임이 DWM에 표시되지 않은 구간이 순간적으로 보인다. 이 색만으로 parent, child surface, DWM 중 어느 계층이 노출됐는지를 확정할 수는 없다. 다만 앱의 정상 scene이 아닌 중간 presentation 상태가 보인다는 것은 분명하다.

Parent를 앱의 light/dark 배경색으로 칠하는 fallback은 사용하지 않는다. 그것은 ownership 간격을 제거하지 않고 결함을 가리기 때문이다.

### 2. 내부 raster가 뒤늦게 창 크기에 맞춰짐

창 외곽은 이미 새 위치와 크기로 보이는데, 내부에는 직전 logical viewport로 그린 프레임이 잠시 남는다. 이후 exact-size 프레임이 도착하면 내용이 새 크기로 다시 배치되며, 사용자는 raster가 창을 한 단계 늦게 따라오는 것으로 느낀다.

### 3. 왼쪽·위쪽 조절에서 더 큰 떨림

오른쪽·아래쪽 resize는 일반적으로 창 origin을 유지한 채 extent만 바뀐다. 왼쪽·위쪽 resize는 화면상의 origin과 extent가 동시에 바뀐다. 그 결과 이전 프레임의 화면 위치 이동과 새 layout 교체가 겹쳐 보이고, 같은 presentation 지연도 더 큰 내부 흔들림으로 인식된다.

## 현재 구조

현재 visible 경로에는 두 개의 독립적인 소유자가 있다.

1. USER32/DWM은 top-level HWND의 화면 위치, 크기와 client clip을 갱신한다.
2. Doroti raster worker와 Vulkan WSI는 child HWND surface에 새 logical viewport 프레임을 그리고 present한다.

Retained Vulkan 경로는 child HWND와 swapchain을 monitor work-area 수준의 grow-only capacity로 유지한다. Top-level client가 그 oversized child를 `WS_CLIPCHILDREN`으로 자르므로 일반적인 resize에서는 child 또는 swapchain을 매번 재생성하지 않는다.

현재 resize 순서는 개념적으로 다음과 같다.

```text
pointer drag
  → USER32가 top-level origin/extent 변경
  → 실제 WM_SIZE 전달
  → Doroti가 새 logical metrics와 generation 발행
  → raster worker가 layout/draw/copy/present
  → HWND thread가 exact present terminal까지 최대 100 ms 대기
  → 성공 시 DwmFlush 후 WM_SIZE 반환
```

Vulkan 호출과 scene rendering은 HWND thread가 아니라 raster worker에서 수행된다. HWND thread는 렌더링을 직접 하지 않고 exact generation의 terminal을 기다린다.

## 핵심 원인

핵심은 **window geometry와 rendered frame이 하나의 atomic transaction으로 표시되지 않는 two-owner 구조**다.

### 대기가 시작될 때는 geometry가 이미 변경됨

`WM_SIZE`는 새 client 크기의 authority로서는 정확하지만, USER32가 창 크기를 변경한 뒤 전달된다. 따라서 `WM_SIZE` 안에서 exact frame까지 기다리면 다음 resize step이 렌더보다 계속 앞서가는 현상은 억제할 수 있지만, 현재 step의 geometry가 먼저 노출된 사실까지 취소할 수는 없다.

### `vkQueuePresentKHR` 반환은 실제 화면 표시 완료가 아님

현재 exact-present terminal은 올바른 generation이 `vkQueuePresentKHR`를 통과했음을 뜻한다. 이는 presentation engine에 요청이 enqueue됐다는 강한 render-side 증거지만, 해당 프레임과 USER32 geometry가 동일한 DWM composition에서 원자적으로 보였다는 증거는 아니다.

### `DwmFlush`도 cross-owner atomic commit을 만들지 못함

`DwmFlush`는 outstanding compositor 작업의 진행을 기다려 ordering과 pacing을 개선한다. 그러나 USER32의 top-level 위치/clip 변경과 Vulkan child surface의 buffer를 하나의 공개된 transaction으로 묶는 계약은 아니다. 따라서 노출 시간을 줄일 수는 있어도 중간 상태가 절대로 보이지 않는다고 보장하지 못한다.

## 지금까지 개선된 부분

### Per-size swapchain recreation 제거

이전 exact-size child/swapchain 경로에서는 AMD에서 매 resize step의 `vkCreateSwapchainKHR`가 보통 약 19~25 ms를 차지했다. Retained oversized child/surface로 ordinary resize의 이 비용을 제거했다. 현재 focused run에서는 surface recreate 1회, retained reuse 37회였다.

### Flutter-style actual-size blocking

실패한 `WM_SIZING` provisional layout과 16 ms prepare 상태를 제거했다. 현재는 실제 `WM_SIZE`가 새 generation을 발행한 뒤 exact present까지 최대 100 ms 기다린다. 이 변경으로 체감이 미세하게 개선됐지만 two-owner atomicity 문제는 남아 있다.

### 고정 FPS 제한을 사용하지 않음

30/60 Hz 같은 native-window limiter는 이전 프레임이 남아 있는 시간을 늘릴 수 있다. 현재 구현은 fixed timer가 아니라 render completion이 USER32 진행에 backpressure를 주는 방식이다.

### 배경색 masking을 사용하지 않음

Parent의 노출 영역을 앱 배경색으로 채우지 않는다. 흰색/검정 영역이 계속 존재하면 그대로 관찰 가능해야 하며, 실제 구조 수정으로 제거됐을 때만 해결로 판정한다.

## 현재 evidence가 말하는 것

간섭 없이 실행한 focused `TopLeft` 1회 결과는 다음과 같다.

| 항목 | 결과 |
|---|---:|
| outer-rect changes | 33 |
| accepted targets during motion | 31 |
| presented terminals during motion | 32 |
| superseded targets | 0 |
| platform wait timeouts | 0 |
| marker regressions | 0 |
| presentation rate | 54.09 Hz |
| target→present p95 / max | 22.71 / 26.47 ms |
| Vulkan acquire / present | 44 / 44 |
| surface recreate / retained reuse | 1 / 37 |

이 결과는 request/terminal 누락, timeout, 반복 swapchain recreation이 현재 증상의 직접 원인일 가능성을 낮춘다. 반대로 165 Hz display에서 render-side target→present p95 22.71 ms는 여러 refresh interval에 해당하며, 그 뒤의 presentation-engine 처리와 geometry 결합은 계측하지 못한다.

WGC capture도 모든 물리 scan-out을 포착하지 않는다. 따라서 blank frame 0이나 marker regression 0만으로 실제 흰색/검정 transient가 없었다고 결론 내릴 수 없다.

## 이미 실패했거나 채택하지 않은 접근

- 매 크기마다 child HWND와 swapchain을 exact extent로 재생성: AMD driver 비용과 exposed interval이 큼
- `WM_SIZING` proposed extent를 16 ms 동안 미리 렌더링: 약간 개선됐지만 실제 resize에서 흰색/검정과 떨림이 남음
- Proposed frame 뒤 `DwmFlush`: ordering을 개선했지만 geometry/frame atomicity를 만들지 못함
- Parent를 앱 배경색으로 채우기: 문제를 가리므로 명시적으로 제외
- 창 resize를 임의의 30/60 FPS로 제한: stale frame 지속 시간을 늘릴 수 있어 제외
- 자동 검증 반복 확대: 물리 transient 검출력이 충분하지 않아 focused 1회로 축소

## 남은 해결 방향

현재 direct Vulkan WSI + visible child HWND 구조 안에서 할 수 있는 backpressure와 surface-retention 완화는 대부분 적용됐다. 다음 구조 변경의 목표는 **top-level geometry/clip과 visible buffer를 같은 compositor ownership 아래에서 commit하는 것**이다.

검토할 수 있는 방향은 다음과 같다.

1. Vulkan은 offscreen raster만 담당하고, 최종 이미지를 DXGI/DirectComposition 또는 Windows App SDK composition surface로 전달한다.
2. Visual offset, clip, size와 새 buffer를 하나의 composition commit에 포함한다.
3. 가능하다면 visible child HWND 경계를 제거해 top-level visual과 presentation surface의 소유자를 통합한다.
4. AMD뿐 아니라 NVIDIA/Intel에서도 동작하는 Windows 표준 interop 및 synchronization API만 사용한다.

이는 아직 선택된 구현안이 아니라 다음 실험이 만족해야 할 구조적 조건이다. Vulkan→DXGI 공유 방식, 복사 비용, keyed fence/semaphore 지원, device-loss 수명주기와 SkiaSharp 제약을 별도로 검증해야 한다.

## 완료 조건

다음 조건을 모두 만족하기 전에는 해결로 판정하지 않는다.

- 실제 마우스로 왼쪽 및 왼쪽 위를 반복 확대·축소할 때 흰색/검정 영역이 보이지 않음
- 내부 raster가 새 창 위치나 크기에 뒤늦게 맞춰지는 움직임이 보이지 않음
- App-color parent masking 없이 통과
- resize 중 request/terminal 누락, timeout, Vulkan 오류가 없음
- resize 종료 후 final viewport와 client extent가 정확히 일치
- AMD에서 우선 통과하고, 이후 NVIDIA/Intel 및 DPI/monitor 이동 matrix를 별도 확인

현재 결론은 **Flutter-style window-thread blocking으로 증상의 빈도와 정도는 줄었지만, USER32 geometry와 Vulkan presentation의 분리된 소유권 때문에 근본 문제는 남아 있다**는 것이다.

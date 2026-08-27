# Windows App SDK HwndExactCpp / ANGLE resize 작업 요약

- 기록 기준일: 2026-08-26
- 요약·보관일: 2026-08-27
- 원본: 삭제한 루트 `idea.md`, `work3.md`의 설계·구현·검증 결과를 통합한 역사 기록
- 보관 시점 HEAD: `486d12a8`
- Windows App SDK: repository 고정 `2.4.0`, self-contained unpackaged `win-x64`
- Flutter source 비교 기준: `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- 최종 제품 경로: `WindowsAppSdk` + `HwndExactCpp` + managed ANGLE/EGL-D3D11 + Skia
- 최종 판정: **Windows 기본 경로와 C10 사용자 acceptance는 PASS. D3D12 제품 경로는 FAIL 이력, Vulkan은 제거된 실험 이력, C7/C8 수동·물리 범위는 `notVerified`, 전체 Product 회귀는 Windows host의 macOS `sips` 부재 때문에 FAIL로 보존한다.**

이 문서는 2026-08-25의 설계 조사와 2026-08-26의 구현·검증을 압축한 종료 기록이다. 보관 과정에서 build, runtime, capture를 다시 실행하지 않았으며 아래 판정과 수치는 삭제 전 두 원본 및 이미 생성된 evidence를 기준으로 한다. 작업 자체를 종료·보관하는 것과 모든 플랫폼·수동 gate가 PASS라는 주장은 구분한다.

## 1. 설계 결론

Flutter식 Windows resize를 복제하는 primary topology로 ContentIsland/composition swap chain이 아닌 **앱 소유 child HWND**를 채택했다.

```text
standard top-level HWND + AppWindow
  └─ one app-owned WS_CHILD render HWND
       └─ managed ANGLE/EGL hardware-D3D11 fixed-size surface
            └─ Skia direct raster -> eglSwapBuffers
                 └─ first-surface/resize DwmFlush -> terminal wake
```

초기 `idea.md`는 `CreateSwapChainForHwnd`와 SkiaSharp D3D12 exact backing을 첫 후보로 제안했다. 실제 구현 과정에서 native/managed D3D12 lease와 실제 Doroti scene이 각각 debug/runtime 오류를 일으켜 제품 기본 경로로 승격하지 못했다. 최종 구현은 같은 child-HWND topology와 exact-generation 계약을 유지하면서 presenter만 managed ANGLE/EGL-D3D11로 확정했다.

ContentIsland는 renderer와 size authority에서 제외했다. 향후 필요성이 입증되더라도 입력·접근성·popup 같은 bounded integration boundary로만 검토한다. MAUI backend는 삭제하지 않고 명시적 독립 선택지로 남겼다.

## 2. 최종 ownership과 resize 계약

### native C++20

- top-level HWND, AppWindow, render child HWND, message-only task HWND를 소유한다.
- WndProc, DPI/lifecycle, child geometry, resize generation, bounded wait와 input ingress를 소유한다.
- GPU device/context/swap chain/back buffer/fence를 만들거나 ABI로 전달하지 않는다.

### managed .NET

- Doroti application/framework, layout/build, immutable scene와 `SkiaSceneRenderer`를 소유한다.
- raster/presentation thread가 ANGLE display/context, `EGL_FIXED_SIZE_ANGLE` window surface, Skia `GRContext`, submit/swap을 단독 소유한다.
- C ABI의 GPU pointer count는 항상 0이며 raw HWND, COM/Vortice type, native ABI struct를 public Doroti API에 노출하지 않는다.

### resize protocol

1. child `WM_SIZE`의 physical client extent를 새 generation으로 게시한다.
2. framework가 같은 generation과 exact size의 scene을 만든다.
3. raster worker는 실행 중 1개와 latest pending 1개만 유지하고 stale generation을 present하지 않는다.
4. current extent와 같은 fixed-size EGL surface에 Skia가 직접 raster한다.
5. `eglSwapBuffers` 성공 뒤 resize generation을 exactly once `Presented`, `Superseded`, `Failed` 중 하나로 정산한다.
6. product resize wait는 최대 100 ms이며 일반 top-level message를 재진입시키지 않는다.
7. 새 EGL surface의 첫 swap과 resize surface의 첫 swap 뒤 managed `DwmFlush`를 완료한 다음 terminal/first-show를 진행한다.

큰 capacity backing, full-frame provisional stretch, edge-pixel repetition, `SetSourceSize`, debounce, mouse-up geometry replay는 제품 해법에서 제외했다. runtime presenter 자동 fallback도 없다.

## 3. 구현 경로와 주요 결정

### D3D12

- native-owned frame lease 실험은 `GetGPUDescriptorHandleForHeapStart` 관련 D3D12 debug error ID 1315가 8건 발생해 FAIL이었다.
- managed 단독 GPU ownership 자체는 ABI/resource contract를 통과했지만, 실제 self-contained `DorotiDemoApp.WindowsAppSdk` scene에서 Skia submit 중 D3D12 operational error ID 1422가 6건 발생해 C5-D3D12가 FAIL했다.
- 오류 필터, private reflection, CPU fallback, SkiaSharp source patch로 실패를 숨기지 않았다.
- D3D12 presenter는 `DOROTI_WINDOWS_PRESENTER=D3D12` 명시 진단 경로로만 남고 기본값이나 자동 fallback이 아니다.

### ANGLE/EGL-D3D11

- managed presenter가 hardware D3D11 ANGLE context, exact fixed-size child-HWND surface와 Skia GL surface를 소유하도록 구현했다.
- x64 runtime은 `Avalonia.Angle.Windows.Natives 2.1.27548.20260419`의 `av_libglesv2.dll`을 사용한다. 검토한 `Silk.NET.OpenGLES.ANGLE.Native 2025.9.12`는 x64/x86 DLL이 모두 PE machine `0x014c`여서 x64 제품 runtime으로 채택하지 않았다.
- direct default-framebuffer raster, swap interval 0, latest-only raster worker로 초기 high-speed right-resize 진단을 `gapFrame=29`, 최대 연속 `551.524 ms`에서 `gapFrame=7`, 최대 `60.604 ms`까지 줄였다. 남은 최대 gap 32 px 때문에 strict pixel/cadence 결과는 FAIL로 보존했다.
- 사용자가 상·하·좌·우 실제 border drag에서 실시간 추종, 떨림, raster 왜곡을 확인해 당시 ANGLE physical resize를 PASS로 판정했다.
- 이후 Vulkan 제거 뒤 ANGLE 경로를 복원하면서 첫 창이 흰 상태로 보이고 resize 후에만 content가 나타나는 physical FAIL이 발견됐다. 원인은 최초 EGL surface의 첫 swap에만 `DwmFlush`가 빠진 compositor ordering 차이였다. 모든 새 surface의 첫 swap 뒤 flush하도록 수정했고 resize 입력 없는 750 ms capture에서 초기 content가 확인됐다.

### 제거된 Vulkan 실험

- direct Vulkan presenter는 한때 automated ownership, publish, exact-intermediate 제출을 통과했지만 제품 최종 경로에서는 완전히 제거했다.
- capacity surface + provisional stretch는 자동 capture의 빈 영역을 줄였으나 사용자가 resize 중 전체 화면 확대를 직접 확인해 physical FAIL로 폐기했다.
- exact-intermediate 경로의 우측·하단은 사용자 물리 PASS였지만 좌측·상단 origin-moving jitter가 남았다. stationary opposite-edge child 실험도 strict asynchronous gap oracle FAIL을 해소하지 못했다.
- `WindowsManagedVulkanPresenter`, `Vulkan` 선택지, Vulkan runtime-effect backend, `Silk.NET.Vulkan*` 의존성은 현재 source/package/runtime에 없다. 관련 PASS/FAIL은 비교 진단 이력일 뿐 재실행 가능한 current gate가 아니다.

## 4. 최종 gate ledger

시간 순서상 뒤에 기록된 C10 사용자 결정이 원본 상단과 이전 ledger의 `notVerified` 표기보다 우선한다.

| gate | 최종 상태 | 보존할 근거와 경계 |
| --- | --- | --- |
| C0 source contract | **PASS** | pinned Flutter source 5 files, 12 anchors, 3 mappings. source 계약만 증명한다. |
| C1 native ABI/package skeleton | **PASS** | x64 Release DLL, ABI v1 layout, 3 exports, empty `PATH` app-directory load, native host/bootstrap exactly once. |
| C2 standalone C++ topology | **PASS** | 10-cycle + warmup, accepted/terminal 30/30, wrong-size/stale/unaccounted 0, resource counts stable. 물리 화면 증거는 아니다. |
| C3 native D3D12 lease | **FAIL** | D3D12 ID 1315 8건을 보존한다. |
| C3 managed ownership | **PASS** | C++ GPU object 0, ABI GPU pointer 0, managed device/context ownership과 exactly-once terminal 확인. |
| C3-A ANGLE owner | **PASS** | hardware Direct3D11 ANGLE, fixed-size surface, EGL/GLES operational error 0. |
| C3-Vulkan | **historical PASS / removed** | 삭제 전 자동 결과만 보존하며 current executable gate가 아니다. |
| C4 bounded coordinator | **PASS** | filtered wait success/timeout 1/1, max 109 ms, accepted/terminal 34/34, duplicate/mismatch/unterminated 0. |
| C5-D3D12 product | **FAIL** | 실제 Demo scene에서 operational ID 1422 6건, runner fail-closed. |
| C5-A ANGLE product | **PASS** | resize accepted/presented/superseded 18/18/0, present/submit 18/18, EGL/GLES error 0, initial no-resize capture visible. 수정 전 first-frame physical FAIL도 이력으로 유지한다. |
| C5-Vulkan | **historical mixed / removed** | 자동 PASS와 edge별 physical PASS/FAIL, strict oracle FAIL을 모두 보존한다. |
| C6 input/threading | **automated PASS** | pointer/key/focus/cursor/clipboard contract, platform ingress와 raster dispatch 분리, latest-only resize coalescing 확인. 실제 capture/re-entry/cursor/Alt+Tab/focus는 `notVerified`. |
| C7 IME/UIA | **automated partial PASS / physical `notVerified`** | IMM32 `한` composing → `한글` final, UIA root/fragment `Invoke`/`Value`, semantics action 확인. 실제 두벌식 후보창/caret/selection/focus restore, Narrator, Accessibility Insights는 미검증. |
| C8 lifecycle/device | **automated partial PASS / matrix `notVerified`** | minimize/restore/display-change/detach와 ANGLE context recreation 10회 PASS. DPI 100~200%, mixed monitor, Snap/system menu, 실제 device removal, wait-point shutdown, visible restore는 미검증. |
| C9 publish/provenance | **PASS** | ANGLE self-contained Release publish, empty `PATH` launch, native/ANGLE DLL x64 provenance, missing/wrong runtime·architecture·ABI/version fail-fast. installer/MSIX는 범위 밖. |
| C10 visible acceptance | **PASS by user acceptance** | 사용자가 실제 창을 확인하고 strict 진단 실패를 그대로 보존하는 조건으로 hard stop을 해제했다. 자동 pixel/cadence FAIL과 resize-driver input qualification FAIL은 PASS로 재분류하지 않는다. |
| C11 default cutover | **Windows 범위 PASS / global regression FAIL** | target/buildTransitive/template/Demo/CLI 기본을 `WindowsAppSdk` + `HwndExactCpp`로 일치시켰고 명시적 MAUI build도 PASS. 전체 `Doroti.Product.slnx`는 Windows에 없는 macOS `sips` 호출(MSB3073/9009)로 FAIL. |

## 5. C10 사용자 acceptance의 정확한 의미

current ANGLE 제품 build의 대표 `Right`, reverse, 600 px/600 ms 자동 재실행은 capture runner가 요청 edge excursion과 시작 rect 복귀를 충족하지 못해 input qualification FAIL로 종료했다. 별도 pixel/cadence evidence에도 `uncoveredEdgeGapFrames=7`, 최대 gap 32 px, 최대 연속 60.604 ms가 남았다.

사용자는 실제 창을 육안으로 확인한 뒤 이 두 진단 실패를 삭제하거나 PASS로 바꾸지 않는 조건으로 C10을 최종 승인했다. 따라서 C10은 **사용자 acceptance PASS**이지만 다음을 의미하지 않는다.

- 자동 capture/cadence PASS
- 모든 corner, speed, DPI, monitor 조합의 일반화
- 실제 IME, Narrator, UIA 도구, device-loss matrix의 PASS
- D3D12 또는 삭제된 Vulkan presenter의 승인

결정과 잔여 진단은 `.doroti/evidence/c10-user-acceptance.json`, `.doroti/evidence/c10-product-right-reverse-current-failure.json`, `.doroti/evidence/c5a-c6-f6r-right-default-final-2.json`에 분리되어 있다.

## 6. default cutover 결과

- Windows CLI 기본 backend: `WindowsAppSdk`
- Windows App SDK 기본 adapter/capability: `HwndExactCpp`
- 기본 graphics backend identity: managed ANGLE/EGL-D3D11 Skia
- 명시적 대안: `-WindowsBackend Maui`
- 명시적 presenter 진단: `DOROTI_WINDOWS_PRESENTER=D3D12`
- 자동 presenter fallback: 없음
- Vulkan/Silk.NET Vulkan: 제거됨

target → host → ANGLE native runtime package 의존성, package 내부 native host exactly once, 환경변수 없는 Demo launch, 기본 CLI WindowsAppSdk run, 명시적 MAUI build를 확인했다. Windows 경로의 기본 전환은 완료됐다.

## 7. 주요 evidence

- `.doroti/evidence/hwnd-exact-cpp-c5-angle-first-frame-flush.json`
- `.doroti/evidence/winappsdk-initial-frame-first-surface-flush.png`
- `.doroti/evidence/hwnd-exact-cpp-c4-angle-pre-vulkan-resize.json`
- `.doroti/evidence/hwnd-exact-cpp-c5-angle-pre-vulkan-resize.json`
- `.doroti/evidence/hwnd-exact-cpp-c7-ime-uia.json`
- `.doroti/evidence/hwnd-exact-cpp-c8-lifecycle-device.json`
- `.doroti/evidence/hwnd-exact-cpp-c8-cycle-{1..10}.json`
- `.doroti/evidence/hwnd-exact-cpp-c9-angle-no-vulkan.json`
- `.doroti/evidence/c10-user-acceptance.json`
- `.doroti/evidence/c11-default-cutover.json`

D3D12 실패 evidence와 삭제 전 Vulkan의 자동·capture·physical 혼합 결과도 진단 역사로 유지한다. build나 counter가 깨끗하다는 이유로 사용자-visible 실패를 덮지 않고, 사용자 acceptance가 있다는 이유로 자동 FAIL을 지우지 않는 것이 이 작업의 최종 evidence 원칙이다.

## 8. 보관 후 경계

이 작업은 Windows 기본 backend cutover와 사용자 C10 acceptance까지 완료된 것으로 종료한다. 다음 항목은 새 active plan이 아니라 별도 후속 검증 후보다.

- 실제 mouse capture/re-entry, resize cursor, Alt+Tab/focus
- 실제 Korean IME 후보창/caret/selection과 focus restore
- Narrator와 Accessibility Insights
- DPI/혼합 monitor/window-management/device-removal/shutdown matrix
- macOS 도구가 있는 host에서 전체 Product regression 재실행

이 항목들은 현재 `notVerified` 또는 global FAIL 경계로 남으며, 이번 Windows resize 작업의 완료·보관 과정에서 성공으로 추론하지 않는다.

> 문서 성격: 삭제한 루트 `idea.md`, `work3.md`의 2026-08-25~26 설계·구현·실험·최종 acceptance 요약. 새로운 active roadmap이 아니다.

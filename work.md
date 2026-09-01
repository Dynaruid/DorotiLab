# Doroti WindowsAppSdk Acrylic + 실시간 exact resize 작업계획

- 작성일: 2026-09-01
- 상태: 계획 작성 완료 / 구현 notStarted / 자동 검증 notVerified / 실물 검증 notVerified
- 대상: Doroti.Host.WindowsAppSdk의 HwndExactCpp + managed ANGLE/EGL-D3D11 + SkiaSharp 경로
- 지원 OS: Windows 11 24H2, build 26100 이상만 지원
- 목표: 전체 client 영역의 반투명 픽셀 뒤로 configurable Windows Desktop Acrylic(Default/Base/Thin, theme별 tint)을 보이고 앱 실행 중 kind/tint를 바꾸면서도, 현재의 창 테두리 drag 실시간 추종성과 exact-frame 계약을 유지한다.
- 현재 문서는 실행 계획만 다룬다. 이 문서를 작성하면서 제품 코드, 빌드, 자동 테스트, 실물 border-drag acceptance는 수행하지 않았다.

## 1. 결론과 후보 우선순위

현재 잘 동작하는 resize 경로에 별도의 Acrylic/DirectComposition surface를 단순히 겹치면 안 된다. 이전 실험에서 shell HWND geometry, ANGLE surface, 별도 composition surface가 서로 다른 시계로 움직였고, 동기 wait는 계단식 resize를, 비동기 commit은 한 세대 늦은 content/backdrop을 만들었다.

이번 작업은 아래 순서로 진행한다.

1. **P0 — 기존 HWND/ANGLE content owner 보존형 Acrylic spike**
   - Windows 11 build 26100 이상에서만 가능한 DWMWA_REDIRECTIONBITMAP_ALPHA를 이용한다.
   - P0-DWM은 top-level의 DWMSBT_TRANSIENTWINDOW + child redirection alpha를 조합한 고정-material latency 기준선이다. DWM attribute만으로는 Base/Thin/tint를 구성할 수 있으므로 제품 완료 후보가 아니라 비교 control이다.
   - P0-Controller는 같은 top/child HWND topology를 유지하면서 top-level WindowId + DesktopWindowTarget에 DesktopAcrylicController.SetTarget을 연결하는 첫 제품 후보다. Kind(Default/Base/Thin), TintColor, TintOpacity, LuminosityOpacity를 여기서 적용한다.
   - P0-DWM과 P0-Controller를 동시에 켜지 않는다. controller 후보는 top-level HWND에 DWMWA_USE_HOSTBACKDROPBRUSH를 적용하고, DWMWA_SYSTEMBACKDROP_TYPE은 적용하지 않는다.
   - top/child HWND topology, ANGLE/EGL content ownership, exact-size target lifecycle, 현재 resize-generation protocol은 바꾸지 않는다. Alpha-capable EGL config와 clear/output 의미는 별도 검증 후 필요한 최소 변경만 허용한다.
   - 새 visible content owner, CPU readback/upload, resize hot path의 추가 wait를 만들지 않는다.
   - child redirection alpha와 controller backdrop의 실제 화면 정렬은 공식 보장이 없으므로 독립 spike와 실물 검증으로 판정한다.

2. **P1 — 동일 ContentIsland owner의 Acrylic composition presenter**
   - 지원 대상인 Windows 11 24H2+에서도 P0-Controller가 옵션 또는 실물 resize 검증에 실패할 때만 진행한다.
   - DesktopAttachedSiteBridge + ContentIsland 하나를 Acrylic controller의 geometry target이자 Doroti content visual의 owner로 사용한다.
   - ANGLE/Skia에서 동일 D3D11 composition texture로 직접 그리거나, 최대 한 번의 same-GPU D3D11 copy만 허용한다.
   - exact-size hidden surface slot을 준비한 뒤 content surface와 visual geometry를 한 commit에서 교체한다.

3. **P2 — fixed outer envelope/custom resize region**
   - P0/P1의 원인 분리용 진단 후보일 뿐 기본 제품 경로로 승격하지 않는다.
   - 표준 shell resize, Snap, system menu, taskbar, IME, UIA 의미를 바꿀 위험 때문에 별도 제품 결정 없이는 채택하지 않는다.

P0-Controller와 P1이 모두 탈락하면 현재 opaque HwndExactCpp 경로를 유지한다. P0-DWM control만 성공한 상태도 Base/Thin/tint 계약을 충족하지 않으므로 제품 완료가 아니다. “Acrylic이 보이지만 resize가 나빠진 상태”를 부분 성공으로 출시하지 않는다.

## 2. 범위와 전제

### 포함

- WindowBackdropMode.acrylic과 Acrylic Default/Base/Thin, theme별 tint/luminosity 설정을 WindowsAppSdk host까지 전달하는 versioned ABI
- 창을 다시 만들지 않고 view별 Acrylic kind와 light/dark tint profile을 교체하는 비동기 public API와 versioned native command/completion 계약
- 전체 client-area Desktop Acrylic과 Doroti의 premultiplied-alpha scene 합성
- border drag 중 geometry, framework metrics, render target, present/commit의 generation 일치
- 활성/비활성, light/dark, transparency policy, DPI 전환, device loss, minimize/restore
- pointer, keyboard, 한국어 IME, accessibility ownership 보존
- opaque 기준선과 Acrylic 후보의 자동 캡처 및 실물 비교

### 전제

- 요청한 Acrylic은 title bar에만 적용하는 장식이 아니라 Doroti의 투명/반투명 client 픽셀 뒤로 보이는 전체 창 Desktop Acrylic로 해석한다.
- Windows App SDK backend의 지원 하한은 Windows 11 24H2, build 26100으로 확정한다. 그 미만 Windows는 구현·fallback·acceptance 범위에서 제외한다.
- Windows 문서상 DWMSBT_TRANSIENTWINDOW는 Windows 11에서 전체 창 Desktop Acrylic에 해당하고, build 26100부터 P0에 필요한 redirection-bitmap alpha를 사용할 수 있다.
- Base/Thin/tint는 제품 요구사항이다. 고정 DWMSBT_TRANSIENTWINDOW를 최종 제품 구현으로 취급하지 않고 DesktopAcrylicController 기반 후보가 이 옵션을 모두 통과해야 한다.
- DorotiViewConfiguration.backdrop은 창 생성 시의 초기 requested state이고, 옵션 값 객체 자체는 immutable로 유지한다. 이후 앱은 view별 비동기 API로 전체 WindowAcrylicOptions snapshot을 교체할 수 있으며 host는 runtime theme/activation/policy 반응과 앱 요청을 같은 platform queue에서 직렬화한다.
- 현재 ViewConfiguration은 Acrylic과 transparent fallback을 요청하지만 WindowsAppSdk runner는 backdrop 값을 native configuration으로 전달하지 않는다. 이 gap은 후보가 통과한 뒤 제품 ABI 단계에서 해결한다.
- fallback.transparent/solid는 24H2+에서 Acrylic API 실패, transparency policy, battery saver, high contrast, RDP 같은 runtime 상태에만 적용한다. 구버전 Windows 지원 수단으로 사용하지 않는다.

### 제외

- in-app BackdropFilter와 OS Desktop Acrylic을 같은 기능으로 취급하는 것
- Flutter의 제거된 transparency helper를 복원하거나 복사하는 것
- WinUI 3 SwapChainPanel을 전체 창 Acrylic surface로 쓰는 것
- SkiaSharp D3D12 backend를 제품 D3D11 경로에 억지로 연결하는 것
- Windows 공식 DesktopAcrylicController 밖에서 blur/noise shader나 비공개 accent policy로 Acrylic을 임의 재현하는 것
- 현재 기본 renderer를 검증 전에 교체하는 것

### 공개 Acrylic 옵션 계약

계획상 public model은 기존 WindowBackdropOptions에 Acrylic 전용 immutable 설정을 추가하고, DorotiView에 runtime 교체 API를 둔다. 정확한 이름은 API review에서 고정하되 의미는 아래와 같이 유지한다.

    WindowAcrylicKind
      - systemDefault
      - base
      - thin

    WindowAcrylicAppearance
      - tintColor: Color?
      - tintOpacity: double?
      - luminosityOpacity: double?

    WindowAcrylicOptions
      - kind: WindowAcrylicKind = systemDefault
      - light: WindowAcrylicAppearance?
      - dark: WindowAcrylicAppearance?

    WindowBackdropOptions
      - mode
      - fallback
      - acrylic: WindowAcrylicOptions?

    WindowAcrylicUpdateStatus
      - applied
      - superseded
      - invalidState
      - unsupported
      - failed
      - closed

    WindowAcrylicUpdateResult
      - backdropRevision
      - status
      - requested
      - effective
      - resolvedTheme
      - reason

    DorotiView.UpdateAcrylicAsync(
      WindowAcrylicOptions options)
      -> ValueTask<WindowAcrylicUpdateResult>

계약 규칙:

- systemDefault/Base/Thin은 각각 DesktopAcrylicKind.Default/Base/Thin에 1:1 대응한다.
- light/dark appearance가 null이면 해당 theme에는 Windows system default를 사용한다.
- tintOpacity와 luminosityOpacity는 null 또는 finite 0.0~1.0만 허용하고 clamp하지 않는다. 범위를 벗어나면 창 생성 또는 runtime command acceptance 전에 명시적으로 거부한다.
- tintColor alpha와 tintOpacity라는 두 opacity source를 중복시키지 않는다. tintColor는 opaque RGB만 허용하고 실제 강도는 tintOpacity로 표현한다.
- custom appearance를 하나라도 적용하면 Windows가 theme 변경 시 네 custom material property의 기본값을 자동 갱신하지 않는다. host가 현재 light/dark profile을 다시 완전 적용한다.
- system default로 돌아갈 때는 일부 property만 덮지 않고 ResetProperties 후 Kind와 현재 configuration을 다시 적용한다.
- controller FallbackColor는 fallback.solid일 때 현재 theme의 backgroundColor/darkBackgroundColor RGB에서 계산하고 alpha는 solid 의미에 맞게 255로 고정한다. fallback.transparent는 FallbackColor를 투명색으로 간주하지 않고 controller 제거/alpha 유지가 실제로 가능한지 별도 gate로 검증한다.
- ABI에는 null과 0.0을 구분하는 presence bit를 둔다. NaN, 특정 색상값, enum reserved 값을 sentinel로 사용하지 않는다.
- UpdateAcrylicAsync는 patch가 아니라 kind와 light/dark appearance를 모두 포함한 immutable snapshot 교체다. 누락 필드와 이전 요청을 암묵적으로 merge하지 않는다.
- 이 API는 이미 Acrylic로 생성된 한 view의 material option만 바꾼다. mode, fallback, HWND/ContentIsland topology, renderer를 실행 중 전환하지 않으며 acrylic이 아닌 view에서는 명시적 unsupported/invalid-state 결과를 반환한다.
- host는 view별 monotonic backdropRevision을 부여한다. 이 값은 resize generation, scene/input sequence와 완전히 별개이며 업데이트 때문에 resize generation을 만들거나 framework frame을 요청하지 않는다.
- accepted request마다 정확히 하나의 terminal result를 반환한다. current apply 1개 + latest pending 1개만 유지하고, 시작 전 더 최신 snapshot으로 대체된 요청은 superseded로 끝낸다. native 적용 여부를 숨기는 await cancellation은 이 API에 넣지 않고, 창 close/device loss를 포함해 accepted revision은 반드시 terminal result로 끝낸다.
- controller property는 platform/UI DispatcherQueue의 한 callback에서 ResetProperties, Kind, FallbackColor, TintColor, TintOpacity, LuminosityOpacity, SystemBackdropConfiguration 순으로 완전 적용한다. 이 callback 단위 직렬화를 DWM scan-out 원자성으로 표현하지 않는다.
- 앱 요청과 theme 전환이 경합하면 같은 platform queue에서 처리하고, 적용 시점의 최신 theme에 요청된 전체 light/dark snapshot을 resolve한다. 이후 theme가 바뀌면 같은 requested snapshot의 반대 profile을 새 backdropRevision 없이 host-originated theme transition으로 재적용하고 진단에는 resolved theme와 원인을 남긴다.
- systemDefault 또는 custom-null 복귀는 ResetProperties로 과거 custom 값을 제거한 뒤 Kind와 현재 configuration/profile을 완전 적용한다. runtime 변경 때문에 controller, DesktopWindowTarget, root visual 또는 SetTarget 연결을 다시 만들지 않는다.

DorotiDemoApp는 public API가 고정된 뒤 초기 acrylic.kind와 light/dark appearance를 명시하고, 실행 중 systemDefault/Base/Thin과 tint/opacity를 교체하면서 requested/effective revision/status를 보여 주는 실제 consumer 예제로 갱신한다. 임의의 hard-coded native tint를 demo 전용 우회로 넣지 않는다.

## 3. 현재 경로와 문제의 소유권

### 현재 exact-resize 경로

1. top-level WM_SIZE가 client rect와 정확히 같은 크기로 child HWND를 SetWindowPos한다.
2. child WM_SIZE가 새 physical extent와 resize generation을 publish하고 render를 queue한다.
3. child WM_SIZE가 filtered task polling으로 최대 100ms의 기존 WaitForExactResize를 수행하는 동안 framework/render 쪽은 그 generation의 metrics로 scene과 exact frame을 만든다.
4. WindowsManagedAngleEglPresenter가 EGL_FIXED_SIZE_ANGLE surface와 GRBackendRenderTarget을 정확한 physical size로 다시 만든다.
5. exact frame을 premultiplied backing에 paint하고 window surface로 한 번의 GPU Src blit/submit을 한다. runner는 paint 후, submit 후, eglSwapBuffers 직전에 latest resize generation과 input sequence를 검사한다.
6. 마지막 predicate가 참일 때만 eglSwapBuffers한다. swap 뒤에는 같은 predicate를 다시 검사하지 않는다.
7. 기본값에서는 초기 생성 및 매 재생성된 EGL surface의 첫 성공 swap 뒤 DwmFlush가 실행된다. DOROTI_WINDOWS_DWM_FLUSH=1 진단 override에서는 매 present마다 실행된다.

관련 코드:

- [native top/child HWND와 resize](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp)
- [ANGLE/EGL/Skia presenter](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedAngleEglPresenter.cs)
- [generation admission과 terminal](Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs)
- [현재 accepted contract](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md)

### Acrylic을 별도 owner로 붙였을 때 늦어지는 이유

DirectComposition은 비동기 commit을 VBlank에 소비한다. shell HWND geometry, ANGLE swap, 별도 Desktop Acrylic/composition visual이 서로 다른 device 또는 commit 경계를 가지면 각 단계가 정상이어도 화면에는 서로 다른 generation이 함께 보일 수 있다. WndProc에서 Commit completion이나 DwmFlush를 기다리면 message pump와 border-drag cadence를 막아 계단식 추종이 생긴다.

이 저장소의 이전 Acrylic 조사에서도 다음 경로는 탈락했다.

- 별도 DirectComposition surface/visual을 existing child HWND 위에 추가
- 매 resize 동기 commit wait
- ANGLE GPU → CPU readback → D3D11 upload
- D3D12/D3D11On12를 제품 presenter에 바로 투입
- frozen front buffer로 ResizeBuffers를 회피

근거 기록: [이전 Acrylic resize 조사](history/26-08-29/windows-appsdk-acrylic-resize-investigation.md)

그보다 앞선 exact-resize 실험에서 capacity surface, provisional stretch, clip/SetSourceSize도 physical FAIL로 제외되었다. 근거 기록: [WindowsAppSdk HWND-exact 정리](history/26-08-26/windows-appsdk-hwnd-exact-summary.md)

이번 P0-Controller가 과거 실패와 다른 점은 Doroti content를 DirectComposition surface로 옮기지 않고 기존 exact child HWND/EGL owner에 남긴다는 것이다. 다만 DesktopAcrylicController 자체는 여전히 external backdrop clock이므로 이 차이는 가설일 뿐이며, physical resize gate를 통과하기 전에는 해결로 간주하지 않는다.

## 4. 검토한 upstream에서 가져올 계약

### Flutter

Flutter Windows도 top-level HWND와 WS_CHILD render HWND를 사용한다. child WM_SIZE가 physical metrics를 보내고, 최대 100ms 동안 exact-size frame을 기다리며, 크기가 틀린 frame은 present하지 않는다. fixed EGL surface를 resize 때 재생성하고 exact-size blit/swap 뒤 resize waiter를 깨운 다음 raster thread에서 DwmFlush한다.

가져올 것은 다음뿐이다.

- target size와 frame size의 exact equality
- stale frame admission 거부
- platform wait와 raster/present 완료의 분리
- 완전 투명 output과 비정사각 크기를 포함한 회귀 테스트. 현재 Doroti의 empty disposition은 present하지 않으므로 실제 Doroti gate는 all-transparent인 exact non-empty scene으로 번역한다.

그대로 복사하지 않을 것은 다음이다.

- 100ms를 새로운 composition wait에 중복 적용
- 매 WM_SIZE에서 surface/commit 완료를 강제로 기다리는 것
- DwmFlush가 backdrop과 HWND geometry까지 atomic하게 만든다고 가정하는 것
- EGL alpha 8bit만으로 Desktop Acrylic이 보인다고 가정하는 것

Flutter는 2026년에 실제 transparency를 만들지 못하던 EnableTransparentWindowBackground와 DWMWA_SYSTEMBACKDROP_TYPE 관련 코드를 제거했다. 즉 Flutter는 exact-resize 참고 구현이지 Acrylic 완성 구현이 아니다.

주요 근거:

- [Flutter host WM_SIZE → child MoveWindow](https://github.com/flutter/flutter/blob/c77a5798c59f0643bc835c12d473924fd0206cc5/engine/src/flutter/shell/platform/windows/host_window.cc#L473-L483)
- [exact target wait/admission](https://github.com/flutter/flutter/blob/c77a5798c59f0643bc835c12d473924fd0206cc5/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L208-L259)
- [fixed EGL surface resize](https://github.com/flutter/flutter/blob/c77a5798c59f0643bc835c12d473924fd0206cc5/engine/src/flutter/shell/platform/windows/egl/manager.cc#L297-L316)
- [present 완료와 DwmFlush 순서](https://github.com/flutter/flutter/blob/c77a5798c59f0643bc835c12d473924fd0206cc5/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L747-L779)
- [투명 배경 helper 제거 PR #187848](https://github.com/flutter/flutter/pull/187848)
- [비정사각 empty-frame resize 수정 PR #187954](https://github.com/flutter/flutter/pull/187954)
- [fixed surface resize 비용 이슈 #79427](https://github.com/flutter/flutter/issues/79427)

### Windows / Windows App SDK

- DWMSBT_TRANSIENTWINDOW는 Windows 11 build 22621부터 전체 창 bounds 뒤의 Desktop Acrylic을 DWM이 그리는 계약이다.
- DWMWA_REDIRECTIONBITMAP_ALPHA는 build 26100부터 window redirection bitmap의 premultiplied alpha를 사용하게 한다.
- 이 alpha 속성은 child HWND를 배제하지 않지만 ANGLE child HWND 동작을 보증하는 공식 샘플도 없다. HRESULT 성공만으로 PASS 처리하지 않는다.
- DWMSBT_TRANSIENTWINDOW는 material 세부 옵션을 노출하지 않는다. configurable Acrylic은 DesktopAcrylicController를 사용해야 한다.
- DesktopAcrylicController는 Default/Base/Thin Kind와 FallbackColor, TintColor, TintOpacity, LuminosityOpacity를 제공한다.
- Windows App SDK 2.0 문서의 SetTarget(WindowId, CompositionTarget)은 Win32 HWND/AppWindow 연결을 공식 지원하고, UI DispatcherQueue와 top-level DWMWA_USE_HOSTBACKDROPBRUSH를 요구한다. 저장소가 pin한 2.4 projection/runtime에서 실제 API와 ABI를 다시 확인한다.
- custom FallbackColor/TintColor/TintOpacity/LuminosityOpacity를 설정하면 system theme 변경 시 기본 light/dark 값이 자동 갱신되지 않는다. theme별 profile 재적용 또는 ResetProperties가 필요하다.
- ContentIsland는 output/layout/input/accessibility surface이므로 이름만 도입해도 geometry clock이 자동 통합되는 것은 아니다.
- ContentIsland를 쓰면 parent Acrylic이 transparent island를 통과한다고 가정하지 않는다. DesktopAcrylicController가 그 ContentIsland를 backdrop target으로 사용하고 Doroti scene도 같은 island geometry owner 아래에 둔다.
- Desktop Acrylic은 compositor의 external content이므로 Doroti visual mutation과 같은 app compositor transaction에 묶인다고 가정하지 않는다. backdrop/content 정렬은 여전히 capture와 실물 gate로 판정한다.
- WinUI 3 SwapChainPanel은 transparency와 Acrylic/CompositionBackdropBrush sampling을 공식적으로 지원하지 않으므로 후보에서 제외한다.

주요 근거:

- [DWM system backdrop 종류](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwm_systembackdrop_type)
- [DWMWA_REDIRECTIONBITMAP_ALPHA](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute)
- [Windows system backdrop](https://learn.microsoft.com/en-us/windows/apps/develop/ui/system-backdrops)
- [DesktopAcrylicController API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacryliccontroller?view=windows-app-sdk-2.0)
- [DesktopAcrylicKind Default/Base/Thin](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacrylickind?view=windows-app-sdk-2.0)
- [Win32 HWND SetTarget 계약](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacryliccontroller.settarget?view=windows-app-sdk-2.0)
- [공식 Win32 DesktopWindowTarget/root sample](https://github.com/microsoft/WindowsAppSDK-Samples/blob/main/Samples/Mica/cpp-win32/WinAppSDKMicaSample/MicaWindow.cpp)
- [custom property reset 계약](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacryliccontroller.resetproperties?view=windows-app-sdk-2.0)
- [ContentIsland](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.contentisland?view=windows-app-sdk-2.0)
- [ContentIsland host/layout 계약](https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island)
- [DirectComposition 비동기 transaction](https://learn.microsoft.com/en-us/windows/win32/directcomp/architecture-and-components)
- [SwapChainPanel의 transparency 제한](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel?view=windows-app-sdk-2.0)
- [visual layer의 external-content 계약](https://learn.microsoft.com/en-us/windows/apps/develop/composition/visual-layer)
- [공식 Windows App SDK Islands sample](https://github.com/microsoft/WindowsAppSDK-Samples/tree/main/Samples/Islands/UXFrameworksOnIslands)

### SkiaSharp / Skia / ANGLE

- native EGL context/D3D device와 HWND present/DirectComposition commit은 host/presenter가 소유한다. presenter가 만든 GRContext와 Skia cache는 presenter가 수명 관리하지만 Flush/Submit은 GPU work 제출이지 화면 관찰 완료가 아니다.
- GRBackendRenderTarget의 extent는 immutable 계약으로 취급한다. 실제 target 크기가 바뀌면 기존 SKSurface와 backend target을 폐기하고 exact physical size로 다시 wrap한다.
- framework color는 straight/unpremultiplied 의미로 입력되고 Skia target/D3D/compositor 저장은 premultiplied여야 한다. 변환은 정확히 한 번만 일어나고 이후 단계는 그 값을 보존한다.
- 이 계획은 buffer-age/damage 복원을 구현하지 않고 BeginDraw update의 초기 내용도 보존된다고 가정하지 않는다. 따라서 새 buffer/재사용 slot의 전체 픽셀을 clear/draw하고 offscreen → output 복사는 Src로 alpha까지 교체한다.
- 저장소는 Windows App SDK 2.4.0과 SkiaSharp 4.151.1을 사용한다. 현재 SkiaSharp의 public GRD3D 타입은 D3D12용이므로 D3D11 composition texture에 직접 그리려면 ANGLE/EGL interop을 실제 packaged runtime에서 증명해야 한다.
- EGL_ANGLE_d3d_texture_client_buffer의 EGL_D3D_TEXTURE_ANGLE token, EGL_ANGLE_surface_d3d_texture_2d_share_handle export, EGL_ANGLE_d3d_share_handle_client_buffer import는 각각 독립 capability로 검사한다. 동일 D3D11 device, format, offset, synchronization 조건을 만족할 때만 쓴다.

주요 근거:

- [Skia GPU canvas/context ownership](https://skia.org/docs/user/api/skcanvas_creation/)
- [SkiaSharp GRBackendRenderTarget](https://github.com/mono/SkiaSharp-API-docs/blob/main/SkiaSharpAPI/SkiaSharp/GRBackendRenderTarget.xml)
- [SkiaSharp WinUI size-change rewrap](https://github.com/mono/SkiaSharp/blob/279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764/source/SkiaSharp.Views/SkiaSharp.Views.WinUI/SKSwapChainPanel.cs#L41-L98)
- [Skia flush/submit 계약](https://github.com/google/skia/blob/bdd0c3a8eaba1afa7148f02bba3a07f94e682847/include/gpu/ganesh/GrDirectContext.h#L323-L495)
- [ANGLE D3D11 texture client buffer extension](https://chromium.googlesource.com/angle/angle/+/main/extensions/EGL_ANGLE_d3d_texture_client_buffer.txt)
- [ANGLE D3D surface share handle extension](https://chromium.googlesource.com/angle/angle/+/main/extensions/EGL_ANGLE_surface_d3d_texture_2d_share_handle.txt)
- [ANGLE D3D share handle import extension](https://chromium.googlesource.com/angle/angle/+/main/extensions/EGL_ANGLE_d3d_share_handle_client_buffer.txt)
- [DirectComposition BeginDraw lifetime/offset](https://learn.microsoft.com/en-us/windows/win32/api/dcomp/nf-dcomp-idcompositionsurface-begindraw)

## 5. 모든 후보가 지켜야 할 불변조건

1. **기본 경로 보존**
   - 기존 opaque HwndExactCpp/ANGLE renderer를 기본값과 비교 기준으로 유지한다.
   - Acrylic mode는 명시적 opt-in으로 시작하며, 창 생성 후 renderer/topology를 바꾸지 않는다.
   - P0-DWM과 P0-Controller는 별도 실험 arm이다. 같은 HWND에 두 backdrop 방식을 동시에 적용하지 않는다.

2. **한 app-content visible owner**
   - P0에서는 기존 child HWND만 Doroti content를 소유한다. P0-DWM은 DWM이, P0-Controller는 top-level DesktopWindowTarget의 DesktopAcrylicController가 backdrop만 담당한다.
   - P1에서는 ContentIsland root의 Doroti visual 하나만 app content를 소유한다.
   - 같은 scene을 HWND와 composition visual 양쪽에 동시에 보이지 않는다.

3. **exact generation**
   - 하나의 immutable packet에 resize generation, logical size, physical size, scale, scene/input sequence, target/surface slot을 묶는다.
   - width와 height가 모두 정확히 맞지 않으면 그 결과를 visible target으로 admit하거나 swap/present/commit하지 않는다.
   - current render 1개 + latest pending 1개만 유지한다. stale generation은 swap/commit 전에 superseded terminal로 끝낸다.
   - debounce나 geometry throttle은 금지한다. raster work만 latest-only로 coalesce할 수 있다.

4. **premultiplied alpha end-to-end**
   - framework의 straight color 의미를 Skia target 저장에서 정확히 한 번 premultiplied로 바꾸고, Src blit → EGL/D3D texture → DWM/compositor까지 그 저장 의미를 보존한다.
   - buffer-age/damage 복원을 쓰지 않으므로 새/재사용 surface의 모든 픽셀을 transparent 또는 해당 frame 배경색으로 덮는다.
   - EGL_ALPHA_SIZE = 8과 HRESULT 성공은 필요조건일 뿐 시각적 PASS 근거가 아니다.

5. **GPU-only**
   - CPU ReadPixels/readback, staging texture map, bitmap/GDI upload는 0이어야 한다.
   - P1 fallback도 최대 한 번의 same-GPU D3D11 copy까지만 허용한다.
   - D3D11/D3D12 device 오류, cross-device implicit copy, per-frame texture allocation을 허용하지 않는다.

6. **resize hot path에 새 blocking clock을 추가하지 않음**
   - P0는 현재 child WM_SIZE의 최대 100ms WaitForExactResize와 filtered task polling을 그대로 기준선으로 보존한다.
   - P1은 우선 WM_SIZING/WM_SIZE에서 generation을 publish한 뒤 즉시 반환하는 후보로 측정한다.
   - 새 composition DwmFlush, WaitForCommitCompletion, EnsurePreviousCommitCompletedAsync, fence 무한대기를 WndProc/platform hot path에 넣지 않는다.
   - Submit(true)나 CPU-blocking GPU wait는 interactive render hot path에도 넣지 않는다. GL→D3D 전달에는 비차단 fence/keyed-mutex 또는 선택한 API가 보장하는 명시적 ownership handoff를 쓴다.
   - 현재 exact present 뒤 DwmFlush 위치는 먼저 그대로 둔다. 제거/추가는 별도 A/B이며 항상 render/present thread에서만 수행한다.
   - Kind/tint/luminosity는 창 생성, 명시적 UpdateAcrylicAsync, theme/policy 전환 때만 갱신하고 WM_SIZING/WM_SIZE마다 다시 설정하지 않는다. controller target은 창 생성 시 한 번만 연결한다.

7. **보이는 source를 resize하지 않음**
   - P1에서는 현재 front surface의 ResizeBuffers/resize를 금지한다.
   - 정확한 새 크기의 hidden/free slot을 준비·render한 뒤 surface와 geometry를 같은 commit에서 교체한다.
   - capacity backing, clip-only, SetSourceSize, full-frame stretch로 exactness를 대체하지 않는다.

8. **visible evidence 분리**
   - swap/EndDraw/Commit 완료는 내부 단계 완료일 뿐 scan-out 관찰이 아니다.
   - Windows Graphics Capture 또는 동등한 캡처 관찰과 실제 모니터 border drag를 별도 gate로 둔다.

9. **runtime backdrop command와 resize 분리**
   - IWindowBackdropHostCapability는 view별 capability이며 전역 current-window setter를 만들지 않는다.
   - backdropRevision은 요청/적용/terminal 상관관계용 command revision일 뿐 Acrylic scan-out generation이 아니다.
   - runtime update는 platform task window/DispatcherQueue로 marshal하고 render thread나 WndProc에서 WinRT controller property를 직접 건드리지 않는다.
   - update burst는 current apply 1 + latest pending 1로 제한하되 WM_SIZE, input, frame terminal을 굶기지 않는다.
   - invalid mode, close/device-loss/controller failure 때 모든 accepted revision을 applied, superseded, invalidState, unsupported, failed, closed 중 하나로 정확히 한 번 끝내고 dispose 이후 callback을 만들지 않는다.

## 6. 단계별 작업

### A0. 기준선과 제품 계약 고정

- [ ] 같은 PC, 같은 monitor, 같은 DPI/refresh rate, 같은 Demo app에서 현재 opaque 경로의 10초 border-drag 기준선을 저장한다.
- [ ] left/top/right/bottom/corner, expand/shrink/reverse, slow/medium/fast drag를 각각 기록한다.
- [ ] native QPC ledger, frame terminal, surface recreate/swap, capture frame을 같은 run id로 묶는다.
- [ ] DOROTI_WINDOWS_DWM_FLUSH와 DOROTI_WINDOWS_EGL_SWAP_INTERVAL의 실제 값을 기록하고 opaque/P0/P1 비교 run에서 동일하게 고정한다.
- [ ] ViewConfiguration의 acrylic, system, solid, transparent와 fallback.transparent/solid가 Windows에서 정확히 무엇을 의미할지 표로 확정한다.
- [x] Windows App SDK backend의 지원 floor를 Windows 11 24H2, build 26100으로 확정한다. 그 미만 OS용 P1 또는 별도 fallback renderer는 만들지 않는다.
- [x] Acrylic systemDefault/Base/Thin과 theme별 tintColor/tintOpacity/luminosityOpacity를 public configuration에 포함한다.
- [x] 실행 중 kind/tint 변경은 immutable WindowAcrylicOptions snapshot을 받는 DorotiView 비동기 API와 view별 capability로 설계한다.
- [ ] runtime API의 invalid-state, close-during-apply terminal 의미와 requested/effective state 조회 계약을 API review에서 고정한다. 호출 전 값 검증 실패는 exception이고 revision을 발급하지 않으며, accepted 요청은 ValueTask를 취소하지 않고 반드시 terminal result로 끝낸다.
- [ ] fallback.solid/transparent와 controller FallbackColor의 정확한 mapping을 고정한다.
- [ ] 현 default renderer, ABI layout, package self-contained 계약을 snapshot하고 negative contract로 잠근다.

완료 조건:

- baseline 자동 수치와 실물 판정이 함께 기록되어 있다.
- 미실행 항목은 PASS가 아니라 notVerified로 남아 있다.

### A1. P0 기존-HWND redirection-alpha + controller spike

제품 코드에 바로 넣지 말고 Doroti/validation/windows-dwm-redirection-alpha-spike 같은 독립 native/ANGLE spike를 만든다. 기존 HwndExactCpp의 top/child 구조와 fixed EGL surface 코드를 최소 단위로 재사용하고 P0-DWM/P0-Controller를 같은 executable의 상호 배타적 arm으로 비교한다.

- [ ] Windows build number, DWM composition, transparency policy, DWMWA_SYSTEMBACKDROP_TYPE 결과, DWMWA_REDIRECTIONBITMAP_ALPHA HRESULT를 구조화해 출력한다.
- [ ] 사용한 Windows SDK/header version과 DWMWA_REDIRECTIONBITMAP_ALPHA symbol availability를 기록한다. compile-time availability를 runtime support로 간주하지 않는다.
- [ ] redirection alpha를 control/off, top only, child only, top+child 네 조합으로 실행한다.
- [ ] WS_EX_NOREDIRECTIONBITMAP은 사용하지 않는다.
- [ ] 완전 투명, 25/50/80% alpha, 완전 불투명, 비정사각 방향표시 grid를 한 frame에 그린다.
- [ ] child EGL surface가 실제 premultiplied RGBA를 DWM까지 전달하는지 화면과 capture로 확인한다.
- [ ] 투명 영역 뒤의 다른 창을 움직여 Acrylic sampling이 살아 있는지 확인하고, 단순 색/가짜 blur와 구분한다.
- [ ] 현재 exact WM_SIZE → fixed EGL recreate → swap 경로와 현재 DwmFlush 위치를 그대로 유지한다.
- [ ] fast border drag 중 black/white band, old-size stretch, exposed raw desktop, Acrylic 한 세대 지연을 검사한다.
- [ ] build 26100+에서 API 실패, transparency off, battery saver, high contrast, RDP의 deterministic fallback과 진단 reason을 확인한다.
- [ ] build 26100 미만은 지원되지 않는 OS로 fail-fast하고 명확한 진단을 남긴다. 해당 OS의 Acrylic/resize 품질은 acceptance 대상으로 삼지 않는다.

P0-DWM control:

- [ ] top-level에 DWMSBT_TRANSIENTWINDOW를 창 생성 시 한 번 적용하고 resize 중 다시 설정하지 않는다.
- [ ] DesktopAcrylicController, DesktopWindowTarget, DWMWA_USE_HOSTBACKDROPBRUSH는 만들지 않는다.
- [ ] 이 arm은 fixed Desktop Acrylic의 alpha/latency control이며 Base/Thin/tint 제품 PASS를 주장하지 않는다.

P0-Controller 제품 후보:

- [ ] pin된 Windows App SDK 2.4 projection/runtime에서 DesktopAcrylicController.SetTarget(WindowId, CompositionTarget)의 존재와 성공 반환을 확인한다.
- [ ] platform thread의 DispatcherQueue, top-level WindowId, 같은 top-level HWND에 연결된 DesktopWindowTarget과 최소 ContainerVisual root를 수명 객체로 만든다. 공식 sample처럼 SetTarget 전에 Root를 먼저 설정한다.
- [ ] top-level에 DWMWA_USE_HOSTBACKDROPBRUSH = TRUE를 적용하고 이 arm에서는 DWMWA_SYSTEMBACKDROP_TYPE을 호출하지 않는다. arm 전환은 새 process/window로만 수행해 residual 이중 backdrop을 막는다.
- [ ] non-null SystemBackdropConfiguration에 initial Theme와 IsInputActive를 설정한다.
- [ ] custom property가 없는 profile은 ResetProperties 후 Kind를 적용한다.
- [ ] custom profile은 Kind, theme별 FallbackColor, TintColor, TintOpacity, LuminosityOpacity를 target 연결 전에 완전 적용한다.
- [ ] SetTarget은 창 생성 시 한 번 호출하고 false/HRESULT/StateChanged를 구조화해 기록한다.
- [ ] WM_ACTIVATE/brightness/theme 변경 시 SystemBackdropConfiguration과 선택된 light/dark appearance를 갱신하되 target을 다시 만들지 않는다.
- [ ] 창이 뜬 뒤 같은 controller에서 systemDefault → Base → Thin → custom tint → systemDefault를 순차/연속 적용한다. property mutation마다 controller/target/root/SetTarget 생성 횟수가 그대로 1인지 검증한다.
- [ ] runtime 요청과 theme 전환을 같은 DispatcherQueue에서 직렬화하고, 적용 시점의 theme와 최신 전체 snapshot이 일치하는지 확인한다.
- [ ] 100회와 500회 burst에서 current apply 1 + latest pending 1, accepted revision terminal one-to-one, 마지막 요청의 requested/effective 일치를 검증한다.
- [ ] controller target/configuration/property를 WM_SIZING/WM_SIZE에서 다시 설정하지 않는다.
- [ ] SetTarget arm의 teardown은 controller Close/Dispose로 target 연결을 먼저 끝낸 뒤 DesktopWindowTarget/root/compositor → DispatcherQueue controller 순서로 수행하고 lifetime test로 고정한다. RemoveSystemBackdropTarget은 AddSystemBackdropTarget을 쓰는 P1에서만 사용한다.
- [ ] 과거 실패와 달리 Doroti content를 별도 DirectComposition surface로 옮기지 않고 기존 child HWND/EGL visible content owner를 유지한다.

옵션 matrix:

- [ ] Kind: systemDefault, Base, Thin
- [ ] system default: light/dark 전환 뒤 ResetProperties 기반 자동 기본값 복귀
- [ ] custom tint: light/dark 서로 다른 opaque RGB tintColor
- [ ] tintOpacity: 0.0, 중간값, 1.0
- [ ] luminosityOpacity: 0.0, 중간값, 1.0
- [ ] invalid: NaN, infinity, 0 미만, 1 초과, unknown enum, non-opaque tintColor
- [ ] policy fallback: transparency off, battery saver, high contrast, RDP
- [ ] 각 조합에서 effective Kind/color/opacity/controller State를 진단하고, 통제된 checker/color source window를 뒤에 둔 같은 run의 ROI 통계 차이를 확인한다.
- [ ] idle, border drag, light/dark 전환 중에 Base/Thin/tint를 바꾸고 option 변경이 resize generation/frame request를 만들지 않는지 확인한다.
- [ ] update 직후 close, 연속 update 중 device loss/controller State 실패, acrylic이 아닌 view 호출의 terminal을 확인한다.
- [ ] DWM noise와 machine별 material 차이 때문에 cross-machine exact pixel golden을 만들지 않는다. 같은 machine/run의 ROI 평균·분산·histogram과 시각 판정을 함께 사용한다.

P0 PASS:

- transparent/semitransparent/opaque 픽셀이 모두 기대대로 합성된다.
- P0-Controller가 systemDefault/Base/Thin과 light/dark tint profile을 public 계약대로 모두 적용한다.
- runtime API의 마지막 accepted snapshot이 controller effective state와 화면에 반영되고 모든 이전 accepted revision이 정확히 한 terminal로 끝난다.
- Base/Thin 및 대표 tint 조합이 same-run capture ROI 통계에서 서로 구분되고 system default reset이 원복된다.
- resize 연속성 수치와 실물 판정이 opaque 기준선보다 나빠지지 않는다.
- 새 visible content owner, CPU copy, 새 blocking wait가 없다. DesktopWindowTarget/controller는 backdrop만 소유한다.

P0 즉시 FAIL:

- DwmSetWindowAttribute 성공인데 child alpha가 무시되어 검정/불투명으로 보인다.
- alpha를 위해 layered window, CPU copy, 별도 Doroti content composition surface가 필요해진다.
- SetTarget/Kind/tint 중 하나라도 pin된 2.4 runtime에서 지원되지 않거나 요청값과 effective output이 일치하지 않는다.
- controller Acrylic 또는 child redirection bitmap이 border-drag에서 후행한다.

### A2. P0-Controller 제품 통합

A1의 P0-Controller arm이 옵션과 resize gate를 모두 PASS할 때만 진행한다. P0-DWM control은 제품 fallback으로 승격하지 않는다.

- [ ] Doroti.Ui에 WindowAcrylicKind, WindowAcrylicAppearance, WindowAcrylicOptions를 추가하고 DorotiViewConfiguration.backdrop을 WindowsNative configuration으로 전달한다.
- [ ] DorotiCapabilityIds에 view별 WindowBackdropControl capability를 추가하고 IWindowBackdropHostCapability.UpdateAcrylicAsync를 WindowsManagedProductHost가 구현한다. DorotiView.UpdateAcrylicAsync는 capability를 명시적으로 require하며 process-global current window를 찾지 않는다.
- [ ] public update argument는 변경 가능한 controller wrapper가 아니라 검증된 전체 WindowAcrylicOptions 값 객체다. 호출 시 managed snapshot을 복사해 호출자가 이후 상태를 간접 변경할 수 없게 한다.
- [ ] 현재 native minimum-size 검사와 managed exact-size 검사가 서로 다르므로 ABI v2를 우선한다.
- [ ] v1 struct-size append를 선택하려면 managed exact-size 검사를 legacy minimum-size로 바꾸고 offset별 field-presence 검사, old managed ↔ new native와 new managed ↔ old native 양방향 호환 test를 먼저 추가한다.
- [ ] ABI v2에는 kind, light/dark appearance presence bit, tint RGB, tint/luminosity opacity presence bit와 값을 명시한다.
- [ ] 초기 configuration ABI와 별도로 runtime BackdropUpdateRequest/BackdropUpdateTerminal packet을 정의한다. 두 packet은 abiVersion/structSize, viewId, backdropRevision을 포함하고 terminal은 status, requested/effective option, resolved theme, controller State, HRESULT/reason을 포함한다.
- [ ] native Host table에는 versioned RequestBackdropUpdate function pointer를, callback table에는 BackdropUpdateTerminal을 추가한다. managed pointer를 넘기지 않고 기존 task HWND/platform-thread marshal 경로로 owned packet을 전달한다.
- [ ] native host는 view별 monotonic revision, current apply 1 + latest pending 1을 소유한다. 새 요청이 pending을 대체하면 대체된 revision을 superseded로 즉시 terminal 처리하고, controller property 적용을 시작한 revision은 결과를 숨기지 않는다.
- [ ] managed public validation과 native defensive validation이 동일한 enum/finite/range/alpha 규칙을 적용하는지 contract test로 고정한다.
- [ ] native 창 생성 전에 mode/fallback을 결정하고, 창 수명 동안 topology를 바꾸지 않는다.
- [ ] support 판정은 OS build + API HRESULT + runtime visual validation 결과를 구분해 기록한다.
- [ ] fallback.solid는 현재 배경색으로 확정한다.
- [ ] fallback.transparent는 지원 OS인 build 26100+의 runtime policy/API 실패 상황에서 실제 alpha contract를 충족하는지 확인한다.
- [ ] activation/theme 변화는 SystemBackdropConfiguration과 theme별 appearance에만 반영하며 resize generation을 만들지 않는다.
- [ ] systemDefault 복귀는 ResetProperties → Kind → configuration 순서로 수행하고 부분적으로 오래된 tint 값이 남지 않는지 검증한다.
- [ ] runtime apply는 한 platform callback에서 전체 profile을 적용하고 target/controller를 재생성하지 않는다. 완료 callback은 controller property 적용 완료를 뜻하며 DWM visible/scan-out 완료라고 명명하지 않는다.
- [ ] target/backend diagnostic slug와 evidence JSON에 controller/DWM arm, requested/applied backdropRevision, enqueue/apply/terminal QPC, requested/effective Kind, active/resolved theme, tint RGB, tintOpacity, luminosityOpacity, fallback/controller State, terminal reason을 명시한다.
- [ ] DorotiDemoApp/src/App.cs를 public WindowAcrylicOptions의 실제 Base/Thin 및 light/dark tint consumer 예제로 갱신하고, 실행 중 kind 버튼과 tint/opacity controls 및 requested/effective revision/status 표시를 추가한다.
- [ ] 현재 input, cursor, child focus, IME, UIA owner는 변경하지 않는다.

예상 변경 지점:

- Doroti/src/Doroti.Ui/ViewContracts.cs
- DorotiDemoApp/src/App.cs
- Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_host_v1.h
- Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp
- Doroti/src/Doroti.Host.WindowsAppSdk/WindowsNativeV1.cs
- Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs
- ABI/layout/packaging validator와 WindowsAppSdk README/ADR

### B0. P1 진입 전 capability hard gate

P0-Controller가 옵션 또는 resize gate에서 FAIL해도 곧바로 제품 host를 재작성하지 않는다. 먼저 현재 Windows App SDK 2.4 projection과 packaged ANGLE binary에서 아래 capability를 독립 검증한다.

- [ ] ContentIsland.CreateForSystemVisual, DesktopAttachedSiteBridge, ICompositionSupportsSystemBackdrop target의 실제 runtime 사용 가능성
- [ ] DesktopAcrylicController.IsSupported, AddSystemBackdropTarget, Default/Base/Thin과 custom properties, non-null SystemBackdropConfiguration, activation/theme/high-contrast update와 teardown
- [ ] 하나의 compositor/dispatcher/thread ownership
- [ ] ANGLE D3D11 device 조회와 composition D3D11 device 동일성
- [ ] EGL_ANGLE_d3d_texture_client_buffer, surface share-handle export, share-handle client-buffer import를 각각 query
- [ ] BGRA/RGBA format, premultiplied alpha, bind flags, EGL_TEXTURE_OFFSET_X_ANGLE/Y_ANGLE, row/texture lifetime
- [ ] GL render 완료, EGL unbind, keyed mutex/fence 등 D3D consumer가 읽기 전의 명시적 ownership handoff
- [ ] device-loss와 island/bridge teardown 순서

직접 import와 GPU-only copy가 모두 불가능하면 P1은 FAIL이다. CPU readback/upload로 우회하지 않는다.

### B1. 동일 ContentIsland의 Acrylic + scene spike

기존 [ContentIsland ownership spike](Doroti/validation/winrt-content-island-spike)와 [composition surface spike](Doroti/validation/windows-composition-surface)의 검증된 ownership/slot 아이디어만 재사용한다. 과거 D3D12 product path나 capacity-backed source는 재사용하지 않는다.

목표 topology:

    standard top-level HWND
      └─ DesktopAttachedSiteBridge
          └─ ContentIsland (DesktopAcrylicController backdrop target)
              └─ root visual
                  └─ Doroti content visual / one visible front surface

- [ ] 현재 child WndProc가 소유하는 pointer/key/focus/IME/UIA를 top HWND 또는 Windows App SDK InputSite 중 정확히 한 owner로 이전한다. top HWND 직접 소유를 우선 후보로 측정한다.
- [ ] Doroti의 기존 render child HWND와 Windows App SDK가 내부적으로 사용할 수 있는 platform InputSite child를 구분해 진단한다.
- [ ] child HWND를 optional로 표현할 새 ABI/capability와 topology-neutral host contract를 설계한다.
- [ ] WindowsManagedProductHost의 nonzero ChildHwnd 강제와 HWND 전용 presenter base를 topology-neutral하게 분리한다.
- [ ] duplicate pointer/key/focus/IME event와 duplicate UIA provider가 0인지 검증한다.
- [ ] 별도의 visible render child HWND는 이 mode에서 만들지 않는다.
- [ ] top-level physical client rect와 DPI를 target/generation authority로 유지한다.
- [ ] ContentSiteView.ClientSize와 RasterizationScale은 applied acknowledgement로 기록하고, target과 정확히 일치한 뒤에만 그 generation을 admit한다.
- [ ] host target과 island applied size를 동기화하되 ANGLE surface가 별도 독립 resize clock을 갖지 않게 한다. SiteView를 authority로 바꾸는 대안은 native generation/FrameRequest/C ABI 교체를 포함하는 별도 설계로 취급한다.
- [ ] DesktopAcrylicController와 Doroti content가 같은 ContentIsland geometry target을 사용하게 한다.
- [ ] P0-Controller와 동일한 public WindowAcrylicOptions를 AddSystemBackdropTarget 경로의 Kind/FallbackColor/TintColor/TintOpacity/LuminosityOpacity에 적용한다.
- [ ] systemDefault reset, light/dark custom profile, invalid-value rejection과 effective diagnostics가 P0-Controller와 동일한 contract를 만족하게 한다.
- [ ] P0와 같은 IWindowBackdropHostCapability/runtime ABI/terminal 계약을 구현하고, update마다 ContentIsland, bridge, compositor, target 또는 content surface를 재생성하지 않는다.
- [ ] Desktop Acrylic external content와 Doroti visual이 같은 compositor transaction이라고 가정하지 않고 visible alignment를 측정한다.
- [ ] parent HWND Acrylic이 transparent island를 통과한다고 가정하지 않는다.
- [ ] WinUI XAML/SwapChainPanel을 중간 owner로 넣지 않는다.

렌더 경로 우선순위:

1. CompositionDrawingSurface.BeginDraw가 준 D3D11 update texture를 같은-device ANGLE EGL surface/FBO로 import하고 SkiaSharp가 exact target을 직접 wrap한다.
2. 1번이 불가능할 때만 ANGLE-owned exact shared D3D11 texture에서 hidden composition surface로 한 번의 GPU copy를 허용한다.
3. 어느 쪽도 성립하지 않으면 중단한다.

BeginDraw 직접-import protocol:

- [ ] persistent slot은 CompositionDrawingSurface이며 BeginDraw 반환 texture 자체를 영구 slot으로 저장하지 않는다.
- [ ] 매 BeginDraw가 반환한 transient update object와 POINT offset을 해당 호출 범위에서만 사용한다.
- [ ] EGL_TEXTURE_OFFSET_X_ANGLE/Y_ANGLE을 적용해 update rect가 정확한 destination pixel에 대응하게 한다.
- [ ] 그 범위 안에서 EGL surface/FBO와 Skia wrapper를 bind/wrap하고, Skia 사용 전 GRContext.ResetContext(GRGlBackendState.All)을 실행한다.
- [ ] render/flush/Submit(false) 뒤 명시적 GPU handoff를 걸고 EGL current/binding을 해제한다.
- [ ] imported pbuffer/texture가 EGL의 current read/draw surface이거나 texture에 bound된 동안 D3D에서 읽지 않는다.
- [ ] SKSurface/GRBackendRenderTarget/EGL wrapper와 update object를 정리한 뒤 EndDraw한다.

### B2. exact hidden-slot + atomic commit protocol

- [ ] 최대 3개 slot만 둔다: visible front, preparing/current, retiring.
- [ ] visible front는 resize하지 않는다.
- [ ] 새 generation의 exact physical extent로 free slot을 만들거나, safe-reuse가 별도로 증명된 retired slot만 재사용한다.
- [ ] SKSurface와 GRBackendRenderTarget wrapper를 먼저 dispose하고 GPU handoff/compositor retirement를 확인한 뒤에만 native EGL surface/texture를 destroy/reuse한다.
- [ ] 새 slot의 모든 픽셀을 clear/draw하고 exact scene을 render/flush/submit한 뒤 비차단 GPU handoff를 수행한다.
- [ ] swap/EndDraw 전에 generation과 input sequence를 다시 검사한다.
- [ ] brush.Surface, visual Size, Clip, transform을 같은 compositor commit에 넣는다.
- [ ] commit 완료 전 old front를 파괴하거나 다시 쓰지 않는다.
- [ ] commit 완료만으로 retiring slot을 free로 돌리지 않는다.
- [ ] 선택한 surface API가 보장하는 acquire/release 또는 별도 GPU/compositor safe-reuse 신호를 증명한 뒤에만 retiring slot을 mutate/reuse한다.
- [ ] 안전한 3-slot 재사용 신호를 증명하지 못하면 pool을 무제한 늘리지 않고 P1을 FAIL 처리한다.
- [ ] commit 완료를 scan-out 완료로 기록하지 않는다. capture observation을 별도 필드로 둔다.
- [ ] queue depth는 current 1 + latest 1, surface pool은 3 이하를 강제한다.

generation ledger 최소 필드:

- run id / resize generation / scene sequence / input sequence
- 별도 backdropRevision / requested-applied-terminal QPC / origin(app, theme, policy) / requested-effective profile / resolved theme / terminal reason
- WM_SIZING candidate / WM_SIZE actual / SiteView actual
- logical size / physical size / DPI / rasterization scale
- slot id / target extent / SKSurface extent
- metrics publish / scene admitted / render begin-end
- GL flush-submit / GPU sync / EndDraw or copy
- swap or commit requested / commit completed
- terminal reason
- capture frame id/QPC와 test scene marker에서 decode한 content generation/target extent
- capture 시점의 shell/client extent, SiteView applied extent, commit serial, backdrop boundary

P0-DWM/P0-Controller/P1의 system Acrylic 자체에는 app이 부여할 수 있는 generation이 없다. backdropRevision은 command 처리 상관관계일 뿐이다. 따라서 “capture가 Acrylic generation을 관찰했다”고 기록하지 않고, machine-readable content marker와 WGC timestamp/QPC를 shell/client, 마지막 applied backdropRevision 및 backdrop boundary 관찰과 연계한다.

### B3. shell wait 정책 A/B

shell geometry와 composition scan-out은 완전히 같은 transaction이 아닐 수 있으므로 wait 값을 직감으로 정하지 않는다.

- [ ] 후보 1: WM_SIZE는 즉시 반환하고 current+latest exact render/commit만 수행한다.
- [ ] 후보 2: 후보 1에서 visible lag가 있을 때만 render thread의 bounded commit-aware gate를 실험한다.
- [ ] platform/WndProc thread에서 commit completion, fence, DwmFlush를 기다리는 후보는 만들지 않는다.
- [ ] wait를 넣어 평균 수치가 좋아져도 실물 drag가 계단식이면 FAIL이다.
- [ ] DwmFlush는 exact present 뒤 render thread에서만 현재 방식과 A/B한다.

P1 PASS는 public Acrylic 옵션 전체가 정확히 적용되고 content marker, shell/island extent, backdrop boundary가 함께 정렬되며 표준 shell resize가 opaque 기준선 수준인 경우뿐이다. Acrylic 자체의 가상 generation을 만들지 않는다.

### C. 제품 통합과 fallback

P1이 통과하면 existing presenter에 조건문을 누적하지 말고 별도 Acrylic composition host/presenter로 격리한다.

- [ ] topology는 native window 생성 전에 선택한다.
- [ ] native ABI에는 backdrop/topology 설정과 진단 상태만 둔다. managed/native GPU pointer를 ABI로 노출하지 않는다.
- [ ] framework scene renderer와 exact frame coordinator는 공유한다.
- [ ] HWND presenter와 composition presenter는 동일한 terminal/exactness contract를 구현한다.
- [ ] mode 전환은 다음 창 생성부터 적용하며 한 창에서 mid-session 전환하지 않는다.
- [ ] Acrylic mode 안의 kind/tint runtime update는 topology 전환이 아니므로 동일 capability로 지원하고, solid/transparent/system ↔ acrylic mode 전환 API와 혼합하지 않는다.
- [ ] Acrylic 초기화/device loss/runtime policy 실패 시 새 창은 명시된 fallback으로 시작한다. 보이는 창을 다른 topology로 즉시 갈아타지 않는다.
- [ ] opaque 기본값은 별도 승격 결정 전까지 유지한다.

후보 파일:

- Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedAcrylicCompositionPresenter.cs
- Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedProductHost.cs
- Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedHwndPresenterBase.cs
- Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs
- Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp
- Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_host_v1.h
- Doroti/src/Doroti.Host.WindowsAppSdk/WindowsNativeV1.cs
- Doroti/validation/windows-acrylic-composition-spike/
- Doroti/validation/contracts/의 신규 opt-in contract
- Doroti/eng/의 신규 validator/capture runner

## 7. 검증 계획

저장소 지침에 따라 모든 build/test command의 timeout은 20분으로 설정한다. 자동 검증은 물리 모니터 acceptance를 대체하지 않는다.

### 자동 build/contract

- [ ] Release win-x64 self-contained build
- [ ] empty PATH/package-consumer launch
- [ ] native/managed ABI size, offset, feature-bit 일치
- [ ] WindowAcrylicKind와 light/dark appearance의 managed → ABI → native requested/effective round-trip 일치
- [ ] null과 0.0 presence 구분, invalid enum/NaN/infinity/out-of-range/non-opaque tintColor rejection
- [ ] ResetProperties 후 system default 복귀와 theme profile 재적용
- [ ] DorotiView capability 등록/미등록, acrylic/non-acrylic view, disposed view의 runtime API contract
- [ ] BackdropUpdateRequest/Terminal ABI size·offset·presence bit와 managed → native → managed round-trip
- [ ] 100/500 update burst에서 current apply 1 + latest pending 1, accepted backdropRevision terminal one-to-one, last-request-wins
- [ ] update/theme/close/device-loss 경합에서 duplicate/missing terminal 0, target/controller/SetTarget 재생성 0
- [ ] kind/tint update만으로 새 resize generation, framework frame, EGL surface recreate가 발생하지 않음
- [ ] current opaque contract가 그대로 PASS
- [ ] opt-in이 없을 때 Acrylic/ContentIsland code path가 실행되지 않음
- [ ] latest generation admission, stale reject, terminal one-to-one
- [ ] queue depth 2 이하, slot pool 3 이하
- [ ] CPU readback/upload/GDI/bitmap copy 0
- [ ] D3D/EGL/Skia device 오류와 resource leak 0

기존 기준 명령:

    pwsh -NoProfile -File ./Doroti/eng/validate-hwnd-exact-cpp-c0.ps1
    pwsh -NoProfile -File ./Doroti/eng/validate-winrt-content-island-w1r.ps1
    pwsh -NoProfile -File ./Doroti/eng/validate-winrt-composition-w0.ps1
    pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

A1/B1에는 독립 신규 validator를 만들고 동일한 20분 상한을 적용한다. 현재 저장소에 없는 과거 live-resize script 이름을 완료 명령으로 가정하지 않는다.

### 자동 visible capture

비대칭 grid/oracle과 Windows Graphics Capture를 이용해 각 frame의 네 모서리/edge phase를 판정한다.

- [ ] 500×300 같은 비정사각, exact non-empty이면서 모든 픽셀이 transparent인 scene
- [ ] Flutter식 empty-frame present contract를 별도로 도입할 경우에만 true empty disposition도 추가
- [ ] 0%, 25%, 50%, 80%, 100% alpha 영역
- [ ] systemDefault/Base/Thin 각각의 same-run ROI 통계 reference
- [ ] light/dark custom tintColor와 tintOpacity/luminosityOpacity 대표값의 ROI differential
- [ ] theme switch 전후 requested/effective profile과 화면 일치
- [ ] idle과 fast border drag 중 systemDefault → Base → Thin 및 서로 다른 tint snapshot 연속 교체
- [ ] applied terminal 뒤 안정된 마지막 요청이 capture ROI에 반영되는 시간을 별도 측정하되 terminal 자체를 scan-out 완료로 간주하지 않음
- [ ] fallback.solid/transparent와 controller State 전환 시 화면 일치
- [ ] fast 연속 expand/shrink/reverse
- [ ] maximize/restore/minimize/0-size
- [ ] left/top/right/bottom/corner drag
- [ ] old-size stretch, 검은/흰 band, raw desktop 노출, 한 세대 늦은 Acrylic 없음

캡처 기반 exactness gate:

- wrong-size present/commit = 0
- stale generation present/commit = 0
- duplicate/unterminated frame terminal = 0
- capture에서 asymmetric edge phase 불일치 = 0
- marker에서 decode한 content target과 shell/client extent 불일치 = 0
- content edge와 backdrop/window boundary 분리 = 1 physical pixel 이하이고 2개 연속 capture frame에 지속되지 않음
- blank/solid-black/previous-size frame = 0
- requested/effective Kind/tint/profile 불일치 = 0
- accepted backdropRevision의 duplicate/missing terminal = 0
- 마지막 runtime 요청과 effective/capture profile 불일치 = 0
- runtime update 중 SetTarget/controller/target/root 재생성 = 0
- Base/Thin과 대표 tint 조합 간 expected ROI differential 미검출 = 0

동일 machine/monitor/run 조건의 opaque 기준선 대비 timing gate:

- target → swap(P0) 또는 target → content commit(P1) p95는 기준선 + 1 refresh interval 이하
- target → visible capture p95는 기준선 + 1 refresh interval 이하, max는 기준선 + 2 refresh interval 이하
- 100ms exact-wait timeout 수는 기준선 이하이고 final settle generation의 timeout은 0
- queue depth 2 이하, surface slot 3 이하
- failed/wrong-size/stale/duplicate/missing terminal은 모두 0
- runtime option churn을 함께 실행한 border-drag의 target → visible capture p95/max도 같은 기준을 만족한다.
- 안정된 마지막 backdrop update의 applied terminal → visible ROI 반영은 동일 run의 controller 기준선 + 2 refresh interval 이하로 관찰한다. 이 수치는 app command latency 측정이며 Acrylic generation 보장은 아니다.

### 실물 acceptance

아래는 사람이 실제 창 border를 잡고 모니터에서 확인해야 한다.

- [ ] 네 방향과 네 모서리, slow/medium/fast, expand/shrink/reverse를 각 10초 이상
- [ ] 가능한 60/120/144/165Hz monitor
- [ ] 100/125/150/200% DPI와 monitor crossing
- [ ] Snap layout, maximize, restore, minimize, Alt+Tab, occlusion
- [ ] light/dark, 활성/비활성, transparency off, battery saver, high contrast, RDP fallback
- [ ] systemDefault/Base/Thin과 light/dark tint profile을 각각 선택해 blur/tint/fallback이 의도대로 보이는지 확인
- [ ] border를 계속 drag하는 동안 Demo controls/자동 입력으로 kind와 tint를 반복 변경해 resize가 멈추거나 뒤늦게 계단식으로 따라오지 않는지 확인
- [ ] pointer hit-test, resize cursor, focus, keyboard, clipboard
- [ ] 한국어 IME 조합/후보창/caret 위치
- [ ] Narrator 또는 Accessibility Insights로 UIA tree와 actions

실물 PASS 기준:

- border와 content/Acrylic 사이에 눈에 보이는 후행, 출렁임, 계단식 추종이 없다.
- 한 프레임이라도 old-size stretch, 검은 띠, raw background band가 반복 재현되지 않는다.
- 같은 run의 opaque 기준선보다 체감이 나빠지지 않는다.
- 자동 run을 3회 연속 통과하고 실물 acceptance까지 끝난다.

실행하지 않은 monitor/DPI/IME/accessibility 항목은 notVerified로 남긴다.

## 8. 승격, 문서화, rollback

### 승격 조건

- P0-Controller 또는 P1 하나가 Acrylic 옵션, exactness, GPU, visible, physical gate를 모두 통과한다. P0-DWM control 단독 통과는 제품 승격 조건이 아니다.
- 실행 중 kind/tint API의 ordering, terminal, last-request-wins, theme race, close/device-loss 및 border-drag 동시 부하 gate가 모두 통과한다.
- opaque fallback과 기존 package contract가 회귀하지 않는다.
- 지원 하한 Windows 11 24H2(build 26100), fallback, theme/policy 동작이 사용자 문서에 명시된다.
- 새로운 ADR이 visible owner, alpha path, generation/commit/retirement 계약을 고정한다.
- 처음에는 opt-in으로 배포하고 기본값 변경은 별도 결정으로 남긴다.

### 실패 시

- 실패 후보를 제품 코드에 남기지 않고 독립 spike와 evidence만 보존한다.
- current opaque HwndExactCpp path를 계속 기본으로 사용한다.
- API HRESULT 성공, 깨끗한 GPU counter, automated capture만으로 partial implementation을 승격하지 않는다.
- P0-Controller 실패 사유가 child redirection alpha, SetTarget/options, visible resize 중 하나이면 P1 capability gate로 이동한다.
- P1의 direct import와 승인된 one-copy GPU-only 경로가 모두 실패하거나 실물 resize gate에서 실패하면 Acrylic 제품화를 중단하고 원인을 문서화한다.

### 완료 문서

- 새 ADR 또는 ADR-025 후속 ADR
- WindowsAppSdk host README/README.ko.md
- renderer/feature manifest와 validation contract
- support matrix: Windows 11 24H2+ build, Acrylic mode, fallback, transparency policy, remote session
- 자동 evidence와 실물 acceptance 기록

## 9. 최종 완료 정의

다음 조건이 모두 참일 때만 작업 완료다.

1. DorotiDemoApp의 WindowBackdropMode.acrylic이 실제 Windows Desktop Acrylic을 보인다.
2. systemDefault/Base/Thin, theme별 tintColor/tintOpacity/luminosityOpacity가 public 요청과 effective controller/화면에서 일치한다.
3. 앱 실행 중 DorotiView API로 kind/tint snapshot을 바꿀 수 있고, accepted revision의 terminal one-to-one, last-request-wins, theme/close/device-loss 동작이 계약과 일치한다.
4. runtime update는 controller/target/topology를 다시 만들거나 resize generation/frame을 만들지 않으며 border drag 품질을 떨어뜨리지 않는다.
5. 투명/반투명/불투명 Doroti 픽셀이 premultiplied alpha로 정확히 합성된다.
6. border drag 중 geometry와 마지막 visible content가 exact generation으로 일치한다.
7. 현재 opaque 경로와 비교해 실시간 resize 품질이 나빠지지 않는다.
8. CPU readback/upload 없이 GPU-only 경로다.
9. input, 한국어 IME, UIA, DPI, device-loss, fallback이 통과했다.
10. 3회 연속 자동 PASS와 물리 모니터 acceptance가 모두 기록됐다.
11. 통과하지 않은 항목이 PASS로 표현되지 않고 notVerified 또는 FAIL로 남아 있다.

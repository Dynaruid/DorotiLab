# Windows App SDK Acrylic P0/P1 gate results

Date: 2026-09-01  
Decision: P0 `FAIL`, P1 `FAIL`, product integration `notRun`, opaque HwndExactCpp remains default

## Scope and environment

This run executed the staged gates in `work.md`; it did not reinterpret a
capability spike as product completion. The machine was Windows 11 25H2,
build 26200.9168, at 200% DPI. The host and spikes resolved Windows App SDK
2.4.0. Comparison processes pinned `DOROTI_WINDOWS_DWM_FLUSH=0` and
`DOROTI_WINDOWS_EGL_SWAP_INTERVAL=1`; both variables were unset in the parent
shell.

The official DWM contract says `DWMWA_REDIRECTIONBITMAP_ALPHA` is available
from build 26100 and expects premultiplied alpha. The controller arm also used
the documented Win32 `DWMWA_USE_HOSTBACKDROPBRUSH` prerequisite for
`DesktopAcrylicController.SetTarget`:

- <https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute>
- <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacryliccontroller.settarget>

## P0 result

The A1 executable preserved the existing top/child HWND topology and linked
the current `WindowsManagedAngleEglPresenter` source into the independent
validation assembly. No candidate behavior was added to the product host.

The complete matrix was deterministic:

| Arm | alpha off | top | child | both |
| --- | --- | --- | --- | --- |
| opaque | PASS | n/a | n/a | n/a |
| DWM transient backdrop | PASS | PASS | FAIL | FAIL |
| DesktopAcrylicController.SetTarget | PASS | PASS | FAIL | FAIL |

Every top-level redirection-alpha call returned `S_OK`. Every case that
included the app-owned render child returned `0x80070006 (E_HANDLE)` for
attribute 39. The controller itself was supported and active: one compositor,
root, target, controller, and SetTarget; Default/Base/Thin/custom/reset options;
605 accepted updates; duplicate and missing terminals 0; maximum pending depth
1; GPU and exactness errors 0.

The plan makes child alpha rejection an immediate P0 failure. Top-level
success and controller option success therefore do not authorize A2.

## P1 capability and geometry result

B0 established both prerequisites:

- one system compositor/dispatcher owned the ContentIsland visual and
  `DesktopAcrylicController.AddSystemBackdropTarget` returned true;
- `ICompositorInterop::CreateGraphicsDevice` used the Windows.UI.Composition
  IID and vtable slot from the installed 26100 SDK;
- `ICompositionDrawingSurfaceInterop::BeginDraw` returned a transient D3D11
  texture with offset `(1,2)`;
- the packaged ANGLE advertised
  `EGL_ANGLE_d3d_texture_client_buffer`, exposed the EXT device-query entry
  points, returned its D3D11 device, and accepted the BeginDraw texture through
  `EGL_D3D_TEXTURE_ANGLE`;
- GL clear/flush, EGL unbind/destroy, texture release, and EndDraw completed
  with EGL success and GL error 0; CPU readback, staging map, GDI copy, and
  bitmap upload were all 0.

The direct-import ownership and same-device constraints follow ANGLE's public
extension specifications:

- <https://chromium.googlesource.com/angle/angle/+/HEAD/extensions/EGL_ANGLE_d3d_texture_client_buffer.txt>
- <https://chromium.googlesource.com/angle/angle/+/refs/heads/chromium/7370/extensions/EGL_ANGLE_device_d3d11.txt>

B1 then connected Acrylic and one Doroti content visual to the same
ContentIsland. One controller/target applied Default/Base/Thin, light/dark
custom, reset, and final profiles without recreation. The first WGC run exposed
a real 200% DPI bug: using physical
client pixels as visual DIPs enlarged one alpha quadrant over the whole
client. The corrected spike admits only a `SiteView.ClientSize` acknowledgement
that matches the latest physical HWND target, creates an exact physical
CompositionDrawingSurface, uses `ActualSize` for the visual/root, and applies
`1 / RasterizationScale` to the surface brush. The corrected capture showed
the four alpha quadrants plus the transparent center strip at exact geometry.

## B2 blocker and final P1 decision

Three immutable exact surfaces were created and retained as front/retiring
slots. The fourth resize would require a retired surface to be resized or
redrawn. `BeginDraw` documents the transient update object and offset, and
`Resize` documents changing the drawing-surface extent, but the reviewed API
surface did not provide a documented signal proving that the compositor had
retired its prior read before mutation:

- <https://learn.microsoft.com/en-us/windows/win32/api/windows.ui.composition.interop/nf-windows-ui-composition-interop-icompositiondrawingsurfaceinterop-begindraw>
- <https://learn.microsoft.com/en-us/uwp/api/windows.ui.composition.compositiondrawingsurface.resize>

Per `work.md`, commit completion alone was not relabeled as scan-out or safe
reuse. The spike capped the pool at three, performed no unsafe mutation, and
terminalized every later request as `failed-safe-retirement-unproven`. This is
a B2/P1 failure, so B3 and C were not run.

## Evidence and validation boundary

Representative local manifests:

- `.doroti/evidence/acrylic-a1-20260901-151919-4b3fe0a127da/manifest.json`
- `.doroti/evidence/acrylic-b1-20260901-152243-fad375c38842/manifest.json`
- `.doroti/evidence/w1r-20260901-151717-31099135e7d8/w1r-manifest.json`
- `.doroti/evidence/w0-20260901-151910-f462a601bbea/w0-manifest.json`

The B1 visible run captured 175 WGC frames and encoded 18 PNGs with capture,
drop, and capacity errors all 0. Five distinct alpha-scene sample colors were
observed. This proves capture transport and a visible premultiplied-alpha test
scene, not physical border-drag quality, Acrylic blur policy, scan-out,
multi-monitor/DPI crossing, Korean IME, UIA, device loss, fallback, or three
consecutive qualification runs. Those remain `notVerified`.

`validate-winrt-content-island-w1r.ps1` and the repaired XML lookup in
`validate-winrt-composition-w0.ps1` passed. The opaque C0 validator was blocked
before Doroti validation because `reference/flutter-master` already contained
large tracked deletions. Those reference-tree changes were not modified.

No public Acrylic mode/API/ABI, demo UI, product presenter, package contract,
or default renderer changed. A future retry needs a documented safe-retirement
primitive or a different bounded surface API; it must not use an unbounded
pool, CPU readback/upload, or a commit-completion assumption.

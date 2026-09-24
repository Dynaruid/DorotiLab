# Windows MAUI resize and Acrylic regression

**Current status: PARTIAL.** Native Windows caption is now enabled at window
construction, as explicitly selected by the user. Final capture found no black
resize strips or caption geometry mismatch; content/outer-edge timing still
fails the full visual gate. See the latest
[handoff report](../../../windows-maui-resize-report.md) for final changes,
passed operational checks, and the **failing** `verify-flicker.py` visual gate.
Earlier passing size/lifetime results below do not qualify visual continuity.

The 23:10 KST continuation enables alpha in the WinUI root redirection bitmap
using `DwmEnableBlurBehindWindow` with an empty region. Client Acrylic stays under
the existing `SystemBackdrop` owner. An ON/OFF comparison found 0/6 black-edge
frames in 93 samples each. Raw WindowsAppSDK creates its HWND with
`WS_EX_NOREDIRECTIONBITMAP`; setting that style after WinUI creation did not work.
The dedicated MAUI app now resets the caption during `OnPlatformWindowSubclassed`,
before `NavigationRootManager` captures its title-row policy. Late reset produced
a duplicate row. The title bar uses standard Windows appearance; client Acrylic
is preserved. Embedded MAUI window caption behavior is unchanged.

Final default Release validation: **636 acquired DXGI frames over eight edges,
zero black strips detected**. The separate strict gate still **failed**: 81
samples, black/white/title-blank/caption-mismatch counts all 0, exposed-edge and
content-mismatch counts both 6. Size/cancellation (24), drag, maximize/restore,
Acrylic/click, clean exit and four geometry regression checks passed. Default and
x64 Release builds passed with no warnings. Raw WindowsAppSDK's comparison also
had zero black strips/caption mismatches but 5 exposed-edge and 17 content-mismatch
frames in 87 samples. These are bounded capture observations, not scan-out proof.

`capture-resize-frames.py` stores every acquired frame in bounded memory during
each drag and writes PNGs/metadata afterwards. `dense-result.json` includes binary
SHA256, per-edge frame counts and DXGI accumulated-frame metadata. Initial
accumulation is separate from coalesced updates during capture; neither is hidden.
The final run coalesced 41 desktop updates after the first acquisition per edge.
The pure-black strip detector excludes the persistent standard native caption
and cursor neighborhood. It does not qualify all dark colors or general geometry.
Default edges are right, bottom, bottom-right, left, top, top-left, top-right and
bottom-left. Set `DOROTI_CAPTURE_EDGES` to a comma-separated subset if needed.

```powershell
$env:DOROTI_MAUI_VALIDATION_EXE=(Resolve-Path 'DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe').Path
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/capture-resize-frames.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
# For a raw WindowsAppSDK comparison, set DOROTI_RESIZE_HOST=windowsappsdk and
# DOROTI_MAUI_VALIDATION_EXE to that host's absolute executable path.
```

Historical 22:15 results below precede the root-alpha/native-caption fixes.

The 22:15 KST continuation corrected a **measurement bug**: the Acrylic caption
fill was within 5 RGB levels of the background, so the old tolerance-6 search
mistook the capture's far edge for the window boundary. The old 36/100 mismatch
count and 72 px maximum are superseded. The gate now matches the calibrated DWM
outline, checks the close glyph independently, and rejects missing/ambiguous
borders or foreground loss as `notMeasured`. Four synthetic geometry regressions
cover this failure mode. It also requires actual width and height changes.

The native output no longer calls `DwmFlush` from its nested `WM_SIZE` present;
GPU fence ownership is retained with a non-pumping kernel wait. The 22:15 DXGI
capture: **failed**, 97 samples, 0 content geometry mismatches, 6 exposed right
strips, 11 caption mismatches. Build, size/cancellation, dispatch, OS drag and
maximize/restore passed. Acrylic rerun was `notMeasured` because the test window
did not retain foreground. This is not visual completion or physical proof.

`verify-flicker.py` enables bounded native QPC logging to `timeline.json` in its
output directory. After the app closes, correlate the CPU capture intervals with
window message entry/exit, child HWND geometry and Present calls:

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/test-flicker-geometry.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/analyze-timeline.py <evidence-directory>
```

Install optional capture packages into removable artifacts with
`python -m pip install --target Doroti/artifacts/validation/capture-python dxcam==0.3.0`.
`DOROTI_FLICKER_CAPTURE=gdi` selects an optional CPU-sampled Pillow cross-check;
it does not identify unique display frames. The default is `dxgi`.
`DOROTI_FLICKER_OBSERVE_ONLY=1` suppresses the visual-failure assertion for
investigation but keeps `status=failed`; it is not acceptance mode. Neither
backend nor the QPC join proves physical scan-out or latency.

Run from the repository root. Each command has the repository's 1,200-second
process-tree deadline. Close other validation instances first; the scripts
only resize/close the process they start. Acrylic validation temporarily moves
the pointer and opens a red/blue background window, then restores the pointer.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-resize.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/windows-maui/ResizeDispatch -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-drag.py
```

The resize gate sends 24 size changes, including all eight `WM_SIZING` edges,
programmatic `SetWindowPos`, growth/shrinkage, and a cancelled proposal. It checks
pre-geometry frame readiness, no readiness timeouts, multiple presented size
generations, one Graphite device, final client/content dimensions, no renderer
failures, and clean shutdown. Its last resize occurs after the diagnostics write
interval so an older snapshot is not mistaken for a stale final frame.

The drag gate uses injected OS mouse input to exercise USER32's real modal sizing
loop on the right and top-left edges, then maximizes/restores the window. It
restores the pointer and only controls the process it starts. The queue contract
checks UI affinity, deferred moving-origin presentation, cancellation, timeout,
error propagation and exactly-once execution without pumping arbitrary messages.

Scripts default to the x64 build above. Set `DOROTI_MAUI_VALIDATION_EXE` to an
absolute executable path to validate the plain `dotnet build ... -c Release`
output under `windows/bin/Release` instead.

The Acrylic gate compares the same app-bar region over controlled red and blue
backgrounds, with the sample's effect off and on. It exercises the sample toggle
with OS mouse input and captures the real desktop. These pixel checks do not
measure subjective material quality or physical display latency.

## 2026-09-24 result

- Windows MAUI Release build: passed, 0 warnings/errors.
- Final resize gate: passed; 24 size requests, 24 target generations, 23 presented
  generations (latest-only coalescing), one Graphite device, final Skia size
  934 × 623 physical pixels matching the client below the native title bar.
- Surface preparation p95: 2,247 microseconds in this bounded run. This is a
  renderer preparation measurement, not FPS or input-to-display latency.
- Acrylic off: red/blue background change produced RGB difference `(0, 0, 0)`.
  Acrylic on: `(18, 3, 22)` after Thin material and the lighter scaffold wash.
  Before those appearance adjustments, with native Acrylic newly connected, the
  same region measured `(7, 1, 13)`.
- Both scripts completed clean application shutdown without renderer exceptions.
- Local artifacts: `Doroti/artifacts/validation/windows-maui/20260924-090803/`
  (resize), `20260924-090730/` (Acrylic); removable under repository policy.
- Physical edge dragging, mixed-DPI monitors, light/high-contrast modes, and
  unsupported-device fallback remain `notVerified`. Captures used dark mode at
  200% scaling. Windows App SDK sample execution was not repeated for its shared
  appearance change.

## Implementation

MAUI previously recreated the Vulkan device and Graphite session for every
changed backing size. The presenter now releases the imported images before
replacing the D3D allocation and retains its device/session. Existing Vulkan and
D3D copy drains preserve GPU ownership across the import replacement.

The earlier implementation let WinUI handle `WM_SIZE`/`WM_DPICHANGED` and then
called `UpdateLayout`. Live tracing showed that XAML could still report the old
size at that point. The full-window runner now uses the native path below;
embedded surfaces retain the XAML layout authority.

The MAUI window now consumes its requested Acrylic options through a window-owned
`SystemBackdrop`. Activation, theme, high contrast, and disconnection use the
[WinUI SystemBackdrop lifecycle](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.systembackdrop).
MAUI page/container backgrounds no longer apply a second layer over the renderer.
The shared Windows Testbed selects Thin Acrylic and changes only the enabled
sample scaffold opacity from 153/255 to 96/255; the disabled effect stays opaque.

## 2026-09-24 native resize correction

The dedicated MAUI window runner explicitly opts its root Doroti surface into
native size authority. Its premultiplied DXGI chains are attached directly to
the HWND through DirectComposition, avoiding XAML layout and the asynchronous
CompositionDrawingSurface retirement queue for visible rendering. The renderer
now retains one monitor-capacity chain and attaches it once. Changed-size frames
are prepared in its back buffer, with the HWND clipping capacity at natural pixel
size. `SetSourceSize` is deliberately not used because it stretches the cropped
content to retained capacity on this composition path.

`WM_SIZING` and `WM_WINDOWPOSCHANGING` prepare an exact native content epoch.
All edges defer the prepared front until actual geometry is reconciled, with a
compositor-clock alignment before `WM_WINDOWPOSCHANGING` applies geometry. A cancelled or mismatched
proposal is rejected at the latest-generation gate. The readiness wait is
100 ms, services only owned Composition callbacks, and uses a kernel wait to
avoid CLR STA message-pump reentrancy. Native GPU/commit waits are additional
to that readiness budget; this is not a hard end-to-end latency guarantee.

The native plane lies outside WinUI's visual hit-test tree. Mouse and touch/pen
ingress are attached to the existing WinUI input HWND with the same content
origin, while WinUI remains the focus, IME and accessibility owner. Native
caption and resize borders remain system-owned. Embedded `DorotiMauiSurface`
instances continue to use their existing XAML composition/input path.

Earlier operational validation on the local NVIDIA RTX 4060 Laptop GPU, 200% DPI
(superseded by the final report above for visual acceptance):

- x64 and plain Release builds passed with zero warnings/errors.
- 24 changes plus cancellation: 41 pre-geometry readiness observations,
  zero readiness timeouts, 25 presented size generations, one Graphite device,
  final 934 × 623 content pixels, clean exit. Readiness p95 was 22.391 ms;
  maximum complete synthetic request was 107.877 ms, not display latency.
- OS-injected right and top-left drags: 14 `WM_SIZING` ready observations,
  zero timeouts, correct geometry; maximize/restore and clean exit passed.
- Native dispatcher contract passed all seven checked behaviors.
- Click/Acrylic regression passed: opaque RGB response `(0,0,0)`, enabled
  response `(84,14,102)` to controlled red/blue backgrounds. Final desktop
  captures were inspected for content scale and bounds.
- An extra focused-button Enter activation probe failed on both the old
  09:13 binary and this revision. Keyboard activation/IME is **notVerified**;
  this resize change does not claim to repair the existing keyboard path.
- Physical edge-drag smoothness, scan-out latency, mixed-DPI monitors, touch/pen,
  assistive technology and device-loss recovery remain **notVerified**.

Removable evidence directories under `Doroti/artifacts/validation/windows-maui/`:
`20260924-100102` (size protocol), `20260924-100157` (Acrylic/click),
`20260924-100344` (plain Release OS drag/maximize/restore). Earlier failed
intermediate attempts are not acceptance evidence.

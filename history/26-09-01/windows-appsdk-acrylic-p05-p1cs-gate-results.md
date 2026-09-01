# Windows App SDK Acrylic P0.5/P1-CS gate results

- Date: 2026-09-01
- Scope: validation-only follow-up to the P0 child-HWND and P1
  `CompositionDrawingSurface` failures
- Final decision: **P0.5 FAIL / P1-CS FAIL / product integration notRun /
  opaque HwndExactCpp retained**

## Outcome

The follow-up investigation is complete under the hard-gate rules in
`work.md`. Both candidates proved useful capability, but neither preserved the
visible exact-frame contract during the required Acrylic topology. No file
under `Doroti/src`, public Acrylic API/ABI, demo control, or product fallback was
added.

P0.5 used one standard overlapped top-level HWND as the shell, input owner,
Acrylic target, redirection-alpha target, and direct ANGLE/EGL render target.
Its automated API/topology run passed, but the candidate WGC stream contained
the Acrylic backdrop without the Doroti frame marker or alpha-stripe scene.
Because Acrylic plus the actual Doroti output was not visible together, P0.5
failed before physical promotion.

P1-CS passed the Presentation API and bounded-buffer capability gates using the
actual hardware ANGLE D3D11 device. It sustained available-event-authorized
buffer reuse and showed the Acrylic/alpha scene. During native right-border
resize, however, the presentation surface and ContentIsland geometry were not
one transaction: 160 of 236 decoded/matched WGC frames carried a presented
buffer whose extent differed from that frame's client extent. The plan forbids
accepting asynchronous geometry/content separation, scaling, extra slots, CPU
copy, or a blocking WndProc wait, so P1-CS failed.

## Fixed environment

- OS: Windows 11 build `26200`
- Windows App SDK: `2.4.0`
- Windows SDK: `10.0.26100.0`
- ANGLE adapter: AMD Radeon 780M, vendor `0x1002`, device `0x1900`, LUID
  `0:77473`, driver `32.0.13031.3015`
- WDDM: `3.2`
- active display evidence: 2560x1600 at 165 Hz; DPI `192` (200%)
- parent `DOROTI_WINDOWS_DWM_FLUSH` and
  `DOROTI_WINDOWS_EGL_SWAP_INTERVAL`: both unset
- comparison runs: `DOROTI_WINDOWS_DWM_FLUSH=0` and
  `DOROTI_WINDOWS_EGL_SWAP_INTERVAL=1`
- root repository status before implementation: clean; subsequent validator
  manifests record the task's validation-only changes

## P0.5 result

Manifest:
`.doroti/evidence/acrylic-p05-20260901-162530-02a5d0bb70f3/manifest.json`

Passed:

- top-HWND `DWMWA_REDIRECTIONBITMAP_ALPHA`: `S_OK`
- one visible HWND, zero visible/created child render HWNDs
- one Desktop Acrylic controller/target/root; Default/Base/Thin and custom
  runtime churn; 605 accepted updates with duplicate/missing terminal count 0
- automated 500-size script: 815 presents, queue depth 1, exact mismatch 0,
  render/GPU error 0; no CPU readback/upload path was introduced and the
  present/GPU-submit/GPU-copy counters remained one-to-one
- automatic focus, pointer, keyboard, cursor, and hit-test message probes
- WGC/native pointer transport: 265 captured frames, 53 encoded PNGs, no capture
  error/ring drop/capacity overflow

Failed:

- candidate decoded Doroti frame markers: `0`; opaque controls decoded 216 and
  213 matched frames
- Acrylic ROI differed from opaque by `61.73`, but the Doroti scene was absent,
  so this is backdrop-only evidence rather than an Acrylic-plus-content pass
- target-to-visible timing could not qualify without candidate markers and was
  recorded `FAIL`

## P1-CS result

Manifest:
`.doroti/evidence/acrylic-p1cs-20260901-163103-883fe1bbe050/manifest.json`

Passed:

- fresh B0 ContentIsland Acrylic target and same-device ANGLE direct import
- `CreatePresentationFactory`, `IsPresentationSupported`, manager,
  `DCompositionCreateSurfaceHandle`, `IPresentationSurface`, retiring fence,
  and `ICompositorInterop.CreateCompositionSurfaceForHandle`
- `IsPresentationSupportedWithIndependentFlip == true` (informational)
- 500 varying exact-size presents through three slots: 497 available reuses,
  249 render-worker waits, unavailable reuse 0, fourth slot 0, wrong/stale
  present 0, CPU copy 0
- Release self-contained publish and empty-`PATH` capability/buffer launch
- visible native pointer/WGC run: 821 accepted generations, 330 presented, 491
  superseded, failed 0, queue depth 2, slots 3, duplicate/missing terminal 0
- 241 captured frames, 25 PNGs, capture/drop/capacity errors 0; 46 alpha-scene
  colors and 11 transparent-center colors observed while one controller/target
  applied runtime backdrop churn
- target-to-present p50/p95/max: 4.48/7.48/9.18 ms; timing is diagnostic only
  because exactness failed

Failed:

- matched decoded frames: 236; presented-buffer/client extent mismatch: 160
- failure class:
  `composition-geometry-and-presented-buffer-were-not-exact-in-the-same-captured-frame`
- three consecutive automated qualification runs were stopped after the first
  hard-gate failure as required

The Presentation API's availability event solved the old B2 buffer-retirement
problem. It did not solve the independent WinComp visual-geometry versus
PresentationManager present boundary. This is the root cause of the P1-CS
rejection on this machine/session.

## Regression and acceptance boundary

- `validate-winrt-content-island-w1r.ps1`: PASS
- `validate-winrt-composition-w0.ps1`: PASS
- `validate-windows-dwm-redirection-alpha-a1.ps1`: validator PASS, expected P0
  child-alpha FAIL reproduced
- `validate-windows-acrylic-composition-b1.ps1`: validator PASS, expected B2/P1
  safe-retirement FAIL reproduced
- `validate-hwnd-exact-cpp-c0.ps1`: blocked before Doroti checks because the
  pinned `reference/flutter-master` checkout has tracked local changes

WGC plus scripted right-border input is not physical scan-out or human drag
acceptance. Eight-direction/corner drag, 60/120/144/165 Hz physical comparison,
100/125/150/200% monitor crossing, Snap/window lifecycle, transparency and
power/high-contrast/RDP policy, pointer/keyboard/clipboard, Korean IME,
Narrator/Accessibility Insights, and device-loss acceptance remain
`notVerified`. They were not run after the automated hard gates had already
failed.

## Reproduction

```powershell
pwsh -NoProfile -File ./Doroti/eng/validate-windows-acrylic-top-hwnd-p05.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windows-acrylic-composition-swapchain-p1cs.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-winrt-content-island-w1r.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-winrt-composition-w0.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windows-dwm-redirection-alpha-a1.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windows-acrylic-composition-b1.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-hwnd-exact-cpp-c0.ps1
```

The Composition Swapchain implementation follows the official
[programming guide](https://learn.microsoft.com/en-us/windows/win32/comp_swapchain/comp-swapchain)
and [API reference](https://learn.microsoft.com/en-us/windows/win32/api/_comp_swapchain/):
buffer availability is the final reuse authority, while the retiring fence and
present IDs are retained for ledger/diagnostic correlation.

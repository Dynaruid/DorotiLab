# Windows Acrylic and PlatformView composition regression

The top-level Composition HWND retains `WS_EX_NOREDIRECTIONBITMAP`. One visible
DirectComposition output composes premultiplied raster surfaces and live layered
EDIT/BUTTON HWND wrappers. Original native HWNDs are visually cloaked; their real
window procedures own native focus, capture and editing. Raster source windows
stay hidden. Native and raster scene geometry is published in the same tree,
without changing the output HWND Z-order on every scroll update.

Build the Windows Testbed in Release, then run from the repository root:

```powershell
python -c "import subprocess; subprocess.run(['python','Doroti/validation/windows-acrylic-composition/verify.py'],timeout=1200,check=True)"
```

Requires Windows, hardware Vulkan/Graphite, Python and Pillow. Outputs are
written under `Doroti/artifacts/validation/platform-views/windows-acrylic-composition/`.
The script temporarily raises its own Testbed window for screen pixel checks.
It checks parent redirection style, native/raster ordering and alpha for ten
overlap states, native HWND identity and text preservation, modal shields,
Tab/Shift+Tab, three create/dispose cycles, GDI counts, and successful close.

Input uses synthetic messages dispatched through actual HWND procedures.
Physical mouse/keyboard delivery, IME, resize smoothness, mixed DPI, and display
atomicity are not qualified. The ordinary gallery backdrop also needs a visual
check against a nonuniform desktop background; `Acrylic.Active` alone is not
an appearance test.

The gate adapts the repository's earlier PlatformView product validator
(`2d53dba1`, `Doroti/validation/platform-views/windows/product.py`). It waits
for committed native geometry when the widget's stage probe changes, rather
than treating the requested stage as a completed frame.

## Acrylic sample input regression

`verify-sample-input.py` opens the default gallery's Material sample, enables
Acrylic, and enters Platform views twice. It uses OS `SendInput` with a 150 ms
button hold across animation frames. It checks stable visible raster HWNDs,
overlap controls, foreground clicks, native editor focus, wheel scrolling,
navigation away, and bounded shutdown. Run it under a 1200-second parent timeout:

```powershell
python -c 'import subprocess; r=subprocess.run(["python","Doroti/validation/windows-acrylic-composition/verify-sample-input.py"],timeout=1200); raise SystemExit(r.returncode)'
```

The script brings its test window forward and moves the mouse. Captures, logs,
and results go to `Doroti/artifacts/validation/windows-platform-view-freeze`.
OS input injection does not qualify human-operated physical input or IME.

## Presented scroll frames

`verify-scroll-frames.py` opens the same gallery path and captures unique DXGI
desktop-duplication frames during wheel scrolling. It checks the green raster
and native editor top-edge separation, editor coverage, and checkerboard presence.
Missing content is a failure, not an omitted sample. The normal editor border
accounts for two physical pixels at 200% DPI. This bounded scene oracle does not
qualify all platform controls, monitors, or physical scan-out timing.

Requires `dxcam`, `comtypes`, NumPy and Pillow. Optional validation-only packages
can live under `Doroti/artifacts/validation/capture-python`.

```powershell
$env:DOROTI_SCROLL_CAPTURE_CYCLES='3'
$env:DOROTI_SCROLL_REQUIRE_COHERENT='1'
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-acrylic-composition/verify-scroll-frames.py
```

Set `DOROTI_SCROLL_REENTRY=1` for the deeper scroll that fully removes the native
scene from the viewport and brings it back. This mode derives expected native
editor coverage from the visible checkerboard, checks the first reentry pixels,
requires complete culling and the requested number of reentries, and verifies
native identity retention and focus release while offscreen. The ordinary
top-edge separation metric does not apply to partially clipped controls.

## Page entry and exit

`verify-entry-frames.py` captures three Material gallery → Platform views → gallery
cycles. The persistent title must remain present across every handoff; native
content must not disappear after first becoming visible. It uses the same DXGI
capture dependencies and writes `entry-frames.json` and bounded screenshots to
the artifact directory.

```powershell
$env:DOROTI_ENTRY_REQUIRE_COHERENT='1'
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-acrylic-composition/verify-entry-frames.py
```

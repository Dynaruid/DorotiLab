# Windows Acrylic PlatformView animation and input

The reported path is the default Windows App SDK Testbed, Open Material sample,
Acrylic enabled, then Platform views. The symptom was flicker and unusable
interaction while the loading indicator animates.

`WindowsPlatformViewHost` alternated two banks of child raster HWNDs on every
frame, including paint-only spinner/hover frames. Each swap hid the previous
bank, restaged native controls, and synchronously repainted them. Each raster
upload also called `WaitForCommitCompletion` on the HWND UI thread. The existing
standalone composition gate passed despite that behavior; it dispatched clicks
directly to HWND procedures and did not check stable raster HWNDs.

The host now retains visible raster HWNDs while the ordered raster/native
topology is unchanged. Native placement is reapplied only when geometry, order,
identity, or DPI changes. Unchanged raster sizes and alpha regions are retained;
changed alpha regions no longer request synchronous GDI redraw. Native raster
uploads return after DirectComposition Commit without blocking for compositor
completion. GPU readback completion and owner retirement are unchanged.

## Verification

- Windows Testbed Release build: zero warnings/errors.
- Existing `verify.py`: ten overlap states and pixel ordering/alpha checks,
  native text/identity preservation, modal shielding, Tab/Shift+Tab, three
  create/dispose cycles, and exit code 0 passed.
  Evidence: `Doroti/artifacts/validation/platform-views/windows-acrylic-composition/20260914-175711`.
- `verify-sample-input.py`: default gallery -> sample -> Acrylic on -> Platform
  views, two navigation cycles, stable visible HWNDs across animation commits,
  150 ms OS SendInput button holds, overlap/foreground clicks, native editor
  focus, wheel scrolling, and exit code 0 passed.
  Evidence: `Doroti/artifacts/validation/windows-platform-view-freeze/20260914-175859`.
  Final rebuild including the DPI placement guard repeated this gate successfully:
  `Doroti/artifacts/validation/windows-platform-view-freeze/20260914-180043`.
- The observed standalone UI commit p95 changed from 52.1907 ms before the
  patch (`20260914-174952`) to 21.7445 ms after it (`20260914-175711`). These are
  diagnostic run samples, not a controlled performance benchmark.

Captures were visually inspected with Acrylic enabled and the editor focused
after scrolling. Human-operated physical input, Korean IME, mixed-monitor DPI,
and atomic display of all independent DComp surfaces remain `notVerified`.
All validation processes were run under a 1200-second parent timeout.

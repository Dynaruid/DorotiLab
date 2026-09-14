# Windows Acrylic and PlatformView composition regression

The top-level Composition HWND must retain `WS_EX_NOREDIRECTIONBITMAP` even
when the application registers native views. Platform raster siblings use
premultiplied DirectComposition surfaces instead of `UpdateLayeredWindow`.
This prevents the opaque parent redirection surface from covering Acrylic
without losing the raster slices around live EDIT/BUTTON controls.

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

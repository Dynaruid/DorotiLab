# R4 Windows platform engine

## Runtime path

```text
Doroti.Samples.NakedWindow
  -> Win32WindowBackend
     -> internal NativeWindowHost (HWND + message pump + IMM connection)
  -> SkiaSurfaceFactory
     -> internal NativeFramebufferRenderTarget (Skia BGRA8888)
     -> IBgra8888FramebufferTarget
     -> StretchDIBits -> HWND
```

The public bridge is `IBgra8888FramebufferTarget`: logical window metrics enter the renderer, and a BGRA8888 pixel span returns for presentation. HWND, HDC, Skia objects and vendor records never cross that boundary.

## Window and input behavior

- `WM_SIZE` and `WM_DPICHANGED` become `WindowMetrics` with logical size, pixel size, scale and minimized state.
- Mouse and keyboard messages become backend-neutral raw events with logical coordinates and message timestamps.
- `WM_CHAR` and IMM start/update/end messages are connected to `ITextInputConnection`; R9 owns full editing semantics.
- Exceptions raised by a managed event sink are captured inside the unmanaged callback and rethrown by the managed message-pump boundary.
- Close requests are reported before HWND destruction; `WM_DESTROY` produces one closed notification and removes the root reference.

## Surface behavior

`SkiaSurfaceFactory` selects an audited WGL/OpenGL-backed Skia GPU surface by default. Context and `GRContext` creation are deferred to the raster thread, and the active diagnostic records the OpenGL renderer and version. A missing/incompatible GPU or Skia library falls back to the Skia BGRA8888 surface and then the managed BGRA8888 surface with the original exception type and message. The DXGI/EGL/ANGLE closure remains excluded.

Both surfaces read the current DPI-scaled pixel size at `BeginFrame`. A size or DPI change reallocates the target and increments `SurfaceGeneration`; an older frame cannot present afterward.

## Validation

The sample and `migration/scenes/naked-window-r4.json` share the exact 800x600 `#2952CCFF` contract. `Doroti.R4.Tests` verifies:

- exact RGBA-to-BGRA color presentation shared with SceneLab;
- resize/DPI pixel-size conversion and stale-generation rejection;
- concrete GPU-to-software fallback diagnostics;
- real HWND show, resize storm, minimize/restore and close behavior on Windows;
- 20 repeated create/destroy cycles returning process handle and thread counts to the warmed baseline.

The native sample smoke run is the runtime proof that cannot be replaced by compilation alone.

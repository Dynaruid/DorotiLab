# Windows MAUI resize and Acrylic regression

Run from the repository root. Each command has the repository's 1,200-second
process-tree deadline. Close other validation instances first; the scripts
only resize/close the process they start. Acrylic validation temporarily moves
the pointer and opens a red/blue background window, then restores the pointer.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-resize.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
```

The resize gate sends 24 bounded `SetWindowPos` changes, including moving-origin
growth and shrinkage. It checks multiple presented size generations, a single
Graphite device, final client/content dimensions, no renderer failures, and clean
shutdown. Its last resize occurs after the diagnostics write interval so an older
throttled evidence snapshot is not mistaken for a stale final frame.

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

The top-level observer now lets WinUI handle `WM_SIZE`/`WM_DPICHANGED` first and
updates XAML layout before publishing exact host dimensions. Proposed `WM_SIZING`
rectangles do not become the content size authority.

The MAUI window now consumes its requested Acrylic options through a window-owned
`SystemBackdrop`. Activation, theme, high contrast, and disconnection use the
[WinUI SystemBackdrop lifecycle](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.systembackdrop).
MAUI page/container backgrounds no longer apply a second layer over the renderer.
The shared Windows Testbed selects Thin Acrylic and changes only the enabled
sample scaffold opacity from 153/255 to 96/255; the disabled effect stays opaque.

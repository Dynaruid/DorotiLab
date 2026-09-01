# Windows Acrylic Composition Swapchain P1-CS spike

This opt-in validation project passes the actual ANGLE D3D11 device to
`CreatePresentationFactory`, checks composition and independent-flip support,
creates a `DCompositionCreateSurfaceHandle`/`IPresentationSurface`, and connects
that handle to a Windows.UI.Composition surface. The interop is isolated in the
validation project and requires the Windows SDK `presentation.h`,
`presentationtypes.h`, and `dcomp.lib`.

If this S0 capability gate fails, no presentation buffers or product API/ABI are
created. Physical scan-out, input, IME, UIA, and monitor acceptance remain
separate gates.

Run the complete opt-in validator from the repository root:

```powershell
pwsh -NoProfile -File ./Doroti/eng/validate-windows-acrylic-composition-swapchain-p1cs.ps1
```

The validator builds the native bridge, executes a fresh B0 probe, presents 500
varying buffers through a maximum three-slot pool, checks a Release
self-contained empty-`PATH` launch, and drives a visible right-border resize
while collecting WGC frames. A validator `FAIL` is an expected diagnostic
outcome when any hard gate rejects the candidate; it is not authorization to
integrate the spike into `Doroti/src`.

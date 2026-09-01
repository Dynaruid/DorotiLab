# Windows Acrylic top-HWND P0.5 spike

This opt-in validation project reuses the product ANGLE/EGL presenter while
making one standard overlapped top-level HWND the shell, input, Acrylic target,
and only visible render owner. It does not modify `Doroti/src` or public ABI.

Run through `eng/validate-windows-acrylic-top-hwnd-p05.ps1`. Automated WGC and
native pointer driving do not replace physical scan-out, IME, or UIA acceptance.

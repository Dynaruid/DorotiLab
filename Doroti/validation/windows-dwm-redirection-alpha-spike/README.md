# Windows DWM redirection-alpha spike

This target is the independent A1 gate for `work.md`. It keeps Doroti content on
one child HWND and reuses the product ANGLE/EGL fixed-size presenter without
changing product window creation or renderer selection.

The executable has mutually exclusive `opaque`, `dwm`, and `controller` arms.
Only the DWM arm sets `DWMWA_SYSTEMBACKDROP_TYPE`; only the controller arm creates
a `DesktopAcrylicController`, `DesktopWindowTarget`, and host backdrop target.
All arms can independently apply `DWMWA_REDIRECTIONBITMAP_ALPHA` to the top HWND,
child HWND, both, or neither.

Automated reports prove API activation, exact target/present counters, option
ordering, and terminal accounting. They do not prove that Acrylic or resize looks
correct on a physical monitor. Use the repository Windows Graphics Capture tool
and the physical checklist in `work.md` before promoting either Acrylic arm.

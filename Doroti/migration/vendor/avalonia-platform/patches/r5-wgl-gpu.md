# R5 WGL and Skia GPU adaptation

Source snapshot: `selected-content-sha256:75460dd7cb693dafaf5386a11fb878e222490e8a179e1a3f9d8b1283c10099cd`.

R5 selects the narrow WGL/OpenGL route because it uses the Windows-provided `opengl32.dll` and avoids the Avalonia ANGLE native package, MicroCom, DXGI, DComposition and Avalonia composition closure. The Win32 vendor slice owns pixel-format selection, WGL context current/restore, driver identification, swap and deterministic thread-affine cleanup.

The Skia vendor slice owns only `GRContext`, the default OpenGL framebuffer wrapper and primitive drawing lifetime. Doroti backend contracts hide HWND, WGL and Skia types. `Auto` creates GPU objects lazily on the raster thread, rejects known Microsoft software OpenGL renderers and switches to the existing Skia/managed software path after initialization or present failure. WGL disposal checks current-context clearing and `wglDeleteContext`; a failed native context release is surfaced instead of being counted as balanced. The device context belongs to the `CS_OWNDC` HWND lifetime, for which `ReleaseDC` has no effect and may return zero.

DisplayList decoding, LayerTree traversal, mailbox ordering, generation translation, ACK, recovery policy and fallback selection remain in Doroti-owned assemblies. Avalonia UI, render-loop and composition assemblies are not referenced.

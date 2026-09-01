# Acrylic ContentIsland B0 capability gate

This independent target runs only after the P0 child-HWND redirection-alpha arm
fails. It verifies the pinned Windows App SDK 2.4 `ContentIsland` backdrop target
and the packaged ANGLE D3D11 direct-import boundary before any product host is
changed.

The graphics probe creates a composition drawing surface from ANGLE's own D3D11
device, imports the transient `BeginDraw` texture through
`EGL_D3D_TEXTURE_ANGLE`, applies the returned X/Y offset, clears it on the GPU,
unbinds/destroys the EGL wrapper, and only then calls `EndDraw`. No CPU readback,
staging map, GDI, bitmap upload, or managed/native GPU pointer ABI is introduced.

Passing this gate proves runtime capability and teardown ordering only. It does
not prove a product presenter, safe three-slot retirement, visible alignment,
input/IME/UIA ownership, or physical border-drag quality.

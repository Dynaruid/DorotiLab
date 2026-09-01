# Windows Acrylic Composition B1/B2 spike

This executable is an isolated, non-product probe for the P1 path in `work.md`.
It connects a system-composition `ContentIsland`, `DesktopAttachedSiteBridge`,
`DesktopAcrylicController`, one content visual, and exact-sized
`CompositionDrawingSurface` slots. ANGLE draws directly into each transient
`BeginDraw` D3D11 update texture using premultiplied alpha and no CPU readback.

The probe deliberately caps the pool at three. It does not mutate a retired
slot unless an API-backed compositor-retirement/acquire signal is proven. On
the fourth distinct resize it therefore records
`failed-safe-retirement-unproven` and reports B2/P1 `FAIL`. This is the hard
stop required by the plan, not an implementation fallback.

The window is useful for automated capture, but a successful capture is not a
physical border-drag, IME, UIA, input-ownership, device-loss, or scan-out
acceptance.

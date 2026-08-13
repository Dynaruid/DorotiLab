# ADR-004: Surface generation and device loss

- Status: Accepted
- Date: 2026-08-01

## Decision

Every render surface has a monotonically increasing `SurfaceGeneration`. Resize, target replacement, context loss or device loss invalidates outstanding frames and advances the generation before a replacement surface is used. A frame whose generation differs from the active surface is stale and must not paint or present.

`IRenderSurface.BeginFrame` returns an `ISurfaceFrame` that captures generation and pixel size. Native handles and backend target objects remain private to the backend/vendor pair.

## Consequences

R3's fake surface rejects stale presentation. R4's GPU, Skia and managed software surfaces advance generation when logical size or DPI changes and reject frames that captured an older target. R5's `SurfaceSession` rejects a stale frame before paint, advances its logical generation across target recovery, and records either recovery or explicit termination in the failed ACK. The WGL/Skia GPU closure is approved and GPU-first; initialization or context/present loss switches to software with a concrete diagnostic. The larger DXGI/ANGLE closure remains excluded.

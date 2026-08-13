# R5 render pipeline

## Ownership

`DisplayListBuilder` is mutable only until `Build`. `LayerTreeSnapshot.Create` copies the layer topology into immutable snapshot nodes, and `SceneCommitter` retains immutable resource snapshots before a frame can enter the mailbox. UI-owned layers, builders and live backend objects never cross that boundary.

`FrameMailbox<CommittedScene>` holds at most one in-flight and one pending frame. A newer pending frame completes the replaced frame as `Superseded`. The dedicated raster thread alone performs preroll cache lookup, DisplayList execution and present. `RasterCache` binds itself to that thread and clears when the logical surface generation changes.

## Terminal frame order

Each committed scene ends exactly once as `Presented`, `Stale`, `Superseded`, `Failed` or `Cancelled`.

1. A stale generation is rejected before any paint command executes.
2. A surface frame is acquired and cleared.
3. The immutable LayerTree executes the same DisplayList against the selected backend-neutral raster canvas.
4. Present succeeds, or `SurfaceSession` attempts an explicit target recreation and advances its logical generation.
5. Resource leases are released in commit order.
6. The terminal ACK becomes observable and the mailbox advances.

Shutdown blocks new scene commits, cancels the pending frame, lets the in-flight frame reach a terminal ACK, and only then disposes the surface and registry owner.

## Evidence

`Doroti.R5.Tests` covers command validation, layer bounds and visible reference-scene output, managed/Skia tolerance, mailbox coalescing, stale resize, injected device loss, recovery, shutdown ordering, frame/microtask/dirty-queue order and trace replay. It does not require an exact pixel hash. A test run writes `artifacts/r5/frame-trace.json` and `artifacts/r5/performance-baseline.json`. The committed contracts are `migration/goldens/r5-layer-scene.json` and `migration/baselines/r5-render-baseline.json`.

R5.1 adds explicit `FrameFaultKind` classification. Raster/preroll programming faults do not recreate surfaces, while begin-frame/present and explicit device-loss failures may recover through `SurfaceSession`; unrecoverable backend failure remains distinct. Trace replay now verifies ordered commit/retain/enqueue and dequeue/raster/present/release/ACK transitions plus exactly-once resource balance. Test and target runs also write `doroti.runtime-report/v1` with warm-up, first-present, p50/p95/p99, resource deltas and explicit `not-verified` target gates.

The selected vendor closure now includes the narrow Avalonia-derived WGL context and Skia OpenGL render-target lifetime. `SurfaceBackendPreference.Auto` and `Gpu` are GPU-first; initialization is lazy on the raster thread, known Microsoft software renderers are rejected, and initialization/context/present failures advance surface generation before switching to software. GPU diagnostics name the real renderer and OpenGL version. GPU/software image tolerance and resource counters remain runtime gates on each target machine rather than compile-only evidence.

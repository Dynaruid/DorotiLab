# ADR-002: UI and raster thread model

- Status: Accepted
- Date: 2026-08-01

## Decision

The UI thread owns Widget, Element and RenderObject mutation and creates a committed scene. The raster thread is the only owner of backend canvases, GPU contexts and present operations. A commit transfers an immutable snapshot identified by `FrameId` and `SurfaceGeneration`; it does not transfer mutable UI objects.

`IFrameDispatcher` is the platform scheduling port. `IFrameScheduler` coalesces requests and begins frame production. `ICompositor` accepts only `ICommittedScene`. The platform message pump and a vendor implementation may request work, but do not own Doroti frame ordering.

## Consequences

R3 provides the ports and deterministic fake. R5 supplies the bounded mailbox, dedicated raster thread, fixed-thread raster cache ownership and ordered shutdown behavior. No temporary single-thread shortcut may change the public contracts.

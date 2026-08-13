# ADR-011: Render phase and fault taxonomy

## Decision

`PipelineOwner` is UI-thread owned and moves through `idle → layout → compositingBits → paint → commit → idle`. Tree mutation is allowed only while idle. Layout/paint invalidation during layout is coalesced; paint/compositing/commit re-entry fails with the active phase and node. An exception always restores `idle` and keeps the failed node dirty so a later frame can retry.

Raster failures use `FrameFaultKind`: programming, stale, superseded, cancelled, recoverable surface loss and fatal backend. Preroll/raster programming failures never recreate a surface. Begin-frame/present or explicit `SurfaceDeviceLostException` failures may use `SurfaceSession` recovery. Every terminal path releases resources and emits one ACK.

## Evidence

`Doroti.R5.Tests` checks fault classification and trace/resource replay. `Doroti.R6.Tests` checks phase violations and layout recovery.

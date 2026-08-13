# ADR-003: Immutable DisplayList and LayerTree

- Status: Accepted
- Date: 2026-08-01

## Decision

DisplayList is a backend-neutral value command stream. LayerTree snapshots reference immutable commands and resource identifiers, never Skia objects or live RenderObjects. Builders may be mutable before commit; their output is frozen when it becomes an `ICommittedScene`.

Geometry uses Doroti-owned immutable `Color`, `Size`, `Offset`, `Rect` and `Matrix` values. Finite geometry, balanced save/restore and immutable resource validation occur before a scene can enter the raster mailbox.

## Consequences

R5 implements the command stream, validation, `PictureLayer`, `TransformLayer`, `ClipRectLayer`, `OpacityLayer`, `ContainerLayer`, preroll bounds and immutable `LayerTreeSnapshot`. `SceneCommitter` retains only immutable resource snapshots and backend-neutral identifiers.

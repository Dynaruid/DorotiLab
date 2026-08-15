# ADR-014: Element inactive tree and Key lifecycle boundary

Status: superseded by the reviewed G5-3 Widgets implementation; the handwritten `Doroti.Widgets` owner has been removed.

## Decision

R6 does not introduce Element identity into RenderObject. R7 owns inactive Elements, local/global Key reservations and reconciliation rollback in `Doroti.Widgets`; RenderObject keeps only parent/order, ParentData and geometry state.

An inactive Element may temporarily retain a RenderObject only through the Widgets adapter. A root of the inactive forest is either retaken by the same `GlobalKey` once or unmounted by `BuildOwner.FinalizeTree`. Local keys reconcile only under their current parent; `GlobalKey` duplicate registration and unsupported nested inactive moves fail before silently changing identity. At finalization every inactive root is disposed and every reservation is cleared.

The FlutterCompat facade creates internal Widget adapters whose `IdentityType` is the source compatibility Widget type. Generated-code convenience APIs may not add identity or app state to RenderObject, and public FlutterCompat APIs may not expose native Element or RenderObject types.

This ADR freezes the boundary needed to prevent R6 tree APIs from becoming an alternate Element lifecycle.

# ADR-012: RenderObject ownership and boundaries

## Decision

One `PipelineOwner` and its creating UI thread own an attached RenderObject tree. A child has exactly one parent, one typed `ParentData`, a derived depth and at most one owner. Adoption rejects duplicate parents and cycles; detach removes dirty-queue entries recursively.

Generated packages bind through public `Doroti.FlutterCompat` value/facade types. Native RenderObject instances do not cross that public boundary.

# ADR-012: RenderObject ownership and boundaries

## Decision

One `PipelineOwner` and its creating UI thread own an attached RenderObject tree. A child has exactly one parent, one typed `ParentData`, a derived depth and at most one owner. Adoption rejects duplicate parents and cycles; detach removes dirty-queue entries recursively.

`RenderObject` owns invalidation and paint-layer reuse. `RenderBox` owns constraints, size, coordinate conversion and hit testing. Layout may only produce a finite non-negative size accepted by its `BoxConstraints`. RenderObjects record backend-neutral DisplayLists and Layers; they never own Skia, Win32, Avalonia/vendor or native handles.

Generated packages bind through public `Doroti.FlutterCompat` value/facade types. Native RenderObject instances do not cross that public boundary.

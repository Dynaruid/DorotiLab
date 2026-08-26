# R3 contract map

> Historical roadmap evidence. The handwritten `Doroti.Widgets` and root `Doroti` facade listed below were removed after the G5-3 reviewed framework cutover.

R3 fixes ownership and dependency direction; it does not implement a native window or rasterizer.

| Contract area | Assembly | R3 types | Next implementation |
|---|---|---|---|
| Time and frame dispatch | `Doroti.Core` | `IClock`, `IFrameDispatcher` | R4 platform loop, R5 scheduler |
| Immutable values | `Doroti.Graphics` | `Color`, `Size`, `Offset`, `Rect`, `Matrix` | R5 paint/path/DisplayList |
| Window and raw input | `Doroti.Platform` | `IWindowBackend`, `IWindow`, input/cursor/text ports | R4 Win32 adapter, R9 text behavior |
| Surface and frame identity | `Doroti.Composition` | `IRenderSurface`, `ISurfaceFrame`, `SurfaceGeneration`, `FrameId` | R4 surface, R5 mailbox/compositor |
| Resource lifetime | `Doroti.Composition` | `ResourceId`, `IResourceRegistry`, `IResourceLease` | R5 retain/release/ACK |
| Render commit | `Doroti.Rendering` | `IRenderNode`, `ISceneBuilder` | R5 immutable layer snapshot |
| Native widgets | `Doroti.Widgets` | `IWidget`, `IWidgetHost` | roadmap 2 |
| Runtime composition | `Doroti.Engine` | `IEngineHost`, `IEngineFactory` | R4/R5 composition root |
| Flutter facade | `Doroti.FlutterCompat` | baseline marker, `FrameCallback`, scheduler facade | R5 and roadmap 2 fixtures |

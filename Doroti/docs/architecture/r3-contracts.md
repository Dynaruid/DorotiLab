# R3 contract map

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
| Vendor translation | matching vendor/backend pairs | internal vendor shapes, public backend-neutral adapters | R4 selected Avalonia source |

`Doroti.Architecture.Tests` originally verified an exact project graph. Goal2 T0 supersedes that name-locked check with `migration/avalonia-shell/source-port-boundaries.json`: every product project is classified by a pattern, cycles and forbidden edges fail, and the comparison-only Avalonia package allowance records its A1 expiry. Vendor friend allowlists, exported member types, runtime isolation, analyzer failure fixtures, Flutter manifest reproduction, scheduler coalescing and stale-surface rejection remain executable checks.

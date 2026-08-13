# H2 Avalonia frame, surface and resource lifecycle

`Doroti.Host.Avalonia` now exposes one `IFrameDispatcher` and one `IAvaloniaFramePipeline` per window. Repeated engine invalidations enter the same Avalonia render-priority clock, while committed scenes enter the existing one-in-flight plus one-latest `FrameMailbox`. The host does not add an Avalonia-owned Widget tree, frame ACK meaning or resource registry.

The pipeline commits backend-neutral `DisplayList` and immutable image snapshots through `SceneCommitter`, `SurfaceSession` and `RasterCompositor`. Resize/DPI generation changes and adapter-detected size races end as `Stale`; a present/device failure ends as `Failed` with `RecoverableSurfaceLoss`, advances the logical generation and permits a replacement frame. Shutdown blocks commits, cancels pending work, waits for the in-flight terminal ACK, then releases every resource lease.

Surface construction is lazy: the backend-neutral Doroti managed BGRA8888 surface, its frames and its disposal all occur on the dedicated raster thread. Official Avalonia owns its ANGLE/OS GPU context and compositor thread; Doroti neither obtains nor releases that context. The raster thread stages premultiplied BGRA8888 bytes, and the Avalonia UI thread alone creates, replaces and disposes the `WriteableBitmap` imported into the window. `AvaloniaFramePipelineSnapshot` records both ownership boundaries and post-dispose balances.

`migration/host/h2-frame-fixture.json` is the backend-neutral image/golden/trace contract. `samples/AvaloniaHostCounter` executes it in separate strict ANGLE EGL and strict software Windows processes, covers presented/stale/superseded/failed ACKs, replays the frame trace, compares readback bytes and performs five create/dispose cycles per mode. The same fixture and sample are reusable on Linux and macOS; those real target runs remain explicitly `not-verified` until executed.

`IAvaloniaWindowCapture` is a Windows target-diagnostic feature, not a rendering dependency. It captures the visible DWM window frame for screenshot evidence and returns only Doroti-owned pixel/bounds records; no HWND or Avalonia type crosses the public boundary. `DorotiDemoApp --verify-target` compares its native client rectangle with the imported render pixels and logical-size/DPI calculation.

H2 does not implement pointer, keyboard, focus, pointer capture, text input, IME, clipboard or accessibility. Those remain H3.

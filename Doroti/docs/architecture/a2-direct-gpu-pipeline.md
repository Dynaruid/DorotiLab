# A2 direct GPU and asynchronous frame pipeline

`Doroti.Host.Desktop.DesktopGpuFrameSink` is the product composition root for the Windows A2 slice. It selects a strict WGL/OpenGL Skia surface and the engine `RasterCompositor`; applications and templates no longer select `Doroti.Backends.Skia` themselves. The host API exposes only Doroti-owned diagnostics. Skia, WGL, HWND and imported Avalonia types remain internal.

Hardware selection is fail-closed. `SkiaSurfaceFactory.CreateHardware` requires `IOpenGlWindowTarget`, rejects software OpenGL renderer identities, and uses `HardwareOnlyRenderSurface`. Resize or context loss increments surface generation and terminates the affected frame as stale or recoverable failure. The next frame recreates GPU state; A2 never turns `ManagedBgraRenderSurface`, Skia framebuffer upload, or any managed pixel buffer into a successful hardware run.

`IAsyncInteractiveFrameSink.PresentAsync` separates commit submission from terminal ACK. `InteractiveApplication.PumpFrameNonBlocking` completes build/layout/paint/commit on the UI thread, submits to the one-in-flight/one-latest mailbox, and posts the terminal ACK bookkeeping back through `QueuedUiDispatcher`. The product entrypoints do not call `GetAwaiter().GetResult()` for frame presentation. Synchronous `IInteractiveFrameSink.Present` remains only as a compatibility contract for deterministic fixtures.

`Doroti.Vendor.Avalonia.Base.SleepLoopRenderTimer` is the pinned source-port adaptation behind `DesktopFrameDispatcher`. On Windows it uses `DwmFlush` as the presentation-clock signal and coalesces all callbacks for a tick before posting them to the shell UI dispatcher. Dispatcher priority is no longer the clock. The precision sleep loop remains a non-Windows/failure fallback for the timer itself, not a rendering backend fallback.

Both GPU and diagnostic Skia canvases call `SkiaSharp.HarfBuzz.SKShaper`; the managed rectangle-glyph implementation remains reachable only through explicitly selected managed software diagnostics. `SkiaSharp.HarfBuzz` is pinned at the same `4.151.1` version as SkiaSharp.

The former `DorotiDemoApp --runtime-v2` diagnostic entrypoint was removed when
the product demo was consolidated into the Goal6 Material application. This
document describes the historical A2 application path. The remaining
current-machine target-report verifier is:

```powershell
./eng/doroti.ps1 test -TestSuite gpu-target -TargetReport artifacts/runtime-v2/<environment>/a2-gpu-target/gpu-target-target-report.json
```

The report derives present intervals from raster-thread `present` trace timestamps, measures state-change submission to terminal ACK latency, replays exact terminal ACK coverage, records queue high-watermark, GPU resource activity and backend/device identity, and performs a current-run GPU/software readback tolerance probe. It declares managed full-frame copies, managed framebuffer allocation, `WriteableBitmap` allocation and UI-thread synchronous ACK waits as zero only because the strict product graph contains none of those operations. Software reference rendering has separate visual evidence and is never counted as hardware success.

# Native Graphite session ownership

Status: **NG2 PARTIAL**. `SkiaGraphiteSession` implements the Metal recording and GPU-resource boundary. The macOS AppKit candidate is selected explicitly with `DOROTI_MACOS_GRAPHITE=1`; promotion remains subject to the gates in [work.md](../../../work.md). Vulkan creation is intentionally absent until the enabled-feature/extension and external-state bridge is qualified. UIKit, Qt and Android adapters are still required.

Each session represents one context generation and one owner thread. It owns a 256 MiB context budget, a 64 MiB recorder budget, one recorder, one image-provider cache and at most three outstanding frames. A second view gets a separate session and cache, even when it shares a platform device or queue. `SkiaGpuSurfaces.Register` lets existing renderer offscreen captures use that recorder. Runtime effects identify this path as `skiasharp-graphite-metal-gpu` and retain the renderer's existing per-owner/per-generation cache keys.

The host owns and retains the device, queue and drawable texture. `BeginMetalFrame` creates an external texture wrapper and a registered surface; only one frame may be actively recorded at a time. `CancelRecording` discards an unsubmitted frame. `Submit` snaps, inserts and submits asynchronously. Neither operation reports a frame as presented.

After submission, the host puts its presentation command buffer on the **same Metal queue**. A command-buffer completion is marshalled back to the session owner thread before `CompleteGpuWork` disposes the surface, backend wrapper and recording. The host separately checks surface generation and records the presentation terminal. The AppKit candidate preserves its transaction-presentation path during live layout. Committed callbacks retain their drawable until GPU completion; the next frame is requested after bounded-queue backpressure clears.

Readbacks are optional capture/diagnostic requests. `RequestReadback` must precede `Submit`; the native callback copies bytes into a managed result without throwing across the native boundary. A frame cannot be returned while its callback is pending, and readback pixels never feed presentation. The window probe has zero CPU readbacks/full-frame copies; the separate texture contract uses readback solely for pixel assertions.

Shutdown stops admission, waits for host completion callbacks, releases renderer-owned GPU caches, then releases the session's image cache, recorder and context before the platform queue/device. `Dispose` rejects outstanding frames and pending readbacks rather than waiting indefinitely or freeing live work. Failed submission faults the session; recovery must first establish a host GPU terminal, then create a new generation. There is no automatic Ganesh fallback. Device-loss teardown, a missing completion marker, reattaching a previously disposed native view and an unavailable owner dispatcher remain qualification gates; the current product candidate must not be described as passing them.

The shared session does not change Framework build/layout admission, exact viewport descriptors, or the Web worker's existing Graphite/Dawn and explicit WebGL implementations. The AppKit candidate keeps picture raster caching disabled while that separate cache policy is qualified. Its handler-release event invalidates renderer caches before the session is destroyed.

Reproduce the Metal contract and Ganesh/Graphite window probes with:

```sh
python3 Doroti/validation/appkit-metal-spike/run-graphite.py
```

The runner builds once, launches the actual app executable to capture Metal validation output, and gives every build/test child process an external 1,200-second timeout. Its PASS applies only to the selected probe contracts. Product default promotion, device loss, physical display/input/accessibility acceptance, performance, and cross-platform deployment require separate evidence.

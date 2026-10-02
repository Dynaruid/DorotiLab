# Rendering and lifetime baseline protocol

Keep CPU framework work, CPU raster, GPU completion, readback/upload, native presentation, managed memory and VRAM separate. `requestAnimationFrame` and GPU completion are not proof of displayed FPS. Missing measurements are `null`/`notMeasured`, never zero.

| Scene | Fixed configuration | Correctness gate |
| --- | --- | --- |
| Long list | `HotReloadSample`, 1,000 rows × 40 logical px; 160 px scroll | Pointer routing; mounted Text count ≤60 at 480×320; state/offset survives reassemble |
| VariableBlur | SampleApp2 Variable Blur; 180 px overlay, sigma 32; full/adaptive/fast/fixed 1/4 | Same logical capture domain, halo/pixel origin; clamp crop and repeated/mirror full domain; odd-sized backing keeps sampling phase |
| Multiple effects | Testbed effect gallery, fixed 450×800, same enabled effects per comparison | No displacement, stale region, unexpected clipping or input offset; record effect settings with capture |
| WebView overlay | Testbed WebView scene; same HTML, viewport and overlay bounds | Overlay ordering, native input and recreation; do not combine unsupported native-editor/WebView topologies |
| Image/video texture | Testbed texture validation export, fixed source size and frame sequence | Owner/context generation, freeze/unfreeze, latest upload and lease release; real video decode requires a real video source |
| Resize/DPR | CPU marker at (20,30), 80×60; 320×240 / 480×320; DPR 1 / 1.25 / 2 | Red at logical (40,50), background at (110,50); renderer failures=0; engine-layer delta after dispose=0 |

Target comparison profiles are Windows App SDK Graphite/Vulkan at 450×800 and 960×640; desktop Chrome WebGL2/WebGPU at the same logical sizes/DPR 1 and 2; physical Android Web at 360×800/DPR 3. Record actual CPU/GPU/device/browser/renderer and backing size. The Android profile is a requested physical qualification target, not a completed desktop emulation result.

Before any performance optimization, capture its baseline with unchanged settings, warm the scene, then collect at most 30 samples. Default relative acceptance budget: p95 of each measured phase and bytes per frame must not regress by >10%; steady retained bytes must stay within the configured cache limit; post-teardown owned-resource delta must be zero. A 60 Hz target has a 16.67 ms presentation interval budget, but only actual presentation evidence can qualify it. These are prospective budgets, not claims that every scene currently meets them. A geometry/input/composition failure rejects a change even if timing improves.

`WidgetTester.FrameTrace` exposes opt-in real monotonic recording times. Build → layout, layout → paint (includes compositing bits), paint → scene build, and raster → rasterEnd delimit distinct CPU intervals. The virtual activity clock drives settle deadlines; it does not replace measurement time. `setViewport`, `pixel`, `RendererDiagnostics` and `CacheMemory` are available to regression fixtures. CPU tests do not execute the VariableBlur GPU kernel; its capture-rectangle checks protect geometry policy only.

Web transport admits four resize messages plus one replaceable latest slot. `ResizeAdmissionWindow` is exercised by `tests/web_rendering.mts`, including unknown/duplicate acknowledgment and reset after context replacement. Mobile backing starts at exact physical viewport size, shrinks after 1 s stable/2 s allocation hysteresis when ≥25% excess, and shrinks axes before growing on orientation changes. The test bounds this intermediate area at 2,592,000 pixels for 1080×2400 ↔2400×1080; it is a backing-pixel bound, not measured VRAM. Actual pause/resume, GPU context/device loss, PlatformView CPU upload and mobile memory still require browser/device runs.

Font checks must observe bytes registered with Skia, the rendered Korean glyphs and network requests. SampleApp2 exposes CDN mode and `-p:DorotiSampleWebFontSource=Assets` with local decoder/CSS assets; use a cold profile and block external networking for the offline case. Verify every font response is nonempty, that local mode has no required CDN requests, and that initial fallback/replacement does not lose text. The template no longer preloads the removed host `NanumGothic-Regular.ttf` URL. Default language preloading remains opt-in.

Run `python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug` and `python Doroti/eng/run-with-timeout.py node --experimental-transform-types --test Doroti/tests/web_rendering.mts`. Node 24's TypeScript transformation is used only for these source-level policy tests. See [04 work results](../../history/26-10-03/works/common/04-rendering-lifetime.md) for measured values and remaining host qualifications.


2026-09-29 Linux Qt recorded [VM phase timings and lifetime results](../../history/26-10-03/works/results/2026-09-29-linux-qt.md#렌더링-수치의-범위)
for Quick/Graphite software Vulkan on Wayland and XWayland. They report recording,
fence waits, R/P reservation and retirement, not physical present FPS or VRAM.
Widgets/OpenGL VMware SVGA3D remains a separate comparison. No physical Linux GPU
budget or long-run memory baseline has been accepted.


AppKit (2026-09-29) now records `PresentedDrawables`, up to 30
`PresentationIntervalsMilliseconds` from positive MTLDrawable.PresentedTime
callbacks, and `MetalAllocatedBytes`. These differ from GPU command-buffer
completion. Metal allocation is device/process resource memory, not per-window
VRAM. The [M1 Graphite/Ganesh resize measurements](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md#metal-계측-수치)
are driven by scripted resizes, so their p95 intervals are not a sustained FPS
or 60 Hz performance gate. Native interleaving sometimes supplied too few
presentation timestamps: report notMeasured rather than substituting GPU completion.

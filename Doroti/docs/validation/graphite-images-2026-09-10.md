# Graphite image cancellation regression — 2026-09-10

The Windows Material sample could retain an empty image after Graphite cancelled
its first recording. `Frame.CancelRecording()` discarded GPU uploads while
`SKGraphiteImageCache` retained their texture handles. Picture raster snapshots
created by that recording also survived in `SkiaSceneRenderer`.

The initial repair replaced the session image cache and changed a recorder-scoped
cache identity. Before its next paint, the scene renderer detects that identity
change and releases picture raster caches and image-filter surfaces. Submitted
frames keep their existing completion and resource ownership rules. The native
library, backend defaults and decoded CPU image cache are unchanged.

## Regression evidence

The external-texture probe now draws colors distinct from the background; the
old same-color check could pass when the image draw was absent. It tests a
downscaled mipmapped image, compatible offscreen capture, and runtime shader.
It also decodes the sample WebP with the product decoder, promotes its scene into
the product raster cache inside a cancelled recording, and compares subsequent
GPU pixels with a CPU reference (RGB tolerance 2 for the photograph, exact solid
colors and alpha).

Evidence under `Doroti/artifacts/native-graphite/runs/`:

- `20260910T015738402Z`: before the fix, cancelled upload reuse fails the first
  image pixel check.
- `20260910T020324427Z`: negative control with scene-cache invalidation temporarily
  disabled fails at frame 0, pixel 1024 (the first sample-photo pixel). The final
  source restores invalidation.
- `20260910T020427638Z`: final NVIDIA RTX 4060 Laptop and AMD Radeon 780M runs each
  pass 3 context generations and 108 external-texture frames, including sample
  pixel comparison, GPU completion, Composition presentation and normal teardown.
  Both report zero Vulkan validation warnings/errors. Raw probe exits are 2 and
  report status remains `PARTIAL`, as required by the diagnostic qualification
  contract; this is not whole-platform qualification. The wrapper itself returned
  1, so results above are taken from the individual JSON/process records, not a
  wrapper-level PASS.

## Product execution

The user's Release command was rebuilt and run with `DOROTI_TESTBED_MODE=sample`
and `DOROTI_RESIZE_FIXTURE=none`, adding only a 10-second smoke timer and diagnostics.
Evidence: `.doroti/evidence/graphite-images-20260910/sample-final.log` and
`sample-final-result.json`. Exit 0; AMD Graphite/Vulkan/Composition-Swapchain;
12 successful presents; zero failed terminals, initialization/operational debug
errors or device-loss results. Test processes used a 1,200-second timeout and
the sample process exited normally.

Live screenshot and manual interaction verification remain `notVerified`:
Computer Use could not connect to its native pipe, including after reset/retry.
No Android, Linux or Apple execution was performed for this fix.

## Follow-up: preserve submitted caches during cancellation

The user confirmed that images became visible but reported severe stutter. The
initial repair invalidated every image and picture cache on any cancellation,
including entries whose GPU work had already been submitted successfully. This
could repeatedly upload images and replay expensive pictures.

The current repair tracks uploads created by the current recording separately.
Cancellation evicts only those uploads; submitted entries retain their bounded
256-entry LRU. Each picture-raster and image-filter snapshot retains the recording
that created it. Discarded snapshots are rejected on lookup, while snapshots from
successful submissions, command recordings, cache warmups and reusable surface
pools survive later cancellations. No synchronous GPU drain was added.

`20260910T021758492Z` under the same probe evidence root passed both GPUs, each
with 3 context generations and 108 presented frames. Across those runs, 144
additional cancelled paints reused committed image caches: zero extra uploads
or raster promotions of the existing sample image. Each cancelled paint also
created a fresh upload, and the regression asserts that exactly that fresh upload
is discarded. The original cancelled-first-upload/photo-pixel checks still pass,
with Vulkan validation warnings/errors zero. Diagnostic `PARTIAL`/exit 2 and the
physical-validation boundaries above remain unchanged.

Additional evidence is under
`.doroti/evidence/graphite-cache-performance-20260910/`: the picture-command and
GPU compositing moving/owners/exception regressions pass (all exit 0 with
1,200-second timeouts). The rebuilt sample also exits 0 after a 10-second smoke:
Graphite/Vulkan/Composition-Swapchain, zero failed terminals and operational debug
errors (`sample-release.json` / `sample-release.log`). These checks establish cache reuse and pixel correctness;
they do not establish interactive scrolling FPS or the user's perceived latency.
